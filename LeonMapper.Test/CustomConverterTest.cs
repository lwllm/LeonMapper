using System;
using System.Linq;
using LeonMapper.Convert;
using LeonMapper.Plan;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 测试 ConvertFactory 和内置转换器功能
/// </summary>
[TestClass]
public class CustomConverterTest
{
    #region 测试模型

    public class IntToStringSource
    {
        public int Number { get; set; }
    }

    public class IntToStringTarget
    {
        public string Number { get; set; } = string.Empty;
    }

    public class StringToIntSource
    {
        public string Number { get; set; } = string.Empty;
    }

    public class StringToIntTarget
    {
        public int Number { get; set; }
    }

    public class DoubleToIntSource
    {
        public double Value { get; set; }
    }

    public class DoubleToIntTarget
    {
        public int Value { get; set; }
    }

    #endregion

    #region ConvertFactory 基础测试

    [TestMethod]
    public void ConvertFactory_HasConverter_IntToString_Common_ShouldReturnTrue()
    {
        var hasConverter = ConvertFactory.HasConverter(typeof(int), typeof(string), ConverterScope.Common);
        Assert.IsTrue(hasConverter);
    }

    [TestMethod]
    public void ConvertFactory_HasConverter_StringToInt_Common_ShouldReturnTrue()
    {
        var hasConverter = ConvertFactory.HasConverter(typeof(string), typeof(int), ConverterScope.Common);
        Assert.IsTrue(hasConverter);
    }

    [TestMethod]
    public void ConvertFactory_HasConverter_DoubleToInt_Common_ShouldReturnTrue()
    {
        var hasConverter = ConvertFactory.HasConverter(typeof(double), typeof(int), ConverterScope.Common);
        Assert.IsTrue(hasConverter);
    }

    [TestMethod]
    public void ConvertFactory_HasConverter_NonExistent_ShouldReturnFalse()
    {
        // 没有 int -> DateTime 的转换器
        var hasConverter = ConvertFactory.HasConverter(typeof(int), typeof(DateTime), ConverterScope.All);
        Assert.IsFalse(hasConverter);
    }

    [TestMethod]
    public void ConvertFactory_GetConverter_IntToString_ShouldReturnInstance()
    {
        var converter = ConvertFactory.GetConverter(typeof(int), typeof(string), ConverterScope.Common);

        Assert.IsNotNull(converter);
    }

    [TestMethod]
    public void ConvertFactory_GetConverter_NonExistent_ShouldReturnNull()
    {
        var converter = ConvertFactory.GetConverter(typeof(int), typeof(DateTime), ConverterScope.All);
        Assert.IsNull(converter);
    }

    #endregion

    #region 内置转换器映射测试

    [TestMethod]
    public void Mapper_IntToString_ShouldUseBuiltInConversion()
    {
        var source = new IntToStringSource
        {
            Number = 42
        };

        var mapper = new Mapper<IntToStringSource, IntToStringTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual("42", result.Number);
    }

    [TestMethod]
    public void Mapper_StringToInt_ShouldUseBuiltInConversion()
    {
        var source = new StringToIntSource
        {
            Number = "123"
        };

        var mapper = new Mapper<StringToIntSource, StringToIntTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(123, result.Number);
    }

    [TestMethod]
    public void Mapper_DoubleToInt_ShouldUseBuiltInConversion()
    {
        var source = new DoubleToIntSource
        {
            Value = 3.14
        };

        var mapper = new Mapper<DoubleToIntSource, DoubleToIntTarget>();
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Value);
    }

    #endregion

    #region ConverterScope 测试

    [TestMethod]
    public void Mapper_WithCommonScope_ShouldUseCommonConverters()
    {
        var source = new IntToStringSource
        {
            Number = 100
        };

        var options = new PlanBuildOptions
        {
            ConverterScope = ConverterScope.Common,
            AutoConvert = true
        };

        var plan = LeonMapper.Plan.Builder.MappingPlanBuilder.Build<IntToStringSource, IntToStringTarget>(options);

        // int -> string 是 Common 转换器，应该有映射
        Assert.IsTrue(plan.AllMappings.Any(m => m.SourceMember.Name == "Number"));
    }

    [TestMethod]
    public void Mapper_WithAllScope_ShouldIncludeAllConverters()
    {
        var source = new IntToStringSource
        {
            Number = 100
        };

        var options = new PlanBuildOptions
        {
            ConverterScope = ConverterScope.All,
            AutoConvert = true
        };

        var plan = LeonMapper.Plan.Builder.MappingPlanBuilder.Build<IntToStringSource, IntToStringTarget>(options);

        // int -> string 在 All 范围也应该可用
        Assert.IsTrue(plan.AllMappings.Any(m => m.SourceMember.Name == "Number"));
    }

    #endregion

    #region 转换器执行测试

    [TestMethod]
    public void BuiltInConverter_IntToString_Execute_ShouldReturnCorrectResult()
    {
        var converter = ConvertFactory.GetConverter(typeof(int), typeof(string), ConverterScope.Common);
        Assert.IsNotNull(converter);

        // 通过反射调用 Convert 方法
        var convertMethod = converter.GetType().GetMethod("Convert");
        Assert.IsNotNull(convertMethod);

        var result = convertMethod.Invoke(converter, new object[] { 42 });
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void BuiltInConverter_StringToInt_Execute_ShouldReturnCorrectResult()
    {
        var converter = ConvertFactory.GetConverter(typeof(string), typeof(int), ConverterScope.Common);
        Assert.IsNotNull(converter);

        var convertMethod = converter.GetType().GetMethod("Convert");
        Assert.IsNotNull(convertMethod);

        var result = convertMethod.Invoke(converter, new object[] { "456" });
        Assert.AreEqual(456, result);
    }

    #endregion
}
