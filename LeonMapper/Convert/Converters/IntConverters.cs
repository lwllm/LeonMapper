using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 int 类型转换为 sbyte 类型
/// </summary>
public class IntToSByteConverter : IConverter<int, sbyte>
{
    public sbyte Convert(int input)
    {
        if (input < sbyte.MinValue || input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

/// <summary>
/// 将 int 类型转换为 byte 类型
/// </summary>
public class IntToByteConverter : IConverter<int, byte>
{
    public byte Convert(int input)
    {
        if (input < byte.MinValue || input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

/// <summary>
/// 将 int 类型转换为 short 类型
/// </summary>
[CommonConverter]
public class IntToShortConverter : IConverter<int, short>
{
    public short Convert(int input)
    {
        if (input < short.MinValue || input > short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input;
    }
}

/// <summary>
/// 将 int 类型转换为 ushort 类型
/// </summary>
public class IntToUShortConverter : IConverter<int, ushort>
{
    public ushort Convert(int input)
    {
        if (input < ushort.MinValue || input > ushort.MaxValue)
        {
            throw new OverflowException();
        }
        return (ushort)input;
    }
}

/// <summary>
/// 将 int 类型转换为 uint 类型
/// </summary>
public class IntToUIntConverter : IConverter<int, uint>
{
    public uint Convert(int input)
    {
        if (input < uint.MinValue)
        {
            throw new OverflowException();
        }
        return (uint)input;
    }
}

/// <summary>
/// 将 int 类型转换为 long 类型
/// </summary>
[CommonConverter]
public class IntToLongConverter : IConverter<int, long>
{
    public long Convert(int input)
    {
        return input;
    }
}

/// <summary>
/// 将 int 类型转换为 ulong 类型
/// </summary>
public class IntToULongConverter : IConverter<int, ulong>
{
    public ulong Convert(int input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return (ulong)input;
    }
}

/// <summary>
/// 将 int 类型转换为 float 类型
/// </summary>
public class IntToFloatConverter : IConverter<int, float>
{
    public float Convert(int input)
    {
        return input;
    }
}

/// <summary>
/// 将 int 类型转换为 double 类型
/// </summary>
[CommonConverter]
public class IntToDoubleConverter : IConverter<int, double>
{
    public double Convert(int input)
    {
        return input;
    }
}

/// <summary>
/// 将 int 类型转换为 decimal 类型
/// </summary>
[CommonConverter]
public class IntToDecimalConverter : IConverter<int, decimal>
{
    public decimal Convert(int input)
    {
        return (decimal)input;
    }
}

/// <summary>
/// 将 int 类型转换为 char 类型
/// </summary>
public class IntToCharConverter : IConverter<int, char>
{
    public char Convert(int input)
    {
        if (input < char.MinValue || input > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input;
    }
}

/// <summary>
/// 将 int 类型转换为 bool 类型
/// </summary>
[CommonConverter]
public class IntToBoolConverter : IConverter<int, bool>
{
    public bool Convert(int input)
    {
        return input != 0;
    }
}

/// <summary>
/// 将 int 类型转换为 IntPtr 类型
/// </summary>
public class IntToIntPtrConverter : IConverter<int, IntPtr>
{
    public IntPtr Convert(int input)
    {
        return new IntPtr(input);
    }
}

/// <summary>
/// 将 int 类型转换为 UIntPtr 类型
/// </summary>
public class IntToUIntPtrConverter : IConverter<int, UIntPtr>
{
    public UIntPtr Convert(int input)
    {
        if (UIntPtr.Size == 4)
        {
            if (input < uint.MinValue)
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
/// 将 int 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class IntToStringConverter : IConverter<int, string>
{
    public string Convert(int input)
    {
        return input.ToString();
    }
}