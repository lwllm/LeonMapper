using System.Linq.Expressions;
using System.Reflection;

namespace LeonMapper.Plan;

/// <summary>
/// 单个成员的映射规则
/// </summary>
public class MemberMapping
{
    /// <summary>源成员（自定义表达式映射时可为 null）</summary>
    public MemberInfo? SourceMember { get; }

    /// <summary>目标成员</summary>
    public MemberInfo TargetMember { get; }

    /// <summary>映射策略</summary>
    public MappingStrategy Strategy { get; }

    /// <summary>类型转换器实例（仅 Convert 策略有值）</summary>
    public object? Converter { get; }

    /// <summary>嵌套映射计划（仅 Complex 策略有值）</summary>
    public TypeMappingPlan? NestedPlan { get; }

    /// <summary>Dictionary Key 类型（仅 Dictionary 策略有值）</summary>
    public Type? DictionarySourceKeyType { get; }

    /// <summary>Dictionary Value 类型（仅 Dictionary 策略有值）</summary>
    public Type? DictionarySourceValueType { get; }

    /// <summary>Dictionary 目标 Key 类型（仅 Dictionary 策略有值）</summary>
    public Type? DictionaryTargetKeyType { get; }

    /// <summary>Dictionary 目标 Value 类型（仅 Dictionary 策略有值）</summary>
    public Type? DictionaryTargetValueType { get; }

    /// <summary>Key 类型转换器（仅 Dictionary 策略且 Key 类型不同时有值）</summary>
    public object? DictionaryKeyConverter { get; }

    /// <summary>Value 类型转换器（仅 Dictionary 策略且 Value 是基础类型时有值）</summary>
    public object? DictionaryValueConverter { get; }

    /// <summary>Value 嵌套映射计划（仅 Dictionary 策略且 Value 是复杂类型时有值）</summary>
    public TypeMappingPlan? DictionaryValueNestedPlan { get; }

    /// <summary>
    /// 自定义源表达式（用于 Fluent API MapFrom 的非成员表达式映射，如 s => s.A + s.B）
    /// </summary>
    public LambdaExpression? CustomSourceExpression { get; }

    /// <summary>
    /// 条件表达式（用于 Fluent API Condition，映射仅当条件满足时执行）
    /// </summary>
    public LambdaExpression? ConditionExpression { get; }

    /// <summary>
    /// 构造常规映射规则（非 Dictionary 策略）
    /// </summary>
    public MemberMapping(MemberInfo sourceMember, MemberInfo targetMember, MappingStrategy strategy,
        object? converter = null, TypeMappingPlan? nestedPlan = null)
    {
        SourceMember = sourceMember;
        TargetMember = targetMember;
        Strategy = strategy;
        Converter = converter;
        NestedPlan = nestedPlan;
    }

    /// <summary>
    /// 构造自定义表达式映射规则（含 Condition）
    /// </summary>
    public MemberMapping(MemberInfo sourceMember, MemberInfo targetMember, MappingStrategy strategy,
        LambdaExpression? customSourceExpression, LambdaExpression? conditionExpression)
    {
        SourceMember = sourceMember;
        TargetMember = targetMember;
        Strategy = strategy;
        CustomSourceExpression = customSourceExpression;
        ConditionExpression = conditionExpression;
    }

    /// <summary>
    /// 构造完整配置的映射规则（含转换器、嵌套计划、自定义表达式和条件）
    /// </summary>
    public MemberMapping(MemberInfo sourceMember, MemberInfo targetMember, MappingStrategy strategy,
        object? converter, TypeMappingPlan? nestedPlan,
        LambdaExpression? customSourceExpression, LambdaExpression? conditionExpression)
        : this(sourceMember, targetMember, strategy, converter, nestedPlan)
    {
        CustomSourceExpression = customSourceExpression;
        ConditionExpression = conditionExpression;
    }

    /// <summary>
    /// 构造 Dictionary 映射规则
    /// </summary>
    public MemberMapping(MemberInfo sourceMember, MemberInfo targetMember, MappingStrategy strategy,
        Type dictionarySourceKeyType, Type dictionarySourceValueType,
        Type dictionaryTargetKeyType, Type dictionaryTargetValueType,
        object? dictionaryKeyConverter = null, object? dictionaryValueConverter = null,
        TypeMappingPlan? dictionaryValueNestedPlan = null)
    {
        SourceMember = sourceMember;
        TargetMember = targetMember;
        Strategy = strategy;
        DictionarySourceKeyType = dictionarySourceKeyType;
        DictionarySourceValueType = dictionarySourceValueType;
        DictionaryTargetKeyType = dictionaryTargetKeyType;
        DictionaryTargetValueType = dictionaryTargetValueType;
        DictionaryKeyConverter = dictionaryKeyConverter;
        DictionaryValueConverter = dictionaryValueConverter;
        DictionaryValueNestedPlan = dictionaryValueNestedPlan;
    }

    public override string ToString()
    {
        var strategyStr = Strategy switch
        {
            MappingStrategy.Convert => $"Convert[{Converter?.GetType().Name}]",
            MappingStrategy.Complex => "Complex",
            MappingStrategy.Collection => "Collection",
            MappingStrategy.Dictionary => $"Dictionary<{DictionarySourceKeyType?.Name},{DictionarySourceValueType?.Name} -> {DictionaryTargetKeyType?.Name},{DictionaryTargetValueType?.Name}>",
            _ => "Direct"
        };

        if (CustomSourceExpression != null)
        {
            return $"[Expr] -> {TargetMember.Name} ({strategyStr})";
        }

        return $"{SourceMember.Name} -> {TargetMember.Name} ({strategyStr})";
    }
}
