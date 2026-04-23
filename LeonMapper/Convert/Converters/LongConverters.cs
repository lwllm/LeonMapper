using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 long 类型转换为 sbyte 类型
/// </summary>
public class LongToSByteConverter : IConverter<long, sbyte>
{
    public sbyte Convert(long input)
    {
        if (input < sbyte.MinValue || input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

/// <summary>
/// 将 long 类型转换为 byte 类型
/// </summary>
public class LongToByteConverter : IConverter<long, byte>
{
    public byte Convert(long input)
    {
        if (input < byte.MinValue || input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

/// <summary>
/// 将 long 类型转换为 short 类型
/// </summary>
[CommonConverter]
public class LongToShortConverter : IConverter<long, short>
{
    public short Convert(long input)
    {
        if (input < short.MinValue || input > short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input;
    }
}

/// <summary>
/// 将 long 类型转换为 ushort 类型
/// </summary>
public class LongToUShortConverter : IConverter<long, ushort>
{
    public ushort Convert(long input)
    {
        if (input < ushort.MinValue || input > ushort.MaxValue)
        {
            throw new OverflowException();
        }
        return (ushort)input;
    }
}

/// <summary>
/// 将 long 类型转换为 int 类型
/// </summary>
[CommonConverter]
public class LongToIntConverter : IConverter<long, int>
{
    public int Convert(long input)
    {
        if (input < int.MinValue || input > int.MaxValue)
        {
            throw new OverflowException();
        }
        return (int)input;
    }
}

/// <summary>
/// 将 long 类型转换为 uint 类型
/// </summary>
public class LongToUIntConverter : IConverter<long, uint>
{
    public uint Convert(long input)
    {
        if (input < uint.MinValue || input > uint.MaxValue)
        {
            throw new OverflowException();
        }
        return (uint)input;
    }
}

/// <summary>
/// 将 long 类型转换为 ulong 类型
/// </summary>
public class LongToULongConverter : IConverter<long, ulong>
{
    public ulong Convert(long input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return (ulong)input;
    }
}

/// <summary>
/// 将 long 类型转换为 float 类型
/// </summary>
public class LongToFloatConverter : IConverter<long, float>
{
    public float Convert(long input)
    {
        return input;
    }
}

/// <summary>
/// 将 long 类型转换为 double 类型
/// </summary>
[CommonConverter]
public class LongToDoubleConverter : IConverter<long, double>
{
    public double Convert(long input)
    {
        return input;
    }
}

/// <summary>
/// 将 long 类型转换为 decimal 类型
/// </summary>
[CommonConverter]
public class LongToDecimalConverter : IConverter<long, decimal>
{
    public decimal Convert(long input)
    {
        return input;
    }
}

/// <summary>
/// 将 long 类型转换为 char 类型
/// </summary>
public class LongToCharConverter : IConverter<long, char>
{
    public char Convert(long input)
    {
        if (input < char.MinValue || input > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input;
    }
}

/// <summary>
/// 将 long 类型转换为 bool 类型
/// </summary>
public class LongToBoolConverter : IConverter<long, bool>
{
    public bool Convert(long input)
    {
        return input != 0;
    }
}

/// <summary>
/// 将 long 类型转换为 IntPtr 类型
/// </summary>
public class LongToIntPtrConverter : IConverter<long, IntPtr>
{
    public IntPtr Convert(long input)
    {
        if (IntPtr.Size == 4)
        {
            if (input < int.MinValue || input > int.MaxValue)
            {
                throw new OverflowException();
            }
            return new IntPtr((int)input);
        }
        else
        {
            return new IntPtr(input);
        }
    }
}

/// <summary>
/// 将 long 类型转换为 UIntPtr 类型
/// </summary>
public class LongToUIntPtrConverter : IConverter<long, UIntPtr>
{
    public UIntPtr Convert(long input)
    {
        if (UIntPtr.Size == 4)
        {
            if (input < uint.MinValue || input > uint.MaxValue)
            {
                throw new OverflowException();
            }
            return new UIntPtr((uint)input);
        }
        else
        {
            return new UIntPtr((ulong)input);
        }
    }
}

/// <summary>
/// 将 long 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class LongToStringConverter : IConverter<long, string>
{
    public string Convert(long input)
    {
        return input.ToString();
    }
}