namespace LeonMapper.Convert.Converters;

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

public class SByteToShortConverter : IConverter<sbyte, short>
{
    public short Convert(sbyte input)
    {
        return input;
    }
}

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

public class SByteToIntConverter : IConverter<sbyte, int>
{
    public int Convert(sbyte input)
    {
        return input;
    }
}

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

public class SByteToLongConverter : IConverter<sbyte, long>
{
    public long Convert(sbyte input)
    {
        return input;
    }
}

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

public class SByteToFloatConverter : IConverter<sbyte, float>
{
    public float Convert(sbyte input)
    {
        return input;
    }
}

public class SByteToDoubleConverter : IConverter<sbyte, double>
{
    public double Convert(sbyte input)
    {
        return input;
    }
}

public class SByteToDecimalConverter : IConverter<sbyte, decimal>
{
    public decimal Convert(sbyte input)
    {
        return input;
    }
}

public class SByteToCharConverter : IConverter<sbyte, char>
{
    public char Convert(sbyte input)
    {
        if (input < char.MinValue || input > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input;
    }
}

public class SByteToBoolConverter : IConverter<sbyte, bool>
{
    public bool Convert(sbyte input)
    {
        return input != 0;
    }
}

public class SByteToIntPtrConverter : IConverter<sbyte, IntPtr>
{
    public IntPtr Convert(sbyte input)
    {
        return new IntPtr(input);
    }
}

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

public class SByteToStringConverter : IConverter<sbyte, string>
{
    public string Convert(sbyte input)
    {
        return input.ToString();
    }
}
