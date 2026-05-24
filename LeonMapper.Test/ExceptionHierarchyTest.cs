using System;
using LeonMapper.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 异常类型体系测试（覆盖新增的 MappingException 基类和子类）
/// </summary>
[TestClass]
public class ExceptionHierarchyTest
{
    #region MappingException 基类测试

    [TestMethod]
    public void NoEmptyConstructorException_ShouldInheritFrom_MappingException()
    {
        var ex = new NoEmptyConstructorException();
        Assert.IsInstanceOfType(ex, typeof(MappingException));
    }

    [TestMethod]
    public void ConverterNotFoundException_ShouldInheritFrom_MappingException()
    {
        var ex = new ConverterNotFoundException(typeof(int), typeof(Guid));
        Assert.IsInstanceOfType(ex, typeof(MappingException));
    }

    [TestMethod]
    public void InvalidMappingConfigurationException_ShouldInheritFrom_MappingException()
    {
        var ex = new InvalidMappingConfigurationException("test");
        Assert.IsInstanceOfType(ex, typeof(MappingException));
    }

    #endregion

    #region NoEmptyConstructorException 测试

    [TestMethod]
    public void NoEmptyConstructorException_WithType_ShouldIncludeTypeName()
    {
        var ex = new NoEmptyConstructorException(typeof(string));
        Assert.IsNotNull(ex.Message);
        Assert.IsTrue(ex.Message.Contains("String"));
        Assert.IsNull(ex.SourceType);
        Assert.AreEqual(typeof(string), ex.TargetType);
    }

    #endregion

    #region ConverterNotFoundException 测试

    [TestMethod]
    public void ConverterNotFoundException_WithTypes_ShouldSetProperties()
    {
        var ex = new ConverterNotFoundException(typeof(int), typeof(Guid));

        Assert.AreEqual(typeof(int), ex.SourceType);
        Assert.AreEqual(typeof(Guid), ex.TargetType);
        Assert.IsNotNull(ex.Message);
        Assert.IsTrue(ex.Message.Contains("Int32"));
        Assert.IsTrue(ex.Message.Contains("Guid"));
    }

    [TestMethod]
    public void ConverterNotFoundException_WithMessage_ShouldSetMessage()
    {
        var ex = new ConverterNotFoundException("custom message");
        Assert.AreEqual("custom message", ex.Message);
    }

    #endregion

    #region InvalidMappingConfigurationException 测试

    [TestMethod]
    public void InvalidMappingConfigurationException_WithMessage_ShouldSetMessage()
    {
        var ex = new InvalidMappingConfigurationException("config error");
        Assert.AreEqual("config error", ex.Message);
        Assert.IsNull(ex.TargetMemberName);
    }

    [TestMethod]
    public void InvalidMappingConfigurationException_WithMemberName_ShouldSetProperties()
    {
        var ex = new InvalidMappingConfigurationException("bad config", "MyProperty");
        Assert.AreEqual("bad config", ex.Message);
        Assert.AreEqual("MyProperty", ex.TargetMemberName);
    }

    #endregion

    #region 异常可捕获性集成测试

    public class NoCtorTarget
    {
        public int Value { get; set; }
        public NoCtorTarget(int value) => Value = value;
    }

    public class SimpleSource
    {
        public int Value { get; set; }
    }

    [TestMethod]
    public void Mapper_ThrowsNoEmptyConstructor_ShouldBeCatchableAsMappingException()
    {
        try
        {
            var mapper = new Mapper<SimpleSource, NoCtorTarget>();
            Assert.Fail("Should have thrown");
        }
        catch (MappingException ex)
        {
            // 应该能通过基类捕获
            Assert.IsInstanceOfType(ex, typeof(NoEmptyConstructorException));
        }
    }

    [TestMethod]
    public void Mapper_ThrowsNoEmptyConstructor_ShouldBeCatchableAsSystemException()
    {
        try
        {
            var mapper = new Mapper<SimpleSource, NoCtorTarget>();
            Assert.Fail("Should have thrown");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(NoEmptyConstructorException));
        }
    }

    #endregion
}
