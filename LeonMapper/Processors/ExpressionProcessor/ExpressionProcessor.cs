using System.Linq.Expressions;
using System.Reflection;
using LeonMapper.Config;
using LeonMapper.Convert;
using LeonMapper.Processors.Model;

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
                if (propertyPair.Key.CanRead && propertyPair.Value.CanWrite)
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
                        if (InputAndOutputAreBothBaseTypes(propertyPair.Key.PropertyType,
                                propertyPair.Value.PropertyType))
                        {
                            var baseTypeConverterType = typeof(ConvertFactory)
                                .GetMethod(GET_THE_BASE_TYPE_CONVERTER_METHOD_NAME)
                                .MakeGenericMethod(propertyPair.Key.PropertyType, propertyPair.Value.PropertyType);
                            var baseTypeConverter = baseTypeConverterType.Invoke(null, null);
                            if (baseTypeConverter == null)
                            {
                                continue;
                            }

                            var baseTypeConvertMethod = baseTypeConverter.GetType().GetMethod(CONVERT_METHOD_NAME);
                            var property = Expression.Property(sourceParameterExpression, propertyPair.Key);
                            var convertExpression = Expression.Call(Expression.Constant(baseTypeConverter),
                                baseTypeConvertMethod, property);
                            var getAutoConvertMethod =
                                typeof(MapperConfig).GetMethod(GET_AUTO_CONVERT_CONFIG_METHOD_NAME);
                            var getAutoConvertExpression = Expression.Call(getAutoConvertMethod);
                            var ifTrue = Expression.Bind(propertyPair.Value, convertExpression);
                            var ifFalse = Expression.Bind(propertyPair.Value,
                                Expression.Default(propertyPair.Value.PropertyType));
                            var memberExpression = Expression.Condition(getAutoConvertExpression, ifTrue.Expression,
                                ifFalse.Expression, propertyPair.Value.PropertyType);
                            var memberBinding = Expression.Bind(propertyPair.Value, memberExpression);
                            memberBindingList.Add(memberBinding);
                        }
                        else if (InputAndOutputAreComplexTypes(propertyPair.Key.PropertyType,
                                     propertyPair.Value.PropertyType))
                        {
                            var methodKey =
                                $"MapToMethod_{propertyPair.Key.PropertyType.FullName}|{propertyPair.Value.PropertyType.FullName}";
                            MethodInvoker methodInvoker;
                            if (!Constants.COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY.ContainsKey(methodKey))
                            {
                                methodInvoker = new MethodInvoker();
                                var mapperClass = typeof(Mapper<,>).MakeGenericType(
                                    propertyPair.Key.PropertyType,
                                    propertyPair.Value.PropertyType);
                                methodInvoker.Invoker = Activator.CreateInstance(mapperClass);
                                methodInvoker.MethodInfo = mapperClass.GetMethod(MAP_TO_METHOD_NAME,
                                    types: new Type[] { propertyPair.Key.PropertyType });
                                Constants.COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY.Add(methodKey, methodInvoker);
                            }
                            else
                            {
                                methodInvoker = Constants.COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY[methodKey];
                            }

                            var convertExpression = Expression.Call(Expression.Constant(methodInvoker.Invoker),
                                methodInvoker.MethodInfo,
                                Expression.Property(sourceParameterExpression, propertyPair.Key));
                            var memberBinding = Expression.Bind(propertyPair.Value, convertExpression);
                            memberBindingList.Add(memberBinding);
                        }
                    }
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
                memberInitExpression, sourceParameterExpression);
            CreateTargetObjectFunc = lambda.Compile();
        }

        public override TOutput MapTo(TInput input)
        {
            return Equals(input, default(TInput)) ? default(TOutput) : CreateTargetObjectFunc.Invoke(input);
        }
    }
}