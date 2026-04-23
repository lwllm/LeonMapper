using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 double 类型转换为 sbyte 类型
/// </summary>
public class DoubleToSByteConverter : IConverter<double, sbyte>
{
    public sbyte Convert(double input)
    {
        if (input < sbyte.MinValue || input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }

        return (sbyte)input;
    }
}

/// <summary>
/// 将 double 类型转换为 byte 类型
/// </summary>
public class DoubleToByteConverter : IConverter<double, byte>
{
    public byte Convert(double input)
    {
        if (input < byte.MinValue || input > byte.MaxValue)
        {
            throw new OverflowException();
        }

        return (byte)input;
    }
}

/// <summary>
/// 将 double 类型转换为 short 类型
/// </summary>
public class DoubleToShortConverter : IConverter<double, short>
{
    public short Convert(double input)
    {
        if (input < short.MinValue || input > short.MaxValue)
        {
            throw new OverflowException();
        }

        return (short)input;
    }
}

/// <summary>
/// 将 double 类型转换为 ushort 类型
/// </summary>
public class DoubleToUShortConverter : IConverter<double, ushort>
{
    public ushort Convert(double input)
    {
        if (input < ushort.MinValue || input > ushort.MaxValue)
        {
            throw new OverflowException();
        }

        return (ushort)input;
    }
}

/// <summary>
/// 将 double 类型转换为 int 类型
/// </summary>
[CommonConverter]
public class DoubleToIntConverter : IConverter<double, int>
{
    public int Convert(double input)
    {
        if (input < int.MinValue || input > int.MaxValue)
        {
            throw new OverflowException();
        }

        return (int)input;
    }
}

/// <summary>
/// 将 double 类型转换为 uint 类型
/// </summary>
public class DoubleToUIntConverter : IConverter<double, uint>
{
    public uint Convert(double input)
    {
        if (input < uint.MinValue || input > uint.MaxValue)
        {
            throw new OverflowException();
        }

        return (uint)input;
    }
}

/// <summary>
/// 将 double 类型转换为 long 类型
/// </summary>
[CommonConverter]
public class DoubleToLongConverter : IConverter<double, long>
{
    public long Convert(double input)
    {
        if (input < long.MinValue || input > long.MaxValue)
        {
            throw new OverflowException();
        }

        return (long)input;
    }
}

/// <summary>
/// 将 double 类型转换为 ulong 类型
/// </summary>
public class DoubleToULongConverter : IConverter<double, ulong>
{
    public ulong Convert(double input)
    {
        if (input < ulong.MinValue || input > ulong.MaxValue)
        {
            throw new OverflowException();
        }

        return (ulong)input;
    }
}

/// <summary>
/// 将 double 类型转换为 float 类型
/// </summary>
public class DoubleToFloatConverter : IConverter<double, float>
{
    public float Convert(double input)
    {
        if (input < float.MinValue || input > float.MaxValue)
        {
            throw new OverflowException();
        }

        return (float)input;
    }
}

/// <summary>
/// 将 double 类型转换为 decimal 类型
/// </summary>
[CommonConverter]
public class DoubleToDecimalConverter : IConverter<double, decimal>
{
    public decimal Convert(double input)
    {
        return (decimal)input;
    }
}

/// <summary>
/// 将 double 类型转换为 char 类型
/// </summary>
public class DoubleToCharConverter : IConverter<double, char>
{
    public char Convert(double input)
    {
        if (input < char.MinValue || input > char.MaxValue)
        {
            throw new OverflowException();
        }

        return (char)input;
    }
}

/// <summary>
/// 将 double 类型转换为 bool 类型
/// </summary>
public class DoubleToBoolConverter : IConverter<double, bool>
{
    public bool Convert(double input)
    {
        return input != 0.0;
    }
}

/// <summary>
/// 将 double 类型转换为 IntPtr 类型
/// </summary>
public class DoubleToIntPtrConverter : IConverter<double, IntPtr>
{
    public IntPtr Convert(double input)
    {
        if (IntPtr.Size == 4 && (input < int.MinValue || input > int.MaxValue))
        {
            throw new OverflowException();
        }
        else if (IntPtr.Size == 8 && (input < long.MinValue || input > long.MaxValue))
        {
            throw new OverflowException();
        }

        return (IntPtr)input;
    }
}

/// <summary>
/// 将 double 类型转换为 UIntPtr 类型
/// </summary>
public class DoubleToUIntPtrConverter : IConverter<double, UIntPtr>
{
    public UIntPtr Convert(double input)
    {
        if (UIntPtr.Size == 4 && (input < uint.MinValue || input > uint.MaxValue))
        {
            throw new OverflowException();
        }
        else if (UIntPtr.Size == 8 && (input < ulong.MinValue || input > ulong.MaxValue))
        {
            throw new OverflowException();
        }

        return (UIntPtr)input;
    }
}

/// <summary>
/// 将 double 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class DoubleToStringConverter : IConverter<double, string>
{
    public string Convert(double input)
    {
        return input.ToString();
    }
}