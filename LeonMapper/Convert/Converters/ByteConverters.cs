namespace LeonMapper.Convert.Converters;

public class ByteToSByteConverter : IConverter<byte, sbyte>
{
    public sbyte Convert(byte input)
    {
        if (input > sbyte.MaxValue)
            throw new OverflowException();
        return (sbyte)input;
    }
}

public class ByteToByteConverter : IConverter<byte, byte>
{
    public byte Convert(byte input)
    {
        return input;
    }
}

public class ByteToShortConverter : IConverter<byte, short>
{
    public short Convert(byte input)
    {
        return input;
    }
}

public class ByteToUShortConverter : IConverter<byte, ushort>
{
    public ushort Convert(byte input)
    {
        return input;
    }
}

public class ByteToIntConverter : IConverter<byte, int>
{
    public int Convert(byte input)
    {
        return input;
    }
}

public class ByteToUIntConverter : IConverter<byte, uint>
{
    public uint Convert(byte input)
    {
        return input;
    }
}

public class ByteToLongConverter : IConverter<byte, long>
{
    public long Convert(byte input)
    {
        return input;
    }
}

public class ByteToULongConverter : IConverter<byte, ulong>
{
    public ulong Convert(byte input)
    {
        return input;
    }
}

public class ByteToFloatConverter : IConverter<byte, float>
{
    public float Convert(byte input)
    {
        return input;
    }
}

public class ByteToDoubleConverter : IConverter<byte, double>
{
    public double Convert(byte input)
    {
        return input;
    }
}

public class ByteToDecimalConverter : IConverter<byte, decimal>
{
    public decimal Convert(byte input)
    {
        return input;
    }
}

public class ByteToCharConverter : IConverter<byte, char>
{
    public char Convert(byte input)
    {
        return (char)input;
    }
}

public class ByteToBoolConverter : IConverter<byte, bool>
{
    public bool Convert(byte input)
    {
        return input != 0;
    }
}

public class ByteToIntPtrConverter : IConverter<byte, IntPtr>
{
    public IntPtr Convert(byte input)
    {
        return IntPtr.Size == 4 ? new IntPtr((int)input) : new IntPtr((long)input);
    }
}

public class ByteToUIntPtrConverter : IConverter<byte, UIntPtr>
{
    public UIntPtr Convert(byte input)
    {
        return UIntPtr.Size == 4 ? new UIntPtr((uint)input) : new UIntPtr((ulong)input);
    }
}

public class ByteToStringConverter : IConverter<byte, string>
{
    public string Convert(byte input)
    {
        return input.ToString();
    }
}