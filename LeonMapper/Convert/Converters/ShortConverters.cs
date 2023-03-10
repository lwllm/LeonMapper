using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

public class ShortToSByteConverter : IConverter<short, sbyte>
{
    public sbyte Convert(short input)
    {
        if (input < sbyte.MinValue || input > sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input;
    }
}

public class ShortToByteConverter : IConverter<short, byte>
{
    public byte Convert(short input)
    {
        if (input < byte.MinValue || input > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input;
    }
}

public class ShortToUShortConverter : IConverter<short, ushort>
{
    public ushort Convert(short input)
    {
        if (input < ushort.MinValue)
        {
            throw new OverflowException();
        }
        return (ushort)input;
    }
}

[CommonConverter]
public class ShortToIntConverter : IConverter<short, int>
{
    public int Convert(short input)
    {
        return input;
    }
}

public class ShortToUIntConverter : IConverter<short, uint>
{
    public uint Convert(short input)
    {
        if (input < uint.MinValue)
        {
            throw new OverflowException();
        }
        return (uint)input;
    }
}

[CommonConverter]
public class ShortToLongConverter : IConverter<short, long>
{
    public long Convert(short input)
    {
        return input;
    }
}

public class ShortToULongConverter : IConverter<short, ulong>
{
    public ulong Convert(short input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return (ulong)input;
    }
}

public class ShortToFloatConverter : IConverter<short, float>
{
    public float Convert(short input)
    {
        return input;
    }
}

[CommonConverter]
public class ShortToDoubleConverter : IConverter<short, double>
{
    public double Convert(short input)
    {
        return input;
    }
}

[CommonConverter]
public class ShortToDecimalConverter : IConverter<short, decimal>
{
    public decimal Convert(short input)
    {
        return input;
    }
}

public class ShortToCharConverter : IConverter<short, char>
{
    public char Convert(short input)
    {
        if (input < char.MinValue || input > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input;
    }
}

[CommonConverter]
public class ShortToBoolConverter : IConverter<short, bool>
{
    public bool Convert(short input)
    {
        return input != 0;
    }
}

public class ShortToIntPtrConverter : IConverter<short, IntPtr>
{
    public IntPtr Convert(short input)
    {
        return new IntPtr(input);
    }
}

public class ShortToUIntPtrConverter : IConverter<short, UIntPtr>
{
    public UIntPtr Convert(short input)
    {
        if (input < 0)
        {
            throw new OverflowException();
        }
        return new UIntPtr((ushort)input);
    }
}

[CommonConverter]
public class ShortToStringConverter : IConverter<short, string>
{
    public string Convert(short input)
    {
        return input.ToString();
    }
}