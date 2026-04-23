using System.Reflection;
using LeonMapper.Plan;
using LeonMapper.Utils;

namespace LeonMapper.Validation;

/// <summary>
/// 映射计划验证器：检测未映射字段、类型转换可行性等问题
/// </summary>
public static class MappingValidator
{
    /// <summary>
    /// 验证映射计划的完整性和正确性
    /// </summary>
    /// <param name="plan">待验证的映射计划</param>
    /// <returns>包含错误和警告的验证结果</returns>
    public static ValidationResult Validate(TypeMappingPlan plan)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // 检查目标类型是否有无参构造函数
        if (plan.TargetType.GetConstructor(Type.EmptyTypes) == null)
        {
            errors.Add($"目标类型 {plan.TargetType.Name} 没有无参构造函数");
        }

        // 检查映射规则
        foreach (var mapping in plan.AllMappings)
        {
            ValidateMemberMapping(mapping, errors, warnings);
        }

        // 检查未映射的目标属性（可能有默认值，所以是 warning）
        foreach (var member in plan.UnmappedTargetMembers)
        {
            var hasDefault = member switch
            {
                PropertyInfo p => p.PropertyType.IsValueType || p.PropertyType == typeof(string),
                FieldInfo f => f.FieldType.IsValueType || f.FieldType == typeof(string),
                _ => true
            };
            if (hasDefault)
            {
                warnings.Add($"目标成员 {member.Name} 未被映射（将使用默认值）");
            }
            else
            {
                warnings.Add($"目标成员 {member.Name} 未被映射（引用类型，默认为 null）");
            }
        }

        return new ValidationResult(errors, warnings);
    }

    /// <summary>
    /// 验证单个成员映射规则的正确性
    /// </summary>
    private static void ValidateMemberMapping(MemberMapping mapping, List<string> errors, List<string> warnings)
    {
        switch (mapping.Strategy)
        {
            case MappingStrategy.Convert:
                if (mapping.Converter == null)
                {
                    errors.Add(
                        $"{mapping.SourceMember.Name} -> {mapping.TargetMember.Name}: Convert 策略但无转换器");
                }

                break;

            case MappingStrategy.Complex:
                if (mapping.NestedPlan == null)
                {
                    warnings.Add(
                        $"{mapping.SourceMember.Name} -> {mapping.TargetMember.Name}: 复杂类型映射未构建嵌套计划");
                }

                break;

            case MappingStrategy.Collection:
                ValidateCollectionMapping(mapping, errors, warnings);
                break;
        }
    }

    /// <summary>
    /// 验证集合类型映射规则的正确性
    /// </summary>
    private static void ValidateCollectionMapping(MemberMapping mapping, List<string> errors, List<string> warnings)
    {
        var sourceType = MemberUtils.GetMemberType(mapping.SourceMember);
        var targetType = MemberUtils.GetMemberType(mapping.TargetMember);

        var sourceElementType = TypeUtils.GetCollectionElementType(sourceType);
        var targetElementType = TypeUtils.GetCollectionElementType(targetType);

        if (sourceElementType == null)
        {
            errors.Add($"{mapping.SourceMember.Name}: 不是有效的集合类型");
            return;
        }

        if (targetElementType == null)
        {
            errors.Add($"{mapping.TargetMember.Name}: 不是有效的集合类型");
            return;
        }

        // 验证目标集合类型是否可构造
        if (!IsValidCollectionTargetType(targetType, targetElementType))
        {
            errors.Add($"{mapping.TargetMember.Name}: 不支持的集合目标类型 {targetType.Name}");
        }

        // 验证元素映射是否可行
        if (TypeUtils.IsBaseType(sourceElementType) && TypeUtils.IsBaseType(targetElementType))
        {
            // 基础类型集合：元素映射使用 ConvertFactory 获取转换器，不需要 NestedPlan
            // 转换器在编译时查找，此处不做额外验证
        }
        else if (!TypeUtils.IsBaseType(sourceElementType) && !TypeUtils.IsBaseType(targetElementType))
        {
            // 复杂类型集合：检查是否有嵌套计划
            if (mapping.NestedPlan == null)
            {
                warnings.Add(
                    $"{mapping.SourceMember.Name} -> {mapping.TargetMember.Name}: 复杂类型集合映射未构建元素嵌套计划");
            }
        }
        else
        {
            // 一个基础类型一个复杂类型，无法映射
            errors.Add(
                $"{mapping.SourceMember.Name} -> {mapping.TargetMember.Name}: 集合元素类型不兼容（{sourceElementType.Name} vs {targetElementType.Name}）");
        }
    }

    /// <summary>
    /// 判断目标集合类型是否支持映射
    /// </summary>
    private static bool IsValidCollectionTargetType(Type targetType, Type elementType)
    {
        // 数组类型
        if (targetType.IsArray)
        {
            return true;
        }

        // List<T>
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            return true;
        }

        // IEnumerable<T>
        if (targetType == typeof(IEnumerable<>).MakeGenericType(elementType))
        {
            return true;
        }

        // 其他实现了 IEnumerable<T> 构造函数的类型
        var ctor = targetType.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
        if (ctor != null)
        {
            return true;
        }

        return false;
    }
}
