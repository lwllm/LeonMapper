namespace LeonMapper.Convert.Converters;

public class IntPtrToSbyteConverter : IConverter<IntPtr, sbyte>
{
    public sbyte Convert(IntPtr input)
    {
        if (input.ToInt64() < sbyte.MinValue || input.ToInt64() > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input.ToInt64();
    }
}

public class IntPtrToByteConverter : IConverter<IntPtr, byte>
{
    public byte Convert(IntPtr input)
    {
        if (input.ToInt64() < byte.MinValue || input.ToInt64() > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input.ToInt64();
    }
}

public class IntPtrToShortConverter : IConverter<IntPtr, short>
{
    public short Convert(IntPtr input)
    {
        if (input.ToInt64() < short.MinValue || input.ToInt64() > short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input.ToInt64();
    }
}

public class IntPtrToUshortConverter : IConverter<IntPtr, ushort>
{
    public ushort Convert(IntPtr input)
    {
        if (input.ToInt64() < ushort.MinValue || input.ToInt64() > ushort.MaxValue)
        {
            throw new OverflowException();
        }
        return (ushort)input.ToInt64();
    }
}

public class IntPtrToIntConverter : IConverter<IntPtr, int>
{
    public int Convert(IntPtr input)
    {
        if (input.ToInt64() < int.MinValue || input.ToInt64() > int.MaxValue)
        {
            throw new OverflowException();
        }
        return (int)input.ToInt64();
    }
}

public class IntPtrToUintConverter : IConverter<IntPtr, uint>
{
    public uint Convert(IntPtr input)
    {
        if (input.ToInt64() < uint.MinValue || input.ToInt64() > uint.MaxValue)
        {
            throw new OverflowException();
        }
        return (uint)input.ToInt64();
    }
}

public class IntPtrToLongConverter : IConverter<IntPtr, long>
{
    public long Convert(IntPtr input)
    {
        return input.ToInt64();
    }
}

public class IntPtrToUlongConverter : IConverter<IntPtr, ulong>
{
    public ulong Convert(IntPtr input)
    {
        if (input.ToInt64() < 0)
        {
            throw new OverflowException();
        }
        return (ulong)input.ToInt64();
    }
}

public class IntPtrToDecimalConverter : IConverter<IntPtr, decimal>
{
    public decimal Convert(IntPtr input)
    {
        if (input.ToInt64() < decimal.MinValue || input.ToInt64() > decimal.MaxValue)
        {
            throw new OverflowException();
        }
        return (decimal)input.ToInt64();
    }
}

public class IntPtrToBoolConverter : IConverter<IntPtr, bool>
{
    public bool Convert(IntPtr input)
    {
        return input != IntPtr.Zero;
    }
}

public class IntPtrToUIntPtrConverter : IConverter<IntPtr, UIntPtr>
{
    public UIntPtr Convert(IntPtr input)
    {
        if ((ulong)input.ToInt64() < UIntPtr.Zero.ToUInt64() || (ulong)input.ToInt64() > UIntPtr.MaxValue.ToUInt64())
        {
            throw new OverflowException();
        }
        return new UIntPtr((ulong)input.ToInt64());
    }
}

public class IntPtrToStringConverter : IConverter<IntPtr, string>
{
    public string Convert(IntPtr input)
    {
        return input.ToString();
    }
}
