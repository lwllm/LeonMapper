namespace LeonMapper.Plan;

/// <summary>
/// 映射计划构建选项
/// </summary>
public class PlanBuildOptions : IEquatable<PlanBuildOptions>
{
    /// <summary>是否自动转换不同基础类型</summary>
    public bool AutoConvert { get; set; } = true;

    /// <summary>转换器范围</summary>
    public ConverterScope ConverterScope { get; set; } = ConverterScope.Common;

    /// <summary>是否构建嵌套复杂类型的映射计划</summary>
    public bool BuildNestedPlans { get; set; } = true;

    /// <summary>默认构建选项（静态只读，避免重复分配）</summary>
    public static readonly PlanBuildOptions Default = new();

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