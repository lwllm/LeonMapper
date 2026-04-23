using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 sbyte 类型转换为 byte 类型
/// </summary>
public class SByteToByteConverter : IConverter<sbyte, byte>
{
    public byte Convert(sbyte input)
    {
        if (input < byte.MinValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 short 类型
/// </summary>
public class SByteToShortConverter : IConverter<sbyte, short>
{
    public short Convert(sbyte input)
    {
        return input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 ushort 类型
/// </summary>
public class SByteToUShortConverter : IConverter<sbyte, ushort>
{
    public ushort Convert(sbyte input)
    {
        if (input < ushort.MinValue)
        {
            throw new OverflowException();
        }
        return (ushort)input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 int 类型
/// </summary>
public class SByteToIntConverter : IConverter<sbyte, int>
{
    public int Convert(sbyte input)
    {
        return input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 uint 类型
/// </summary>
public class SByteToUIntConverter : IConverter<sbyte, uint>
{
    public uint Convert(sbyte input)
    {
        if (input < uint.MinValue)
        {
            throw new OverflowException();
        }
        return (uint)input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 long 类型
/// </summary>
public class SByteToLongConverter : IConverter<sbyte, long>
{
    public long Convert(sbyte input)
    {
        return input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 ulong 类型
/// </summary>
public class SByteToULongConverter : IConverter<sbyte, ulong>
{
    public ulong Convert(sbyte input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return (ulong)input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 float 类型
/// </summary>
public class SByteToFloatConverter : IConverter<sbyte, float>
{
    public float Convert(sbyte input)
    {
        return input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 double 类型
/// </summary>
public class SByteToDoubleConverter : IConverter<sbyte, double>
{
    public double Convert(sbyte input)
    {
        return input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 decimal 类型
/// </summary>
public class SByteToDecimalConverter : IConverter<sbyte, decimal>
{
    public decimal Convert(sbyte input)
    {
        return input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 char 类型
/// </summary>
public class SByteToCharConverter : IConverter<sbyte, char>
{
    public char Convert(sbyte input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }

        return (char)input;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 bool 类型
/// </summary>
public class SByteToBoolConverter : IConverter<sbyte, bool>
{
    public bool Convert(sbyte input)
    {
        return input != 0;
    }
}

/// <summary>
/// 将 sbyte 类型转换为 IntPtr 类型
/// </summary>
public class SByteToIntPtrConverter : IConverter<sbyte, IntPtr>
{
    public IntPtr Convert(sbyte input)
    {
        return new IntPtr(input);
    }
}

/// <summary>
/// 将 sbyte 类型转换为 UIntPtr 类型
/// </summary>
public class SByteToUIntPtrConverter : IConverter<sbyte, UIntPtr>
{
    public UIntPtr Convert(sbyte input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return new UIntPtr((byte)input);
    }
}

/// <summary>
/// 将 sbyte 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class SByteToStringConverter : IConverter<sbyte, string>
{
    public string Convert(sbyte input)
    {
        return input.ToString();
    }
}
