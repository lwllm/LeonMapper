namespace LeonMapper.Benchmarks;

/// <summary>
/// 简单对象映射模型（同名同类型属性）
/// </summary>
public class SimpleSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Score { get; set; }
    public bool IsActive { get; set; }
}

public class SimpleTarget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Score { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// 嵌套复杂类型映射模型
/// </summary>
public class AddressSource
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

public class AddressTarget
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

public class NestedSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AddressSource Address { get; set; } = new();
}

public class NestedTarget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AddressTarget Address { get; set; } = new();
}

/// <summary>
/// 集合映射模型
/// </summary>
public class ItemSource
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class ItemTarget
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class CollectionSource
{
    public int Id { get; set; }
    public List<ItemSource> Items { get; set; } = new();
}

public class CollectionTarget
{
    public int Id { get; set; }
    public List<ItemTarget> Items { get; set; } = new();
}

/// <summary>
/// 类型转换映射模型（不同类型属性）
/// </summary>
public class TypeConvertSource
{
    public int IntValue { get; set; }
    public double DoubleValue { get; set; }
    public string StringValue { get; set; } = string.Empty;
}

public class TypeConvertTarget
{
    public string IntValue { get; set; } = string.Empty;
    public string DoubleValue { get; set; } = string.Empty;
    public int StringValue { get; set; }
}
