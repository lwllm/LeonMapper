namespace LeonMapper.Plan;

/// <summary>
/// 映射计划构建选项
/// </summary>
public class PlanBuildOptions
{
    /// <summary>是否自动转换不同基础类型</summary>
    public bool AutoConvert { get; set; } = true;

    /// <summary>转换器范围</summary>
    public ConverterScope ConverterScope { get; set; } = ConverterScope.Common;

    /// <summary>是否构建嵌套复杂类型的映射计划</summary>
    public bool BuildNestedPlans { get; set; } = true;

    public static PlanBuildOptions Default => new PlanBuildOptions();
}