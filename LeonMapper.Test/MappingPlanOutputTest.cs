using LeonMapper.Plan.Builder;
using System.Collections.Generic;
using System;
using System.Linq;
using LeonMapper.Attributes;
using LeonMapper.Plan;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// TypeMappingPlan.ToString() 输出和 MemberMapping.ToString() 测试
/// </summary>
[TestClass]
public class MappingPlanOutputTest
{
    #region 测试模型

    public class PlanSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Score { get; set; }
        public ChildSource? Child { get; set; }
    }

    public class ChildSource
    {
        public string Tag { get; set; } = string.Empty;
    }

    public class PlanTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Score { get; set; }
        public ChildTarget? Child { get; set; }
        public string Extra { get; set; } = string.Empty;
    }

    public class ChildTarget
    {
        public string Tag { get; set; } = string.Empty;
    }

    // 带集合属性
    public class CollectionPlanSource
    {
        public string[] Tags { get; set; } = Array.Empty<string>();
    }

    public class CollectionPlanTarget
    {
        public string[] Tags { get; set; } = Array.Empty<string>();
    }

    // 带字典属性
    public class DictPlanSource
    {
        public Dictionary<string, int> Data { get; set; } = new();
    }

    public class DictPlanTarget
    {
        public Dictionary<string, int> Data { get; set; } = new();
    }

    // 带类型转换
    public class ConvertPlanSource
    {
        public int Value { get; set; }
    }

    public class ConvertPlanTarget
    {
        public string Value { get; set; } = string.Empty;
    }

    #endregion

    [TestMethod]
    public void Plan_ToString_ShouldContainSourceAndTargetNames()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var str = plan.ToString();

        Assert.IsTrue(str.Contains("PlanSource"), "应包含源类型名");
        Assert.IsTrue(str.Contains("PlanTarget"), "应包含目标类型名");
    }

    [TestMethod]
    public void Plan_ToString_ShouldContainPropertyMappings()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var str = plan.ToString();

        Assert.IsTrue(str.Contains("Id"), "应包含 Id 属性映射");
        Assert.IsTrue(str.Contains("Name"), "应包含 Name 属性映射");
        Assert.IsTrue(str.Contains("Score"), "应包含 Score 属性映射");
    }

    [TestMethod]
    public void Plan_ToString_ShouldContainUnmappedTargetMembers()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var str = plan.ToString();

        Assert.IsTrue(str.Contains("Extra"), "应包含未映射的目标成员 Extra");
    }

    [TestMethod]
    public void Plan_ToString_ShouldContainComplexMapping()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var str = plan.ToString();

        Assert.IsTrue(str.Contains("Child"), "应包含 Child 嵌套映射");
        Assert.IsTrue(str.Contains("Complex"), "嵌套映射策略应显示为 Complex");
    }

    [TestMethod]
    public void Plan_ToString_ShouldContainDirectMapping()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var str = plan.ToString();

        // 同类型映射应显示为 Direct
        Assert.IsTrue(str.Contains("Direct"), "应包含 Direct 策略");
    }

    [TestMethod]
    public void Plan_ToString_ShouldContainConvertMapping()
    {
        var plan = MappingPlanBuilder.Build<ConvertPlanSource, ConvertPlanTarget>();
        var str = plan.ToString();

        Assert.IsTrue(str.Contains("Convert"), "int->string 应使用 Convert 策略");
    }

    [TestMethod]
    public void Plan_ToString_ShouldContainCollectionMapping()
    {
        var plan = MappingPlanBuilder.Build<CollectionPlanSource, CollectionPlanTarget>();
        var str = plan.ToString();

        Assert.IsTrue(str.Contains("Tags"), "应包含 Tags 集合映射");
    }

    [TestMethod]
    public void Plan_ToString_ShouldContainDictionaryMapping()
    {
        var plan = MappingPlanBuilder.Build<DictPlanSource, DictPlanTarget>();
        var str = plan.ToString();

        Assert.IsTrue(str.Contains("Data"), "应包含 Data 字典映射");
    }

    [TestMethod]
    public void Plan_AllMappings_ShouldCombinePropertiesAndFields()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var allCount = plan.AllMappings.Count();
        var propCount = plan.PropertyMappings.Count;
        var fieldCount = plan.FieldMappings.Count;

        Assert.AreEqual(propCount + fieldCount, allCount);
    }

    [TestMethod]
    public void MemberMapping_ToString_DirectStrategy()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var directMapping = plan.PropertyMappings.FirstOrDefault(m => m.SourceMember.Name == "Id");
        Assert.IsNotNull(directMapping);

        var str = directMapping.ToString();
        Assert.IsTrue(str.Contains("Id"));
        Assert.IsTrue(str.Contains("Direct"));
    }

    [TestMethod]
    public void MemberMapping_ToString_ComplexStrategy()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();
        var complexMapping = plan.PropertyMappings.FirstOrDefault(m => m.SourceMember.Name == "Child");
        Assert.IsNotNull(complexMapping);

        var str = complexMapping.ToString();
        Assert.IsTrue(str.Contains("Child"));
        Assert.IsTrue(str.Contains("Complex"));
    }

    [TestMethod]
    public void MemberMapping_ToString_ConvertStrategy()
    {
        var plan = MappingPlanBuilder.Build<ConvertPlanSource, ConvertPlanTarget>();
        var convertMapping = plan.PropertyMappings.FirstOrDefault(m => m.TargetMember.Name == "Value");
        Assert.IsNotNull(convertMapping);

        var str = convertMapping.ToString();
        Assert.IsTrue(str.Contains("Convert"));
    }

    [TestMethod]
    public void Plan_SourceAndTargetTypes_ShouldMatch()
    {
        var plan = MappingPlanBuilder.Build<PlanSource, PlanTarget>();

        Assert.AreEqual(typeof(PlanSource), plan.SourceType);
        Assert.AreEqual(typeof(PlanTarget), plan.TargetType);
    }

    [TestMethod]
    public void Plan_EmptyPlan_ShouldHaveNoMappings()
    {
        var plan = new TypeMappingPlan(
            typeof(PlanSource), typeof(PlanTarget),
            Array.Empty<MemberMapping>(), Array.Empty<MemberMapping>(),
            Array.Empty<System.Reflection.MemberInfo>(), Array.Empty<System.Reflection.MemberInfo>());

        Assert.AreEqual(0, plan.PropertyMappings.Count);
        Assert.AreEqual(0, plan.FieldMappings.Count);
        Assert.AreEqual(0, plan.UnmappedSourceMembers.Count);
        Assert.AreEqual(0, plan.UnmappedTargetMembers.Count);
    }
}
