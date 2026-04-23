using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 bool 类型转换为 sbyte 类型
/// </summary>
public class BoolToSByteConverter : IConverter<bool, sbyte>
{
    public sbyte Convert(bool input)
    {
        return input ? (sbyte)1 : (sbyte)0;
    }
}

/// <summary>
/// 将 bool 类型转换为 byte 类型
/// </summary>
public class BoolToByteConverter : IConverter<bool, byte>
{
    public byte Convert(bool input)
    {
        return input ? (byte)1 : (byte)0;
    }
}

/// <summary>
/// 将 bool 类型转换为 short 类型
/// </summary>
[CommonConverter]
public class BoolToShortConverter : IConverter<bool, short>
{
    public short Convert(bool input)
    {
        return input ? (short)1 : (short)0;
    }
}

/// <summary>
/// 将 bool 类型转换为 ushort 类型
/// </summary>
public class BoolToUShortConverter : IConverter<bool, ushort>
{
    public ushort Convert(bool input)
    {
        return input ? (ushort)1 : (ushort)0;
    }
}

/// <summary>
/// 将 bool 类型转换为 int 类型
/// </summary>
[CommonConverter]
public class BoolToIntConverter : IConverter<bool, int>
{
    public int Convert(bool input)
    {
        return input ? 1 : 0;
    }
}

/// <summary>
/// 将 bool 类型转换为 uint 类型
/// </summary>
public class BoolToUIntConverter : IConverter<bool, uint>
{
    public uint Convert(bool input)
    {
        return input ? (uint)1 : (uint)0;
    }
}

/// <summary>
/// 将 bool 类型转换为 long 类型
/// </summary>
public class BoolToLongConverter : IConverter<bool, long>
{
    public long Convert(bool input)
    {
        return input ? (long)1 : (long)0;
    }
}

/// <summary>
/// 将 bool 类型转换为 ulong 类型
/// </summary>
public class BoolToULongConverter : IConverter<bool, ulong>
{
    public ulong Convert(bool input)
    {
        return input ? (ulong)1 : (ulong)0;
    }
}

/// <summary>
/// 将 bool 类型转换为 float 类型
/// </summary>
public class BoolToFloatConverter : IConverter<bool, float>
{
    public float Convert(bool input)
    {
        return input ? 1.0f : 0.0f;
    }
}

/// <summary>
/// 将 bool 类型转换为 double 类型
/// </summary>
public class BoolToDoubleConverter : IConverter<bool, double>
{
    public double Convert(bool input)
    {
        return input ? 1.0 : 0.0;
    }
}

/// <summary>
/// 将 bool 类型转换为 decimal 类型
/// </summary>
public class BoolToDecimalConverter : IConverter<bool, decimal>
{
    public decimal Convert(bool input)
    {
        return input ? 1.0m : 0.0m;
    }
}

/// <summary>
/// 将 bool 类型转换为 char 类型
/// </summary>
public class BoolToCharConverter : IConverter<bool, char>
{
    public char Convert(bool input)
    {
        return input ? 'T' : 'F';
    }
}

/// <summary>
/// 将 bool 类型转换为 IntPtr 类型
/// </summary>
public class BoolToIntPtrConverter : IConverter<bool, IntPtr>
{
    public IntPtr Convert(bool input)
    {
        if (input)
        {
            return new IntPtr(1);
        }

        return IntPtr.Zero;
    }
}

/// <summary>
/// 将 bool 类型转换为 UIntPtr 类型
/// </summary>
public class BoolToUIntPtrConverter : IConverter<bool, UIntPtr>
{
    public UIntPtr Convert(bool input)
    {
        if (input)
        {
            return new UIntPtr(1);
        }

        return UIntPtr.Zero;
    }
}

/// <summary>
/// 将 bool 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class BoolToStringConverter : IConverter<bool, string>
{
    public string Convert(bool input)
    {
        return input ? "True" : "False";
    }
}
