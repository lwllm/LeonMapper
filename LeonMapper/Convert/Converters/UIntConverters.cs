using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 uint 类型转换为 sbyte 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 byte 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 short 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 ushort 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 int 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 long 类型
/// </summary>
public class UIntToLongConverter : IConverter<uint, long>
{
    public long Convert(uint input)
    {
        return input;
    }
}

/// <summary>
/// 将 uint 类型转换为 ulong 类型
/// </summary>
public class UIntToULongConverter : IConverter<uint, ulong>
{
    public ulong Convert(uint input)
    {
        return input;
    }
}

/// <summary>
/// 将 uint 类型转换为 float 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 double 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 decimal 类型
/// </summary>
public class UIntToDecimalConverter : IConverter<uint, decimal>
{
    public decimal Convert(uint input)
    {
        return input;
    }
}

/// <summary>
/// 将 uint 类型转换为 char 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 bool 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 IntPtr 类型
/// </summary>
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

/// <summary>
/// 将 uint 类型转换为 UIntPtr 类型
/// </summary>
public class UIntToUIntPtrConverter : IConverter<uint, UIntPtr>
{
    public UIntPtr Convert(uint input)
    {
        return (UIntPtr)input;
    }
}

/// <summary>
/// 将 uint 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class UIntToStringConverter : IConverter<uint, string>
{
    public string Convert(uint input)
    {
        return input.ToString();
    }
}