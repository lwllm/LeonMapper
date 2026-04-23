using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 string 类型转换为 int 类型
/// </summary>
[CommonConverter]
public class StringToIntConverter : IConverter<string, int>
{
    public int Convert(string input)
    {
        return int.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 uint 类型
/// </summary>
public class StringToUintConverter : IConverter<string, uint>
{
    public uint Convert(string input)
    {
        return uint.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 short 类型
/// </summary>
[CommonConverter]
public class StringToShortConverter : IConverter<string, short>
{
    public short Convert(string input)
    {
        return short.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 ushort 类型
/// </summary>
public class StringToUshortConverter : IConverter<string, ushort>
{
    public ushort Convert(string input)
    {
        return ushort.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 long 类型
/// </summary>
[CommonConverter]
public class StringToLongConverter : IConverter<string, long>
{
    public long Convert(string input)
    {
        return long.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 ulong 类型
/// </summary>
public class StringToUlongConverter : IConverter<string, ulong>
{
    public ulong Convert(string input)
    {
        return ulong.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 double 类型
/// </summary>
[CommonConverter]
public class StringToDoubleConverter : IConverter<string, double>
{
    public double Convert(string input)
    {
        return double.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 float 类型
/// </summary>
public class StringToFloatConverter : IConverter<string, float>
{
    public float Convert(string input)
    {
        return float.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 decimal 类型
/// </summary>
[CommonConverter]
public class StringToDecimalConverter : IConverter<string, decimal>
{
    public decimal Convert(string input)
    {
        return decimal.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 DateTime 类型
/// </summary>
[CommonConverter]
public class StringToDateTimeConverter : IConverter<string, DateTime>
{
    public DateTime Convert(string input)
    {
        return DateTime.Parse(input);
    }
}

// public class StringToEnumConverter<TEnum> : IConverter<string, TEnum> where TEnum : struct, Enum
// {
//     public TEnum Convert(string input)
//     {
//         return Enum.Parse<TEnum>(input);
//     }
// }

/// <summary>
/// 将 string 类型转换为 sbyte 类型
/// </summary>
public class StringToSbyteConverter : IConverter<string, sbyte>
{
    public sbyte Convert(string input)
    {
        return sbyte.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 byte 类型
/// </summary>
public class StringToByteConverter : IConverter<string, byte>
{
    public byte Convert(string input)
    {
        return byte.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 bool 类型
/// </summary>
[CommonConverter]
public class StringToBoolConverter : IConverter<string, bool>
{
    public bool Convert(string input)
    {
        return bool.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 char 类型
/// </summary>
public class StringToCharConverter : IConverter<string, char>
{
    public char Convert(string input)
    {
        return char.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 IntPtr 类型
/// </summary>
public class StringToIntPtrConverter : IConverter<string, IntPtr>
{
    public IntPtr Convert(string input)
    {
        return IntPtr.Parse(input);
    }
}

/// <summary>
/// 将 string 类型转换为 UIntPtr 类型
/// </summary>
public class StringToUIntPtrConverter : IConverter<string, UIntPtr>
{
    public UIntPtr Convert(string input)
    {
        return UIntPtr.Parse(input);
    }
}