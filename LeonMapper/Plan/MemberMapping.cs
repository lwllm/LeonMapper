using System.Reflection;

namespace LeonMapper.Plan;

/// <summary>
/// 单个成员的映射规则
/// </summary>
public class MemberMapping
{
    /// <summary>源成员</summary>
    public MemberInfo SourceMember { get; }

    /// <summary>目标成员</summary>
    public MemberInfo TargetMember { get; }

    /// <summary>映射策略</summary>
    public MappingStrategy Strategy { get; }

    /// <summary>类型转换器实例（仅 Convert 策略有值）</summary>
    public object? Converter { get; }

    /// <summary>嵌套映射计划（仅 Complex 策略有值）</summary>
    public TypeMappingPlan? NestedPlan { get; }

    public MemberMapping(MemberInfo sourceMember, MemberInfo targetMember, MappingStrategy strategy,
        object? converter = null, TypeMappingPlan? nestedPlan = null)
    {
        SourceMember = sourceMember;
        TargetMember = targetMember;
        Strategy = strategy;
        Converter = converter;
        NestedPlan = nestedPlan;
    }

    public override string ToString()
    {
        var strategyStr = Strategy switch
        {
            MappingStrategy.Convert => $"Convert[{Converter?.GetType().Name}]",
            MappingStrategy.Complex => "Complex",
            MappingStrategy.Collection => "Collection",
            _ => "Direct"
        };
        return $"{SourceMember.Name} -> {TargetMember.Name} ({strategyStr})";
    }
}
