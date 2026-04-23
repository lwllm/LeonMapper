using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 char 类型转换为 sbyte 类型
/// </summary>
public class CharToSByteConverter : IConverter<char, sbyte>
{
    public sbyte Convert(char input)
    {
        if (input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

/// <summary>
/// 将 char 类型转换为 byte 类型
/// </summary>
[CommonConverter]
public class CharToByteConverter : IConverter<char, byte>
{
    public byte Convert(char input)
    {
        if (input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

/// <summary>
/// 将 char 类型转换为 short 类型
/// </summary>
[CommonConverter]
public class CharToShortConverter : IConverter<char, short>
{
    public short Convert(char input)
    {
        return (short)input;
    }
}

/// <summary>
/// 将 char 类型转换为 ushort 类型
/// </summary>
public class CharToUShortConverter : IConverter<char, ushort>
{
    public ushort Convert(char input)
    {
        return input;
    }
}

/// <summary>
/// 将 char 类型转换为 int 类型
/// </summary>
[CommonConverter]
public class CharToIntConverter : IConverter<char, int>
{
    public int Convert(char input)
    {
        return input;
    }
}

/// <summary>
/// 将 char 类型转换为 uint 类型
/// </summary>
public class CharToUIntConverter : IConverter<char, uint>
{
    public uint Convert(char input)
    {
        return input;
    }
}

/// <summary>
/// 将 char 类型转换为 long 类型
/// </summary>
public class CharToLongConverter : IConverter<char, long>
{
    public long Convert(char input)
    {
        return input;
    }
}

/// <summary>
/// 将 char 类型转换为 ulong 类型
/// </summary>
public class CharToULongConverter : IConverter<char, ulong>
{
    public ulong Convert(char input)
    {
        return input;
    }
}
/// <summary>
/// 将 char 类型转换为 float 类型
/// </summary>
public class CharToFloatConverter : IConverter<char, float>
{
    public float Convert(char input)
    {
        return input;
    }
}

/// <summary>
/// 将 char 类型转换为 double 类型
/// </summary>
public class CharToDoubleConverter : IConverter<char, double>
{
    public double Convert(char input)
    {
        return input;
    }
}

/// <summary>
/// 将 char 类型转换为 decimal 类型
/// </summary>
public class CharToDecimalConverter : IConverter<char, decimal>
{
    public decimal Convert(char input)
    {
        return input;
    }
}

/// <summary>
/// 将 char 类型转换为 bool 类型
/// </summary>
public class CharToBoolConverter : IConverter<char, bool>
{
    public bool Convert(char input)
    {
        return input != '0';
    }
}

/// <summary>
/// 将 char 类型转换为 IntPtr 类型
/// </summary>
public class CharToIntPtrConverter : IConverter<char, IntPtr>
{
    public IntPtr Convert(char input)
    {
        if (IntPtr.Size == 4)
        {
            return new IntPtr((int)input);
        }

        if (IntPtr.Size == 8)
        {
            return new IntPtr((long)input);
        }

        throw new PlatformNotSupportedException();
    }
}

/// <summary>
/// 将 char 类型转换为 UIntPtr 类型
/// </summary>
public class CharToUIntPtrConverter : IConverter<char, UIntPtr>
{
    public UIntPtr Convert(char input)
    {
        if (UIntPtr.Size == 4)
        {
            return new UIntPtr((uint)input);
        }

        if (UIntPtr.Size == 8)
        {
            return new UIntPtr((ulong)input);
        }

        throw new PlatformNotSupportedException();
    }
}

/// <summary>
/// 将 char 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class CharToStringConverter : IConverter<char, string>
{
    public string Convert(char input)
    {
        return input.ToString();
    }
}