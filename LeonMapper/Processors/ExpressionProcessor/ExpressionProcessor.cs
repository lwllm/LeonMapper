using System.Linq.Expressions;
using LeonMapper.Convert;

namespace LeonMapper.Processors.ExpressionProcessor
{
    public class ExpressionProcessor<TInput, TOutput> : AbstractProcessor<TInput, TOutput>
    {
        private static readonly Func<TInput, TOutput> CreateTargetObjectFunc;

        static ExpressionProcessor()
        {
            var sourceParameterExpression =
                System.Linq.Expressions.Expression.Parameter(typeof(TInput), "pIn");
            var memberBindingList = new List<MemberBinding>();
            foreach (var propertyPair in PropertyDictionary)
            {
                if (propertyPair.Key.PropertyType == propertyPair.Value.PropertyType)
                {
                    var property =
                        System.Linq.Expressions.Expression.Property(sourceParameterExpression,
                            propertyPair.Key);
                    var memberBinding = System.Linq.Expressions.Expression.Bind(propertyPair.Value, property);
                    memberBindingList.Add(memberBinding);
                }
                else
                {
                    var converterType = typeof(ConvertFactory).GetMethod("GetConverter")
                        .MakeGenericMethod(propertyPair.Key.PropertyType, propertyPair.Value.PropertyType);
                    var converter = converterType.Invoke(null, null);
                    if (converter == null)
                    {
                        continue;
                    }
                    var convertMethod = converter.GetType().GetMethod("Convert");
                    var property = Expression.Property(sourceParameterExpression, propertyPair.Key);
                    var convertExpression = Expression.Call(Expression.Constant(converter), convertMethod, property);
                    var memberBinding = Expression.Bind(propertyPair.Value, convertExpression);
                    memberBindingList.Add(memberBinding);
                }
            }

            foreach (var fieldPare in FieldDictionary)
            {
                if (fieldPare.Key.FieldType == fieldPare.Value.FieldType)
                {
                    var field =
                        System.Linq.Expressions.Expression.Field(sourceParameterExpression, fieldPare.Key);
                    var memberBinding = System.Linq.Expressions.Expression.Bind(fieldPare.Value, field);
                    memberBindingList.Add(memberBinding);
                }
            }

            var memberInitExpression =
                System.Linq.Expressions.Expression.MemberInit(System.Linq.Expressions.Expression.New(typeof(TOutput)),
                    memberBindingList.ToArray());
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<TInput, TOutput>>(
                memberInitExpression, new ParameterExpression[]
                {
                    sourceParameterExpression
                });
            CreateTargetObjectFunc = lambda.Compile();
        }

        public override TOutput MapTo(TInput input)
        {
            return Equals(input, default(TInput)) ? default(TOutput) : CreateTargetObjectFunc.Invoke(input);
        }
    }
}