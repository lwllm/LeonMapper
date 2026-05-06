using System.Collections.Concurrent;
using System.Reflection;
using LeonMapper.Attributes;
using LeonMapper.Convert;
using LeonMapper.Utils;

namespace LeonMapper.Plan.Builder;

/// <summary>
/// 映射计划构建器：从源类型和目标类型生成 TypeMappingPlan
/// </summary>
public static class MappingPlanBuilder
{
    private static readonly ConcurrentDictionary<(Type, Type, int), TypeMappingPlan> _planCache = new();

    /// <summary>
    /// 构建映射计划（泛型入口）
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TTarget">目标类型</typeparam>
    /// <param name="options">构建选项，为 null 时使用默认配置</param>
    /// <returns>类型映射计划</returns>
    public static TypeMappingPlan Build<TSource, TTarget>(PlanBuildOptions? options = null)
    {
        return Build(typeof(TSource), typeof(TTarget), options);
    }

    /// <summary>
    /// 构建映射计划（非泛型入口，带缓存）
    /// </summary>
    /// <param name="sourceType">源类型</param>
    /// <param name="targetType">目标类型</param>
    /// <param name="options">构建选项，为 null 时使用默认配置</param>
    /// <returns>类型映射计划</returns>
    public static TypeMappingPlan Build(Type sourceType, Type targetType, PlanBuildOptions? options = null)
    {
        options ??= PlanBuildOptions.Default;
        var cacheKey = (sourceType, targetType, options.GetHashCode());

        return _planCache.GetOrAdd(cacheKey, _ => BuildCore(sourceType, targetType, options));
    }

    /// <summary>
    /// 核心构建逻辑：发现成员映射关系并生成计划
    /// </summary>
    private static TypeMappingPlan BuildCore(Type sourceType, Type targetType, PlanBuildOptions options)
    {
        // 属性映射
        var propertyPairs = DiscoverPropertyMappings(sourceType, targetType);
        var propertyMappings = new List<MemberMapping>();
        var mappedSourceProps = new HashSet<string>();
        var mappedTargetProps = new HashSet<string>();

        foreach (var (sourceProp, targetProp) in propertyPairs)
        {
            var mapping = CreateMemberMapping(sourceProp, targetProp, options);
            if (mapping != null)
            {
                propertyMappings.Add(mapping);
                mappedSourceProps.Add(sourceProp.Name);
                mappedTargetProps.Add(targetProp.Name);
            }
        }

        // 字段映射
        var fieldPairs = DiscoverFieldMappings(sourceType, targetType);
        var fieldMappings = new List<MemberMapping>();
        var mappedSourceFields = new HashSet<string>();
        var mappedTargetFields = new HashSet<string>();

        foreach (var (sourceField, targetField) in fieldPairs)
        {
            var mapping = CreateMemberMapping(sourceField, targetField, options);
            if (mapping != null)
            {
                fieldMappings.Add(mapping);
                mappedSourceFields.Add(sourceField.Name);
                mappedTargetFields.Add(targetField.Name);
            }
        }

        // 收集未映射的源成员
        var unmappedSource = new List<MemberInfo>();
        var unmappedTarget = new List<MemberInfo>();

        foreach (var prop in sourceType.GetProperties().Where(p => p.CanRead && p.GetIndexParameters().Length == 0))
        {
            if (!mappedSourceProps.Contains(prop.Name))
            {
                unmappedSource.Add(prop);
            }
        }

        // 收集未映射的目标成员
        foreach (var prop in targetType.GetProperties().Where(p => p.CanWrite && p.GetIndexParameters().Length == 0))
        {
            if (!mappedTargetProps.Contains(prop.Name))
            {
                unmappedTarget.Add(prop);
            }
        }

        foreach (var field in sourceType.GetFields())
        {
            if (!mappedSourceFields.Contains(field.Name))
            {
                unmappedSource.Add(field);
            }
        }

        foreach (var field in targetType.GetFields())
        {
            if (!mappedTargetFields.Contains(field.Name))
            {
                unmappedTarget.Add(field);
            }
        }

        return new TypeMappingPlan(sourceType, targetType, propertyMappings, fieldMappings,
            unmappedSource, unmappedTarget);
    }

    /// <summary>
    /// 发现属性映射关系：先处理 MapTo 注解，再处理 MapFrom 注解（MapFrom 优先级更高）
    /// </summary>
    private static List<(PropertyInfo Source, PropertyInfo Target)> DiscoverPropertyMappings(
        Type sourceType, Type targetType)
    {
        var sourceProps = sourceType.GetProperties()
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToDictionary(p => p.Name, p => p);
        var targetProps = targetType.GetProperties()
            .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0)
            .ToDictionary(p => p.Name, p => p);
        var result = new List<(PropertyInfo Source, PropertyInfo Target)>();
        var mappedTargets = new HashSet<string>();

        // Pass 1: MapTo 属性
        foreach (var (_, sourceProp) in sourceProps)
        {
            if (ShouldIgnoreSource(sourceProp))
            {
                continue;
            }

            var mapToAttrs = sourceProp.GetCustomAttributes<MapToAttribute>();
            if (mapToAttrs.Any())
            {
                // 如果设置了 MapTo 注解，则只映射到 MapTo 指定的属性（支持多个）
                foreach (var attr in mapToAttrs)
                {
                    if (targetProps.TryGetValue(attr.MapToName, out var targetProp)
                        && !mappedTargets.Contains(attr.MapToName)
                        && !ShouldIgnoreTarget(targetProp))
                    {
                        result.Add((sourceProp, targetProp));
                        mappedTargets.Add(attr.MapToName);
                    }
                }
            }
            else
            {
                // 无 MapTo 注解时按同名匹配
                if (targetProps.TryGetValue(sourceProp.Name, out var targetProp)
                    && !mappedTargets.Contains(sourceProp.Name)
                    && !ShouldIgnoreTarget(targetProp))
                {
                    result.Add((sourceProp, targetProp));
                    mappedTargets.Add(sourceProp.Name);
                }
            }
        }

        // Pass 2: MapFrom 属性（优先级高于 MapTo，可覆盖已有映射）
        foreach (var (_, targetProp) in targetProps)
        {
            if (ShouldIgnoreTarget(targetProp))
            {
                continue;
            }

            var mapFromAttr = targetProp.GetCustomAttributes<MapFromAttribute>().FirstOrDefault();
            if (mapFromAttr != null && sourceProps.TryGetValue(mapFromAttr.MapFromName, out var sourceProp2))
            {
                if (!ShouldIgnoreSource(sourceProp2))
                {
                    result.RemoveAll(p => p.Target.Name == targetProp.Name);
                    result.Add((sourceProp2, targetProp));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 发现字段映射关系：仅按名称匹配
    /// </summary>
    private static List<(FieldInfo Source, FieldInfo Target)> DiscoverFieldMappings(
        Type sourceType, Type targetType)
    {
        var sourceFields = sourceType.GetFields().ToDictionary(f => f.Name, f => f);
        var targetFields = targetType.GetFields().ToDictionary(f => f.Name, f => f);
        var result = new List<(FieldInfo Source, FieldInfo Target)>();

        foreach (var (name, sourceField) in sourceFields)
        {
            if (ShouldIgnoreSource(sourceField))
            {
                continue;
            }

            if (targetFields.TryGetValue(name, out var targetField) && !ShouldIgnoreTarget(targetField))
            {
                result.Add((sourceField, targetField));
            }
        }

        return result;
    }

    /// <summary>
    /// 根据源成员和目标成员的类型关系创建映射规则
    /// </summary>
    private static MemberMapping? CreateMemberMapping(MemberInfo sourceMember, MemberInfo targetMember,
        PlanBuildOptions options)
    {
        var sourceType = MemberUtils.GetMemberType(sourceMember);
        var targetType = MemberUtils.GetMemberType(targetMember);

        // 处理可空类型：获取底层类型进行判断
        var sourceUnderlyingType = TypeUtils.GetUnderlyingType(sourceType);
        var targetUnderlyingType = TypeUtils.GetUnderlyingType(targetType);

        // 同类型直接赋值（包括可空类型与非可空版本的同底层类型）
        if (sourceType == targetType || sourceUnderlyingType == targetUnderlyingType)
        {
            return new MemberMapping(sourceMember, targetMember, MappingStrategy.Direct);
        }

        // 两个都是 Enum 类型，按名称映射
        if (sourceUnderlyingType.IsEnum && targetUnderlyingType.IsEnum)
        {
            return new MemberMapping(sourceMember, targetMember, MappingStrategy.Convert,
                converter: CreateEnumConverter(sourceUnderlyingType, targetUnderlyingType));
        }

        // Enum 与基础类型之间的转换
        if (sourceUnderlyingType.IsEnum && TypeUtils.IsBaseType(targetUnderlyingType))
        {
            return new MemberMapping(sourceMember, targetMember, MappingStrategy.Convert,
                converter: CreateEnumToBaseTypeConverter(sourceUnderlyingType, targetUnderlyingType));
        }

        if (TypeUtils.IsBaseType(sourceUnderlyingType) && targetUnderlyingType.IsEnum)
        {
            return new MemberMapping(sourceMember, targetMember, MappingStrategy.Convert,
                converter: CreateBaseTypeToEnumConverter(sourceUnderlyingType, targetUnderlyingType));
        }

        // 两个都是基础类型（或可空基础类型），尝试类型转换
        if (TypeUtils.IsBaseType(sourceUnderlyingType) && TypeUtils.IsBaseType(targetUnderlyingType))
        {
            if (options.AutoConvert)
            {
                // 查找底层类型的转换器
                var converter = ConvertFactory.GetConverter(sourceUnderlyingType, targetUnderlyingType, options.ConverterScope);
                if (converter != null)
                {
                    return new MemberMapping(sourceMember, targetMember, MappingStrategy.Convert, converter);
                }
            }

            return null;
        }

        // 两个都是复杂类型（或可空复杂类型），递归构建嵌套映射计划
        if (!TypeUtils.IsBaseType(sourceUnderlyingType) && !TypeUtils.IsBaseType(targetUnderlyingType))
        {
            // 检查是否为 Dictionary 类型（优先级高于 Collection，因为 Dictionary 也实现了 IEnumerable）
            var sourceDictTypes = TypeUtils.GetDictionaryKeyValueTypes(sourceType);
            var targetDictTypes = TypeUtils.GetDictionaryKeyValueTypes(targetType);

            if (sourceDictTypes != null && targetDictTypes != null)
            {
                return CreateDictionaryMapping(sourceMember, targetMember,
                    sourceDictTypes.Value.KeyType, sourceDictTypes.Value.ValueType,
                    targetDictTypes.Value.KeyType, targetDictTypes.Value.ValueType,
                    options);
            }

            // 检查是否为集合类型
            var sourceElementType = TypeUtils.GetCollectionElementType(sourceType);
            var targetElementType = TypeUtils.GetCollectionElementType(targetType);

            if (sourceElementType != null && targetElementType != null)
            {
                // 集合类型映射
                var elementMapping = CreateMemberMapping(
                    CreatePseudoMember(sourceElementType),
                    CreatePseudoMember(targetElementType),
                    options);

                if (elementMapping != null)
                {
                    return new MemberMapping(sourceMember, targetMember, MappingStrategy.Collection,
                        nestedPlan: elementMapping.NestedPlan);
                }

                return null;
            }

            var nestedPlan = options.BuildNestedPlans ? Build(sourceUnderlyingType, targetUnderlyingType, options) : null;
            return new MemberMapping(sourceMember, targetMember, MappingStrategy.Complex, nestedPlan: nestedPlan);
        }

        // 一个基础类型一个复杂类型，不映射
        return null;
    }

    /// <summary>
    /// 创建 Enum 到 Enum 的转换器（按名称映射）
    /// </summary>
    private static object CreateEnumConverter(Type sourceEnumType, Type targetEnumType)
    {
        var converterType = typeof(Convert.Converters.EnumConverter<,>).MakeGenericType(sourceEnumType, targetEnumType);
        return Activator.CreateInstance(converterType)!;
    }

    /// <summary>
    /// 创建 Enum 到基础类型的转换器
    /// </summary>
    private static object CreateEnumToBaseTypeConverter(Type enumType, Type targetType)
    {
        var converterType = typeof(Convert.Converters.EnumToBaseTypeConverter<,>).MakeGenericType(enumType, targetType);
        return Activator.CreateInstance(converterType)!;
    }

    /// <summary>
    /// 创建基础类型到 Enum 的转换器
    /// </summary>
    private static object CreateBaseTypeToEnumConverter(Type sourceType, Type enumType)
    {
        if (sourceType == typeof(string))
        {
            var converterType = typeof(Convert.Converters.StringToEnumConverter<>).MakeGenericType(enumType);
            return Activator.CreateInstance(converterType)!;
        }

        var converterType2 = typeof(Convert.Converters.BaseTypeToEnumConverter<,>).MakeGenericType(sourceType, enumType);
        return Activator.CreateInstance(converterType2)!;
    }

    /// <summary>
    /// 创建 Dictionary 类型的映射规则
    /// </summary>
    private static MemberMapping? CreateDictionaryMapping(MemberInfo sourceMember, MemberInfo targetMember,
        Type sourceKeyType, Type sourceValueType,
        Type targetKeyType, Type targetValueType,
        PlanBuildOptions options)
    {
        object? keyConverter = null;
        object? valueConverter = null;
        TypeMappingPlan? valueNestedPlan = null;

        // 处理 Key 类型 — Key 类型必须精确匹配或可转换，不依赖 AutoConvert
        if (sourceKeyType != targetKeyType)
        {
            var sourceKeyUnderlying = TypeUtils.GetUnderlyingType(sourceKeyType);
            var targetKeyUnderlying = TypeUtils.GetUnderlyingType(targetKeyType);

            if (TypeUtils.IsBaseType(sourceKeyUnderlying) && TypeUtils.IsBaseType(targetKeyUnderlying))
            {
                keyConverter = ConvertFactory.GetConverter(sourceKeyUnderlying, targetKeyUnderlying, options.ConverterScope);
                if (keyConverter == null)
                {
                    return null; // Key 类型不同且无转换器，无法映射
                }
            }
            else
            {
                return null; // 不支持的 Key 类型转换
            }
        }

        // 处理 Value 类型
        if (sourceValueType != targetValueType)
        {
            var sourceValueUnderlying = TypeUtils.GetUnderlyingType(sourceValueType);
            var targetValueUnderlying = TypeUtils.GetUnderlyingType(targetValueType);

            if (TypeUtils.IsBaseType(sourceValueUnderlying) && TypeUtils.IsBaseType(targetValueUnderlying))
            {
                if (options.AutoConvert)
                {
                    valueConverter = ConvertFactory.GetConverter(sourceValueUnderlying, targetValueUnderlying, options.ConverterScope);
                    if (valueConverter == null)
                    {
                        return null;
                    }
                }
            }
            else
            {
                // 复杂 Value 类型，递归构建嵌套映射
                if (options.BuildNestedPlans)
                {
                    valueNestedPlan = Build(sourceValueUnderlying, targetValueUnderlying, options);
                }
            }
        }

        return new MemberMapping(sourceMember, targetMember, MappingStrategy.Dictionary,
            sourceKeyType, sourceValueType, targetKeyType, targetValueType,
            keyConverter, valueConverter, valueNestedPlan);
    }

    /// <summary>
    /// 判断源成员是否应该被忽略（标注了 IgnoreMap 或 IgnoreMapTo）
    /// </summary>
    private static bool ShouldIgnoreSource(MemberInfo member)
    {
        return member.GetCustomAttributes<IgnoreMapAttribute>().Any()
               || member.GetCustomAttributes<IgnoreMapToAttribute>().Any();
    }

    /// <summary>
    /// 判断目标成员是否应该被忽略（标注了 IgnoreMap 或 IgnoreMapFrom）
    /// </summary>
    private static bool ShouldIgnoreTarget(MemberInfo member)
    {
        return member.GetCustomAttributes<IgnoreMapAttribute>().Any()
               || member.GetCustomAttributes<IgnoreMapFromAttribute>().Any();
    }

    /// <summary>
    /// 创建伪成员信息用于集合元素映射
    /// </summary>
    private static MemberInfo CreatePseudoMember(Type elementType)
    {
        return new PseudoMemberInfo(elementType);
    }

    /// <summary>
    /// 伪成员信息类，用于集合元素映射
    /// </summary>
    private class PseudoMemberInfo : MemberInfo
    {
        private readonly Type _elementType;

        public PseudoMemberInfo(Type elementType)
        {
            _elementType = elementType;
        }

        public override Type DeclaringType => _elementType;
        public override MemberTypes MemberType => MemberTypes.Custom;
        public override string Name => "Element";
        public override Type ReflectedType => _elementType;

        public override object[] GetCustomAttributes(bool inherit)
        {
            return Array.Empty<object>();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return Array.Empty<object>();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }
    }
}
