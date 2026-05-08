using System;
using System.Collections.Generic;
using System.Linq;
using LeonMapper.Attributes;
using LeonMapper.Plan;
using LeonMapper.Plan.Builder;
using LeonMapper.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 测试映射验证功能
/// </summary>
[TestClass]
public class ValidationTest
{
    #region 测试模型

    // 完全匹配：所有属性都有对应映射
    public class PerfectSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PerfectTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // 目标有未映射属性
    public class UnmappedTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unmapped { get; set; } = string.Empty;
    }

    // 源有未映射属性
    public class UnmappedSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extra { get; set; } = string.Empty;
    }

    // 无参构造函数缺失
    public class NoEmptyCtorTarget
    {
        public int Id { get; set; }

        public NoEmptyCtorTarget(int id)
        {
            Id = id;
        }
    }

    // 类型转换不可行
    public class IncompatibleSource
    {
        public string Name { get; set; } = string.Empty;
    }

    public class IncompatibleTarget
    {
        // 没有 string -> int 的自动转换
        public int Name { get; set; }
    }

    // 复杂类型嵌套
    public class NestedSource
    {
        public int Id { get; set; }
        public PerfectSource Inner { get; set; } = null!;
    }

    public class NestedTarget
    {
        public int Id { get; set; }
        public PerfectTarget Inner { get; set; } = null!;
    }

    #endregion

    #region Validate() API 测试

    [TestMethod]
    public void Validate_PerfectMapping_ShouldBeValid()
    {
        var result = Mapper<PerfectSource, PerfectTarget>.Validate();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void Validate_UnmappedTarget_ShouldHaveWarning()
    {
        var result = Mapper<PerfectSource, UnmappedTarget>.Validate();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid); // 未映射是 warning，不是 error
        Assert.AreEqual(0, result.Errors.Count);
        Assert.IsTrue(result.Warnings.Count > 0);
        Assert.IsTrue(result.Warnings.Any(w => w.Contains("Unmapped")));
    }

    [TestMethod]
    public void Validate_NoEmptyConstructor_ShouldHaveError()
    {
        var result = Mapper<PerfectSource, NoEmptyCtorTarget>.Validate();

        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Count > 0);
        Assert.IsTrue(result.Errors.Any(e => e.Contains("无参构造函数")));
    }

    [TestMethod]
    public void Validate_IncompatibleType_ShouldBeValidButSkipped()
    {
        // string -> int 没有转换器，该映射会被跳过，不产生 error
        var result = Mapper<IncompatibleSource, IncompatibleTarget>.Validate();

        Assert.IsNotNull(result);
        // 验证器不检查未成功创建的映射，只检查已创建的映射
        // 所以这里应该是 Valid 的
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_NestedComplexType_ShouldBeValid()
    {
        var result = Mapper<NestedSource, NestedTarget>.Validate();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid);
    }

    #endregion

    #region GetPlan() API 测试

    [TestMethod]
    public void GetPlan_ShouldReturnPlan()
    {
        var plan = Mapper<PerfectSource, PerfectTarget>.GetPlan();

        Assert.IsNotNull(plan);
        Assert.AreEqual(typeof(PerfectSource), plan.SourceType);
        Assert.AreEqual(typeof(PerfectTarget), plan.TargetType);
    }

    [TestMethod]
    public void GetPlan_ShouldContainMappings()
    {
        var plan = Mapper<PerfectSource, PerfectTarget>.GetPlan();

        Assert.IsNotNull(plan);
        Assert.IsTrue(plan.AllMappings.Count() > 0);
        Assert.IsTrue(plan.AllMappings.Any(m => m.SourceMember.Name == "Id"));
        Assert.IsTrue(plan.AllMappings.Any(m => m.SourceMember.Name == "Name"));
    }

    [TestMethod]
    public void GetPlan_ShouldContainUnmappedTargetMembers()
    {
        var plan = Mapper<PerfectSource, UnmappedTarget>.GetPlan();

        Assert.IsNotNull(plan);
        Assert.IsTrue(plan.UnmappedTargetMembers.Count > 0);
        Assert.IsTrue(plan.UnmappedTargetMembers.Any(m => m.Name == "Unmapped"));
    }

    [TestMethod]
    public void GetPlan_ShouldContainUnmappedSourceMembers()
    {
        var plan = Mapper<UnmappedSource, PerfectTarget>.GetPlan();

        Assert.IsNotNull(plan);
        Assert.IsTrue(plan.UnmappedSourceMembers.Count > 0);
        Assert.IsTrue(plan.UnmappedSourceMembers.Any(m => m.Name == "Extra"));
    }

    [TestMethod]
    public void GetPlan_ToString_ShouldNotBeEmpty()
    {
        var plan = Mapper<PerfectSource, PerfectTarget>.GetPlan();
        var str = plan.ToString();

        Assert.IsFalse(string.IsNullOrEmpty(str));
    }

    #endregion

    #region ValidationResult 测试

    [TestMethod]
    public void ValidationResult_GetReport_ShouldContainErrorsAndWarnings()
    {
        var result = Mapper<PerfectSource, UnmappedTarget>.Validate();
        var report = result.GetReport();

        Assert.IsFalse(string.IsNullOrEmpty(report));
        // 应该有警告信息
        Assert.IsTrue(report.Contains("警告") || report.Contains("Warning"));
    }

    [TestMethod]
    public void ValidationResult_EmptyResult_ShouldBeValid()
    {
        var result = new ValidationResult(new List<string>(), new List<string>());

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
        Assert.AreEqual(0, result.Warnings.Count);
    }

    [TestMethod]
    public void ValidationResult_WithErrors_ShouldNotBeValid()
    {
        var result = new ValidationResult(new List<string> { "Error 1" }, new List<string>());

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
    }

    #endregion

    #region MappingPlanBuilder 直接测试

    [TestMethod]
    public void MappingPlanBuilder_Build_ShouldCachePlans()
    {
        var plan1 = MappingPlanBuilder.Build<PerfectSource, PerfectTarget>();
        var plan2 = MappingPlanBuilder.Build<PerfectSource, PerfectTarget>();

        // 由于缓存，应该是同一个实例
        Assert.AreSame(plan1, plan2);
    }

    [TestMethod]
    public void MappingPlanBuilder_Build_WithOptions_ShouldRespectAutoConvert()
    {
        var options = new PlanBuildOptions(autoConvert: false, converterScope: ConverterScope.Common, buildNestedPlans: true);

        var plan = MappingPlanBuilder.Build<IncompatibleSource, IncompatibleTarget>(options);

        Assert.IsNotNull(plan);
        // AutoConvert=false 时，string->int 不应创建映射
        Assert.IsFalse(plan.AllMappings.Any(m => m.SourceMember.Name == "Name"));
    }

    #endregion
}
