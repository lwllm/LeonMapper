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
    protected static readonly Dictionary<PropertyInfo, PropertyInfo> PropertyDictionary;

    protected static readonly Dictionary<FieldInfo, FieldInfo> FieldDictionary;

    static AbstractProcessor()
    {
        PropertyDictionary = GetPropertyDictionary(typeof(TInput), typeof(TOutput));
        FieldDictionary = GetFieldDictionary(typeof(TInput), typeof(TOutput));
    }

    private static Dictionary<PropertyInfo, PropertyInfo> GetPropertyDictionary(Type inputType, Type outputType)
    {
        var sourceProperties = inputType.GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);
        var targetProperties = outputType.GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
        var propertyDictionary = new Dictionary<PropertyInfo, PropertyInfo>(sourceProperties.Count);
        foreach (var sourcePropertiesKey in sourceProperties.Keys.Where(sourcePropertiesKey =>
                     targetProperties.ContainsKey(sourcePropertiesKey)))
        {
            propertyDictionary.Add(sourceProperties[sourcePropertiesKey], targetProperties[sourcePropertiesKey]);
        }

        return propertyDictionary;
    }

    private static Dictionary<FieldInfo, FieldInfo> GetFieldDictionary(Type inputType, Type outputType)
    {
        var sourceFields = inputType.GetFields().ToDictionary(f => f.Name, f => f);
        var targetFields = outputType.GetFields().ToDictionary(f => f.Name, f => f);
        var fieldDictionary = new Dictionary<FieldInfo, FieldInfo>(sourceFields.Count);
        foreach (var fieldsKey in sourceFields.Keys.Where(fieldsKey => targetFields.ContainsKey(fieldsKey)))
        {
            fieldDictionary.Add(sourceFields[fieldsKey], targetFields[fieldsKey]);
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
        if (!inputProperty.CanRead || !outputProperty.CanWrite)
        {
            return false;
        }

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
}