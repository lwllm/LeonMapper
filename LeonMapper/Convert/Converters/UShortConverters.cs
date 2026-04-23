using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 ushort 类型转换为 sbyte 类型
/// </summary>
public class UShortToSByteConverter : IConverter<ushort, sbyte>
{
    public sbyte Convert(ushort input)
    {
        if (input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 byte 类型
/// </summary>
public class UShortToByteConverter : IConverter<ushort, byte>
{
    public byte Convert(ushort input)
    {
        if (input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 short 类型
/// </summary>
public class UShortToShortConverter : IConverter<ushort, short>
{
    public short Convert(ushort input)
    {
        if (input > short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 int 类型
/// </summary>
public class UShortToIntConverter : IConverter<ushort, int>
{
    public int Convert(ushort input)
    {
        return input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 uint 类型
/// </summary>
public class UShortToUIntConverter : IConverter<ushort, uint>
{
    public uint Convert(ushort input)
    {
        return input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 long 类型
/// </summary>
public class UShortToLongConverter : IConverter<ushort, long>
{
    public long Convert(ushort input)
    {
        return input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 ulong 类型
/// </summary>
public class UShortToULongConverter : IConverter<ushort, ulong>
{
    public ulong Convert(ushort input)
    {
        return input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 float 类型
/// </summary>
public class UShortToFloatConverter : IConverter<ushort, float>
{
    public float Convert(ushort input)
    {
        if (input > float.MaxValue)
        {
            throw new OverflowException();
        }
        return input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 double 类型
/// </summary>
public class UShortToDoubleConverter : IConverter<ushort, double>
{
    public double Convert(ushort input)
    {
        return input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 decimal 类型
/// </summary>
public class UShortToDecimalConverter : IConverter<ushort, decimal>
{
    public decimal Convert(ushort input)
    {
        return input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 char 类型
/// </summary>
public class UShortToCharConverter : IConverter<ushort, char>
{
    public char Convert(ushort input)
    {
        if (input > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input;
    }
}

/// <summary>
/// 将 ushort 类型转换为 bool 类型
/// </summary>
public class UShortToBoolConverter : IConverter<ushort, bool>
{
    public bool Convert(ushort input)
    {
        return input != 0;
    }
}

/// <summary>
/// 将 ushort 类型转换为 IntPtr 类型
/// </summary>
public class UShortToIntPtrConverter : IConverter<ushort, IntPtr>
{
    public IntPtr Convert(ushort input)
    {
        return new IntPtr(input);
    }
}

/// <summary>
/// 将 ushort 类型转换为 UIntPtr 类型
/// </summary>
public class UShortToUIntPtrConverter : IConverter<ushort, UIntPtr>
{
    public UIntPtr Convert(ushort input)
    {
        return new UIntPtr(input);
    }
}

/// <summary>
/// 将 ushort 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class UShortToStringConverter : IConverter<ushort, string>
{
    public string Convert(ushort input)
    {
        return input.ToString();
    }
}