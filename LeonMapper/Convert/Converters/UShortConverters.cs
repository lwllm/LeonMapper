namespace LeonMapper.Convert.Converters;

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

public class UShortToUShortConverter : IConverter<ushort, ushort>
{
    public ushort Convert(ushort input)
    {
        return input;
    }
}

public class UShortToIntConverter : IConverter<ushort, int>
{
    public int Convert(ushort input)
    {
        return input;
    }
}

public class UShortToUIntConverter : IConverter<ushort, uint>
{
    public uint Convert(ushort input)
    {
        return input;
    }
}

public class UShortToLongConverter : IConverter<ushort, long>
{
    public long Convert(ushort input)
    {
        return input;
    }
}

public class UShortToULongConverter : IConverter<ushort, ulong>
{
    public ulong Convert(ushort input)
    {
        return input;
    }
}

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

public class UShortToDoubleConverter : IConverter<ushort, double>
{
    public double Convert(ushort input)
    {
        return input;
    }
}

public class UShortToDecimalConverter : IConverter<ushort, decimal>
{
    public decimal Convert(ushort input)
    {
        return input;
    }
}

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

public class UShortToBoolConverter : IConverter<ushort, bool>
{
    public bool Convert(ushort input)
    {
        return input != 0;
    }
}

public class UShortToIntPtrConverter : IConverter<ushort, IntPtr>
{
    public IntPtr Convert(ushort input)
    {
        return new IntPtr(input);
    }
}

public class UShortToUIntPtrConverter : IConverter<ushort, UIntPtr>
{
    public UIntPtr Convert(ushort input)
    {
        return new UIntPtr(input);
    }
}

public class UShortToStringConverter : IConverter<ushort, string>
{
    public string Convert(ushort input)
    {
        return input.ToString();
    }
}