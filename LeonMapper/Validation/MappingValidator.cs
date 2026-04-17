using System.Reflection;
using LeonMapper.Plan;

namespace LeonMapper.Validation;

/// <summary>
/// 映射计划验证器：检测未映射字段、类型转换可行性等问题
/// </summary>
public static class MappingValidator
{
    /// <summary>
    /// 验证映射计划
    /// </summary>
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
                warnings.Add($"目标成员 {member.Name} 未被映射（将使用默认值）");
            else
                warnings.Add($"目标成员 {member.Name} 未被映射（引用类型，默认为 null）");
        }

        return new ValidationResult(errors, warnings);
    }

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
                    warnings.Add(
                        $"{mapping.SourceMember.Name} -> {mapping.TargetMember.Name}: 复杂类型映射未构建嵌套计划");
                break;
        }
    }
}
