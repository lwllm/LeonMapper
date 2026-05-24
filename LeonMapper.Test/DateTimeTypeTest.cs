using System;
using LeonMapper.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// DateTime/Guid/DateTimeOffset/TimeSpan/DateOnly/TimeOnly 类型的直接赋值和可空映射测试
/// 验证 TypeUtils.IsBaseType 扩展后这些类型走 Direct 策略而非 Complex 嵌套映射
/// </summary>
[TestClass]
public class DateTimeTypeTest
{
    #region 测试模型

    public class DateTimeSource
    {
        public DateTime CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Duration { get; set; }
        public DateOnly BirthDate { get; set; }
        public TimeOnly StartTime { get; set; }
    }

    public class DateTimeTarget
    {
        public DateTime CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Duration { get; set; }
        public DateOnly BirthDate { get; set; }
        public TimeOnly StartTime { get; set; }
    }

    // 可空版本
    public class NullableDateTimeSource
    {
        public DateTime? CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public Guid? Id { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateOnly? BirthDate { get; set; }
        public TimeOnly? StartTime { get; set; }
    }

    public class NullableDateTimeTarget
    {
        public DateTime? CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public Guid? Id { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateOnly? BirthDate { get; set; }
        public TimeOnly? StartTime { get; set; }
    }

    // 混合可空→非可空
    public class MixedDateTimeTarget
    {
        public DateTime CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public Guid Id { get; set; }
        public TimeSpan Duration { get; set; }
    }

    // 字段版本
    public class DateTimeFieldSource
    {
        public DateTime CreatedAt;
        public Guid Id;
        public TimeSpan Duration;
    }

    public class DateTimeFieldTarget
    {
        public DateTime CreatedAt;
        public Guid Id;
        public TimeSpan Duration;
    }

    // 作为嵌套属性
    public class ParentSource
    {
        public string Name { get; set; } = string.Empty;
        public DateTimeSource Inner { get; set; } = new();
    }

    public class ParentTarget
    {
        public string Name { get; set; } = string.Empty;
        public DateTimeTarget Inner { get; set; } = new();
    }

    #endregion

    #region 同类型直接赋值（Expression）

    [TestMethod]
    public void DateTime_SameType_Expression_AllPropertiesMapped()
    {
        var source = new DateTimeSource
        {
            CreatedAt = new DateTime(2024, 6, 15, 10, 30, 0),
            ModifiedAt = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.FromHours(8)),
            Id = Guid.NewGuid(),
            Duration = TimeSpan.FromHours(2),
            BirthDate = new DateOnly(1990, 5, 20),
            StartTime = new TimeOnly(9, 30, 0)
        };

        var mapper = new Mapper<DateTimeSource, DateTimeTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt, result.CreatedAt);
        Assert.AreEqual(source.ModifiedAt, result.ModifiedAt);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Duration, result.Duration);
        Assert.AreEqual(source.BirthDate, result.BirthDate);
        Assert.AreEqual(source.StartTime, result.StartTime);
    }

    [TestMethod]
    public void DateTime_SameType_Emit_AllPropertiesMapped()
    {
        var source = new DateTimeSource
        {
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTimeOffset.Now,
            Id = Guid.NewGuid(),
            Duration = TimeSpan.FromMinutes(90),
            BirthDate = new DateOnly(2000, 1, 1),
            StartTime = new TimeOnly(14, 0)
        };

        var mapper = new Mapper<DateTimeSource, DateTimeTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt, result.CreatedAt);
        Assert.AreEqual(source.ModifiedAt, result.ModifiedAt);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Duration, result.Duration);
        Assert.AreEqual(source.BirthDate, result.BirthDate);
        Assert.AreEqual(source.StartTime, result.StartTime);
    }

    #endregion

    #region 可空同类型映射

    [TestMethod]
    public void NullableDateTime_AllWithValues_Expression_MappedCorrectly()
    {
        var source = new NullableDateTimeSource
        {
            CreatedAt = new DateTime(2024, 1, 1),
            ModifiedAt = DateTimeOffset.Now,
            Id = Guid.NewGuid(),
            Duration = TimeSpan.FromHours(1),
            BirthDate = new DateOnly(1995, 3, 15),
            StartTime = new TimeOnly(8, 0)
        };

        var mapper = new Mapper<NullableDateTimeSource, NullableDateTimeTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt, result.CreatedAt);
        Assert.AreEqual(source.ModifiedAt, result.ModifiedAt);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Duration, result.Duration);
        Assert.AreEqual(source.BirthDate, result.BirthDate);
        Assert.AreEqual(source.StartTime, result.StartTime);
    }

    [TestMethod]
    public void NullableDateTime_AllNull_Expression_MappedToNull()
    {
        var source = new NullableDateTimeSource
        {
            CreatedAt = null,
            ModifiedAt = null,
            Id = null,
            Duration = null,
            BirthDate = null,
            StartTime = null
        };

        var mapper = new Mapper<NullableDateTimeSource, NullableDateTimeTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNull(result.CreatedAt);
        Assert.IsNull(result.ModifiedAt);
        Assert.IsNull(result.Id);
        Assert.IsNull(result.Duration);
        Assert.IsNull(result.BirthDate);
        Assert.IsNull(result.StartTime);
    }

    [TestMethod]
    public void NullableDateTime_AllNull_Emit_MappedToNull()
    {
        var source = new NullableDateTimeSource();

        var mapper = new Mapper<NullableDateTimeSource, NullableDateTimeTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNull(result.CreatedAt);
        Assert.IsNull(result.Id);
        Assert.IsNull(result.Duration);
    }

    #endregion

    #region 可空→非可空

    [TestMethod]
    public void NullableDateTime_ToNonNullable_WithValue_Expression_MapsCorrectly()
    {
        var source = new NullableDateTimeSource
        {
            CreatedAt = new DateTime(2024, 6, 1),
            ModifiedAt = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero),
            Id = Guid.NewGuid(),
            Duration = TimeSpan.FromMinutes(30)
        };

        var mapper = new Mapper<NullableDateTimeSource, MixedDateTimeTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt!.Value, result.CreatedAt);
        Assert.AreEqual(source.ModifiedAt!.Value, result.ModifiedAt);
        Assert.AreEqual(source.Id!.Value, result.Id);
        Assert.AreEqual(source.Duration!.Value, result.Duration);
    }

    [TestMethod]
    public void NullableDateTime_ToNonNullable_WithNull_Expression_UsesDefaults()
    {
        var source = new NullableDateTimeSource();

        var mapper = new Mapper<NullableDateTimeSource, MixedDateTimeTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(default(DateTime), result.CreatedAt);
        Assert.AreEqual(default(Guid), result.Id);
        Assert.AreEqual(default(TimeSpan), result.Duration);
    }

    [TestMethod]
    public void NullableDateTime_ToNonNullable_WithNull_Emit_UsesDefaults()
    {
        var source = new NullableDateTimeSource();

        var mapper = new Mapper<NullableDateTimeSource, MixedDateTimeTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(default(DateTime), result.CreatedAt);
        Assert.AreEqual(default(Guid), result.Id);
        Assert.AreEqual(default(TimeSpan), result.Duration);
    }

    #endregion

    #region 字段映射

    [TestMethod]
    public void DateTime_Fields_Expression_MappedCorrectly()
    {
        var source = new DateTimeFieldSource
        {
            CreatedAt = new DateTime(2024, 12, 25),
            Id = Guid.NewGuid(),
            Duration = TimeSpan.FromDays(1)
        };

        var mapper = new Mapper<DateTimeFieldSource, DateTimeFieldTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt, result.CreatedAt);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Duration, result.Duration);
    }

    [TestMethod]
    public void DateTime_Fields_Emit_MappedCorrectly()
    {
        var source = new DateTimeFieldSource
        {
            CreatedAt = DateTime.MinValue,
            Id = Guid.Empty,
            Duration = TimeSpan.Zero
        };

        var mapper = new Mapper<DateTimeFieldSource, DateTimeFieldTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt, result.CreatedAt);
        Assert.AreEqual(source.Id, result.Id);
        Assert.AreEqual(source.Duration, result.Duration);
    }

    #endregion

    #region 嵌套属性中的 DateTime/Guid

    [TestMethod]
    public void DateTime_AsNestedProperty_Expression_MappedCorrectly()
    {
        var source = new ParentSource
        {
            Name = "Parent",
            Inner = new DateTimeSource
            {
                CreatedAt = DateTime.Now,
                Id = Guid.NewGuid(),
                Duration = TimeSpan.FromHours(3),
                ModifiedAt = DateTimeOffset.UtcNow,
                BirthDate = DateOnly.FromDateTime(DateTime.Now),
                StartTime = TimeOnly.FromDateTime(DateTime.Now)
            }
        };

        var mapper = new Mapper<ParentSource, ParentTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual("Parent", result.Name);
        Assert.IsNotNull(result.Inner);
        Assert.AreEqual(source.Inner!.CreatedAt, result.Inner!.CreatedAt);
        Assert.AreEqual(source.Inner!.Id, result.Inner!.Id);
        Assert.AreEqual(source.Inner.Duration, result.Inner.Duration);
        Assert.AreEqual(source.Inner.ModifiedAt, result.Inner.ModifiedAt);
        Assert.AreEqual(source.Inner.BirthDate, result.Inner.BirthDate);
        Assert.AreEqual(source.Inner.StartTime, result.Inner.StartTime);
    }

    [TestMethod]
    public void DateTime_AsNestedProperty_Emit_MappedCorrectly()
    {
        var source = new ParentSource
        {
            Name = "EmitParent",
            Inner = new DateTimeSource
            {
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Duration = TimeSpan.FromMinutes(45),
                ModifiedAt = DateTimeOffset.Now,
                BirthDate = new DateOnly(2000, 1, 1),
                StartTime = new TimeOnly(12, 0)
            }
        };

        var mapper = new Mapper<ParentSource, ParentTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual("EmitParent", result.Name);
        Assert.IsNotNull(result.Inner);
        Assert.AreEqual(source.Inner!.CreatedAt, result.Inner!.CreatedAt);
        Assert.AreEqual(source.Inner!.Id, result.Inner!.Id);
    }

    #endregion

    #region 两种编译器结果一致性

    [TestMethod]
    public void DateTime_BothCompilers_ProduceSameResult()
    {
        var source = new DateTimeSource
        {
            CreatedAt = new DateTime(2025, 3, 15, 14, 30, 0),
            ModifiedAt = new DateTimeOffset(2025, 3, 15, 14, 30, 0, TimeSpan.FromHours(9)),
            Id = Guid.Parse("12345678-1234-1234-1234-123456789012"),
            Duration = TimeSpan.FromTicks(123456789),
            BirthDate = new DateOnly(1990, 12, 25),
            StartTime = new TimeOnly(23, 59, 59)
        };

        var exprMapper = new Mapper<DateTimeSource, DateTimeTarget>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<DateTimeSource, DateTimeTarget>(ProcessTypeEnum.Emit);

        var exprResult = exprMapper.MapTo(source);
        var emitResult = emitMapper.MapTo(source);

        Assert.AreEqual(exprResult!.CreatedAt, emitResult!.CreatedAt);
        Assert.AreEqual(exprResult.ModifiedAt, emitResult.ModifiedAt);
        Assert.AreEqual(exprResult.Id, emitResult.Id);
        Assert.AreEqual(exprResult.Duration, emitResult.Duration);
        Assert.AreEqual(exprResult.BirthDate, emitResult.BirthDate);
        Assert.AreEqual(exprResult.StartTime, emitResult.StartTime);
    }

    #endregion
}
