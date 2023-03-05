namespace LeonMapper.Convert.Converters;

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

public class ULongToIntConverter : IConverter<ulong, int>
{
    public int Convert(ulong input)
    {
        if (input > (ulong)int.MaxValue)
        {
            throw new OverflowException();
        }
        return (int)input;
    }
}

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

public class ULongToLongConverter : IConverter<ulong, long>
{
    public long Convert(ulong input)
    {
        if (input > (ulong)long.MaxValue)
        {
            throw new OverflowException();
        }
        return (long)input;
    }
}

