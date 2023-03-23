using System.Reflection;
using LeonMapper.Attributes;

namespace LeonMapper.Processors;

public abstract class AbstractProcessor<TInput, TOutput> : IProcessor<TInput, TOutput>
{
    protected const string GET_THE_BASE_TYPE_CONVERTER_METHOD_NAME = "GetTheBaseTypeConverter";
    protected const string CONVERT_METHOD_NAME = "Convert";
    protected const string GET_AUTO_CONVERT_CONFIG_METHOD_NAME = "GetAutoConvert";
    protected const string MAP_TO_METHOD_NAME = "MapTo";

    public abstract TOutput? MapTo(TInput input);

    /// <summary>
    /// Key: TIn,Value: TOut
    /// </summary>
    protected static readonly Dictionary<PropertyInfo, HashSet<PropertyInfo>> PropertyDictionary;

    protected static readonly Dictionary<FieldInfo, HashSet<FieldInfo>> FieldDictionary;

    static AbstractProcessor()
    {
        PropertyDictionary = GetPropertyDictionary(typeof(TInput), typeof(TOutput));
        FieldDictionary = GetFieldDictionary(typeof(TInput), typeof(TOutput));
    }

    private static Dictionary<PropertyInfo, HashSet<PropertyInfo>> GetPropertyDictionary(Type inputType,
        Type outputType)
    {
        var sourceProperties = inputType.GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);
        var targetProperties = outputType.GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
        var propertyDictionary = new Dictionary<PropertyInfo, HashSet<PropertyInfo>>(sourceProperties.Count);
        //处理MapTo、MapFrom
        var mappedTargetProperties = new HashSet<string>();
        foreach (var sourcePropertiesKey in sourceProperties.Keys)
        {
            var mapToAttrs = sourceProperties[sourcePropertiesKey]
                .GetCustomAttributes(typeof(MapToAttribute))
                .Cast<MapToAttribute>();
            var mapToAttributes = mapToAttrs as MapToAttribute[] ?? mapToAttrs.ToArray();
            if (mapToAttributes.Any())
            {
                //如果设置了MapTo注解，则不按名称映射，直接映射到MapTo设置的属性（支持多个）
                foreach (var mapToAttr in mapToAttributes)
                {
                    AddPropertyKeyValuePair(
                        targetProperties,
                        sourcePropertiesKey,
                        mapToAttr.MapToName,
                        propertyDictionary,
                        sourceProperties,
                        mappedTargetProperties);
                }
            }
            else
            {
                AddPropertyKeyValuePair(
                    targetProperties,
                    sourcePropertiesKey,
                    sourcePropertiesKey,
                    propertyDictionary,
                    sourceProperties,
                    mappedTargetProperties);
            }
        }

        mappedTargetProperties = new HashSet<string>();
        foreach (var targetPropertiesKey in targetProperties.Keys)
        {
            //MapFrom优先于MapTo
            var mapFromAttr = targetProperties[targetPropertiesKey]
                .GetCustomAttributes(typeof(MapFromAttribute)).FirstOrDefault();
            if (mapFromAttr != null)
            {
                var propertyName = ((MapFromAttribute)mapFromAttr).MapFromName;
                if (sourceProperties.ContainsKey(propertyName))
                {
                    AddPropertyKeyValuePair(
                        targetProperties,
                        propertyName,
                        targetPropertiesKey,
                        propertyDictionary,
                        sourceProperties,
                        mappedTargetProperties);
                }
            }
        }

        return propertyDictionary;
    }

    private static void AddPropertyKeyValuePair(Dictionary<string, PropertyInfo> targetProperties,
        string sourcePropertyKey, string targetPropertyKey,
        Dictionary<PropertyInfo, HashSet<PropertyInfo>> propertyDictionary,
        Dictionary<string, PropertyInfo> sourceProperties,
        HashSet<string> mappedTargetProperties)
    {
        if (targetProperties.ContainsKey(targetPropertyKey) && !mappedTargetProperties.Contains(targetPropertyKey))
        {
            if (!propertyDictionary.ContainsKey(sourceProperties[sourcePropertyKey]))
            {
                propertyDictionary.Add(sourceProperties[sourcePropertyKey], new HashSet<PropertyInfo>());
            }

            propertyDictionary[sourceProperties[sourcePropertyKey]]
                .Add(targetProperties[targetPropertyKey]);
            mappedTargetProperties.Add(targetPropertyKey);
        }
    }

    private static Dictionary<FieldInfo, HashSet<FieldInfo>> GetFieldDictionary(Type inputType, Type outputType)
    {
        var sourceFields = inputType.GetFields().ToDictionary(f => f.Name, f => f);
        var targetFields = outputType.GetFields().ToDictionary(f => f.Name, f => f);
        var fieldDictionary = new Dictionary<FieldInfo, HashSet<FieldInfo>>(sourceFields.Count);
        foreach (var fieldsKey in sourceFields.Keys.Where(fieldsKey => targetFields.ContainsKey(fieldsKey)))
        {
            fieldDictionary.Add(sourceFields[fieldsKey], new HashSet<FieldInfo>());
            fieldDictionary[sourceFields[fieldsKey]].Add(targetFields[fieldsKey]);
        }

        return fieldDictionary;
    }

    /// <summary>
    /// 判断是否基础类型
    /// </summary>
    /// <param name="type">需要判断的类型</param>
    /// <returns>是否基础类型</returns>
    private static bool IsBaseType(Type type)
    {
        return type.IsPrimitive
               || type == typeof(string)
               || type == typeof(decimal);
    }

    /// <summary>
    /// 检查输入输出是否都是基础类型
    /// </summary>
    /// <param name="inputType">输入类型</param>
    /// <param name="outputType">输出类型</param>
    /// <returns>是否都是基础类型</returns>
    protected static bool InputAndOutputAreBothBaseTypes(Type inputType, Type outputType)
    {
        return IsBaseType(inputType) && IsBaseType(outputType);
    }

    /// <summary>
    /// 检查输入输出是否都是复杂类型
    /// </summary>
    /// <param name="inputType">输入类型</param>
    /// <param name="outputType">输出类型</param>
    /// <returns>是否都是复杂类型</returns>
    protected static bool InputAndOutputAreComplexTypes(Type inputType, Type outputType)
    {
        return !IsBaseType(inputType) && !IsBaseType(outputType);
    }

    protected static bool CheckCanMap(PropertyInfo inputProperty, PropertyInfo outputProperty)
    {
        //检查读写状态
        // if (!inputProperty.CanRead || !outputProperty.CanWrite)
        // {
        //     return false;
        // }

        //检查input是否有忽略属性
        if (inputProperty.GetCustomAttributes(typeof(IgnoreMapAttribute)).Any() ||
            inputProperty.GetCustomAttributes(typeof(IgnoreMapToAttribute)).Any())
        {
            return false;
        }

        //检查output是否有忽略属性
        if (outputProperty.GetCustomAttributes(typeof(IgnoreMapAttribute)).Any() ||
            outputProperty.GetCustomAttributes(typeof(IgnoreMapFromAttribute)).Any())
        {
            return false;
        }

        return true;
    }

    protected static bool CheckCanMap(FieldInfo inputField, FieldInfo outputField)
    {
        //检查input是否有忽略属性
        if (inputField.GetCustomAttributes(typeof(IgnoreMapAttribute)).Any() ||
            inputField.GetCustomAttributes(typeof(IgnoreMapToAttribute)).Any())
        {
            return false;
        }

        //检查output是否有忽略属性
        if (outputField.GetCustomAttributes(typeof(IgnoreMapAttribute)).Any() ||
            outputField.GetCustomAttributes(typeof(IgnoreMapFromAttribute)).Any())
        {
            return false;
        }

        return true;
    }
}