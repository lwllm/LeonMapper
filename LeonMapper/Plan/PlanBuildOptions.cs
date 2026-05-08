namespace LeonMapper.Plan;

/// <summary>
/// 映射计划构建选项（不可变）
/// </summary>
public class PlanBuildOptions : IEquatable<PlanBuildOptions>
{
    /// <summary>是否自动转换不同基础类型</summary>
    public bool AutoConvert { get; }

    /// <summary>转换器范围</summary>
    public ConverterScope ConverterScope { get; }

    /// <summary>是否构建嵌套复杂类型的映射计划</summary>
    public bool BuildNestedPlans { get; }

    /// <summary>
    /// 默认构建选项
    /// </summary>
    public static PlanBuildOptions Default { get; } = new();

    /// <summary>
    /// 使用默认选项创建
    /// </summary>
    public PlanBuildOptions()
    {
        AutoConvert = true;
        ConverterScope = ConverterScope.Common;
        BuildNestedPlans = true;
    }

    /// <summary>
    /// 使用指定选项创建
    /// </summary>
    public PlanBuildOptions(bool autoConvert, ConverterScope converterScope, bool buildNestedPlans)
    {
        AutoConvert = autoConvert;
        ConverterScope = converterScope;
        BuildNestedPlans = buildNestedPlans;
    }

    /// <summary>
    /// 判断两个 PlanBuildOptions 是否相等
    /// </summary>
    public bool Equals(PlanBuildOptions? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return AutoConvert == other.AutoConvert
               && ConverterScope == other.ConverterScope
               && BuildNestedPlans == other.BuildNestedPlans;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as PlanBuildOptions);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(AutoConvert, ConverterScope, BuildNestedPlans);
    }
}
