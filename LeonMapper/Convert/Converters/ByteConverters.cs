using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 byte 类型转换为 sbyte 类型
/// </summary>
public class ByteToSByteConverter : IConverter<byte, sbyte>
{
    public sbyte Convert(byte input)
    {
        if (input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

/// <summary>
/// 将 byte 类型转换为 short 类型
/// </summary>
[CommonConverter]
public class ByteToShortConverter : IConverter<byte, short>
{
    public short Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 ushort 类型
/// </summary>
public class ByteToUShortConverter : IConverter<byte, ushort>
{
    public ushort Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 int 类型
/// </summary>
[CommonConverter]
public class ByteToIntConverter : IConverter<byte, int>
{
    public int Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 uint 类型
/// </summary>
public class ByteToUIntConverter : IConverter<byte, uint>
{
    public uint Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 long 类型
/// </summary>
public class ByteToLongConverter : IConverter<byte, long>
{
    public long Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 ulong 类型
/// </summary>
public class ByteToULongConverter : IConverter<byte, ulong>
{
    public ulong Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 float 类型
/// </summary>
public class ByteToFloatConverter : IConverter<byte, float>
{
    public float Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 double 类型
/// </summary>
public class ByteToDoubleConverter : IConverter<byte, double>
{
    public double Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 decimal 类型
/// </summary>
public class ByteToDecimalConverter : IConverter<byte, decimal>
{
    public decimal Convert(byte input)
    {
        return input;
    }
}

/// <summary>
/// 将 byte 类型转换为 char 类型
/// </summary>
[CommonConverter]
public class ByteToCharConverter : IConverter<byte, char>
{
    public char Convert(byte input)
    {
        return (char)input;
    }
}

/// <summary>
/// 将 byte 类型转换为 bool 类型
/// </summary>
public class ByteToBoolConverter : IConverter<byte, bool>
{
    public bool Convert(byte input)
    {
        return input != 0;
    }
}

/// <summary>
/// 将 byte 类型转换为 IntPtr 类型
/// </summary>
public class ByteToIntPtrConverter : IConverter<byte, IntPtr>
{
    public IntPtr Convert(byte input)
    {
        if (IntPtr.Size == 4)
        {
            return new IntPtr((int)input);
        }

        return new IntPtr((long)input);
    }
}

/// <summary>
/// 将 byte 类型转换为 UIntPtr 类型
/// </summary>
public class ByteToUIntPtrConverter : IConverter<byte, UIntPtr>
{
    public UIntPtr Convert(byte input)
    {
        if (UIntPtr.Size == 4)
        {
            return new UIntPtr((uint)input);
        }

        return new UIntPtr((ulong)input);
    }
}

/// <summary>
/// 将 byte 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class ByteToStringConverter : IConverter<byte, string>
{
    public string Convert(byte input)
    {
        return input.ToString();
    }
}