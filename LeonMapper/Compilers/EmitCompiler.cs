using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using LeonMapper.Plan;
using LeonMapper.Exceptions;
using LeonMapper.Utils;

namespace LeonMapper.Compilers;

/// <summary>
/// IL 编译器：从 TypeMappingPlan 通过 IL Emit 生成映射委托
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TTarget">目标类型</typeparam>
public class EmitCompiler<TSource, TTarget> : ICompiler<TSource, TTarget> where TTarget : class
{
    private readonly Func<TSource, TTarget?> _compiledFunc;

    /// <summary>
    /// 从映射计划构建 IL 并编译为委托
    /// </summary>
    /// <param name="plan">类型映射计划</param>
    public EmitCompiler(TypeMappingPlan plan)
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
    /// 构建 DynamicMethod 并生成 IL 映射代码
    /// </summary>
    private static Func<TSource, TTarget?> BuildFunc(TypeMappingPlan plan)
    {
        var methodName =
            $"Map_{plan.SourceType.FullName?.Replace(".", "_")}__to__{plan.TargetType.FullName?.Replace(".", "_")}_{Guid.NewGuid():N}";
        var method = new DynamicMethod(methodName, plan.TargetType, new[] { typeof(TSource) }, true);
        var il = method.GetILGenerator();

        var outCtor = plan.TargetType.GetConstructor(Type.EmptyTypes);
        if (outCtor == null)
        {
            throw new NoEmptyConstructorException();
        }

        var targetLocal = il.DeclareLocal(plan.TargetType);

        // new TTarget()
        il.Emit(OpCodes.Newobj, outCtor);
        il.Emit(OpCodes.Stloc, targetLocal);

        // 属性映射
        foreach (var mapping in plan.PropertyMappings)
        {
            EmitPropertyCopy(il, targetLocal, mapping);
        }

        // 字段映射
        foreach (var mapping in plan.FieldMappings)
        {
            EmitFieldCopy(il, targetLocal, mapping);
        }

        il.Emit(OpCodes.Ldloc, targetLocal);
        il.Emit(OpCodes.Ret);

        return (Func<TSource, TTarget?>)method.CreateDelegate(typeof(Func<TSource, TTarget?>));
    }

    /// <summary>
    /// 生成属性拷贝的 IL 指令
    /// </summary>
    private static void EmitPropertyCopy(ILGenerator il, LocalBuilder targetLocal, MemberMapping mapping)
    {
        var sourceProp = (PropertyInfo)mapping.SourceMember;
        var targetProp = (PropertyInfo)mapping.TargetMember;
        var getMethod = sourceProp.GetGetMethod(true);
        var setMethod = targetProp.GetSetMethod(true);

        if (getMethod == null || setMethod == null)
        {
            return;
        }

        EmitMemberCopy(il, targetLocal, mapping, sourceProp.PropertyType, targetProp.PropertyType,
            () => il.Emit(OpCodes.Ldarg_0),
            () => il.Emit(getMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getMethod),
            () => il.Emit(setMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setMethod));
    }

    /// <summary>
    /// 生成字段拷贝的 IL 指令
    /// </summary>
    private static void EmitFieldCopy(ILGenerator il, LocalBuilder targetLocal, MemberMapping mapping)
    {
        var sourceField = (FieldInfo)mapping.SourceMember;
        var targetField = (FieldInfo)mapping.TargetMember;

        EmitMemberCopy(il, targetLocal, mapping, sourceField.FieldType, targetField.FieldType,
            () => il.Emit(OpCodes.Ldarg_0),
            () => il.Emit(OpCodes.Ldfld, sourceField),
            () => il.Emit(OpCodes.Stfld, targetField));
    }

    /// <summary>
    /// 生成成员拷贝的 IL 指令（属性和字段的公共逻辑）
    /// </summary>
    private static void EmitMemberCopy(ILGenerator il, LocalBuilder targetLocal, MemberMapping mapping,
        Type sourceType, Type targetType, Action emitLoadSource, Action emitGetValue, Action emitSetValue)
    {
        switch (mapping.Strategy)
        {
            case MappingStrategy.Direct:
                EmitDirectBinding(il, targetLocal, sourceType, targetType, emitLoadSource, emitGetValue, emitSetValue);
                break;

            case MappingStrategy.Convert:
                EmitConvertBinding(il, targetLocal, mapping, sourceType, targetType, emitLoadSource, emitGetValue, emitSetValue);
                break;

            case MappingStrategy.Complex:
            {
                var mapMethod = typeof(DelegateInvoker)
                    .GetMethod(nameof(DelegateInvoker.MapComplex))
                    ?.MakeGenericMethod(sourceType, targetType);
                if (mapMethod == null)
                {
                    throw new InvalidOperationException("未找到 DelegateInvoker.MapComplex 方法");
                }

                il.Emit(OpCodes.Ldloc, targetLocal);
                emitLoadSource();
                emitGetValue();
                il.Emit(OpCodes.Call, mapMethod);
                emitSetValue();
                break;
            }

            case MappingStrategy.Collection:
            {
                var sourceElementType = TypeUtils.GetCollectionElementType(sourceType);
                var targetElementType = TypeUtils.GetCollectionElementType(targetType);
                if (sourceElementType == null || targetElementType == null)
                {
                    throw new InvalidOperationException($"成员 {mapping.SourceMember.Name} 或 {mapping.TargetMember.Name} 不是有效的集合类型");
                }

                var mapMethod = typeof(DelegateInvoker)
                    .GetMethod(nameof(DelegateInvoker.MapCollection))
                    ?.MakeGenericMethod(sourceElementType, targetElementType, sourceType, targetType);
                if (mapMethod == null)
                {
                    throw new InvalidOperationException("未找到 DelegateInvoker.MapCollection 方法");
                }

                il.Emit(OpCodes.Ldloc, targetLocal);
                emitLoadSource();
                emitGetValue();
                il.Emit(OpCodes.Call, mapMethod);
                emitSetValue();
                break;
            }
        }
    }

    /// <summary>
    /// 为转换器创建指定类型的 Func 委托
    /// </summary>
    private static Delegate CreateConverterFunc(Type inputType, Type outputType, object converter)
    {
        var convertMethod = converter.GetType().GetMethod("Convert");
        if (convertMethod == null)
        {
            throw new InvalidOperationException($"转换器 {converter.GetType().Name} 缺少 Convert 方法");
        }

        var funcType = typeof(Func<,>).MakeGenericType(inputType, outputType);
        return Delegate.CreateDelegate(funcType, converter, convertMethod);
    }

    /// <summary>
    /// 生成类型转换的 IL 指令（支持可空类型）
    /// </summary>
    private static void EmitConvertBinding(ILGenerator il, LocalBuilder targetLocal, MemberMapping mapping,
        Type sourceType, Type targetType, Action emitLoadSource, Action emitGetValue, Action emitSetValue)
    {
        if (mapping.Converter == null)
        {
            throw new InvalidOperationException("转换策略缺少转换器实例");
        }

        var sourceUnderlyingType = TypeUtils.GetUnderlyingType(sourceType);
        var targetUnderlyingType = TypeUtils.GetUnderlyingType(targetType);
        var isNullableMapping = TypeUtils.IsNullableType(sourceType) || TypeUtils.IsNullableType(targetType);

        if (isNullableMapping)
        {
            var invokeMethod = typeof(DelegateInvoker)
                .GetMethod(nameof(DelegateInvoker.InvokeNullableConverter))
                ?.MakeGenericMethod(sourceUnderlyingType, targetUnderlyingType);
            if (invokeMethod == null)
            {
                throw new InvalidOperationException("未找到 DelegateInvoker.InvokeNullableConverter 方法");
            }

            var converterFunc = CreateConverterFunc(sourceUnderlyingType, targetUnderlyingType, mapping.Converter);
            DelegateInvoker.RegisterConverter(converterFunc);
            var funcId = DelegateInvoker.GetId(converterFunc);

            il.Emit(OpCodes.Ldloc, targetLocal);
            il.Emit(OpCodes.Ldc_I4, funcId);
            emitLoadSource();
            emitGetValue();
            il.Emit(OpCodes.Call, invokeMethod);
            emitSetValue();
        }
        else
        {
            var invokeMethod = typeof(DelegateInvoker)
                .GetMethod(nameof(DelegateInvoker.InvokeConverter))
                ?.MakeGenericMethod(sourceType, targetType);
            if (invokeMethod == null)
            {
                throw new InvalidOperationException("未找到 DelegateInvoker.InvokeConverter 方法");
            }

            var converterFunc = CreateConverterFunc(sourceType, targetType, mapping.Converter);
            DelegateInvoker.RegisterConverter(converterFunc);
            var funcId = DelegateInvoker.GetId(converterFunc);

            il.Emit(OpCodes.Ldloc, targetLocal);
            il.Emit(OpCodes.Ldc_I4, funcId);
            emitLoadSource();
            emitGetValue();
            il.Emit(OpCodes.Call, invokeMethod);
            emitSetValue();
        }
    }

    /// <summary>
    /// 生成直接赋值的 IL 指令（支持可空类型自动转换）
    /// </summary>
    private static void EmitDirectBinding(ILGenerator il, LocalBuilder targetLocal, Type sourceType, Type targetType,
        Action emitLoadSource, Action emitGetValue, Action emitSetValue)
    {
        var isSourceNullable = TypeUtils.IsNullableType(sourceType);
        var isTargetNullable = TypeUtils.IsNullableType(targetType);
        var sourceUnderlyingType = TypeUtils.GetUnderlyingType(sourceType);
        var targetUnderlyingType = TypeUtils.GetUnderlyingType(targetType);

        // 处理可空类型到非可空类型：source.HasValue ? source.Value : default(T)
        if (isSourceNullable && !isTargetNullable)
        {
            var hasValueProp = sourceType.GetProperty("HasValue")!;
            var valueProp = sourceType.GetProperty("Value")!;
            var hasValueGetter = hasValueProp.GetGetMethod()!;
            var valueGetter = valueProp.GetGetMethod()!;

            // 需要临时变量保存可空类型值
            var nullableLocal = il.DeclareLocal(sourceType);

            // 获取值并保存到临时变量
            emitLoadSource();
            emitGetValue();
            il.Emit(OpCodes.Stloc, nullableLocal);

            // 加载目标对象到栈（用于 setter）
            il.Emit(OpCodes.Ldloc, targetLocal);

            // 判断 HasValue，条件加载值
            il.Emit(OpCodes.Ldloca, nullableLocal);
            il.Emit(OpCodes.Call, hasValueGetter);

            var valueLabel = il.DefineLabel();
            var endLabel = il.DefineLabel();

            il.Emit(OpCodes.Brtrue_S, valueLabel);

            // HasValue == false：加载默认值
            if (targetType == typeof(long) || targetType == typeof(ulong))
            {
                il.Emit(OpCodes.Ldc_I8, 0L);
            }
            else if (targetType == typeof(float))
            {
                il.Emit(OpCodes.Ldc_R4, 0f);
            }
            else if (targetType == typeof(double))
            {
                il.Emit(OpCodes.Ldc_R8, 0.0);
            }
            else if (targetType == typeof(decimal))
            {
                il.Emit(OpCodes.Ldc_I4, 0);
                il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new[] { typeof(int) })!);
            }
            else
            {
                // int, short, byte, bool, char 等都用 ldc.i4 0
                il.Emit(OpCodes.Ldc_I4, 0);
            }
            il.Emit(OpCodes.Br_S, endLabel);

            // HasValue == true：加载 source.Value
            il.MarkLabel(valueLabel);
            il.Emit(OpCodes.Ldloca, nullableLocal);
            il.Emit(OpCodes.Call, valueGetter);
            // 如果需要类型转换
            if (sourceUnderlyingType != targetType)
            {
                EmitNumericConversion(il, sourceUnderlyingType, targetType);
            }

            il.MarkLabel(endLabel);
            emitSetValue();
        }
        // 处理非可空类型到可空类型：直接包装 (new Nullable<T>(value))
        else if (!isSourceNullable && isTargetNullable)
        {
            var nullableCtor = targetType.GetConstructor(new[] { targetUnderlyingType })!;

            il.Emit(OpCodes.Ldloc, targetLocal);
            emitLoadSource();
            emitGetValue();
            // 如果需要类型转换
            if (sourceType != targetUnderlyingType)
            {
                EmitNumericConversion(il, sourceType, targetUnderlyingType);
            }
            il.Emit(OpCodes.Newobj, nullableCtor);
            emitSetValue();
        }
        // 两者都是可空类型，但底层类型不同（如 int? -> long?）
        else if (isSourceNullable && isTargetNullable && sourceUnderlyingType != targetUnderlyingType)
        {
            var hasValueProp = sourceType.GetProperty("HasValue")!;
            var valueProp = sourceType.GetProperty("Value")!;
            var hasValueGetter = hasValueProp.GetGetMethod()!;
            var valueGetter = valueProp.GetGetMethod()!;
            var nullableCtor = targetType.GetConstructor(new[] { targetUnderlyingType })!;

            // 临时变量保存源可空类型值
            var sourceNullableLocal = il.DeclareLocal(sourceType);
            // 临时变量保存目标可空类型值
            var targetNullableLocal = il.DeclareLocal(targetType);

            emitLoadSource();
            emitGetValue();
            il.Emit(OpCodes.Stloc, sourceNullableLocal);

            // 判断 HasValue
            il.Emit(OpCodes.Ldloca, sourceNullableLocal);
            il.Emit(OpCodes.Call, hasValueGetter);

            var hasValueLabel = il.DefineLabel();
            var endLabel = il.DefineLabel();

            il.Emit(OpCodes.Brtrue_S, hasValueLabel);

            // HasValue == false：初始化目标可空类型为 null
            il.Emit(OpCodes.Ldloca, targetNullableLocal);
            il.Emit(OpCodes.Initobj, targetType);
            il.Emit(OpCodes.Br_S, endLabel);

            // HasValue == true：转换并构造新的目标可空类型
            il.MarkLabel(hasValueLabel);
            il.Emit(OpCodes.Ldloca, sourceNullableLocal);
            il.Emit(OpCodes.Call, valueGetter);
            EmitNumericConversion(il, sourceUnderlyingType, targetUnderlyingType);
            il.Emit(OpCodes.Newobj, nullableCtor);
            il.Emit(OpCodes.Stloc, targetNullableLocal);

            il.MarkLabel(endLabel);

            // 设置目标值
            il.Emit(OpCodes.Ldloc, targetLocal);
            il.Emit(OpCodes.Ldloc, targetNullableLocal);
            emitSetValue();
        }
        // 同底层类型的可空/非可空转换（如 int? -> int?）
        else if (sourceType != targetType && sourceUnderlyingType == targetUnderlyingType)
        {
            il.Emit(OpCodes.Ldloc, targetLocal);
            emitLoadSource();
            emitGetValue();
            emitSetValue();
        }
        // 不同类型（如 int -> long）
        else if (sourceType != targetType)
        {
            il.Emit(OpCodes.Ldloc, targetLocal);
            emitLoadSource();
            emitGetValue();
            EmitNumericConversion(il, sourceType, targetType);
            emitSetValue();
        }
        else
        {
            // 同类型直接赋值
            il.Emit(OpCodes.Ldloc, targetLocal);
            emitLoadSource();
            emitGetValue();
            emitSetValue();
        }
    }

    /// <summary>
    /// 生成数值类型之间的转换指令
    /// </summary>
    private static void EmitNumericConversion(ILGenerator il, Type fromType, Type toType)
    {
        if (fromType == toType) return;

        // 到 long/ulong
        if (toType == typeof(long) || toType == typeof(ulong))
        {
            il.Emit(OpCodes.Conv_I8);
        }
        // 到 int/uint
        else if (toType == typeof(int) || toType == typeof(uint))
        {
            il.Emit(OpCodes.Conv_I4);
        }
        // 到 short/ushort
        else if (toType == typeof(short) || toType == typeof(ushort))
        {
            il.Emit(OpCodes.Conv_I2);
        }
        // 到 byte/sbyte
        else if (toType == typeof(byte) || toType == typeof(sbyte))
        {
            il.Emit(OpCodes.Conv_I1);
        }
        // 到 float
        else if (toType == typeof(float))
        {
            il.Emit(OpCodes.Conv_R4);
        }
        // 到 double
        else if (toType == typeof(double))
        {
            il.Emit(OpCodes.Conv_R8);
        }
        // 到 decimal - 需要从其他数值类型构造
        else if (toType == typeof(decimal))
        {
            // decimal 没有直接的 IL 转换指令，需要通过构造函数
            // 假设 fromType 是可以隐式转换为 int 的类型
            if (fromType != typeof(int))
            {
                // 先转换为 int
                il.Emit(OpCodes.Conv_I4);
            }
            il.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new[] { typeof(int) })!);
        }
        // IntPtr/UIntPtr 特殊处理
        else if (toType == typeof(IntPtr))
        {
            il.Emit(OpCodes.Conv_I);
        }
        else if (toType == typeof(UIntPtr))
        {
            il.Emit(OpCodes.Conv_U);
        }
    }
}

/// <summary>
/// 委托调用辅助类：供 EmitCompiler 通过静态方法间接调用缓存的委托
/// </summary>
internal static class DelegateInvoker
{
    private static readonly ConcurrentDictionary<int, Delegate> _converters = new();
    private static readonly ConcurrentDictionary<Delegate, int> _converterIds = new();
    private static int _nextId;

    /// <summary>
    /// 注册转换器委托，返回唯一 ID（线程安全）
    /// </summary>
    public static int RegisterConverter(Delegate converterFunc)
    {
        // 先尝试获取已有 ID
        if (_converterIds.TryGetValue(converterFunc, out var existingId))
        {
            return existingId;
        }

        // 使用原子操作确保同一委托只注册一次
        var id = Interlocked.Increment(ref _nextId) - 1;
        if (_converters.TryAdd(id, converterFunc))
        {
            // 尝试添加到反向映射，如果已存在则回滚
            if (!_converterIds.TryAdd(converterFunc, id))
            {
                // 另一个线程已经注册了这个委托，使用已有的 ID
                _converterIds.TryGetValue(converterFunc, out var otherId);
                _converters.TryRemove(id, out _);
                return otherId;
            }
            return id;
        }

        // 如果 TryAdd 失败（理论上不会发生，因为 ID 是唯一的），重试获取
        _converterIds.TryGetValue(converterFunc, out var retryId);
        return retryId;
    }

    /// <summary>
    /// 获取已注册转换器委托的 ID
    /// </summary>
    public static int GetId(Delegate converterFunc)
    {
        if (_converterIds.TryGetValue(converterFunc, out var id))
        {
            return id;
        }

        return -1;
    }

    /// <summary>
    /// 通过 ID 调用缓存的转换器委托（支持可空类型）
    /// </summary>
    public static TOutput InvokeConverter<TInput, TOutput>(int funcId, TInput value)
    {
        if (_converters.TryGetValue(funcId, out var del))
        {
            var func = (Func<TInput, TOutput>)del;
            return func(value);
        }

        throw new InvalidOperationException($"未找到 ID 为 {funcId} 的转换器委托");
    }

    /// <summary>
    /// 通过 ID 调用缓存的转换器委托（支持可空类型解包）
    /// </summary>
    public static TTarget? InvokeNullableConverter<TSource, TTarget>(int funcId, TSource? value)
        where TSource : struct
        where TTarget : struct
    {
        if (_converters.TryGetValue(funcId, out var del))
        {
            var func = (Func<TSource, TTarget>)del;
            if (value.HasValue)
            {
                return func(value.Value);
            }

            return null;
        }

        throw new InvalidOperationException($"未找到 ID 为 {funcId} 的转换器委托");
    }

    /// <summary>
    /// 通过缓存的 Mapper 实例执行复杂类型映射
    /// </summary>
    public static TDestination? MapComplex<TSource, TDestination>(TSource? source) where TDestination : class
    {
        if (source == null)
        {
            return default;
        }

        var mapper = (Mapper<TSource, TDestination>)CachedMapperFactory.GetOrCreateMapper(typeof(TSource), typeof(TDestination));
        return mapper.MapTo(source);
    }

    /// <summary>
    /// 执行集合类型映射，支持 List、Array、IEnumerable 之间的转换
    /// </summary>
    public static TTargetCollection? MapCollection<TSourceElement, TTargetElement, TSourceCollection, TTargetCollection>(
        TSourceCollection? source)
        where TTargetCollection : class
    {
        if (source == null)
        {
            return default;
        }

        var enumerable = (IEnumerable<TSourceElement>)source;
        var isBaseType = typeof(TSourceElement).IsPrimitive || typeof(TSourceElement) == typeof(string) || typeof(TSourceElement) == typeof(decimal);

        IEnumerable<TTargetElement> mapped;

        if (isBaseType)
        {
            // 基础类型转换：使用缓存的强类型委托
            mapped = enumerable.Select(x => CollectionConverterCache<TSourceElement, TTargetElement>.Invoke(x));
        }
        else
        {
            // 复杂类型转换：使用缓存的强类型委托
            mapped = enumerable.Select(x => CollectionMapperCache<TSourceElement, TTargetElement>.Invoke(x));
        }

        return CollectionMaterializer.Materialize<TTargetElement, TTargetCollection>(mapped);
    }
}

/// <summary>
/// 集合元素转换器缓存：缓存基础类型的强类型转换委托，避免每次反射调用
/// </summary>
internal static class CollectionConverterCache<TSource, TTarget>
{
    private static readonly Func<TSource, TTarget> _converterFunc;

    static CollectionConverterCache()
    {
        var converter = Convert.ConvertFactory.GetConverter(typeof(TSource), typeof(TTarget), Config.MapperConfig.GetDefaultConverterScope());
        if (converter == null)
        {
            throw new InvalidOperationException($"未找到 {typeof(TSource).Name} -> {typeof(TTarget).Name} 的转换器");
        }

        var convertMethod = converter.GetType().GetMethod("Convert")
            ?? throw new InvalidOperationException($"转换器 {converter.GetType().Name} 缺少 Convert 方法");

        // 创建强类型委托
        var funcType = typeof(Func<TSource, TTarget>);
        _converterFunc = (Func<TSource, TTarget>)Delegate.CreateDelegate(funcType, converter, convertMethod);
    }

    public static TTarget Invoke(TSource source)
    {
        return _converterFunc(source);
    }
}

/// <summary>
/// 集合元素映射器缓存：缓存复杂类型的强类型映射委托，避免每次反射调用
/// </summary>
internal static class CollectionMapperCache<TSource, TTarget>
{
    private static readonly Func<TSource, TTarget> _mapFunc;

    static CollectionMapperCache()
    {
        var mapper = CachedMapperFactory.GetOrCreateMapper(typeof(TSource), typeof(TTarget));
        var mapToMethod = mapper.GetType().GetMethod("MapTo")
            ?? throw new InvalidOperationException($"Mapper<{typeof(TSource).Name}, {typeof(TTarget).Name}> 缺少 MapTo 方法");

        // 创建强类型委托
        var funcType = typeof(Func<TSource, TTarget>);
        _mapFunc = (Func<TSource, TTarget>)Delegate.CreateDelegate(funcType, mapper, mapToMethod);
    }

    public static TTarget Invoke(TSource source)
    {
        return _mapFunc(source);
    }
}
