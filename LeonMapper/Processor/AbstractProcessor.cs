using System.Reflection;
using LeonMapper.Implement;

namespace LeonMapper.Processor;

public abstract class AbstractProcessor<TIn, TOut> : IProcessor<TIn, TOut>
{
    public abstract TOut MapTo(TIn source);
    /// <summary>
    /// Key: TIn,Value: TOut
    /// </summary>
    protected static readonly Dictionary<PropertyInfo, PropertyInfo> PropertyDictionary;
    protected static readonly Dictionary<FieldInfo, FieldInfo> FieldDictionary;

    static AbstractProcessor()
    {
        PropertyDictionary = GetPropertyDictionary(typeof(TIn), typeof(TOut));
        FieldDictionary = GetFieldDictionary(typeof(TIn), typeof(TOut));
    }

    private static Dictionary<PropertyInfo, PropertyInfo> GetPropertyDictionary(Type sourceType, Type targetType)
    {
        var sourceProperties = sourceType.GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p);
        var targetProperties = targetType.GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
        var propertyDictionary = new Dictionary<PropertyInfo, PropertyInfo>(sourceProperties.Count);
        foreach (var sourcePropertiesKey in sourceProperties.Keys.Where(sourcePropertiesKey =>
                     targetProperties.ContainsKey(sourcePropertiesKey)))
        {
            propertyDictionary.Add(sourceProperties[sourcePropertiesKey], targetProperties[sourcePropertiesKey]);
        }

        return propertyDictionary;
    }

    private static Dictionary<FieldInfo, FieldInfo> GetFieldDictionary(Type sourceType, Type targetType)
    {
        var sourceFields = sourceType.GetFields().ToDictionary(f => f.Name, f => f);
        var targetFields = targetType.GetFields().ToDictionary(f => f.Name, f => f);
        var fieldDictionary = new Dictionary<FieldInfo, FieldInfo>(sourceFields.Count);
        foreach (var fieldsKey in sourceFields.Keys.Where(fieldsKey => targetFields.ContainsKey(fieldsKey)))
        {
            fieldDictionary.Add(sourceFields[fieldsKey], targetFields[fieldsKey]);
        }

        return fieldDictionary;
    }
}