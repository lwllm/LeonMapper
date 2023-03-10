using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

public class DecimalToSbyteConverter : IConverter<decimal, sbyte>
{
    public sbyte Convert(decimal input)
    {
        if (input > sbyte.MaxValue || input < sbyte.MinValue)
        {
            throw new OverflowException();
        }

        return (sbyte)input;
    }
}

public class DecimalToByteConverter : IConverter<decimal, byte>
{
    public byte Convert(decimal input)
    {
        if (input > byte.MaxValue || input < byte.MinValue)
        {
            throw new OverflowException();
        }

        return (byte)input;
    }
}

[CommonConverter]
public class DecimalToShortConverter : IConverter<decimal, short>
{
    public short Convert(decimal input)
    {
        if (input > short.MaxValue || input < short.MinValue)
        {
            throw new OverflowException();
        }

        return (short)input;
    }
}

public class DecimalToUshortConverter : IConverter<decimal, ushort>
{
    public ushort Convert(decimal input)
    {
        if (input > ushort.MaxValue || input < ushort.MinValue)
        {
            throw new OverflowException();
        }

        return (ushort)input;
    }
}

[CommonConverter]
public class DecimalToIntConverter : IConverter<decimal, int>
{
    public int Convert(decimal input)
    {
        if (input > int.MaxValue || input < int.MinValue)
        {
            throw new OverflowException();
        }

        return (int)input;
    }
}

public class DecimalToUintConverter : IConverter<decimal, uint>
{
    public uint Convert(decimal input)
    {
        if (input > uint.MaxValue || input < uint.MinValue)
        {
            throw new OverflowException();
        }

        return (uint)input;
    }
}

[CommonConverter]
public class DecimalToLongConverter : IConverter<decimal, long>
{
    public long Convert(decimal input)
    {
        if (input > long.MaxValue || input < long.MinValue)
        {
            throw new OverflowException();
        }

        return (long)input;
    }
}

public class DecimalToUlongConverter : IConverter<decimal, ulong>
{
    public ulong Convert(decimal input)
    {
        if (input > ulong.MaxValue || input < ulong.MinValue)
        {
            throw new OverflowException();
        }

        return (ulong)input;
    }
}

public class DecimalToFloatConverter : IConverter<decimal, float>
{
    public float Convert(decimal input)
    {
        if (input < System.Convert.ToDecimal(float.MinValue) || input > System.Convert.ToDecimal(float.MaxValue))
        {
            throw new OverflowException();
        }

        return (float)input;
    }
}

[CommonConverter]
public class DecimalToDoubleConverter : IConverter<decimal, double>
{
    public double Convert(decimal input)
    {
        if (input < System.Convert.ToDecimal(double.MinValue) || input > System.Convert.ToDecimal(double.MaxValue))
        {
            throw new OverflowException();
        }

        return (double)input;
    }
}

public class DecimalToCharConverter : IConverter<decimal, char>
{
    public char Convert(decimal input)
    {
        if (input < char.MinValue || input > char.MaxValue)
        {
            throw new OverflowException();
        }

        return (char)input;
    }
}

public class DecimalToBoolConverter : IConverter<decimal, bool>
{
    public bool Convert(decimal input)
    {
        return input != 0;
    }
}

public class DecimalToIntPtrConverter : IConverter<decimal, IntPtr>
{
    public IntPtr Convert(decimal input)
    {
        if (IntPtr.Size == 4 && (input < int.MinValue || input > int.MaxValue))
        {
            throw new OverflowException();
        }

        if (IntPtr.Size == 8 && (input < long.MinValue || input > long.MaxValue))
        {
            throw new OverflowException();
        }

        return (IntPtr)(long)input;
    }
}

public class DecimalToUIntPtrConverter : IConverter<decimal, UIntPtr>
{
    public UIntPtr Convert(decimal input)
    {
        if (UIntPtr.Size == 4 && (input < uint.MinValue || input > uint.MaxValue))
        {
            throw new OverflowException();
        }

        if (UIntPtr.Size == 8 && (input < ulong.MinValue || input > ulong.MaxValue))
        {
            throw new OverflowException();
        }

        return (UIntPtr)(ulong)input;
    }
}

[CommonConverter]
public class DecimalToStringConverter : IConverter<decimal, string>
{
    public string Convert(decimal input)
    {
        return input.ToString();
    }
}