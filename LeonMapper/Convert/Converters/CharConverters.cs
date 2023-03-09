namespace LeonMapper.Convert.Converters;

public class CharToSByteConverter : IConverter<char, sbyte>
{
    public sbyte Convert(char input)
    {
        if (input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

public class CharToByteConverter : IConverter<char, byte>
{
    public byte Convert(char input)
    {
        if (input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

public class CharToShortConverter : IConverter<char, short>
{
    public short Convert(char input)
    {
        return (short)input;
    }
}

public class CharToUShortConverter : IConverter<char, ushort>
{
    public ushort Convert(char input)
    {
        return input;
    }
}

public class CharToIntConverter : IConverter<char, int>
{
    public int Convert(char input)
    {
        return input;
    }
}

public class CharToUIntConverter : IConverter<char, uint>
{
    public uint Convert(char input)
    {
        return input;
    }
}

public class CharToLongConverter : IConverter<char, long>
{
    public long Convert(char input)
    {
        return input;
    }
}

public class CharToULongConverter : IConverter<char, ulong>
{
    public ulong Convert(char input)
    {
        return input;
    }
}
public class CharToFloatConverter : IConverter<char, float>
{
    public float Convert(char input)
    {
        return input;
    }
}

public class CharToDoubleConverter : IConverter<char, double>
{
    public double Convert(char input)
    {
        return input;
    }
}

public class CharToDecimalConverter : IConverter<char, decimal>
{
    public decimal Convert(char input)
    {
        return input;
    }
}

public class CharToCharConverter : IConverter<char, char>
{
    public char Convert(char input)
    {
        return input;
    }
}

public class CharToBoolConverter : IConverter<char, bool>
{
    public bool Convert(char input)
    {
        return input != '0';
    }
}

public class CharToIntPtrConverter : IConverter<char, IntPtr>
{
    public IntPtr Convert(char input)
    {
        if (IntPtr.Size == 4)
        {
            return new IntPtr((int)input);
        }

        if (IntPtr.Size == 8)
        {
            return new IntPtr((long)input);
        }

        throw new PlatformNotSupportedException();
    }
}

public class CharToUIntPtrConverter : IConverter<char, UIntPtr>
{
    public UIntPtr Convert(char input)
    {
        if (UIntPtr.Size == 4)
        {
            return new UIntPtr((uint)input);
        }

        if (UIntPtr.Size == 8)
        {
            return new UIntPtr((ulong)input);
        }

        throw new PlatformNotSupportedException();
    }
}

public class CharToStringConverter : IConverter<char, string>
{
    public string Convert(char input)
    {
        return input.ToString();
    }
}