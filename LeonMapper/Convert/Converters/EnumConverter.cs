namespace LeonMapper.Convert.Converters;

/// <summary>
/// Enum 到 Enum 的转换器（按名称映射）
/// </summary>
public class EnumConverter<TSource, TDestination> : IConverter<TSource, TDestination>
    where TSource : struct, Enum
    where TDestination : struct, Enum
{
    private static readonly Dictionary<string, TDestination> _nameToValue;
    private static readonly TDestination _defaultValue;

    static EnumConverter()
    {
        _nameToValue = new Dictionary<string, TDestination>(StringComparer.Ordinal);
        foreach (var name in Enum.GetNames(typeof(TDestination)))
        {
            _nameToValue[name] = (TDestination)Enum.Parse(typeof(TDestination), name);
        }

        _defaultValue = default;
    }

    public TDestination Convert(TSource input)
    {
        var name = Enum.GetName(typeof(TSource), input);
        if (name != null && _nameToValue.TryGetValue(name, out var value))
        {
            return value;
        }

        return _defaultValue;
    }
}

/// <summary>
/// Enum 到基础类型的转换器
/// </summary>
public class EnumToBaseTypeConverter<TSource, TDestination> : IConverter<TSource, TDestination>
    where TSource : struct, Enum
{
    public TDestination Convert(TSource input)
    {
        if (typeof(TDestination) == typeof(string))
        {
            return (TDestination)(object)input.ToString();
        }

        var underlyingValue = System.Convert.ChangeType(input, Enum.GetUnderlyingType(typeof(TSource)));
        return (TDestination)System.Convert.ChangeType(underlyingValue, typeof(TDestination))!;
    }
}

/// <summary>
/// 基础类型到 Enum 的转换器
/// </summary>
public class BaseTypeToEnumConverter<TSource, TDestination> : IConverter<TSource, TDestination>
    where TDestination : struct, Enum
{
    public TDestination Convert(TSource input)
    {
        var underlyingValue = System.Convert.ChangeType(input, Enum.GetUnderlyingType(typeof(TDestination)));
        return (TDestination)Enum.ToObject(typeof(TDestination), underlyingValue!);
    }
}

/// <summary>
/// 字符串到 Enum 的转换器
/// </summary>
public class StringToEnumConverter<TDestination> : IConverter<string, TDestination>
    where TDestination : struct, Enum
{
    public TDestination Convert(string input)
    {
        if (Enum.TryParse<TDestination>(input, out var result))
        {
            return result;
        }

        return default;
    }
}
