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
                    GeneratePropertyMapExpression(propertyPair, targetProperty, sourceParameterExpression, memberBindingList);
                }
            }

            foreach (var fieldPair in FieldDictionary)
            {
                foreach (var targetField in fieldPair.Value)
                {
                    GenerateFieldMapExpression(fieldPair, targetField, sourceParameterExpression, memberBindingList);
                }
            }

            var memberInitExpression =
                Expression.MemberInit(Expression.New(typeof(TOutput)),
                    memberBindingList.ToArray());
            var lambda = Expression.Lambda<Func<TInput, TOutput>>(
                memberInitExpression, sourceParameterExpression);
            CreateTargetObjectFunc = lambda.Compile();
        }

        private static void GenerateFieldMapExpression(KeyValuePair<FieldInfo, HashSet<FieldInfo>> fieldPair, FieldInfo targetField,
            Expression sourceParameterExpression, ICollection<MemberBinding> memberBindingList)
        {
            if (!CheckCanMap(fieldPair.Key, targetField))
            {
                return;
            }

            if (fieldPair.Key.FieldType == targetField.FieldType)
            {
                var field =
                    Expression.Field(sourceParameterExpression, fieldPair.Key);
                var memberBinding = Expression.Bind(targetField, field);
                memberBindingList.Add(memberBinding);
            }
            else
            {
                if (InputAndOutputAreBothBaseTypes(fieldPair.Key.FieldType,
                        targetField.FieldType))
                {
                    var baseTypeConverterType = typeof(ConvertFactory)
                        .GetMethod(GET_THE_BASE_TYPE_CONVERTER_METHOD_NAME)
                        .MakeGenericMethod(fieldPair.Key.FieldType, targetField.FieldType);
                    var baseTypeConverter = baseTypeConverterType.Invoke(null, null);
                    if (baseTypeConverter == null)
                    {
                        return;
                    }

                    var baseTypeConvertMethod = baseTypeConverter.GetType().GetMethod(CONVERT_METHOD_NAME);
                    var filed = Expression.Field(sourceParameterExpression, fieldPair.Key);
                    var convertExpression = Expression.Call(Expression.Constant(baseTypeConverter),
                        baseTypeConvertMethod, filed);
                    var getAutoConvertMethod =
                        typeof(MapperConfig).GetMethod(GET_AUTO_CONVERT_CONFIG_METHOD_NAME);
                    var getAutoConvertExpression = Expression.Call(getAutoConvertMethod);
                    var ifTrue = Expression.Bind(targetField, convertExpression);
                    var ifFalse = Expression.Bind(targetField,
                        Expression.Default(targetField.FieldType));
                    var memberExpression = Expression.Condition(getAutoConvertExpression, ifTrue.Expression,
                        ifFalse.Expression, targetField.FieldType);
                    var memberBinding = Expression.Bind(targetField, memberExpression);
                    memberBindingList.Add(memberBinding);
                }
                else if (InputAndOutputAreComplexTypes(fieldPair.Key.FieldType,
                             targetField.FieldType))
                {
                    var methodKey =
                        $"MapToMethod_{fieldPair.Key.FieldType.FullName}|{targetField.FieldType.FullName}";
                    var methodInvoker =
                        GetMethodInvoker(methodKey, fieldPair.Key.FieldType, targetField.FieldType);

                    var convertExpression = Expression.Call(Expression.Constant(methodInvoker.Invoker),
                        methodInvoker.MethodInfo,
                        Expression.Field(sourceParameterExpression, fieldPair.Key));
                    var memberBinding = Expression.Bind(targetField, convertExpression);
                    memberBindingList.Add(memberBinding);
                }
            }
        }

        private static void GeneratePropertyMapExpression(KeyValuePair<PropertyInfo, HashSet<PropertyInfo>> propertyPair,
            PropertyInfo targetProperty, ParameterExpression sourceParameterExpression,
            ICollection<MemberBinding> memberBindingList)
        {
            if (sourceParameterExpression == null) throw new ArgumentNullException(nameof(sourceParameterExpression));
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