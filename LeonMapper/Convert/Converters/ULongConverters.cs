using LeonMapper.Convert.Attributes;

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
        if (input > int.MaxValue)
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
        if (input > long.MaxValue)
        {
            throw new OverflowException();
        }
        return (long)input;
    }
}

public class ULongToFloatConverter : IConverter<ulong, float>
{
    public float Convert(ulong input)
    {
        if (input > float.MaxValue)
            throw new OverflowException();

        return input;
    }
}

public class ULongToDoubleConverter : IConverter<ulong, double>
{
    public double Convert(ulong input)
    {
        if (input > double.MaxValue)
            throw new OverflowException();

        return input;
    }
}

public class ULongToDecimalConverter : IConverter<ulong, decimal>
{
    public decimal Convert(ulong input)
    {
        return input;
    }
}

public class ULongToCharConverter : IConverter<ulong, char>
{
    public char Convert(ulong input)
    {
        if (input > char.MaxValue)
            throw new OverflowException();

        return (char)input;
    }
}

public class ULongToBoolConverter : IConverter<ulong, bool>
{
    public bool Convert(ulong input)
    {
        return input != 0;
    }
}

public class ULongToIntPtrConverter : IConverter<ulong, IntPtr>
{
    public IntPtr Convert(ulong input)
    {
        if (input > (ulong)IntPtr.MaxValue)
            throw new OverflowException();

        return (IntPtr)input;
    }
}

public class ULongToUIntPtrConverter : IConverter<ulong, UIntPtr>
{
    public UIntPtr Convert(ulong input)
    {
        if (input > (ulong)UIntPtr.MaxValue)
            throw new OverflowException();

        return (UIntPtr)input;
    }
}

[CommonConverter]
public class ULongToStringConverter : IConverter<ulong, string>
{
    public string Convert(ulong input)
    {
        return input.ToString();
    }
}
