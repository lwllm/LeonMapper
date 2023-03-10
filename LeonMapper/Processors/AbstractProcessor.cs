using System.Reflection;

namespace LeonMapper.Processors;

public abstract class AbstractProcessor<TInput, TOutput> : IProcessor<TInput, TOutput>
{
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
}