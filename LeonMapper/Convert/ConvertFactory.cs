using System.Reflection;
using LeonMapper.Convert.Attributes;

namespace LeonMapper.Converter;

public class ConvertFactory
{
    private static Dictionary<string, object> _converterDictionary =
        new Dictionary<string, object>();

    static ConvertFactory()
    {
        var converterInterfaceType = typeof(IConverter<,>);
        var converters = Assembly.GetAssembly(typeof(ConvertFactory))
            .GetTypes()
            .Where(t =>
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == converterInterfaceType)
                && t.GetCustomAttributes(typeof(ConverterTypesAttribute), false).Any());
        foreach (var converter in converters)
        {
            var convertTypes = converter.GetCustomAttributes(typeof(ConverterTypesAttribute), false);
            if (convertTypes != null && convertTypes.Count() == 1)
            {
                var inputType = ((ConverterTypesAttribute)convertTypes[0]).InputType;
                var outputType = ((ConverterTypesAttribute)convertTypes[0]).OutputType;
                var converterInstance = Activator.CreateInstance(converter);
                _converterDictionary.Add($"{inputType.FullName}|{outputType.FullName}", converterInstance);
            }
        }
    }

    public static IConverter<TInput, TOutput> GetConverter<TInput, TOutput>()
    {
        var key = $"{typeof(TInput).FullName}|{typeof(TOutput).FullName}";
        if (_converterDictionary.ContainsKey(key))
        {
            return (IConverter<TInput, TOutput>)_converterDictionary[key];
        }

        return null;
    }
}