using System;
using LeonMapper.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 测试异常类型
/// </summary>
[TestClass]
public class ExceptionTest
{
    #region NoEmptyConstructorException 测试

    [TestMethod]
    public void NoEmptyConstructorException_DefaultConstructor_ShouldHaveDefaultMessage()
    {
        var ex = new NoEmptyConstructorException();

        Assert.IsNotNull(ex);
        Assert.AreEqual("目标类型缺少无参构造函数", ex.Message);
    }

    [TestMethod]
    public void NoEmptyConstructorException_WithMessage_ShouldHaveCustomMessage()
    {
        var customMessage = "自定义错误消息";
        var ex = new NoEmptyConstructorException(customMessage);

        Assert.AreEqual(customMessage, ex.Message);
    }

    [TestMethod]
    public void NoEmptyConstructorException_WithInnerException_ShouldPreserveInner()
    {
        var inner = new InvalidOperationException("内部错误");
        var ex = new NoEmptyConstructorException("外层错误", inner);

        Assert.AreEqual("外层错误", ex.Message);
        Assert.IsNotNull(ex.InnerException);
        Assert.AreEqual("内部错误", ex.InnerException.Message);
    }

    #endregion

    #region 映射时抛出异常测试

    // 没有无参构造函数的目标类型
    public class NoEmptyCtorClass
    {
        public int Id { get; set; }

        public NoEmptyCtorClass(int id)
        {
            Id = id;
        }
    }

    public class SimpleSource
    {
        public int Id { get; set; }
    }

    [TestMethod]
    [ExpectedException(typeof(NoEmptyConstructorException))]
    public void Mapper_NoEmptyConstructor_ShouldThrow()
    {
        // 创建 Mapper 时会编译映射计划，此时应该检测到缺少无参构造函数
        var mapper = new Mapper<SimpleSource, NoEmptyCtorClass>();
    }

    [TestMethod]
    public void Mapper_NoEmptyConstructor_ExceptionType_ShouldBeCorrect()
    {
        try
        {
            var mapper = new Mapper<SimpleSource, NoEmptyCtorClass>();
            Assert.Fail("应该抛出 NoEmptyConstructorException");
        }
        catch (Exception ex)
        {
            Assert.IsInstanceOfType(ex, typeof(NoEmptyConstructorException));
        }
    }

    #endregion
}
