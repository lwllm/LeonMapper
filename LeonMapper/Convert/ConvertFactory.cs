using System.Collections.Concurrent;
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
    private static readonly ConcurrentDictionary<(Type, Type), object> _commonConverterDictionary = new();
    private static readonly ConcurrentDictionary<(Type, Type), object> _allConverterDictionary = new();

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

        RegisterConverterTypes(types);
    }

    /// <summary>
    /// 注册外部程序集中的所有转换器
    /// </summary>
    /// <param name="assembly">包含 IConverter 实现的程序集</param>
    public static void RegisterAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t != null).ToArray()!;
        }

        RegisterConverterTypes(types);
    }

    /// <summary>
    /// 注册单个转换器类型
    /// </summary>
    /// <typeparam name="TConverter">转换器类型，必须实现 IConverter&lt;TInput, TOutput&gt; 并有无参构造函数</typeparam>
    public static void RegisterConverter<TConverter>() where TConverter : new()
    {
        RegisterConverterTypes(new[] { typeof(TConverter) });
    }

    /// <summary>
    /// 注册一组转换器类型到内部字典
    /// </summary>
    private static void RegisterConverterTypes(Type[] types)
    {
        var converterInterfaceType = typeof(IConverter<,>);
        var converters = types.Where(t =>
            !t.IsAbstract && !t.ContainsGenericParameters &&
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

    private static ConcurrentDictionary<(Type, Type), object> GetDictionary(ConverterScope scope) =>
        scope == ConverterScope.Common ? _commonConverterDictionary : _allConverterDictionary;

    /// <summary>
    /// 根据类型获取转换器（Plan 层使用）
    /// </summary>
    public static object? GetConverter(Type inputType, Type outputType, ConverterScope scope)
    {
        return GetDictionary(scope).GetValueOrDefault((inputType, outputType));
    }

    /// <summary>
    /// 检查是否存在指定类型的转换器
    /// </summary>
    public static bool HasConverter(Type inputType, Type outputType, ConverterScope scope)
    {
        return GetDictionary(scope).ContainsKey((inputType, outputType));
    }

    /// <summary>
    /// 获取当前缓存的转换器数量
    /// </summary>
    public static int GetConverterCount(ConverterScope scope)
    {
        return GetDictionary(scope).Count;
    }
}
