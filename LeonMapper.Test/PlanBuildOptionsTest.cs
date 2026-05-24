using System.Linq;
using LeonMapper.Plan.Builder;
using System;
using LeonMapper.Plan;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// PlanBuildOptions 的等值性、缓存隔离和自定义选项测试
/// </summary>
[TestClass]
public class PlanBuildOptionsTest
{
    #region 等值性和哈希

    [TestMethod]
    public void DefaultOptions_Equals_DefaultOptions()
    {
        var opt1 = PlanBuildOptions.Default;
        var opt2 = new PlanBuildOptions();

        Assert.IsTrue(opt1.Equals(opt2));
        Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
    }

    [TestMethod]
    public void SameOptions_Equals_True()
    {
        var opt1 = new PlanBuildOptions(true, ConverterScope.Common, true);
        var opt2 = new PlanBuildOptions(true, ConverterScope.Common, true);

        Assert.IsTrue(opt1.Equals(opt2));
        Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
    }

    [TestMethod]
    public void DifferentOptions_Equals_False()
    {
        var opt1 = new PlanBuildOptions(true, ConverterScope.Common, true);
        var opt2 = new PlanBuildOptions(false, ConverterScope.Common, true);

        Assert.IsFalse(opt1.Equals(opt2));
    }

    [TestMethod]
    public void DifferentScope_Equals_False()
    {
        var opt1 = new PlanBuildOptions(true, ConverterScope.Common, true);
        var opt2 = new PlanBuildOptions(true, ConverterScope.All, true);

        Assert.IsFalse(opt1.Equals(opt2));
    }

    [TestMethod]
    public void DifferentBuildNested_Equals_False()
    {
        var opt1 = new PlanBuildOptions(true, ConverterScope.Common, true);
        var opt2 = new PlanBuildOptions(true, ConverterScope.Common, false);

        Assert.IsFalse(opt1.Equals(opt2));
    }

    [TestMethod]
    public void Equals_Null_ShouldReturnFalse()
    {
        var opt = PlanBuildOptions.Default;
        Assert.IsFalse(opt.Equals(null));
    }

    [TestMethod]
    public void Equals_DifferentType_ShouldReturnFalse()
    {
        var opt = PlanBuildOptions.Default;
        Assert.IsFalse(opt.Equals("not a PlanBuildOptions"));
    }

    [TestMethod]
    public void Equals_SameReference_ShouldReturnTrue()
    {
        var opt = PlanBuildOptions.Default;
        Assert.IsTrue(opt.Equals(opt));
    }

    [TestMethod]
    public void GetHashCode_SameOptions_SameHash()
    {
        var opt1 = new PlanBuildOptions(true, ConverterScope.All, false);
        var opt2 = new PlanBuildOptions(true, ConverterScope.All, false);
        Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
    }

    #endregion

    #region 属性值验证

    [TestMethod]
    public void DefaultOptions_PropertyValues()
    {
        var opt = PlanBuildOptions.Default;
        Assert.IsTrue(opt.AutoConvert);
        Assert.AreEqual(ConverterScope.Common, opt.ConverterScope);
        Assert.IsTrue(opt.BuildNestedPlans);
    }

    [TestMethod]
    public void CustomOptions_PropertyValues()
    {
        var opt = new PlanBuildOptions(false, ConverterScope.All, false);
        Assert.IsFalse(opt.AutoConvert);
        Assert.AreEqual(ConverterScope.All, opt.ConverterScope);
        Assert.IsFalse(opt.BuildNestedPlans);
    }

    #endregion

    #region IEquatable<object> 测试

    [TestMethod]
    public void Equals_ObjectOverload_WithCorrectType()
    {
        object obj = new PlanBuildOptions(true, ConverterScope.Common, true);
        var opt = new PlanBuildOptions(true, ConverterScope.Common, true);
        Assert.IsTrue(opt.Equals(obj));
    }

    [TestMethod]
    public void Equals_ObjectOverload_WithNull()
    {
        var opt = PlanBuildOptions.Default;
        Assert.IsFalse(opt.Equals((object?)null));
    }

    #endregion

    #region BuildNestedPlans=false 对复杂类型的影响

    public class NestedSource
    {
        public string Name { get; set; } = string.Empty;
        public InnerSource? Inner { get; set; }
    }

    public class InnerSource
    {
        public int Value { get; set; }
    }

    public class NestedTarget
    {
        public string Name { get; set; } = string.Empty;
        public InnerTarget? Inner { get; set; }
    }

    public class InnerTarget
    {
        public int Value { get; set; }
    }

    [TestMethod]
    public void BuildNestedPlans_False_ComplexPropertyShouldBeNull()
    {
        var options = new PlanBuildOptions(true, ConverterScope.Common, buildNestedPlans: false);
        var plan = MappingPlanBuilder.Build<NestedSource, NestedTarget>(options);

        // 嵌套计划应该为 null
        var innerMapping = plan.PropertyMappings.FirstOrDefault(m => m.TargetMember.Name == "Inner");
        Assert.IsNotNull(innerMapping);
        Assert.AreEqual(MappingStrategy.Complex, innerMapping.Strategy);
        Assert.IsNull(innerMapping.NestedPlan);
    }

    [TestMethod]
    public void BuildNestedPlans_True_ComplexPropertyShouldHavePlan()
    {
        var options = new PlanBuildOptions(true, ConverterScope.Common, buildNestedPlans: true);
        var plan = MappingPlanBuilder.Build<NestedSource, NestedTarget>(options);

        var innerMapping = plan.PropertyMappings.FirstOrDefault(m => m.TargetMember.Name == "Inner");
        Assert.IsNotNull(innerMapping);
        Assert.AreEqual(MappingStrategy.Complex, innerMapping.Strategy);
        Assert.IsNotNull(innerMapping.NestedPlan);
    }

    #endregion

    #region AutoConvert=false 对类型转换的影响

    public class TypeConvertSource
    {
        public int Number { get; set; }
    }

    public class TypeConvertTarget
    {
        public string Number { get; set; } = string.Empty;
    }

    [TestMethod]
    public void AutoConvert_False_ShouldNotMapDifferentTypes()
    {
        var options = new PlanBuildOptions(autoConvert: false, ConverterScope.Common, buildNestedPlans: true);
        var plan = MappingPlanBuilder.Build<TypeConvertSource, TypeConvertTarget>(options);

        // Number: int -> string 应该因为 AutoConvert=false 而不映射
        var numberMapping = plan.PropertyMappings.FirstOrDefault(m => m.TargetMember.Name == "Number");
        Assert.IsNull(numberMapping, "AutoConvert=false 时 int->string 不应映射");

        // Number 应该在未映射目标列表中
        Assert.IsTrue(plan.UnmappedTargetMembers.Any(m => m.Name == "Number"));
    }

    [TestMethod]
    public void AutoConvert_True_ShouldMapDifferentTypes()
    {
        var options = new PlanBuildOptions(autoConvert: true, ConverterScope.Common, buildNestedPlans: true);
        var plan = MappingPlanBuilder.Build<TypeConvertSource, TypeConvertTarget>(options);

        var numberMapping = plan.PropertyMappings.FirstOrDefault(m => m.TargetMember.Name == "Number");
        Assert.IsNotNull(numberMapping, "AutoConvert=true 时 int->string 应映射");
        Assert.AreEqual(MappingStrategy.Convert, numberMapping.Strategy);
    }

    #endregion
}
