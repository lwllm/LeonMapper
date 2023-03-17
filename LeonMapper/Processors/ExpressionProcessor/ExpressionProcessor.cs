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
                foreach (var targetProperty in propertyPair.Value)
                {
                    GenerateMapExpression(propertyPair, targetProperty, sourceParameterExpression, memberBindingList);
                }
            }

            foreach (var fieldPare in FieldDictionary)
            {
                if (!CheckCanMap(fieldPare.Key, fieldPare.Value))
                {
                    continue;
                }

                if (fieldPare.Key.FieldType == fieldPare.Value.FieldType)
                {
                    var field =
                        Expression.Field(sourceParameterExpression, fieldPare.Key);
                    var memberBinding = Expression.Bind(fieldPare.Value, field);
                    memberBindingList.Add(memberBinding);
                }
                else
                {
                    if (InputAndOutputAreBothBaseTypes(fieldPare.Key.FieldType,
                            fieldPare.Value.FieldType))
                    {
                        var baseTypeConverterType = typeof(ConvertFactory)
                            .GetMethod(GET_THE_BASE_TYPE_CONVERTER_METHOD_NAME)
                            .MakeGenericMethod(fieldPare.Key.FieldType, fieldPare.Value.FieldType);
                        var baseTypeConverter = baseTypeConverterType.Invoke(null, null);
                        if (baseTypeConverter == null)
                        {
                            continue;
                        }

                        var baseTypeConvertMethod = baseTypeConverter.GetType().GetMethod(CONVERT_METHOD_NAME);
                        var filed = Expression.Field(sourceParameterExpression, fieldPare.Key);
                        var convertExpression = Expression.Call(Expression.Constant(baseTypeConverter),
                            baseTypeConvertMethod, filed);
                        var getAutoConvertMethod =
                            typeof(MapperConfig).GetMethod(GET_AUTO_CONVERT_CONFIG_METHOD_NAME);
                        var getAutoConvertExpression = Expression.Call(getAutoConvertMethod);
                        var ifTrue = Expression.Bind(fieldPare.Value, convertExpression);
                        var ifFalse = Expression.Bind(fieldPare.Value,
                            Expression.Default(fieldPare.Value.FieldType));
                        var memberExpression = Expression.Condition(getAutoConvertExpression, ifTrue.Expression,
                            ifFalse.Expression, fieldPare.Value.FieldType);
                        var memberBinding = Expression.Bind(fieldPare.Value, memberExpression);
                        memberBindingList.Add(memberBinding);
                    }
                    else if (InputAndOutputAreComplexTypes(fieldPare.Key.FieldType,
                                 fieldPare.Value.FieldType))
                    {
                        var methodKey =
                            $"MapToMethod_{fieldPare.Key.FieldType.FullName}|{fieldPare.Value.FieldType.FullName}";
                        var methodInvoker =
                            GetMethodInvoker(methodKey, fieldPare.Key.FieldType, fieldPare.Value.FieldType);

                        var convertExpression = Expression.Call(Expression.Constant(methodInvoker.Invoker),
                            methodInvoker.MethodInfo,
                            Expression.Field(sourceParameterExpression, fieldPare.Key));
                        var memberBinding = Expression.Bind(fieldPare.Value, convertExpression);
                        memberBindingList.Add(memberBinding);
                    }
                }
            }

            var memberInitExpression =
                Expression.MemberInit(Expression.New(typeof(TOutput)),
                    memberBindingList.ToArray());
            var lambda = Expression.Lambda<Func<TInput, TOutput>>(
                memberInitExpression, sourceParameterExpression);
            CreateTargetObjectFunc = lambda.Compile();
        }

        private static void GenerateMapExpression(KeyValuePair<PropertyInfo, HashSet<PropertyInfo>> propertyPair, PropertyInfo targetProperty,
            ParameterExpression sourceParameterExpression, List<MemberBinding> memberBindingList)
        {
            if (!CheckCanMap(propertyPair.Key, targetProperty))
            {
                return;
            }

            if (propertyPair.Key.PropertyType == targetProperty.PropertyType)
            {
                var property =
                    Expression.Property(sourceParameterExpression,
                        propertyPair.Key);
                var memberBinding = Expression.Bind(targetProperty, property);
                memberBindingList.Add(memberBinding);
            }
            else
            {
                if (InputAndOutputAreBothBaseTypes(propertyPair.Key.PropertyType,
                        targetProperty.PropertyType))
                {
                    var baseTypeConverterType = typeof(ConvertFactory)
                        .GetMethod(GET_THE_BASE_TYPE_CONVERTER_METHOD_NAME)
                        .MakeGenericMethod(propertyPair.Key.PropertyType, targetProperty.PropertyType);
                    var baseTypeConverter = baseTypeConverterType.Invoke(null, null);
                    if (baseTypeConverter == null)
                    {
                        return;
                    }

                    var baseTypeConvertMethod = baseTypeConverter.GetType().GetMethod(CONVERT_METHOD_NAME);
                    var property = Expression.Property(sourceParameterExpression, propertyPair.Key);
                    var convertExpression = Expression.Call(Expression.Constant(baseTypeConverter),
                        baseTypeConvertMethod, property);
                    var getAutoConvertMethod =
                        typeof(MapperConfig).GetMethod(GET_AUTO_CONVERT_CONFIG_METHOD_NAME);
                    var getAutoConvertExpression = Expression.Call(getAutoConvertMethod);
                    var ifTrue = Expression.Bind(targetProperty, convertExpression);
                    var ifFalse = Expression.Bind(targetProperty,
                        Expression.Default(targetProperty.PropertyType));
                    var memberExpression = Expression.Condition(getAutoConvertExpression, ifTrue.Expression,
                        ifFalse.Expression, targetProperty.PropertyType);
                    var memberBinding = Expression.Bind(targetProperty, memberExpression);
                    memberBindingList.Add(memberBinding);
                }
                else if (InputAndOutputAreComplexTypes(propertyPair.Key.PropertyType,
                             targetProperty.PropertyType))
                {
                    var methodKey =
                        $"MapToMethod_{propertyPair.Key.PropertyType.FullName}|{targetProperty.PropertyType.FullName}";
                    var methodInvoker =
                        GetMethodInvoker(methodKey, propertyPair.Key.PropertyType,
                            targetProperty.PropertyType);

                    var convertExpression = Expression.Call(Expression.Constant(methodInvoker.Invoker),
                        methodInvoker.MethodInfo,
                        Expression.Property(sourceParameterExpression, propertyPair.Key));
                    var memberBinding = Expression.Bind(targetProperty, convertExpression);
                    memberBindingList.Add(memberBinding);
                }
            }
        }

        private static MethodInvoker GetMethodInvoker(string methodKey, Type inputType, Type outputType)
        {
            MethodInvoker methodInvoker;
            if (!Constants.COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY.ContainsKey(methodKey))
            {
                methodInvoker = new MethodInvoker();
                var mapperClass = typeof(Mapper<,>).MakeGenericType(
                    inputType,
                    outputType);
                methodInvoker.Invoker = Activator.CreateInstance(mapperClass);
                methodInvoker.MethodInfo = mapperClass.GetMethod(MAP_TO_METHOD_NAME,
                    types: new Type[] { inputType });
                Constants.COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY.Add(methodKey, methodInvoker);
            }
            else
            {
                methodInvoker = Constants.COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY[methodKey];
            }

            return methodInvoker;
        }

        public override TOutput MapTo(TInput input)
        {
            return Equals(input, default(TInput)) ? default(TOutput) : CreateTargetObjectFunc.Invoke(input);
        }
    }
}