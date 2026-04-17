using System.Reflection;
using LeonMapper.Attributes;
using LeonMapper.Convert;

namespace LeonMapper.Plan.Builder;

/// <summary>
/// 映射计划构建器：从源类型和目标类型生成 TypeMappingPlan
/// </summary>
public static class MappingPlanBuilder
{
    private static readonly Dictionary<(Type, Type), TypeMappingPlan> _planCache = new();
    private static readonly object _lock = new();

    /// <summary>
    /// 构建映射计划
    /// </summary>
    public static TypeMappingPlan Build<TSource, TTarget>(PlanBuildOptions? options = null)
    {
        return Build(typeof(TSource), typeof(TTarget), options);
    }

    /// <summary>
    /// 构建映射计划
    /// </summary>
    public static TypeMappingPlan Build(Type sourceType, Type targetType, PlanBuildOptions? options = null)
    {
        options ??= PlanBuildOptions.Default;
        var cacheKey = (sourceType, targetType);

        lock (_lock)
        {
            if (_planCache.TryGetValue(cacheKey, out var cached))
                return cached;
        }

        var plan = BuildCore(sourceType, targetType, options);

        lock (_lock)
        {
            _planCache[cacheKey] = plan;
        }

        return plan;
    }

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

        // 未映射成员
        var unmappedSource = new List<MemberInfo>();
        var unmappedTarget = new List<MemberInfo>();

        foreach (var prop in sourceType.GetProperties().Where(p => p.CanRead))
            if (!mappedSourceProps.Contains(prop.Name))
                unmappedSource.Add(prop);

        foreach (var prop in targetType.GetProperties().Where(p => p.CanWrite))
            if (!mappedTargetProps.Contains(prop.Name))
                unmappedTarget.Add(prop);

        foreach (var field in sourceType.GetFields())
            if (!mappedSourceFields.Contains(field.Name))
                unmappedSource.Add(field);

        foreach (var field in targetType.GetFields())
            if (!mappedTargetFields.Contains(field.Name))
                unmappedTarget.Add(field);

        return new TypeMappingPlan(sourceType, targetType, propertyMappings, fieldMappings,
            unmappedSource, unmappedTarget);
    }

    private static List<(PropertyInfo Source, PropertyInfo Target)> DiscoverPropertyMappings(
        Type sourceType, Type targetType)
    {
        var sourceProps = sourceType.GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);
        var targetProps = targetType.GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
        var result = new List<(PropertyInfo Source, PropertyInfo Target)>();
        var mappedTargets = new HashSet<string>();

        // Pass 1: MapTo 属性
        foreach (var (_, sourceProp) in sourceProps)
        {
            if (ShouldIgnoreSource(sourceProp))
                continue;

            var mapToAttrs = sourceProp.GetCustomAttributes<MapToAttribute>();
            if (mapToAttrs.Any())
            {
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
                if (targetProps.TryGetValue(sourceProp.Name, out var targetProp)
                    && !mappedTargets.Contains(sourceProp.Name)
                    && !ShouldIgnoreTarget(targetProp))
                {
                    result.Add((sourceProp, targetProp));
                    mappedTargets.Add(sourceProp.Name);
                }
            }
        }

        // Pass 2: MapFrom 属性（优先级高于 MapTo，可覆盖）
        var mappedTargets2 = new HashSet<string>();
        foreach (var (_, targetProp) in targetProps)
        {
            if (ShouldIgnoreTarget(targetProp))
                continue;

            var mapFromAttr = targetProp.GetCustomAttributes<MapFromAttribute>().FirstOrDefault();
            if (mapFromAttr != null && sourceProps.TryGetValue(mapFromAttr.MapFromName, out var sourceProp2))
            {
                if (!ShouldIgnoreSource(sourceProp2))
                {
                    // 移除之前的同名目标映射（如果存在）
                    result.RemoveAll(p => p.Target.Name == targetProp.Name);
                    result.Add((sourceProp2, targetProp));
                    mappedTargets2.Add(targetProp.Name);
                }
            }
        }

        return result;
    }

    private static List<(FieldInfo Source, FieldInfo Target)> DiscoverFieldMappings(
        Type sourceType, Type targetType)
    {
        var sourceFields = sourceType.GetFields().ToDictionary(f => f.Name, f => f);
        var targetFields = targetType.GetFields().ToDictionary(f => f.Name, f => f);
        var result = new List<(FieldInfo Source, FieldInfo Target)>();

        foreach (var (name, sourceField) in sourceFields)
        {
            if (ShouldIgnoreSource(sourceField))
                continue;

            if (targetFields.TryGetValue(name, out var targetField) && !ShouldIgnoreTarget(targetField))
            {
                result.Add((sourceField, targetField));
            }
        }

        return result;
    }

    private static MemberMapping? CreateMemberMapping(MemberInfo sourceMember, MemberInfo targetMember,
        PlanBuildOptions options)
    {
        var sourceType = GetMemberType(sourceMember);
        var targetType = GetMemberType(targetMember);

        if (sourceType == targetType)
        {
            return new MemberMapping(sourceMember, targetMember, MappingStrategy.Direct);
        }

        if (IsBaseType(sourceType) && IsBaseType(targetType))
        {
            if (options.AutoConvert)
            {
                var converter = ConvertFactory.GetConverter(sourceType, targetType, options.ConverterScope);
                if (converter != null)
                {
                    return new MemberMapping(sourceMember, targetMember, MappingStrategy.Convert, converter);
                }
            }

            return null;
        }

        if (!IsBaseType(sourceType) && !IsBaseType(targetType))
        {
            var nestedPlan = options.BuildNestedPlans ? Build(sourceType, targetType, options) : null;
            return new MemberMapping(sourceMember, targetMember, MappingStrategy.Complex, nestedPlan: nestedPlan);
        }

        return null;
    }

    private static bool ShouldIgnoreSource(MemberInfo member)
    {
        return member.GetCustomAttributes<IgnoreMapAttribute>().Any()
               || member.GetCustomAttributes<IgnoreMapToAttribute>().Any();
    }

    private static bool ShouldIgnoreTarget(MemberInfo member)
    {
        return member.GetCustomAttributes<IgnoreMapAttribute>().Any()
               || member.GetCustomAttributes<IgnoreMapFromAttribute>().Any();
    }

    private static bool IsBaseType(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
    }

    private static Type GetMemberType(MemberInfo member)
    {
        return member switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => throw new ArgumentException($"不支持的成员类型: {member.MemberType}")
        };
    }
}
