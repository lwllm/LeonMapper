using LeonMapper.Compilers;
using LeonMapper.Config;
using LeonMapper.Plan;
using LeonMapper.Plan.Builder;
using LeonMapper.Validation;

namespace LeonMapper;

/// <summary>
/// 对象映射器：将 TIn 映射为 TOut
/// </summary>
/// <typeparam name="TIn">源类型</typeparam>
/// <typeparam name="TOut">目标类型（必须为 class）</typeparam>
public class Mapper<TIn, TOut> where TOut : class
{
    private readonly ICompiler<TIn, TOut?> _compiler;

    /// <summary>
    /// 默认构造函数：自动构建映射计划并使用全局配置的编译策略
    /// </summary>
    public Mapper()
    {
        var plan = MappingPlanBuilder.Build<TIn, TOut>();
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
        var plan = MappingPlanBuilder.Build<TIn, TOut>();
        _compiler = CreateCompiler(plan, processType);
    }

    /// <summary>
    /// 执行映射
    /// </summary>
    public TOut? MapTo(TIn source)
    {
        return _compiler.Map(source);
    }

    /// <summary>
    /// 获取映射计划
    /// </summary>
    public static TypeMappingPlan GetPlan()
    {
        return MappingPlanBuilder.Build<TIn, TOut>();
    }

    /// <summary>
    /// 验证映射配置
    /// </summary>
    public static ValidationResult Validate()
    {
        var plan = GetPlan();
        return MappingValidator.Validate(plan);
    }

    private static ICompiler<TIn, TOut?> CreateCompiler(TypeMappingPlan plan, ProcessTypeEnum? processType = null)
    {
        var compilerType = processType ?? MapperConfig.GetDefaultProcessType();
        return compilerType == ProcessTypeEnum.Emit
            ? new EmitCompiler<TIn, TOut>(plan)
            : new ExpressionCompiler<TIn, TOut>(plan);
    }
}
