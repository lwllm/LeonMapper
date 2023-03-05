namespace LeonMapper.Convert.Converters;

public class UIntToSByteConverter : IConverter<uint, sbyte>
{
    public sbyte Convert(uint input)
    {
        if (input > (uint)sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

public class UIntToByteConverter : IConverter<uint, byte>
{
    public byte Convert(uint input)
    {
        if (input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

public class UIntToShortConverter : IConverter<uint, short>
{
    public short Convert(uint input)
    {
        if (input > (uint)short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input;
    }
}

public class UIntToUShortConverter : IConverter<uint, ushort>
{
    public ushort Convert(uint input)
    {
        if (input > ushort.MaxValue)
        {
            throw new OverflowException();
        }
        return (ushort)input;
    }
}

public class UIntToIntConverter : IConverter<uint, int>
{
    public int Convert(uint input)
    {
        if (input > int.MaxValue)
        {
            throw new OverflowException();
        }
        return (int)input;
    }
}

public class UIntToUIntConverter : IConverter<uint, uint>
{
    public uint Convert(uint input)
    {
        return input;
    }
}

public class UIntToLongConverter : IConverter<uint, long>
{
    public long Convert(uint input)
    {
        return input;
    }
}

public class UIntToULongConverter : IConverter<uint, ulong>
{
    public ulong Convert(uint input)
    {
        return input;
    }
}

public class UIntToFloatConverter : IConverter<uint, float>
{
    public float Convert(uint input)
    {
        if (input > float.MaxValue || input < float.MinValue)
        {
            throw new OverflowException();
        }
        return input;
    }
}

public class UIntToDoubleConverter : IConverter<uint, double>
{
    public double Convert(uint input)
    {
        if (input > double.MaxValue || input < double.MinValue)
        {
            throw new OverflowException();
        }
        return input;
    }
}

public class UIntToDecimalConverter : IConverter<uint, decimal>
{
    public decimal Convert(uint input)
    {
        return input;
    }
}

public class UIntToCharConverter : IConverter<uint, char>
{
    public char Convert(uint input)
    {
        if (input > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input;
    }
}

public class UIntToBoolConverter : IConverter<uint, bool>
{
    public bool Convert(uint input)
    {
        if (input > 1)
        {
            throw new OverflowException();
        }
        return input != 0;
    }
}

public class UIntToIntPtrConverter : IConverter<uint, IntPtr>
{
    public IntPtr Convert(uint input)
    {
        if (input > int.MaxValue)
        {
            throw new OverflowException();
        }
        return (IntPtr)input;
    }
}

public class UIntToUIntPtrConverter : IConverter<uint, UIntPtr>
{
    public UIntPtr Convert(uint input)
    {
        return (UIntPtr)input;
    }
}

public class UIntToStringConverter : IConverter<uint, string>
{
    public string Convert(uint input)
    {
        return input.ToString();
    }
}