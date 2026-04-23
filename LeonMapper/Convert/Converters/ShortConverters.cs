using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 short 类型转换为 sbyte 类型
/// </summary>
public class ShortToSByteConverter : IConverter<short, sbyte>
{
    public sbyte Convert(short input)
    {
        if (input < sbyte.MinValue || input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

/// <summary>
/// 将 short 类型转换为 byte 类型
/// </summary>
public class ShortToByteConverter : IConverter<short, byte>
{
    public byte Convert(short input)
    {
        if (input < byte.MinValue || input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

/// <summary>
/// 将 short 类型转换为 ushort 类型
/// </summary>
public class ShortToUShortConverter : IConverter<short, ushort>
{
    public ushort Convert(short input)
    {
        if (input < ushort.MinValue)
        {
            throw new OverflowException();
        }
        return (ushort)input;
    }
}

/// <summary>
/// 将 short 类型转换为 int 类型
/// </summary>
[CommonConverter]
public class ShortToIntConverter : IConverter<short, int>
{
    public int Convert(short input)
    {
        return input;
    }
}

/// <summary>
/// 将 short 类型转换为 uint 类型
/// </summary>
public class ShortToUIntConverter : IConverter<short, uint>
{
    public uint Convert(short input)
    {
        if (input < uint.MinValue)
        {
            throw new OverflowException();
        }
        return (uint)input;
    }
}

/// <summary>
/// 将 short 类型转换为 long 类型
/// </summary>
[CommonConverter]
public class ShortToLongConverter : IConverter<short, long>
{
    public long Convert(short input)
    {
        return input;
    }
}

/// <summary>
/// 将 short 类型转换为 ulong 类型
/// </summary>
public class ShortToULongConverter : IConverter<short, ulong>
{
    public ulong Convert(short input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return (ulong)input;
    }
}

/// <summary>
/// 将 short 类型转换为 float 类型
/// </summary>
public class ShortToFloatConverter : IConverter<short, float>
{
    public float Convert(short input)
    {
        return input;
    }
}

/// <summary>
/// 将 short 类型转换为 double 类型
/// </summary>
[CommonConverter]
public class ShortToDoubleConverter : IConverter<short, double>
{
    public double Convert(short input)
    {
        return input;
    }
}

/// <summary>
/// 将 short 类型转换为 decimal 类型
/// </summary>
[CommonConverter]
public class ShortToDecimalConverter : IConverter<short, decimal>
{
    public decimal Convert(short input)
    {
        return input;
    }
}

/// <summary>
/// 将 short 类型转换为 char 类型
/// </summary>
public class ShortToCharConverter : IConverter<short, char>
{
    public char Convert(short input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }

        return (char)input;
    }
}

/// <summary>
/// 将 short 类型转换为 bool 类型
/// </summary>
[CommonConverter]
public class ShortToBoolConverter : IConverter<short, bool>
{
    public bool Convert(short input)
    {
        return input != 0;
    }
}

/// <summary>
/// 将 short 类型转换为 IntPtr 类型
/// </summary>
public class ShortToIntPtrConverter : IConverter<short, IntPtr>
{
    public IntPtr Convert(short input)
    {
        return new IntPtr(input);
    }
}

/// <summary>
/// 将 short 类型转换为 UIntPtr 类型
/// </summary>
public class ShortToUIntPtrConverter : IConverter<short, UIntPtr>
{
    public UIntPtr Convert(short input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return new UIntPtr((ushort)input);
    }
}

/// <summary>
/// 将 short 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class ShortToStringConverter : IConverter<short, string>
{
    public string Convert(short input)
    {
        return input.ToString();
    }
}