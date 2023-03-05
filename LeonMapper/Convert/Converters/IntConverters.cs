namespace LeonMapper.Convert.Converters;

public class IntToSByteConverter : IConverter<int, sbyte>
{
    public sbyte Convert(int input)
    {
        if (input < sbyte.MinValue || input > sbyte.MaxValue)
            throw new OverflowException();
        return (sbyte)input;
    }
}

public class IntToByteConverter : IConverter<int, byte>
{
    public byte Convert(int input)
    {
        if (input < byte.MinValue || input > byte.MaxValue)
            throw new OverflowException();
        return (byte)input;
    }
}

public class IntToShortConverter : IConverter<int, short>
{
    public short Convert(int input)
    {
        if (input < short.MinValue || input > short.MaxValue)
            throw new OverflowException();
        return (short)input;
    }
}

public class IntToUShortConverter : IConverter<int, ushort>
{
    public ushort Convert(int input)
    {
        if (input < ushort.MinValue || input > ushort.MaxValue)
            throw new OverflowException();
        return (ushort)input;
    }
}

public class IntToUIntConverter : IConverter<int, uint>
{
    public uint Convert(int input)
    {
        if (input < uint.MinValue)
            throw new OverflowException();
        return (uint)input;
    }
}

public class IntToLongConverter : IConverter<int, long>
{
    public long Convert(int input)
    {
        return input;
    }
}

public class IntToULongConverter : IConverter<int, ulong>
{
    public ulong Convert(int input)
    {
        if (input < 0)
            throw new OverflowException();
        return (ulong)input;
    }
}

public class IntToFloatConverter : IConverter<int, float>
{
    public float Convert(int input)
    {
        return input;
    }
}

public class IntToDoubleConverter : IConverter<int, double>
{
    public double Convert(int input)
    {
        return input;
    }
}

public class IntToDecimalConverter : IConverter<int, decimal>
{
    public decimal Convert(int input)
    {
        return (decimal)input;
    }
}

public class IntToCharConverter : IConverter<int, char>
{
    public char Convert(int input)
    {
        if (input < char.MinValue || input > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input;
    }
}

public class IntToBoolConverter : IConverter<int, bool>
{
    public bool Convert(int input)
    {
        return input != 0;
    }
}

public class IntToIntPtrConverter : IConverter<int, IntPtr>
{
    public IntPtr Convert(int input)
    {
        if (IntPtr.Size == 4)
        {
            if (input < int.MinValue)
                throw new OverflowException();
            return new IntPtr(input);
        }
        else
        {
            return new IntPtr(input);
        }
    }
}

public class IntToUIntPtrConverter : IConverter<int, UIntPtr>
{
    public UIntPtr Convert(int input)
    {
        if (UIntPtr.Size == 4)
        {
            if (input < uint.MinValue)
                throw new OverflowException();
            return new UIntPtr((uint)input);
        }
        else
        {
            return new UIntPtr((ulong)input);
        }
    }
}

public class IntToStringConverter : IConverter<int, string>
{
    public string Convert(int input)
    {
        return input.ToString();
    }
}