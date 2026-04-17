using System.Reflection;
using LeonMapper.Config;
using LeonMapper.Convert.Attributes;
using LeonMapper.Plan;

namespace LeonMapper.Convert;

public class ConvertFactory
{
    private static readonly Dictionary<string, object> _commonConverterDictionary = new();
    private static readonly Dictionary<string, object> _allConverterDictionary = new();

    private const string CONVERTER_METHOD_NAME = "Convert";

    static ConvertFactory()
    {
        var converterInterfaceType = typeof(IConverter<,>);
        var converters = Assembly.GetAssembly(typeof(ConvertFactory))!
            .GetTypes().Where(t =>
                t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == converterInterfaceType));

        foreach (var converter in converters)
        {
            var isCommonConverter = converter.GetCustomAttributes(typeof(CommonConverterAttribute)).Any();
            var convertMethod = converter.GetMethod(CONVERTER_METHOD_NAME);
            if (convertMethod != null && convertMethod.GetParameters().Length == 1)
            {
                var inputType = convertMethod.GetParameters()[0].ParameterType;
                var outputType = convertMethod.ReturnType;
                var converterInstance = Activator.CreateInstance(converter);
                var key = $"{inputType.FullName}|{outputType.FullName}";
                if (isCommonConverter && converterInstance != null)
                {
                    _commonConverterDictionary[key] = converterInstance;
                }

                if (converterInstance != null)
                {
                    _allConverterDictionary[key] = converterInstance;
                }
            }
        }
    }

    /// <summary>
    /// 获取基础类型转换器（泛型版本）
    /// </summary>
    public static IConverter<TInput, TOutput>? GetTheBaseTypeConverter<TInput, TOutput>()
    {
        var key = $"{typeof(TInput).FullName}|{typeof(TOutput).FullName}";
        if (MapperConfig.GetDefaultConverterScope() == ConverterScopeEnum.CommonConverters)
        {
            return _commonConverterDictionary.TryGetValue(key, out var conv)
                ? (IConverter<TInput, TOutput>)conv
                : default;
        }

        // 修复 bug：原来错误地访问 _commonConverterDictionary
        return _allConverterDictionary.TryGetValue(key, out var conv2)
            ? (IConverter<TInput, TOutput>)conv2
            : default;
    }

    /// <summary>
    /// 根据类型获取转换器（Plan 层使用）
    /// </summary>
    public static object? GetConverter(Type inputType, Type outputType, ConverterScope scope)
    {
        var key = $"{inputType.FullName}|{outputType.FullName}";
        var dict = scope == ConverterScope.Common ? _commonConverterDictionary : _allConverterDictionary;
        return dict.TryGetValue(key, out var conv) ? conv : null;
    }

    /// <summary>
    /// 检查是否存在指定类型的转换器
    /// </summary>
    public static bool HasConverter(Type inputType, Type outputType, ConverterScope scope)
    {
        var key = $"{inputType.FullName}|{outputType.FullName}";
        var dict = scope == ConverterScope.Common ? _commonConverterDictionary : _allConverterDictionary;
        return dict.ContainsKey(key);
    }
}
