namespace LeonMapper.Convert.Converters;

public class BoolToSByteConverter : IConverter<bool, sbyte>
{
    public sbyte Convert(bool input)
    {
        return input ? (sbyte)1 : (sbyte)0;
    }
}

public class BoolToByteConverter : IConverter<bool, byte>
{
    public byte Convert(bool input)
    {
        return input ? (byte)1 : (byte)0;
    }
}

public class BoolToShortConverter : IConverter<bool, short>
{
    public short Convert(bool input)
    {
        return input ? (short)1 : (short)0;
    }
}

public class BoolToUShortConverter : IConverter<bool, ushort>
{
    public ushort Convert(bool input)
    {
        return input ? (ushort)1 : (ushort)0;
    }
}

public class BoolToIntConverter : IConverter<bool, int>
{
    public int Convert(bool input)
    {
        return input ? 1 : 0;
    }
}

public class BoolToUIntConverter : IConverter<bool, uint>
{
    public uint Convert(bool input)
    {
        return input ? (uint)1 : (uint)0;
    }
}

public class BoolToLongConverter : IConverter<bool, long>
{
    public long Convert(bool input)
    {
        return input ? (long)1 : (long)0;
    }
}

public class BoolToULongConverter : IConverter<bool, ulong>
{
    public ulong Convert(bool input)
    {
        return input ? (ulong)1 : (ulong)0;
    }
}

public class BoolToFloatConverter : IConverter<bool, float>
{
    public float Convert(bool input)
    {
        return input ? 1.0f : 0.0f;
    }
}

public class BoolToDoubleConverter : IConverter<bool, double>
{
    public double Convert(bool input)
    {
        return input ? 1.0 : 0.0;
    }
}

public class BoolToDecimalConverter : IConverter<bool, decimal>
{
    public decimal Convert(bool input)
    {
        return input ? 1.0m : 0.0m;
    }
}

public class BoolToCharConverter : IConverter<bool, char>
{
    public char Convert(bool input)
    {
        return input ? 'T' : 'F';
    }
}

public class BoolToIntPtrConverter : IConverter<bool, IntPtr>
{
    public IntPtr Convert(bool input)
    {
        return input ? new IntPtr(1) : IntPtr.Zero;
    }
}

public class BoolToUIntPtrConverter : IConverter<bool, UIntPtr>
{
    public UIntPtr Convert(bool input)
    {
        return input ? new UIntPtr(1) : UIntPtr.Zero;
    }
}

public class BoolToStringConverter : IConverter<bool, string>
{
    public string Convert(bool input)
    {
        return input ? "True" : "False";
    }
}
