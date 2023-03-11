using System.Linq.Expressions;
using LeonMapper.Config;
using LeonMapper.Convert;

namespace LeonMapper.Processors.ExpressionProcessor
{
    public class ExpressionProcessor<TInput, TOutput> : AbstractProcessor<TInput, TOutput>
    {
        private static readonly Func<TInput, TOutput> CreateTargetObjectFunc;

        static ExpressionProcessor()
        {
            var sourceParameterExpression =
                Expression.Parameter(typeof(TInput), "pIn");
            var memberBindingList = new List<MemberBinding>();
            foreach (var propertyPair in PropertyDictionary)
            {
                if (propertyPair.Key.PropertyType == propertyPair.Value.PropertyType)
                {
                    var property =
                        Expression.Property(sourceParameterExpression,
                            propertyPair.Key);
                    var memberBinding = Expression.Bind(propertyPair.Value, property);
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
                    // var convertMethod = converter.GetType().GetMethod("Convert");
                    // var property = Expression.Property(sourceParameterExpression, propertyPair.Key);
                    // var convertExpression = Expression.Call(Expression.Constant(converter), convertMethod, property);
                    // var memberBinding = Expression.Bind(propertyPair.Value, convertExpression);
                    // memberBindingList.Add(memberBinding);

                    var convertMethod = converter.GetType().GetMethod("Convert");
                    var property = Expression.Property(sourceParameterExpression, propertyPair.Key);
                    var convertExpression = Expression.Call(Expression.Constant(converter), convertMethod, property);
                    var getAutoConvertMethod = typeof(MapperConfig).GetMethod("GetAutoConvert");
                    var getAutoConvertExpression = Expression.Call(getAutoConvertMethod);
                    var ifTrue = Expression.Bind(propertyPair.Value, convertExpression);
                    var ifFalse = Expression.Bind(propertyPair.Value, property);
                    var memberBinding = Expression.IfThenElse(getAutoConvertExpression,
                        ifTrue.Expression, ifFalse.Expression);
                    // memberBindingList.Add(memberBinding);
                }
            }

            foreach (var fieldPare in FieldDictionary)
            {
                if (fieldPare.Key.FieldType == fieldPare.Value.FieldType)
                {
                    var field =
                        Expression.Field(sourceParameterExpression, fieldPare.Key);
                    var memberBinding = Expression.Bind(fieldPare.Value, field);
                    memberBindingList.Add(memberBinding);
                }
            }

            var memberInitExpression =
                Expression.MemberInit(Expression.New(typeof(TOutput)),
                    memberBindingList.ToArray());
            var lambda = Expression.Lambda<Func<TInput, TOutput>>(
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