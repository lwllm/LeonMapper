using System.Reflection;
using LeonMapper.Config;
using LeonMapper.Convert.Attributes;
using LeonMapper.Plan;

namespace LeonMapper.Convert;

/// <summary>
/// 类型转换器工厂：自动发现并缓存所有 IConverter 实现
/// </summary>
public class ConvertFactory
{
    private static readonly Dictionary<(Type, Type), object> _commonConverterDictionary = new();
    private static readonly Dictionary<(Type, Type), object> _allConverterDictionary = new();

    private const string CONVERTER_METHOD_NAME = "Convert";

    /// <summary>
    /// 静态构造函数：扫描程序集中所有 IConverter 实现并缓存实例
    /// </summary>
    static ConvertFactory()
    {
        var converterInterfaceType = typeof(IConverter<,>);
        Type[] types;
        try
        {
            types = Assembly.GetAssembly(typeof(ConvertFactory))!.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t != null).ToArray()!;
        }

        var converters = types.Where(t =>
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
                var key = (inputType, outputType);

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
    /// 获取基础类型转换器（泛型版本），根据全局配置的 ConverterScope 决定查找范围
    /// </summary>
    /// <typeparam name="TInput">源类型</typeparam>
    /// <typeparam name="TOutput">目标类型</typeparam>
    /// <returns>转换器实例，未找到时返回 null</returns>
    [Obsolete("该方法在当前架构中未被使用，请使用 GetConverter 方法替代")]
    public static IConverter<TInput, TOutput>? GetTheBaseTypeConverter<TInput, TOutput>()
    {
        var key = (typeof(TInput), typeof(TOutput));
        if (MapperConfig.GetDefaultConverterScope() == ConverterScope.Common)
        {
            if (_commonConverterDictionary.TryGetValue(key, out var conv))
            {
                return (IConverter<TInput, TOutput>)conv;
            }

            return default;
        }

        // AllConverters 模式：从全量字典中查找
        if (_allConverterDictionary.TryGetValue(key, out var conv2))
        {
            return (IConverter<TInput, TOutput>)conv2;
        }

        return default;
    }

    /// <summary>
    /// 根据类型获取转换器（Plan 层使用）
    /// </summary>
    /// <param name="inputType">源类型</param>
    /// <param name="outputType">目标类型</param>
    /// <param name="scope">转换器查找范围</param>
    /// <returns>转换器实例，未找到时返回 null</returns>
    public static object? GetConverter(Type inputType, Type outputType, ConverterScope scope)
    {
        var key = (inputType, outputType);
        var dict = scope == ConverterScope.Common ? _commonConverterDictionary : _allConverterDictionary;
        if (dict.TryGetValue(key, out var conv))
        {
            return conv;
        }

        return null;
    }

    /// <summary>
    /// 检查是否存在指定类型的转换器
    /// </summary>
    /// <param name="inputType">源类型</param>
    /// <param name="outputType">目标类型</param>
    /// <param name="scope">转换器查找范围</param>
    /// <returns>是否存在对应转换器</returns>
    public static bool HasConverter(Type inputType, Type outputType, ConverterScope scope)
    {
        var key = (inputType, outputType);
        var dict = scope == ConverterScope.Common ? _commonConverterDictionary : _allConverterDictionary;
        return dict.ContainsKey(key);
    }
}
