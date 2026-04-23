using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 float 类型转换为 sbyte 类型
/// </summary>
public class FloatToSbyteConverter : IConverter<float, sbyte>
{
    public sbyte Convert(float input)
    {
        if (input < sbyte.MinValue || input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }

        return (sbyte)input;
    }
}

/// <summary>
/// 将 float 类型转换为 byte 类型
/// </summary>
public class FloatToByteConverter : IConverter<float, byte>
{
    public byte Convert(float input)
    {
        if (input < byte.MinValue || input > byte.MaxValue)
        {
            throw new OverflowException();
        }

        return (byte)input;
    }
}

/// <summary>
/// 将 float 类型转换为 short 类型
/// </summary>
public class FloatToShortConverter : IConverter<float, short>
{
    public short Convert(float input)
    {
        if (input < short.MinValue || input > short.MaxValue)
        {
            throw new OverflowException();
        }

        return (short)input;
    }
}

/// <summary>
/// 将 float 类型转换为 ushort 类型
/// </summary>
public class FloatToUshortConverter : IConverter<float, ushort>
{
    public ushort Convert(float input)
    {
        if (input < ushort.MinValue || input > ushort.MaxValue)
        {
            throw new OverflowException();
        }

        return (ushort)input;
    }
}

/// <summary>
/// 将 float 类型转换为 int 类型
/// </summary>
public class FloatToIntConverter : IConverter<float, int>
{
    public int Convert(float input)
    {
        if (input < int.MinValue || input > int.MaxValue)
        {
            throw new OverflowException();
        }

        return (int)input;
    }
}

/// <summary>
/// 将 float 类型转换为 uint 类型
/// </summary>
public class FloatToUintConverter : IConverter<float, uint>
{
    public uint Convert(float input)
    {
        if (input < uint.MinValue || input > uint.MaxValue)
        {
            throw new OverflowException();
        }

        return (uint)input;
    }
}

/// <summary>
/// 将 float 类型转换为 long 类型
/// </summary>
public class FloatToLongConverter : IConverter<float, long>
{
    public long Convert(float input)
    {
        if (input < long.MinValue || input > long.MaxValue)
        {
            throw new OverflowException();
        }

        return (long)input;
    }
}

/// <summary>
/// 将 float 类型转换为 ulong 类型
/// </summary>
public class FloatToUlongConverter : IConverter<float, ulong>
{
    public ulong Convert(float input)
    {
        if (input < ulong.MinValue || input > ulong.MaxValue)
        {
            throw new OverflowException();
        }

        return (ulong)input;
    }
}

/// <summary>
/// 将 float 类型转换为 double 类型
/// </summary>
[CommonConverter]
public class FloatToDoubleConverter : IConverter<float, double>
{
    public double Convert(float input)
    {
        return input;
    }
}

/// <summary>
/// 将 float 类型转换为 decimal 类型
/// </summary>
[CommonConverter]
public class FloatToDecimalConverter : IConverter<float, decimal>
{
    public decimal Convert(float input)
    {
        return (decimal)input;
    }
}

/// <summary>
/// 将 float 类型转换为 char 类型
/// </summary>
public class FloatToCharConverter : IConverter<float, char>
{
    public char Convert(float input)
    {
        return (char)input;
    }
}

/// <summary>
/// 将 float 类型转换为 bool 类型
/// </summary>
public class FloatToBoolConverter : IConverter<float, bool>
{
    public bool Convert(float input)
    {
        return input != 0f;
    }
}

/// <summary>
/// 将 float 类型转换为 IntPtr 类型
/// </summary>
public class FloatToIntPtrConverter : IConverter<float, IntPtr>
{
    public IntPtr Convert(float input)
    {
        return new IntPtr((int)input);
    }
}

/// <summary>
/// 将 float 类型转换为 UIntPtr 类型
/// </summary>
public class FloatToUIntPtrConverter : IConverter<float, UIntPtr>
{
    public UIntPtr Convert(float input)
    {
        return new UIntPtr((uint)input);
    }
}

/// <summary>
/// 将 float 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class FloatToStringConverter : IConverter<float, string>
{
    public string Convert(float input)
    {
        return input.ToString();
    }
}
