using System.Linq.Expressions;

namespace LeonMapper.Plan.Builder;

/// <summary>
/// 单个目标成员的映射配置选项
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
public class PropertyMappingOption<TSource>
{
    internal MemberMappingActionType ActionType { get; set; }
    internal LambdaExpression? SourceExpression { get; set; }
    internal object? Converter { get; set; }
    internal Type? ConverterType { get; set; }
    internal LambdaExpression? ConditionExpression { get; set; }

    /// <summary>
    /// 忽略此目标成员，不接收任何映射
    /// </summary>
    public void Ignore()
    {
        ActionType = MemberMappingActionType.Ignore;
    }

    /// <summary>
    /// 从指定的源成员映射（支持任意表达式，如 s => s.FirstName + s.LastName）
    /// 注意：表达式的返回值类型必须与目标属性一致或可隐式转换
    /// </summary>
    /// <typeparam name="TProperty">返回值类型</typeparam>
    /// <param name="sourceExpression">源成员访问表达式，如 s => s.Name 或 s => s.FirstName + s.LastName</param>
    public void MapFrom<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression)
    {
        ActionType = MemberMappingActionType.MapFrom;
        SourceExpression = sourceExpression;
    }

    /// <summary>
    /// 使用自定义转换器进行映射
    /// </summary>
    /// <typeparam name="TConverter">转换器类型，必须有无参构造函数</typeparam>
    public void ConvertUsing<TConverter>() where TConverter : new()
    {
        ActionType = MemberMappingActionType.ConvertUsing;
        Converter = new TConverter();
        ConverterType = typeof(TConverter);
    }

    /// <summary>
    /// 设置条件映射：仅当条件满足时才执行映射
    /// </summary>
    /// <param name="predicate">条件表达式，如 src => src.Age > 18</param>
    public void Condition(Expression<Func<TSource, bool>> predicate)
    {
        ConditionExpression = predicate;
    }
}
