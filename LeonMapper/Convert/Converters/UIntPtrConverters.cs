namespace LeonMapper.Convert.Converters;

public class UIntPtrToSByteConverter : IConverter<UIntPtr, sbyte>
{
    public sbyte Convert(UIntPtr input)
    {
        if (input.ToUInt64() > (long)sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input.ToUInt64();
    }
}

public class UIntPtrToByteConverter : IConverter<UIntPtr, byte>
{
    public byte Convert(UIntPtr input)
    {
        if (input.ToUInt64() > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input.ToUInt64();
    }
}

public class UIntPtrToShortConverter : IConverter<UIntPtr, short>
{
    public short Convert(UIntPtr input)
    {
        if (input.ToUInt64() > (long)short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input.ToUInt64();
    }
}

public class UIntPtrToUShortConverter : IConverter<UIntPtr, ushort>
{
    public ushort Convert(UIntPtr input)
    {
        if (input.ToUInt64() > ushort.MaxValue)
        {
            throw new OverflowException();
        }
        return (ushort)input.ToUInt64();
    }
}

public class UIntPtrToIntConverter : IConverter<UIntPtr, int>
{
    public int Convert(UIntPtr input)
    {
        if (input.ToUInt64() > int.MaxValue)
        {
            throw new OverflowException();
        }
        return (int)input.ToUInt64();
    }
}

public class UIntPtrToUIntConverter : IConverter<UIntPtr, uint>
{
    public uint Convert(UIntPtr input)
    {
        if (input.ToUInt64() > uint.MaxValue)
        {
            throw new OverflowException();
        }
        return (uint)input.ToUInt64();
    }
}

public class UIntPtrToLongConverter : IConverter<UIntPtr, long>
{
    public long Convert(UIntPtr input)
    {
        if (input.ToUInt64() > long.MaxValue)
        {
            throw new OverflowException();
        }
        return (long)input.ToUInt64();
    }
}

public class UIntPtrToULongConverter : IConverter<UIntPtr, ulong>
{
    public ulong Convert(UIntPtr input)
    {
        return input.ToUInt64();
    }
}

public class UIntPtrToFloatConverter : IConverter<UIntPtr, float>
{
    public float Convert(UIntPtr input)
    {
        if (input.ToUInt64() > float.MaxValue)
        {
            throw new OverflowException();
        }
        return (float)input.ToUInt64();
    }
}

public class UIntPtrToDoubleConverter : IConverter<UIntPtr, double>
{
    public double Convert(UIntPtr input)
    {
        return (double)input.ToUInt64();
    }
}

public class UIntPtrToDecimalConverter : IConverter<UIntPtr, decimal>
{
    public decimal Convert(UIntPtr input)
    {
        return (decimal)input.ToUInt64();
    }
}

public class UIntPtrToCharConverter : IConverter<UIntPtr, char>
{
    public char Convert(UIntPtr input)
    {
        if (input.ToUInt64() > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input.ToUInt32();
    }
}

public class UIntPtrToBoolConverter : IConverter<UIntPtr, bool>
{
    public bool Convert(UIntPtr input)
    {
        return input.ToUInt64() != 0;
    }
}

public class UIntPtrToIntPtrConverter : IConverter<UIntPtr, IntPtr>
{
    public IntPtr Convert(UIntPtr input)
    {
        if (input.ToUInt64() > (ulong)IntPtr.MaxValue)
        {
            throw new OverflowException();
        }
        return new IntPtr((long)input.ToUInt64());
    }
}

public class UIntPtrToStringConverter : IConverter<UIntPtr, string>
{
    public string Convert(UIntPtr input)
    {
        return input.ToUInt64().ToString();
    }
}
