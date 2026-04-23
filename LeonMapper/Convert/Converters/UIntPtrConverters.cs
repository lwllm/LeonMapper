using LeonMapper.Convert.Attributes;

namespace LeonMapper.Convert.Converters;

/// <summary>
/// 将 UIntPtr 类型转换为 sbyte 类型
/// </summary>
public class UIntPtrToSByteConverter : IConverter<UIntPtr, sbyte>
{
    public sbyte Convert(UIntPtr input)
    {
        if (input.ToUInt64() > (long)sbyte.MaxValue)
        {
            throw new OverflowException();
        }
        return (sbyte)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 byte 类型
/// </summary>
public class UIntPtrToByteConverter : IConverter<UIntPtr, byte>
{
    public byte Convert(UIntPtr input)
    {
        if (input.ToUInt64() > byte.MaxValue)
        {
            throw new OverflowException();
        }
        return (byte)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 short 类型
/// </summary>
public class UIntPtrToShortConverter : IConverter<UIntPtr, short>
{
    public short Convert(UIntPtr input)
    {
        if (input.ToUInt64() > (long)short.MaxValue)
        {
            throw new OverflowException();
        }
        return (short)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 ushort 类型
/// </summary>
public class UIntPtrToUShortConverter : IConverter<UIntPtr, ushort>
{
    public ushort Convert(UIntPtr input)
    {
        if (input.ToUInt64() > ushort.MaxValue)
        {
            throw new OverflowException();
        }
        return (ushort)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 int 类型
/// </summary>
public class UIntPtrToIntConverter : IConverter<UIntPtr, int>
{
    public int Convert(UIntPtr input)
    {
        if (input.ToUInt64() > int.MaxValue)
        {
            throw new OverflowException();
        }
        return (int)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 uint 类型
/// </summary>
public class UIntPtrToUIntConverter : IConverter<UIntPtr, uint>
{
    public uint Convert(UIntPtr input)
    {
        if (input.ToUInt64() > uint.MaxValue)
        {
            throw new OverflowException();
        }
        return (uint)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 long 类型
/// </summary>
public class UIntPtrToLongConverter : IConverter<UIntPtr, long>
{
    public long Convert(UIntPtr input)
    {
        if (input.ToUInt64() > long.MaxValue)
        {
            throw new OverflowException();
        }
        return (long)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 ulong 类型
/// </summary>
public class UIntPtrToULongConverter : IConverter<UIntPtr, ulong>
{
    public ulong Convert(UIntPtr input)
    {
        return input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 float 类型
/// </summary>
public class UIntPtrToFloatConverter : IConverter<UIntPtr, float>
{
    public float Convert(UIntPtr input)
    {
        if (input.ToUInt64() > float.MaxValue)
        {
            throw new OverflowException();
        }
        return (float)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 double 类型
/// </summary>
public class UIntPtrToDoubleConverter : IConverter<UIntPtr, double>
{
    public double Convert(UIntPtr input)
    {
        return (double)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 decimal 类型
/// </summary>
public class UIntPtrToDecimalConverter : IConverter<UIntPtr, decimal>
{
    public decimal Convert(UIntPtr input)
    {
        return (decimal)input.ToUInt64();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 char 类型
/// </summary>
public class UIntPtrToCharConverter : IConverter<UIntPtr, char>
{
    public char Convert(UIntPtr input)
    {
        if (input.ToUInt64() > char.MaxValue)
        {
            throw new OverflowException();
        }
        return (char)input.ToUInt32();
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 bool 类型
/// </summary>
public class UIntPtrToBoolConverter : IConverter<UIntPtr, bool>
{
    public bool Convert(UIntPtr input)
    {
        return input.ToUInt64() != 0;
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 IntPtr 类型
/// </summary>
public class UIntPtrToIntPtrConverter : IConverter<UIntPtr, IntPtr>
{
    public IntPtr Convert(UIntPtr input)
    {
        if (input.ToUInt64() > (ulong)IntPtr.MaxValue)
        {
            throw new OverflowException();
        }
        return new IntPtr((long)input.ToUInt64());
    }
}

/// <summary>
/// 将 UIntPtr 类型转换为 string 类型
/// </summary>
[CommonConverter]
public class UIntPtrToStringConverter : IConverter<UIntPtr, string>
{
    public string Convert(UIntPtr input)
    {
        return input.ToUInt64().ToString();
    }
}
