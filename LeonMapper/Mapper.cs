using LeonMapper.Compilers;
using LeonMapper.Config;
using LeonMapper.Plan;
using LeonMapper.Plan.Builder;
using LeonMapper.Validation;

namespace LeonMapper;

/// <summary>
/// 对象映射器：将 TSource 映射为 TDestination
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TDestination">目标类型（必须为 class）</typeparam>
public class Mapper<TSource, TDestination> where TDestination : class
{
    private readonly ICompiler<TSource, TDestination?> _compiler;

    /// <summary>
    /// 默认构造函数：自动构建映射计划并使用全局配置的编译策略
    /// </summary>
    public Mapper()
    {
        var plan = MappingPlanBuilder.Build<TSource, TDestination>();
        _compiler = CreateCompiler(plan);
    }

    /// <summary>
    /// 从映射计划创建映射器
    /// </summary>
    public Mapper(TypeMappingPlan plan)
    {
        _compiler = CreateCompiler(plan);
    }

    /// <summary>
    /// 使用指定编译策略创建映射器
    /// </summary>
    public Mapper(ProcessTypeEnum processType)
    {
        var plan = MappingPlanBuilder.Build<TSource, TDestination>();
        _compiler = CreateCompiler(plan, processType);
    }

    /// <summary>
    /// 使用 Fluent API 配置创建映射器
    /// </summary>
    /// <param name="configure">Fluent API 配置委托</param>
    public Mapper(Action<MappingConfiguration<TSource, TDestination>> configure)
    {
        var config = new MappingConfiguration<TSource, TDestination>();
        configure(config);
        var plan = MappingPlanBuilder.Build(config);
        _compiler = CreateCompiler(plan);
    }

    /// <summary>
    /// 执行映射
    /// </summary>
    public TDestination? MapTo(TSource source)
    {
        return _compiler.Map(source);
    }

    /// <summary>
    /// 获取映射计划
    /// </summary>
    public static TypeMappingPlan GetPlan()
    {
        return MappingPlanBuilder.Build<TSource, TDestination>();
    }

    /// <summary>
    /// 验证映射配置
    /// </summary>
    public static ValidationResult Validate()
    {
        var plan = GetPlan();
        return MappingValidator.Validate(plan);
    }

    private static ICompiler<TSource, TDestination?> CreateCompiler(TypeMappingPlan plan, ProcessTypeEnum? processType = null)
    {
        var compilerType = processType ?? MapperConfig.GetDefaultProcessType();
        return compilerType == ProcessTypeEnum.Emit
            ? new EmitCompiler<TSource, TDestination>(plan)
            : new ExpressionCompiler<TSource, TDestination>(plan);
    }
}
