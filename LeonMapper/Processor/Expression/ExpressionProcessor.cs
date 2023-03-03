using System.Linq.Expressions;

namespace LeonMapper.Processor.Expression;

public class ExpressionProcessor<TIn, TOut> : AbstractProcessor<TIn, TOut>
{
    private static readonly Func<TIn, TOut> CreateTargetObjectFunc;

    static ExpressionProcessor()
    {
        var sourceParameterExpression =
            System.Linq.Expressions.Expression.Parameter(typeof(TIn), "pIn");
        var memberBindingList = new List<MemberBinding>();
        foreach (var propertyPare in PropertyDictionary)
        {
            var property =
                System.Linq.Expressions.Expression.Property(sourceParameterExpression,
                    propertyPare.Key);
            var memberBinding = System.Linq.Expressions.Expression.Bind(propertyPare.Value, property);
            memberBindingList.Add(memberBinding);
        }

        foreach (var fieldPare in FieldDictionary)
        {
            var field =
                System.Linq.Expressions.Expression.Field(sourceParameterExpression, fieldPare.Key);
            var memberBinding = System.Linq.Expressions.Expression.Bind(fieldPare.Value, field);
            memberBindingList.Add(memberBinding);
        }

        var memberInitExpression =
            System.Linq.Expressions.Expression.MemberInit(System.Linq.Expressions.Expression.New(typeof(TOut)),
                memberBindingList.ToArray());
        var lambda = System.Linq.Expressions.Expression.Lambda<Func<TIn, TOut>>(
            memberInitExpression, new ParameterExpression[]
            {
                sourceParameterExpression
            });
        CreateTargetObjectFunc = lambda.Compile();
    }

    public override TOut MapTo(TIn source)
    {
        return Equals(source, default(TIn)) ? default(TOut) : CreateTargetObjectFunc.Invoke(source);
    }
}