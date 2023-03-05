namespace LeonMapper.Convert.Converters;

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

public class LongToLongConverter : IConverter<long, long>
{
    public long Convert(long input)
    {
        return input;
    }
}

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

public class LongToFloatConverter : IConverter<long, float>
{
    public float Convert(long input)
    {
        if (input < float.MinValue || input > float.MaxValue)
            throw new OverflowException();
        return input;
    }
}

public class LongToDoubleConverter : IConverter<long, double>
{
    public double Convert(long input)
    {
        if (input < double.MinValue || input > double.MaxValue)
            throw new OverflowException();
        return input;
    }
}

public class LongToDecimalConverter : IConverter<long, decimal>
{
    public decimal Convert(long input)
    {
        return input;
    }
}

public class LongToCharConverter : IConverter<long, char>
{
    public char Convert(long input)
    {
        if (input < char.MinValue || input > char.MaxValue)
            throw new OverflowException();
        return (char)input;
    }
}

public class LongToBoolConverter : IConverter<long, bool>
{
    public bool Convert(long input)
    {
        return input != 0;
    }
}

public class LongToIntPtrConverter : IConverter<long, IntPtr>
{
    public IntPtr Convert(long input)
    {
        if (IntPtr.Size == 4)
        {
            if (input < int.MinValue || input > int.MaxValue)
                throw new OverflowException();
            return new IntPtr((int)input);
        }
        else
        {
            return new IntPtr(input);
        }
    }
}

public class LongToUIntPtrConverter : IConverter<long, UIntPtr>
{
    public UIntPtr Convert(long input)
    {
        if (UIntPtr.Size == 4)
        {
            if (input < uint.MinValue || input > uint.MaxValue)
                throw new OverflowException();
            return new UIntPtr((uint)input);
        }
        else
        {
            return new UIntPtr((ulong)input);
        }
    }
}
