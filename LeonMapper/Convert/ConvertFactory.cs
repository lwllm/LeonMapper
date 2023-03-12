using System.Reflection;
using LeonMapper.Config;
using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert;

public class ConvertFactory
{
    private static readonly Dictionary<string, object> _commonConverterDictionary =
        new Dictionary<string, object>();
    private static readonly Dictionary<string, object> _allConverterDictionary =
        new Dictionary<string, object>();

    private const string CONVERTER_METHOD_NAME = "Convert";

    static ConvertFactory()
    {
        var converterInterfaceType = typeof(IConverter<,>);
        var converters = Assembly.GetAssembly(typeof(ConvertFactory))
            .GetTypes().Where(t =>
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == converterInterfaceType));
        foreach (var converter in converters)
        {
            var isCommonConverter = converter.GetCustomAttributes(typeof(CommonConverterAttribute)).Any();
            var convertMethod = converter.GetMethod(CONVERTER_METHOD_NAME);
            if (convertMethod != null && convertMethod.GetParameters().Length == 1)
            {
                var inputType = convertMethod.GetParameters()[0].ParameterType;
                var outputType = convertMethod.ReturnType;
                var converterInstance = Activator.CreateInstance(converter);
                if (isCommonConverter)
                {
                    _commonConverterDictionary.Add($"{inputType.FullName}|{outputType.FullName}", converterInstance);
                }
                _allConverterDictionary.Add($"{inputType.FullName}|{outputType.FullName}", converterInstance);
            }
        }
    }

    public static IConverter<TInput, TOutput> GetTheBaseTypeConverter<TInput, TOutput>()
    {
        var key = $"{typeof(TInput).FullName}|{typeof(TOutput).FullName}";
        if (MapperConfig.GetDefaultConverterScope() == ConverterScopeEnum.CommonConverters)
        {
            if (_commonConverterDictionary.ContainsKey(key))
            {
                return (IConverter<TInput, TOutput>)_commonConverterDictionary[key];
            }
        }
        else
        {
            if (_allConverterDictionary.ContainsKey(key))
            {
                return (IConverter<TInput, TOutput>)_commonConverterDictionary[key];
            }
        }

        return null;
    }
}