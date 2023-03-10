using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

[CommonConverter]
public class StringToIntConverter : IConverter<string, int>
{
    public int Convert(string input)
    {
        return int.Parse(input);
    }
}

public class StringToUintConverter : IConverter<string, uint>
{
    public uint Convert(string input)
    {
        return uint.Parse(input);
    }
}

[CommonConverter]
public class StringToShortConverter : IConverter<string, short>
{
    public short Convert(string input)
    {
        return short.Parse(input);
    }
}

public class StringToUshortConverter : IConverter<string, ushort>
{
    public ushort Convert(string input)
    {
        return ushort.Parse(input);
    }
}

[CommonConverter]
public class StringToLongConverter : IConverter<string, long>
{
    public long Convert(string input)
    {
        return long.Parse(input);
    }
}

public class StringToUlongConverter : IConverter<string, ulong>
{
    public ulong Convert(string input)
    {
        return ulong.Parse(input);
    }
}

[CommonConverter]
public class StringToDoubleConverter : IConverter<string, double>
{
    public double Convert(string input)
    {
        return double.Parse(input);
    }
}

public class StringToFloatConverter : IConverter<string, float>
{
    public float Convert(string input)
    {
        return float.Parse(input);
    }
}

[CommonConverter]
public class StringToDecimalConverter : IConverter<string, decimal>
{
    public decimal Convert(string input)
    {
        return decimal.Parse(input);
    }
}

[CommonConverter]
public class StringToDateTimeConverter : IConverter<string, DateTime>
{
    public DateTime Convert(string input)
    {
        return DateTime.Parse(input);
    }
}

// public class StringToEnumConverter<TEnum> : IConverter<string, TEnum> where TEnum : struct, Enum
// {
//     public TEnum Convert(string input)
//     {
//         return Enum.Parse<TEnum>(input);
//     }
// }

public class StringToSbyteConverter : IConverter<string, sbyte>
{
    public sbyte Convert(string input)
    {
        return sbyte.Parse(input);
    }
}

public class StringToByteConverter : IConverter<string, byte>
{
    public byte Convert(string input)
    {
        return byte.Parse(input);
    }
}

[CommonConverter]
public class StringToBoolConverter : IConverter<string, bool>
{
    public bool Convert(string input)
    {
        return bool.Parse(input);
    }
}

public class StringToCharConverter : IConverter<string, char>
{
    public char Convert(string input)
    {
        return char.Parse(input);
    }
}

public class StringToIntPtrConverter : IConverter<string, IntPtr>
{
    public IntPtr Convert(string input)
    {
        return IntPtr.Parse(input);
    }
}

public class StringToUIntPtrConverter : IConverter<string, UIntPtr>
{
    public UIntPtr Convert(string input)
    {
        return UIntPtr.Parse(input);
    }
}