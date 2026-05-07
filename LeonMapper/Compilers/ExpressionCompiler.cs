using System.Linq.Expressions;
using System.Reflection;
using LeonMapper.Convert;
using LeonMapper.Plan;
using LeonMapper.Plan.Builder;
using LeonMapper.Utils;

namespace LeonMapper.Compilers;

/// <summary>
/// 表达式树编译器：从 TypeMappingPlan 编译为 Func 委托
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TTarget">目标类型</typeparam>
public class ExpressionCompiler<TSource, TTarget> : ICompiler<TSource, TTarget> where TTarget : class
{
    private readonly Func<TSource, TTarget?> _compiledFunc;

    /// <summary>
    /// 从映射计划构建表达式树并编译为委托
    /// </summary>
    /// <param name="plan">类型映射计划</param>
    public ExpressionCompiler(TypeMappingPlan plan)
    {
        _compiledFunc = BuildFunc(plan);
    }

    /// <summary>
    /// 执行映射，源对象为 null 时返回 null（仅对引用类型生效）
    /// </summary>
    public TTarget? Map(TSource source)
    {
        if (source is null)
        {
            return default;
        }

        return _compiledFunc(source);
    }

    /// <summary>
    /// 从映射计划构建编译后的映射委托
    /// </summary>
    private static Func<TSource, TTarget?> BuildFunc(TypeMappingPlan plan)
    {
        var sourceParam = Expression.Parameter(typeof(TSource), "source");

        // 检查是否为纯集合类型映射（如 List<T> -> List<TNew>）
        var sourceElementType = TypeUtils.GetCollectionElementType(plan.SourceType);
        var targetElementType = TypeUtils.GetCollectionElementType(plan.TargetType);
        // 过滤掉集合类型特有的属性（如 Capacity, Count 等）
        var meaningfulProps = plan.PropertyMappings
            .Where(m => m.SourceMember.Name != "Capacity")
            .ToList();
        if (sourceElementType != null && targetElementType != null
            && meaningfulProps.Count == 0 && plan.FieldMappings.Count == 0)
        {
            // 纯集合映射：直接生成 Select(...).ToList()/ToArray() 表达式
            var collectionExpr = BuildInlineCollectionMapping(
                sourceParam, plan.SourceType, plan.TargetType,
                sourceElementType, targetElementType);
            var lambda = Expression.Lambda<Func<TSource, TTarget?>>(collectionExpr, sourceParam);
            return lambda.Compile();
        }

        if (plan.TargetType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new Exceptions.NoEmptyConstructorException();
        }

        // new TTarget()
        var newTargetExpr = Expression.New(plan.TargetType);

        var bindings = new List<MemberBinding>();

        // 属性映射
        foreach (var mapping in plan.PropertyMappings)
        {
            var binding = BuildPropertyBinding(sourceParam, mapping);
            if (binding != null)
            {
                bindings.Add(binding);
            }
        }

        // 字段映射
        foreach (var mapping in plan.FieldMappings)
        {
            var binding = BuildFieldBinding(sourceParam, mapping);
            if (binding != null)
            {
                bindings.Add(binding);
            }
        }

        var memberInit = Expression.MemberInit(newTargetExpr, bindings);
        var lambda2 = Expression.Lambda<Func<TSource, TTarget?>>(memberInit, sourceParam);
        return lambda2.Compile();
    }

    /// <summary>
    /// 构建内联集合映射表达式：使用 foreach + List.Add 模式，
    /// 当源类型实现 ICollection&lt;T&gt; 时预分配 List 容量避免扩容开销
    /// </summary>
    private static Expression BuildInlineCollectionMapping(Expression sourceAccess,
        Type sourceType, Type targetType, Type sourceElementType, Type targetElementType)
    {
        // 基本类型的元素（int→long 等）：走转换器路径
        if (TypeUtils.IsBaseType(sourceElementType) && TypeUtils.IsBaseType(targetElementType))
        {
            return BuildPrimitiveCollectionExpression(sourceAccess, sourceType, targetType,
                sourceElementType, targetElementType);
        }

        var targetListType = typeof(List<>).MakeGenericType(targetElementType);
        var addMethod = targetListType.GetMethod("Add", new[] { targetElementType })!;

        var loopVar = Expression.Parameter(sourceElementType, "item");
        var elementExpr = BuildElementMappingExpression(loopVar, sourceElementType, targetElementType);

        // 数组目标：先收集到 List，最后 ToArray()
        if (targetType.IsArray)
        {
            var listVar = Expression.Parameter(targetListType, "list");
            var forEachExpr = BuildForEach(sourceAccess, loopVar,
                Expression.Call(listVar, addMethod, elementExpr));

            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray")!
                .MakeGenericMethod(targetElementType);

            var block = Expression.Block(
                new[] { listVar },
                Expression.Assign(listVar, BuildListCreation(sourceAccess, sourceElementType, targetListType)),
                forEachExpr,
                Expression.Call(toArrayMethod, listVar));

            return Expression.Convert(block, targetType);
        }

        // List<T> 目标：直接构建 List 并 Add
        var isTargetList = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>);
        if (isTargetList)
        {
            var listVar = Expression.Parameter(targetListType, "list");
            var forEachExpr = BuildForEach(sourceAccess, loopVar,
                Expression.Call(listVar, addMethod, elementExpr));

            return Expression.Block(
                new[] { listVar },
                Expression.Assign(listVar, BuildListCreation(sourceAccess, sourceElementType, targetListType)),
                forEachExpr,
                listVar);
        }

        // 其他 IEnumerable<T> 派生类型：先构建 List，再用目标类型构造函数转换
        var listVar2 = Expression.Parameter(targetListType, "list");
        var forEachExpr2 = BuildForEach(sourceAccess, loopVar,
            Expression.Call(listVar2, addMethod, elementExpr));

        var ctor = targetType.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(targetElementType) });
        if (ctor != null)
        {
            return Expression.Block(
                new[] { listVar2 },
                Expression.Assign(listVar2, BuildListCreation(sourceAccess, sourceElementType, targetListType)),
                forEachExpr2,
                Expression.New(ctor, listVar2));
        }

        // 最终回退：Select + 物化
        var selectMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
            .MakeGenericMethod(sourceElementType, targetElementType);

        var lambdaType = typeof(Func<,>).MakeGenericType(sourceElementType, targetElementType);
        var lambda = Expression.Lambda(lambdaType, elementExpr, loopVar);
        var selectCall = Expression.Call(selectMethod, sourceAccess, lambda);
        return CollectionMaterializer.BuildMaterializeExpression(selectCall, targetType, targetElementType);
    }

    /// <summary>
    /// 构建 List 创建表达式，当源类型实现 ICollection{T} 时预分配容量
    /// </summary>
    private static Expression BuildListCreation(Expression sourceAccess, Type sourceElementType, Type targetListType)
    {
        var collectionType = typeof(ICollection<>).MakeGenericType(sourceElementType);
        if (collectionType.IsAssignableFrom(sourceAccess.Type))
        {
            var countCtor = targetListType.GetConstructor(new[] { typeof(int) })!;
            var countProp = collectionType.GetProperty("Count")!;
            return Expression.New(countCtor, Expression.Property(sourceAccess, countProp));
        }

        return Expression.New(targetListType.GetConstructor(Type.EmptyTypes)!);
    }

    /// <summary>
    /// 手动构建 foreach 循环表达式（兼容 .NET 6.0，Expression.ForEach 需要 .NET 7+）
    /// </summary>
    private static Expression BuildForEach(Expression collection, ParameterExpression loopVar, Expression body)
    {
        var elementType = loopVar.Type;
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
        var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

        var getEnumeratorMethod = enumerableType.GetMethod("GetEnumerator")!;
        var moveNextMethod = typeof(System.Collections.IEnumerator).GetMethod("MoveNext")!;
        var currentProp = enumeratorType.GetProperty("Current")!;
        var disposeMethod = typeof(IDisposable).GetMethod("Dispose")!;

        var enumeratorVar = Expression.Parameter(enumeratorType, "enumerator");
        var breakLabel = Expression.Label("breakLabel");

        // enumerator = collection.GetEnumerator()
        var assignEnumerator = Expression.Assign(enumeratorVar,
            Expression.Call(collection, getEnumeratorMethod));

        // loopVar = enumerator.Current
        var assignCurrent = Expression.Assign(loopVar,
            Expression.Property(enumeratorVar, currentProp));

        // while (enumerator.MoveNext()) { loopVar = enumerator.Current; body; }
        var whileLoop = Expression.Loop(
            Expression.IfThenElse(
                Expression.Call(enumeratorVar, moveNextMethod),
                Expression.Block(assignCurrent, body),
                Expression.Break(breakLabel)),
            breakLabel);

        // try { while loop } finally { enumerator.Dispose(); }
        var tryFinally = Expression.TryFinally(whileLoop,
            Expression.Call(enumeratorVar, disposeMethod));

        return Expression.Block(
            new[] { enumeratorVar, loopVar },
            assignEnumerator,
            tryFinally);
    }

    /// <summary>
    /// 构建单个元素的映射表达式（内联，避免委托间接调用）
    /// </summary>
    private static Expression BuildElementMappingExpression(ParameterExpression sourceParam,
        Type sourceElementType, Type targetElementType)
    {
        // 检查元素类型是否都是集合类型（嵌套集合场景）
        var nestedSourceElementType = TypeUtils.GetCollectionElementType(sourceElementType);
        var nestedTargetElementType = TypeUtils.GetCollectionElementType(targetElementType);

        if (nestedSourceElementType != null && nestedTargetElementType != null)
        {
            // 元素本身也是集合，需要递归构建集合映射表达式
            return BuildInlineCollectionMapping(sourceParam, sourceElementType, targetElementType,
                nestedSourceElementType, nestedTargetElementType);
        }

        // 获取元素类型的映射计划
        var plan = MappingPlanBuilder.Build(sourceElementType, targetElementType);

        // 检查目标类型是否有无参构造函数
        var constructor = targetElementType.GetConstructor(Type.EmptyTypes);
        if (constructor == null)
        {
            throw new Exceptions.NoEmptyConstructorException(
                $"类型 {targetElementType.Name} 缺少无参构造函数，无法作为集合元素类型");
        }

        // new TTarget()
        var newExpr = Expression.New(constructor);

        var bindings = new List<MemberBinding>();

        // 属性映射
        foreach (var mapping in plan.PropertyMappings)
        {
            var binding = BuildPropertyBinding(sourceParam, mapping);
            if (binding != null)
            {
                bindings.Add(binding);
            }
        }

        // 字段映射
        foreach (var mapping in plan.FieldMappings)
        {
            var binding = BuildFieldBinding(sourceParam, mapping);
            if (binding != null)
            {
                bindings.Add(binding);
            }
        }

        // MemberInit: new TTarget { Prop1 = x.Prop1, Prop2 = x.Prop2, ... }
        return Expression.MemberInit(newExpr, bindings);
    }

    /// <summary>
    /// 构建属性的成员绑定表达式
    /// </summary>
    private static MemberBinding? BuildPropertyBinding(ParameterExpression sourceParam, MemberMapping mapping)
    {
        var sourceProp = (PropertyInfo)mapping.SourceMember;
        var targetProp = (PropertyInfo)mapping.TargetMember;

        var sourceAccess = Expression.Property(sourceParam, sourceProp);

        return mapping.Strategy switch
        {
            MappingStrategy.Direct => BuildDirectBinding(targetProp, sourceAccess),
            MappingStrategy.Convert => BuildConvertBinding(targetProp, sourceAccess, mapping),
            MappingStrategy.Complex => BuildComplexBinding(targetProp, sourceAccess, mapping),
            MappingStrategy.Collection => BuildCollectionBinding(targetProp, sourceAccess, mapping),
            MappingStrategy.Dictionary => BuildDictionaryBinding(targetProp, sourceAccess, mapping),
            _ => null
        };
    }

    /// <summary>
    /// 构建字段的成员绑定表达式
    /// </summary>
    private static MemberBinding? BuildFieldBinding(ParameterExpression sourceParam, MemberMapping mapping)
    {
        var sourceField = (FieldInfo)mapping.SourceMember;
        var targetField = (FieldInfo)mapping.TargetMember;

        var sourceAccess = Expression.Field(sourceParam, sourceField);

        return mapping.Strategy switch
        {
            MappingStrategy.Direct => BuildDirectBinding(targetField, sourceAccess),
            MappingStrategy.Convert => BuildConvertBinding(targetField, sourceAccess, mapping),
            MappingStrategy.Complex => BuildComplexBinding(targetField, sourceAccess, mapping),
            MappingStrategy.Collection => BuildCollectionBinding(targetField, sourceAccess, mapping),
            MappingStrategy.Dictionary => BuildDictionaryBinding(targetField, sourceAccess, mapping),
            _ => null
        };
    }

    /// <summary>
    /// 构建直接赋值的成员绑定表达式（支持可空类型自动转换）
    /// </summary>
    private static MemberBinding BuildDirectBinding(MemberInfo targetMember, Expression sourceAccess)
    {
        var targetMemberType = MemberUtils.GetMemberType(targetMember);
        var sourceType = sourceAccess.Type;

        Expression finalExpr = sourceAccess;

        // 处理可空类型到非可空类型的映射：source ?? default(T)
        if (TypeUtils.IsNullableType(sourceType) && !TypeUtils.IsNullableType(targetMemberType))
        {
            var underlyingType = TypeUtils.GetUnderlyingType(sourceType);
            var hasValueProp = sourceType.GetProperty("HasValue")!;
            var valueProp = sourceType.GetProperty("Value")!;

            var hasValueExpr = Expression.Property(sourceAccess, hasValueProp);
            var valueExpr = Expression.Property(sourceAccess, valueProp);
            var defaultExpr = Expression.Default(targetMemberType);

            finalExpr = Expression.Condition(hasValueExpr, valueExpr, defaultExpr);
        }
        // 处理非可空类型到可空类型的映射：直接包装
        else if (!TypeUtils.IsNullableType(sourceType) && TypeUtils.IsNullableType(targetMemberType))
        {
            finalExpr = Expression.Convert(sourceAccess, targetMemberType);
        }
        // 处理不同类型之间的转换（如 int -> long）
        else if (sourceType != targetMemberType)
        {
            finalExpr = Expression.Convert(sourceAccess, targetMemberType);
        }

        if (targetMember is PropertyInfo prop)
        {
            return Expression.Bind(prop, finalExpr);
        }

        return Expression.Bind((FieldInfo)targetMember, finalExpr);
    }

    /// <summary>
    /// 构建类型转换的成员绑定表达式（支持可空类型）
    /// </summary>
    private static MemberBinding BuildConvertBinding(MemberInfo targetMember, Expression sourceAccess,
        MemberMapping mapping)
    {
        if (mapping.Converter == null)
        {
            throw new InvalidOperationException($"成员 {targetMember.Name} 的转换策略缺少转换器实例");
        }

        var converterType = mapping.Converter.GetType();
        var convertMethod = converterType.GetMethod("Convert");
        if (convertMethod == null)
        {
            throw new InvalidOperationException($"转换器 {converterType.Name} 缺少 Convert 方法");
        }

        var targetMemberType = MemberUtils.GetMemberType(targetMember);
        var sourceType = sourceAccess.Type;

        var sourceUnderlyingType = TypeUtils.GetUnderlyingType(sourceType);
        var targetUnderlyingType = TypeUtils.GetUnderlyingType(targetMemberType);

        var converterInstance = Expression.Constant(mapping.Converter);
        Expression convertCall;

        // 如果源是可空类型，需要处理 null 情况
        if (TypeUtils.IsNullableType(sourceType) && sourceUnderlyingType == convertMethod.GetParameters()[0].ParameterType)
        {
            var hasValueProp = sourceType.GetProperty("HasValue")!;
            var valueProp = sourceType.GetProperty("Value")!;

            var hasValueExpr = Expression.Property(sourceAccess, hasValueProp);
            var valueExpr = Expression.Property(sourceAccess, valueProp);
            var convertedValue = Expression.Call(converterInstance, convertMethod, valueExpr);

            if (TypeUtils.IsNullableType(targetMemberType))
            {
                // source.HasValue ? (TTarget?)convertedValue : null
                var nullExpr = Expression.Constant(null, targetMemberType);
                var wrappedValue = Expression.Convert(convertedValue, targetMemberType);
                convertCall = Expression.Condition(hasValueExpr, wrappedValue, nullExpr);
            }
            else
            {
                // source.HasValue ? convertedValue : default(TTarget)
                var defaultExpr = Expression.Default(targetMemberType);
                var typedValue = Expression.Convert(convertedValue, targetMemberType);
                convertCall = Expression.Condition(hasValueExpr, typedValue, defaultExpr);
            }
        }
        else
        {
            convertCall = Expression.Call(converterInstance, convertMethod, sourceAccess);
            if (targetMemberType != convertCall.Type)
            {
                convertCall = Expression.Convert(convertCall, targetMemberType);
            }
        }

        if (targetMember is PropertyInfo prop)
        {
            return Expression.Bind(prop, convertCall);
        }

        return Expression.Bind((FieldInfo)targetMember, convertCall);
    }

    /// <summary>
    /// 构建复杂类型递归映射的成员绑定表达式
    /// </summary>
    private static MemberBinding BuildComplexBinding(MemberInfo targetMember, Expression sourceAccess,
        MemberMapping mapping)
    {
        var sourceMemberType = MemberUtils.GetMemberType(mapping.SourceMember);
        var targetMemberType = MemberUtils.GetMemberType(targetMember);

        // 使用缓存的映射委托
        var mapper = CachedMapperFactory.GetOrCreateMapper(sourceMemberType, targetMemberType);
        var mapperConstant = Expression.Constant(mapper);
        var mapMethod = mapper.GetType().GetMethod("MapTo");
        if (mapMethod == null)
        {
            throw new InvalidOperationException($"Mapper<{sourceMemberType.Name}, {targetMemberType.Name}> 缺少 MapTo 方法");
        }

        var mapCall = Expression.Call(mapperConstant, mapMethod, sourceAccess);
        var convertedExpr = Expression.Convert(mapCall, targetMemberType);

        if (targetMember is PropertyInfo prop)
        {
            return Expression.Bind(prop, convertedExpr);
        }

        return Expression.Bind((FieldInfo)targetMember, convertedExpr);
    }

    /// <summary>
    /// 构建集合类型映射的成员绑定表达式
    /// </summary>
    private static MemberBinding BuildCollectionBinding(MemberInfo targetMember, Expression sourceAccess,
        MemberMapping mapping)
    {
        var sourceMemberType = MemberUtils.GetMemberType(mapping.SourceMember);
        var targetMemberType = MemberUtils.GetMemberType(targetMember);

        var sourceElementType = TypeUtils.GetCollectionElementType(sourceMemberType);
        var targetElementType = TypeUtils.GetCollectionElementType(targetMemberType);

        if (sourceElementType == null || targetElementType == null)
        {
            throw new InvalidOperationException($"成员 {mapping.SourceMember.Name} 或 {targetMember.Name} 不是有效的集合类型");
        }

        Expression collectionExpr;

        // 判断元素类型是否都是基础类型
        if (TypeUtils.IsBaseType(sourceElementType) && TypeUtils.IsBaseType(targetElementType))
        {
            // 基础类型集合转换：使用 ConvertFactory 获取转换器
            collectionExpr = BuildPrimitiveCollectionExpression(
                sourceAccess, sourceMemberType, targetMemberType,
                sourceElementType, targetElementType);
        }
        else
        {
            // 复杂类型集合转换：使用内联集合映射
            collectionExpr = BuildInlineCollectionMapping(sourceAccess, sourceMemberType, targetMemberType,
                sourceElementType, targetElementType);
        }

        // 构建 null 检查：source == null ? null : mappedCollection
        var nullCheck = Expression.Condition(
            Expression.Equal(sourceAccess, Expression.Constant(null, sourceMemberType)),
            Expression.Constant(null, targetMemberType),
            collectionExpr
        );

        if (targetMember is PropertyInfo prop)
        {
            return Expression.Bind(prop, nullCheck);
        }

        return Expression.Bind((FieldInfo)targetMember, nullCheck);
    }

    /// <summary>
    /// 构建基础类型集合映射的核心表达式（如 List<int> -> List<long>）
    /// </summary>
    private static Expression BuildPrimitiveCollectionExpression(Expression sourceAccess,
        Type sourceType, Type targetType, Type sourceElementType, Type targetElementType)
    {
        // 获取元素转换器
        var converter = Convert.ConvertFactory.GetConverter(sourceElementType, targetElementType, ConverterScope.All);
        if (converter == null)
        {
            throw new InvalidOperationException(
                $"未找到 {sourceElementType.Name} -> {targetElementType.Name} 的转换器");
        }

        var converterType = converter.GetType();
        var convertMethod = converterType.GetMethod("Convert");
        if (convertMethod == null)
        {
            throw new InvalidOperationException($"转换器 {converterType.Name} 缺少 Convert 方法");
        }

        // 构建 Select 表达式：source.Select(x => converter.Convert(x))
        var selectMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
            .MakeGenericMethod(sourceElementType, targetElementType);

        var param = Expression.Parameter(sourceElementType, "x");
        var converterInstance = Expression.Constant(converter);
        var convertCall = Expression.Call(converterInstance, convertMethod, param);
        var lambda = Expression.Lambda(convertCall, param);
        var selectCall = Expression.Call(selectMethod, sourceAccess, lambda);

        // 根据目标类型物化
        return CollectionMaterializer.BuildMaterializeExpression(selectCall, targetType, targetElementType);
    }

    /// <summary>
    /// 构建 Dictionary 类型映射的成员绑定表达式
    /// </summary>
    private static MemberBinding BuildDictionaryBinding(MemberInfo targetMember, Expression sourceAccess,
        MemberMapping mapping)
    {
        var sourceMemberType = MemberUtils.GetMemberType(mapping.SourceMember);
        var targetMemberType = MemberUtils.GetMemberType(targetMember);

        var sourceKeyType = mapping.DictionarySourceKeyType!;
        var sourceValueType = mapping.DictionarySourceValueType!;
        var targetKeyType = mapping.DictionaryTargetKeyType!;
        var targetValueType = mapping.DictionaryTargetValueType!;

        var kvpType = typeof(KeyValuePair<,>).MakeGenericType(sourceKeyType, sourceValueType);
        var kvpParam = Expression.Parameter(kvpType, "kvp");

        // Key selector
        Expression keySelectorExpr;
        if (mapping.DictionaryKeyConverter != null)
        {
            var keyProp = Expression.Property(kvpParam, "Key");
            keySelectorExpr = BuildConverterCall(mapping.DictionaryKeyConverter, keyProp, targetKeyType);
        }
        else if (sourceKeyType != targetKeyType)
        {
            keySelectorExpr = Expression.Convert(Expression.Property(kvpParam, "Key"), targetKeyType);
        }
        else
        {
            keySelectorExpr = Expression.Property(kvpParam, "Key");
        }

        // Value selector
        Expression valueSelectorExpr;
        var valueProp = Expression.Property(kvpParam, "Value");

        if (mapping.DictionaryValueConverter != null)
        {
            valueSelectorExpr = BuildConverterCall(mapping.DictionaryValueConverter, valueProp, targetValueType);
        }
        else if (mapping.DictionaryValueNestedPlan != null)
        {
            valueSelectorExpr = BuildComplexValueMapping(valueProp, sourceValueType, targetValueType);
        }
        else if (sourceValueType != targetValueType)
        {
            valueSelectorExpr = Expression.Convert(valueProp, targetValueType);
        }
        else
        {
            valueSelectorExpr = valueProp;
        }

        // ToDictionary(source, keySelector, elementSelector)
        var toDictionaryMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "ToDictionary"
                        && m.GetGenericArguments().Length == 3
                        && m.GetParameters().Length == 3)
            .MakeGenericMethod(kvpType, targetKeyType, targetValueType);

        var keyLambda = Expression.Lambda(keySelectorExpr, kvpParam);
        var valueLambda = Expression.Lambda(valueSelectorExpr, kvpParam);
        var toDictionaryCall = Expression.Call(toDictionaryMethod, sourceAccess, keyLambda, valueLambda);

        Expression resultExpr = toDictionaryCall;
        if (resultExpr.Type != targetMemberType)
        {
            resultExpr = Expression.Convert(resultExpr, targetMemberType);
        }

        var nullCheck = Expression.Condition(
            Expression.Equal(sourceAccess, Expression.Constant(null, sourceMemberType)),
            Expression.Constant(null, targetMemberType),
            resultExpr
        );

        if (targetMember is PropertyInfo prop)
        {
            return Expression.Bind(prop, nullCheck);
        }

        return Expression.Bind((FieldInfo)targetMember, nullCheck);
    }

    /// <summary>
    /// 构建转换器调用表达式
    /// </summary>
    private static Expression BuildConverterCall(object converter, Expression input, Type targetType)
    {
        var converterType = converter.GetType();
        var convertMethod = converterType.GetMethod("Convert")
            ?? throw new InvalidOperationException($"转换器 {converterType.Name} 缺少 Convert 方法");

        var converterInstance = Expression.Constant(converter);
        Expression call = Expression.Call(converterInstance, convertMethod, input);
        if (call.Type != targetType)
        {
            call = Expression.Convert(call, targetType);
        }
        return call;
    }

    /// <summary>
    /// 构建复杂类型 Value 映射表达式（调用 Mapper）
    /// </summary>
    private static Expression BuildComplexValueMapping(Expression sourceAccess, Type sourceValueType, Type targetValueType)
    {
        var mapper = CachedMapperFactory.GetOrCreateMapper(sourceValueType, targetValueType);
        var mapperConstant = Expression.Constant(mapper);
        var mapMethod = mapper.GetType().GetMethod("MapTo")
            ?? throw new InvalidOperationException($"Mapper<{sourceValueType.Name}, {targetValueType.Name}> 缺少 MapTo 方法");

        var mapCall = Expression.Call(mapperConstant, mapMethod, sourceAccess);
        return Expression.Convert(mapCall, targetValueType);
    }
}
