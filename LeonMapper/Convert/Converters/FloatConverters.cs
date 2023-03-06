namespace LeonMapper.Convert.Converters;

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

public class FloatToDoubleConverter : IConverter<float, double>
{
    public double Convert(float input)
    {
        return input;
    }
}

public class FloatToDecimalConverter : IConverter<float, decimal>
{
    public decimal Convert(float input)
    {
        return (decimal)input;
    }
}

public class FloatToCharConverter : IConverter<float, char>
{
    public char Convert(float input)
    {
        return (char)input;
    }
}

public class FloatToBoolConverter : IConverter<float, bool>
{
    public bool Convert(float input)
    {
        return input != 0f;
    }
}

public class FloatToIntPtrConverter : IConverter<float, IntPtr>
{
    public IntPtr Convert(float input)
    {
        return new IntPtr((int)input);
    }
}

public class FloatToUIntPtrConverter : IConverter<float, UIntPtr>
{
    public UIntPtr Convert(float input)
    {
        return new UIntPtr((uint)input);
    }
}

public class FloatToStringConverter : IConverter<float, string>
{
    public string Convert(float input)
    {
        return input.ToString();
    }
}
