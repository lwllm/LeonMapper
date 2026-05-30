using System.Linq.Expressions;

namespace LeonMapper.Plan.Builder;

/// <summary>
/// Fluent API 映射配置入口
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TTarget">目标类型</typeparam>
public class MappingConfiguration<TSource, TTarget>
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
    /// 添加原始成员配置（用于 ReverseMap 等内部场景）
    /// </summary>
    internal void AddRawAction(string propertyName, Action<PropertyMappingOption<TSource>> optionAction)
    {
        var option = new PropertyMappingOption<TSource>();
        optionAction(option);
        _memberActions.RemoveAll(x => x.TargetPropertyName == propertyName);
        _memberActions.Add((propertyName, option));
    }

    /// <summary>
    /// 生成反向映射配置：将当前配置的映射规则反转
    /// <para>例如 ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName))
    ///      → ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FullName))</para>
    /// <para><b>限制说明</b>：</para>
    /// <list type="bullet">
    ///   <item>MapFrom 只支持简单成员表达式（如 s => s.Prop），复杂表达式（如 s => s.A + s.B）会被忽略</item>
    ///   <item>Condition 条件表达式不会被反向应用</item>
    ///   <item>ConvertUsing 会尝试反向应用，但要求转换器类型对称</item>
    /// </list>
    /// </summary>
    public MappingConfiguration<TTarget, TSource> ReverseMap()
    {
        var reverse = new MappingConfiguration<TTarget, TSource>();

        foreach (var (targetPropName, option) in _memberActions)
        {
            switch (option.ActionType)
            {
                case MemberMappingActionType.MapFrom:
                    if (option.SourceExpression?.Body is MemberExpression memberExpr)
                    {
                        var sourcePropName = memberExpr.Member.Name;
                        var sourceParam = Expression.Parameter(typeof(TTarget), "src");
                        var sourceProp = Expression.Property(sourceParam, targetPropName);
                        var sourceLambda = Expression.Lambda(sourceProp, sourceParam);

                        // 反向：源属性名变成目标属性名，原目标属性变成源表达式
                        reverse.AddRawAction(sourcePropName, opt =>
                        {
                            opt.ActionType = MemberMappingActionType.MapFrom;
                            opt.SourceExpression = sourceLambda;
                        });
                    }

                    break;

                case MemberMappingActionType.Ignore:
                    reverse.AddRawAction(targetPropName, opt => opt.Ignore());
                    break;

                case MemberMappingActionType.ConvertUsing:
                    if (option.ConverterType != null)
                    {
                        var ct = option.ConverterType;
                        reverse.AddRawAction(targetPropName, opt =>
                        {
                            var convertMethod = typeof(PropertyMappingOption<TTarget>)
                                .GetMethod("ConvertUsing")!
                                .MakeGenericMethod(ct);
                            convertMethod.Invoke(opt, null);
                        });
                    }

                    break;
            }
        }

        return reverse;
    }

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

            if (option.ConditionExpression != null)
            {
                hash.Add(option.ConditionExpression.ToString());
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
