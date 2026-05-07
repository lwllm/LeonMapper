using System.Linq.Expressions;

namespace LeonMapper.Plan.Builder;

/// <summary>
/// Fluent API 映射配置入口
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TTarget">目标类型</typeparam>
public class MappingConfiguration<TSource, TTarget> where TTarget : class
{
    private readonly List<(string TargetPropertyName, PropertyMappingOption<TSource> Option)> _memberActions = new();

    /// <summary>
    /// 为目标属性配置映射规则
    /// </summary>
    /// <typeparam name="TProperty">目标属性类型</typeparam>
    /// <param name="targetProperty">目标属性访问表达式，如 d =&gt; d.PropertyName</param>
    /// <param name="optionAction">配置动作</param>
    /// <returns>当前配置实例（支持链式调用）</returns>
    public MappingConfiguration<TSource, TTarget> ForMember<TProperty>(
        Expression<Func<TTarget, TProperty>> targetProperty,
        Action<PropertyMappingOption<TSource>> optionAction)
    {
        var propertyName = GetMemberName(targetProperty);
        var option = new PropertyMappingOption<TSource>();
        optionAction(option);

        // 同一属性重复配置时，后者覆盖前者（last wins）
        _memberActions.RemoveAll(x => x.TargetPropertyName == propertyName);
        _memberActions.Add((propertyName, option));

        return this;
    }

    /// <summary>
    /// 获取所有成员配置（内部使用）
    /// </summary>
    internal IReadOnlyList<(string TargetPropertyName, PropertyMappingOption<TSource> Option)> GetMemberActions()
        => _memberActions;

    /// <summary>
    /// 计算配置的稳定哈希值，用于缓存键区分
    /// </summary>
    internal int GetConfigHash()
    {
        var hash = new HashCode();
        foreach (var (name, option) in _memberActions)
        {
            hash.Add(name);
            hash.Add(option.ActionType);
            if (option.SourceExpression != null)
            {
                hash.Add(option.SourceExpression.ToString());
            }

            if (option.ConverterType != null)
            {
                hash.Add(option.ConverterType);
            }
        }

        return hash.ToHashCode();
    }

    private static string GetMemberName<TProperty>(Expression<Func<TTarget, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpr &&
            memberExpr.Expression != null &&
            memberExpr.Expression is ParameterExpression)
        {
            return memberExpr.Member.Name;
        }

        throw new ArgumentException(
            "ForMember 表达式必须是简单的成员访问，例如 d =&gt; d.PropertyName",
            nameof(expression));
    }
}
