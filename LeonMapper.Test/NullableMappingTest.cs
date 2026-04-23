using LeonMapper.Attributes;
using LeonMapper.Config;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 可空类型映射测试
/// </summary>
[TestClass]
public class NullableMappingTest
{
    public class NullableSource
    {
        public int? Age { get; set; }
        public int? Score { get; set; }
        public long? BigNumber { get; set; }
    }

    public class NullableTarget
    {
        public long? Age { get; set; }
        public int? Score { get; set; }
        public long? BigNumber { get; set; }
    }

    [TestMethod]
    public void NullableIntToNullableLong_BasicMapping()
    {
        var source = new NullableSource { Age = 25 };

        var mapper = new Mapper<NullableSource, NullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(25L, target.Age);
    }

    [TestMethod]
    public void NullableIntToNullableLong_NullValue()
    {
        var source = new NullableSource { Age = null };

        var mapper = new Mapper<NullableSource, NullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.Age);
    }

    [TestMethod]
    public void NullableSameType_DirectMapping()
    {
        var source = new NullableSource { Score = 100 };

        var mapper = new Mapper<NullableSource, NullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(100, target.Score);
    }

    [TestMethod]
    public void NullableSameType_NullValue()
    {
        var source = new NullableSource { Score = null };

        var mapper = new Mapper<NullableSource, NullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.Score);
    }

    [TestMethod]
    public void NullableLongToNullableLong_DirectMapping()
    {
        var source = new NullableSource { BigNumber = 9999999999L };

        var mapper = new Mapper<NullableSource, NullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(9999999999L, target.BigNumber);
    }

    #region EmitCompiler Tests

    [TestMethod]
    public void EmitCompiler_NullableIntToNullableLong_BasicMapping()
    {
        var source = new NullableSource { Age = 25 };

        var mapper = new Mapper<NullableSource, NullableTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(25L, target.Age);
    }

    [TestMethod]
    public void EmitCompiler_NullableIntToNullableLong_NullValue()
    {
        var source = new NullableSource { Age = null };

        var mapper = new Mapper<NullableSource, NullableTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.Age);
    }

    [TestMethod]
    public void EmitCompiler_NullableSameType_DirectMapping()
    {
        var source = new NullableSource { Score = 100 };

        var mapper = new Mapper<NullableSource, NullableTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(100, target.Score);
    }

    [TestMethod]
    public void BothCompilers_NullableMapping_ShouldProduceSameResult()
    {
        var source = new NullableSource
        {
            Age = 30,
            Score = 85,
            BigNumber = 123456789L
        };

        var exprMapper = new Mapper<NullableSource, NullableTarget>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<NullableSource, NullableTarget>(ProcessTypeEnum.Emit);

        var exprResult = exprMapper.MapTo(source);
        var emitResult = emitMapper.MapTo(source);

        Assert.IsNotNull(exprResult);
        Assert.IsNotNull(emitResult);

        Assert.AreEqual(exprResult.Age, emitResult.Age);
        Assert.AreEqual(exprResult.Score, emitResult.Score);
        Assert.AreEqual(exprResult.BigNumber, emitResult.BigNumber);
    }

    #endregion

    #region 边界测试

    public class MixedNullableSource
    {
        [MapTo(nameof(MixedNullableTarget.NullableInt))]
        public int NonNullableInt { get; set; }

        [MapTo(nameof(MixedNullableTarget.NonNullableInt))]
        public int? NullableInt { get; set; }
    }

    public class MixedNullableTarget
    {
        public int NonNullableInt { get; set; }
        public int? NullableInt { get; set; }
    }

    [TestMethod]
    public void NullableToNonNullable_WithValue()
    {
        var source = new MixedNullableSource { NullableInt = 42 };

        var mapper = new Mapper<MixedNullableSource, MixedNullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(42, target.NonNullableInt);
    }

    [TestMethod]
    public void NullableToNonNullable_NullValue_ReturnsDefault()
    {
        var source = new MixedNullableSource { NullableInt = null };

        var mapper = new Mapper<MixedNullableSource, MixedNullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(0, target.NonNullableInt);
    }

    [TestMethod]
    public void NonNullableToNullable()
    {
        var source = new MixedNullableSource { NonNullableInt = 99 };

        var mapper = new Mapper<MixedNullableSource, MixedNullableTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(99, target.NullableInt);
    }

    [TestMethod]
    public void EmitCompiler_NullableToNonNullable_WithValue()
    {
        var source = new MixedNullableSource { NullableInt = 42 };

        var mapper = new Mapper<MixedNullableSource, MixedNullableTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(42, target.NonNullableInt);
    }

    [TestMethod]
    public void EmitCompiler_NonNullableToNullable()
    {
        var source = new MixedNullableSource { NonNullableInt = 99 };

        var mapper = new Mapper<MixedNullableSource, MixedNullableTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.AreEqual(99, target.NullableInt);
    }

    #endregion
}
