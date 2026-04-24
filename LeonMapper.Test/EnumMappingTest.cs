using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 测试 Enum 类型映射支持
/// </summary>
[TestClass]
public class EnumMappingTest
{
    #region 测试模型

    // 同类型 Enum
    public enum StatusEnum
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    // 不同名称但相同值的 Enum
    public enum StatusDtoEnum
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    // 不同值的 Enum
    public enum PriorityEnum
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum PriorityDtoEnum
    {
        Low = 10,
        Medium = 20,
        High = 30
    }

    // 源模型
    public class EnumSource
    {
        public StatusEnum Status { get; set; }
        public PriorityEnum Priority { get; set; }
    }

    // 同类型目标
    public class SameEnumTarget
    {
        public StatusEnum Status { get; set; }
        public PriorityEnum Priority { get; set; }
    }

    // 不同名称目标
    public class DifferentEnumTarget
    {
        public StatusDtoEnum Status { get; set; }
        public PriorityDtoEnum Priority { get; set; }
    }

    // Enum 与 int 转换
    public class EnumToIntTarget
    {
        public int Status { get; set; }
    }

    public class IntToEnumSource
    {
        public int Status { get; set; }
    }

    public class EnumToStringTarget
    {
        public string Status { get; set; } = string.Empty;
    }

    public class StringToEnumSource
    {
        public string Status { get; set; } = string.Empty;
    }

    // 可空 Enum
    public class NullableEnumSource
    {
        public StatusEnum? Status { get; set; }
    }

    public class NullableEnumTarget
    {
        public StatusEnum? Status { get; set; }
    }

    #endregion

    #region 同类型 Enum 映射

    [TestMethod]
    public void SameEnumType_ShouldMapDirectly()
    {
        var source = new EnumSource
        {
            Status = StatusEnum.Active,
            Priority = PriorityEnum.High
        };

        var mapper = new Mapper<EnumSource, SameEnumTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusEnum.Active, result.Status);
        Assert.AreEqual(PriorityEnum.High, result.Priority);
    }

    #endregion

    #region 不同 Enum 类型映射（按名称）

    [TestMethod]
    public void DifferentEnumType_WithSameNames_ShouldMapByName()
    {
        var source = new EnumSource
        {
            Status = StatusEnum.Active,
            Priority = PriorityEnum.High
        };

        var mapper = new Mapper<EnumSource, DifferentEnumTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusDtoEnum.Active, result.Status);
        Assert.AreEqual(PriorityDtoEnum.High, result.Priority);
    }

    [TestMethod]
    public void DifferentEnumType_WithDifferentValues_ShouldMapByNameNotValue()
    {
        var source = new EnumSource
        {
            Priority = PriorityEnum.Medium
        };

        var mapper = new Mapper<EnumSource, DifferentEnumTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        // 按名称映射，不是按值映射
        Assert.AreEqual(PriorityDtoEnum.Medium, result.Priority);
        // 确认值不同（Medium 在源中是 1，在目标中是 20）
        Assert.AreNotEqual((int)source.Priority, (int)result.Priority);
    }

    #endregion

    #region Enum 与基础类型转换

    [TestMethod]
    public void EnumToInt_ShouldConvert()
    {
        var source = new EnumSource
        {
            Status = StatusEnum.Pending
        };

        var mapper = new Mapper<EnumSource, EnumToIntTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Status);
    }

    [TestMethod]
    public void IntToEnum_ShouldConvert()
    {
        var source = new IntToEnumSource
        {
            Status = 2
        };

        var mapper = new Mapper<IntToEnumSource, EnumSource>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusEnum.Inactive, result.Status);
    }

    [TestMethod]
    public void EnumToString_ShouldConvert()
    {
        var source = new EnumSource
        {
            Status = StatusEnum.Active
        };

        var mapper = new Mapper<EnumSource, EnumToStringTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual("Active", result.Status);
    }

    [TestMethod]
    public void StringToEnum_ShouldConvert()
    {
        var source = new StringToEnumSource
        {
            Status = "Pending"
        };

        var mapper = new Mapper<StringToEnumSource, EnumSource>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusEnum.Pending, result.Status);
    }

    #endregion

    #region 可空 Enum 映射

    [TestMethod]
    public void NullableEnum_WithValue_ShouldMap()
    {
        var source = new NullableEnumSource
        {
            Status = StatusEnum.Active
        };

        var mapper = new Mapper<NullableEnumSource, NullableEnumTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusEnum.Active, result.Status);
    }

    [TestMethod]
    public void NullableEnum_NullValue_ShouldMapAsNull()
    {
        var source = new NullableEnumSource
        {
            Status = null
        };

        var mapper = new Mapper<NullableEnumSource, NullableEnumTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNull(result.Status);
    }

    #endregion

    #region 复杂类型中的 Enum 字段

    public class ComplexEnumSource
    {
        public string Name { get; set; } = string.Empty;
        public StatusEnum Status { get; set; }
    }

    public class ComplexEnumTarget
    {
        public string Name { get; set; } = string.Empty;
        public StatusDtoEnum Status { get; set; }
    }

    [TestMethod]
    public void ComplexType_WithEnumProperty_ShouldMap()
    {
        var source = new ComplexEnumSource
        {
            Name = "Test",
            Status = StatusEnum.Inactive
        };

        var mapper = new Mapper<ComplexEnumSource, ComplexEnumTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.Name);
        Assert.AreEqual(StatusDtoEnum.Inactive, result.Status);
    }

    #endregion
}
