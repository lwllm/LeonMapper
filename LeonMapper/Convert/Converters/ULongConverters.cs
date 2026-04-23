using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 ulong 类型转换为 sbyte 类型
/// </summary>
public class ULongToSByteConverter : IConverter<ulong, sbyte>
{
    public sbyte Convert(ulong input)
    {
        if (input > (ulong)sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 byte 类型
/// </summary>
public class ULongToByteConverter : IConverter<ulong, byte>
{
    public byte Convert(ulong input)
    {
        if (input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 short 类型
/// </summary>
public class ULongToShortConverter : IConverter<ulong, short>
{
    public short Convert(ulong input)
    {
        if (input > (ulong)short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 ushort 类型
/// </summary>
public class ULongToUShortConverter : IConverter<ulong, ushort>
{
    public ushort Convert(ulong input)
    {
        if (input > ushort.MaxValue)
        {
            throw new OverflowException();
        }
        return (ushort)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 int 类型
/// </summary>
public class ULongToIntConverter : IConverter<ulong, int>
{
    public int Convert(ulong input)
    {
        if (input > int.MaxValue)
        {
            throw new OverflowException();
        }
        return (int)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 uint 类型
/// </summary>
public class ULongToUIntConverter : IConverter<ulong, uint>
{
    public uint Convert(ulong input)
    {
        if (input > uint.MaxValue)
        {
            throw new OverflowException();
        }
        return (uint)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 long 类型
/// </summary>
public class ULongToLongConverter : IConverter<ulong, long>
{
    public long Convert(ulong input)
    {
        if (input > long.MaxValue)
        {
            throw new OverflowException();
        }
        return (long)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 float 类型
/// </summary>
public class ULongToFloatConverter : IConverter<ulong, float>
{
    public float Convert(ulong input)
    {
        if (input > float.MaxValue)
        {
            throw new OverflowException();
        }

        return input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 double 类型
/// </summary>
public class ULongToDoubleConverter : IConverter<ulong, double>
{
    public double Convert(ulong input)
    {
        if (input > double.MaxValue)
        {
            throw new OverflowException();
        }

        return input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 decimal 类型
/// </summary>
public class ULongToDecimalConverter : IConverter<ulong, decimal>
{
    public decimal Convert(ulong input)
    {
        return input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 char 类型
/// </summary>
public class ULongToCharConverter : IConverter<ulong, char>
{
    public char Convert(ulong input)
    {
        if (input > char.MaxValue)
        {
            throw new OverflowException();
        }

        return (char)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 bool 类型
/// </summary>
public class ULongToBoolConverter : IConverter<ulong, bool>
{
    public bool Convert(ulong input)
    {
        return input != 0;
    }
}

/// <summary>
/// 将 ulong 类型转换为 IntPtr 类型
/// </summary>
public class ULongToIntPtrConverter : IConverter<ulong, IntPtr>
{
    public IntPtr Convert(ulong input)
    {
        if (input > (ulong)IntPtr.MaxValue)
        {
            throw new OverflowException();
        }

        return (IntPtr)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 UIntPtr 类型
/// </summary>
public class ULongToUIntPtrConverter : IConverter<ulong, UIntPtr>
{
    public UIntPtr Convert(ulong input)
    {
        if (input > (ulong)UIntPtr.MaxValue)
        {
            throw new OverflowException();
        }

        return (UIntPtr)input;
    }
}

/// <summary>
/// 将 ulong 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class ULongToStringConverter : IConverter<ulong, string>
{
    public string Convert(ulong input)
    {
        return input.ToString();
    }
}
