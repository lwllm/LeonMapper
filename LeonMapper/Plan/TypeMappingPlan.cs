using System.Reflection;
using System.Text;

namespace LeonMapper.Plan;

/// <summary>
/// 类型映射计划：源类型到目标类型的完整映射规则
/// </summary>
public class TypeMappingPlan
{
    /// <summary>源类型</summary>
    public Type SourceType { get; }

    /// <summary>目标类型</summary>
    public Type TargetType { get; }

    /// <summary>属性映射规则列表</summary>
    public IReadOnlyList<MemberMapping> PropertyMappings { get; }

    /// <summary>字段映射规则列表</summary>
    public IReadOnlyList<MemberMapping> FieldMappings { get; }

    /// <summary>未映射的源成员</summary>
    public IReadOnlyList<MemberInfo> UnmappedSourceMembers { get; }

    /// <summary>未映射的目标成员</summary>
    public IReadOnlyList<MemberInfo> UnmappedTargetMembers { get; }

    /// <summary>
    /// 构造类型映射计划
    /// </summary>
    public TypeMappingPlan(
        Type sourceType,
        Type targetType,
        IReadOnlyList<MemberMapping> propertyMappings,
        IReadOnlyList<MemberMapping> fieldMappings,
        IReadOnlyList<MemberInfo> unmappedSourceMembers,
        IReadOnlyList<MemberInfo> unmappedTargetMembers)
    {
        SourceType = sourceType;
        TargetType = targetType;
        PropertyMappings = propertyMappings;
        FieldMappings = fieldMappings;
        UnmappedSourceMembers = unmappedSourceMembers;
        UnmappedTargetMembers = unmappedTargetMembers;
    }

    /// <summary>
    /// 获取所有映射规则（属性 + 字段）
    /// </summary>
    public IEnumerable<MemberMapping> AllMappings => PropertyMappings.Concat(FieldMappings);

    /// <summary>
    /// 输出映射计划的可读描述
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Mapping Plan: {SourceType.Name} -> {TargetType.Name}");
        sb.AppendLine("  Properties:");
        foreach (var mapping in PropertyMappings)
        {
            sb.AppendLine($"    {mapping}");
        }

        sb.AppendLine("  Fields:");
        foreach (var mapping in FieldMappings)
        {
            sb.AppendLine($"    {mapping}");
        }

        if (UnmappedSourceMembers.Count > 0)
        {
            sb.AppendLine("  Unmapped Source:");
            foreach (var m in UnmappedSourceMembers)
            {
                sb.AppendLine($"    {m.Name}");
            }
        }

        if (UnmappedTargetMembers.Count > 0)
        {
            sb.AppendLine("  Unmapped Target:");
            foreach (var m in UnmappedTargetMembers)
            {
                sb.AppendLine($"    {m.Name}");
            }
        }

        return sb.ToString();
    }
}
