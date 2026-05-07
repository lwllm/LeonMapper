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

    /// <summary>
    /// 忽略此目标成员，不接收任何映射
    /// </summary>
    public void Ignore()
    {
        ActionType = MemberMappingActionType.Ignore;
    }

    /// <summary>
    /// 从指定的源成员映射
    /// </summary>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <param name="sourceExpression">源成员访问表达式，如 s => s.Name</param>
    public void MapFrom<TProperty>(Expression<Func<TSource, TProperty>> sourceExpression)
    {
        if (sourceExpression.Body is not MemberExpression memberExpr)
        {
            throw new ArgumentException(
                "MapFrom 表达式必须是简单的成员访问，例如 s => s.PropertyName",
                nameof(sourceExpression));
        }

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
}
