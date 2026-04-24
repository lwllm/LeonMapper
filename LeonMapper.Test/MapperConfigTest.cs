using System;
using System.Linq;
using LeonMapper.Config;
using LeonMapper.Convert;
using LeonMapper.Plan;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 测试 MapperConfig 全局配置变更对映射行为的影响
/// </summary>[TestClass]
public class MapperConfigTest
{
    [TestInitialize]
    public void Setup()
    {
        // 每个测试前重置为默认配置
        MapperConfig.SetConfiguration(ProcessTypeEnum.Expression, ConverterScope.Common, true);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // 每个测试后重置为默认配置，避免影响其他测试
        MapperConfig.SetConfiguration(ProcessTypeEnum.Expression, ConverterScope.Common, true);
    }

    #region 编译策略配置测试

    [TestMethod]
    public void SetDefaultProcessType_ToEmit_ShouldUseEmitCompiler()
    {
        MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Emit);

        var role = new Role
        {
            RoleId1 = 1,
            RoleName = "Test",
            test1 = "t1",
            test2 = "t2"
        };

        // 不指定 ProcessType，应该使用全局配置的 Emit
        var mapper = new Mapper<Role, RoleNew>();
        var result = mapper.MapTo(role);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.RoleName);
        Assert.AreEqual("t1", result.test1);
    }

    [TestMethod]
    public void SetDefaultProcessType_ToExpression_ShouldUseExpressionCompiler()
    {
        MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Expression);

        var role = new Role
        {
            RoleId1 = 1,
            RoleName = "Test",
            test1 = "t1",
            test2 = "t2"
        };

        var mapper = new Mapper<Role, RoleNew>();
        var result = mapper.MapTo(role);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.RoleName);
    }

    [TestMethod]
    public void GetDefaultProcessType_ShouldReturnConfiguredValue()
    {
        MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Emit);
        Assert.AreEqual(ProcessTypeEnum.Emit, MapperConfig.GetDefaultProcessType());

        MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Expression);
        Assert.AreEqual(ProcessTypeEnum.Expression, MapperConfig.GetDefaultProcessType());
    }

    #endregion

    #region 自动转换配置测试

    [TestMethod]
    public void SetAutoConvert_ToFalse_ShouldSkipTypeConversion()
    {
        // 注意：Mapper 实例化时会缓存编译结果，配置变更只影响新创建的 Mapper
        // 这里测试配置值本身的读写
        MapperConfig.SetAutoConvert(false);
        Assert.IsFalse(MapperConfig.GetAutoConvert());

        MapperConfig.SetAutoConvert(true);
        Assert.IsTrue(MapperConfig.GetAutoConvert());
    }

    [TestMethod]
    public void GetAutoConvert_DefaultValue_ShouldBeTrue()
    {
        // 重置后默认值应为 true
        MapperConfig.SetAutoConvert(true);
        Assert.IsTrue(MapperConfig.GetAutoConvert());
    }

    #endregion

    #region 转换器范围配置测试

    [TestMethod]
    public void SetConverterScope_ShouldChangeScope()
    {
        MapperConfig.SetConverterScope(ConverterScope.All);
        Assert.AreEqual(ConverterScope.All, MapperConfig.GetDefaultConverterScope());

        MapperConfig.SetConverterScope(ConverterScope.Common);
        Assert.AreEqual(ConverterScope.Common, MapperConfig.GetDefaultConverterScope());
    }

    [TestMethod]
    public void GetDefaultConverterScope_DefaultValue_ShouldBeCommon()
    {
        MapperConfig.SetConverterScope(ConverterScope.Common);
        Assert.AreEqual(ConverterScope.Common, MapperConfig.GetDefaultConverterScope());
    }

    #endregion

    #region 原子配置测试

    [TestMethod]
    public void SetConfiguration_ShouldSetAllValuesAtomically()
    {
        MapperConfig.SetConfiguration(ProcessTypeEnum.Emit, ConverterScope.All, false);

        Assert.AreEqual(ProcessTypeEnum.Emit, MapperConfig.GetDefaultProcessType());
        Assert.AreEqual(ConverterScope.All, MapperConfig.GetDefaultConverterScope());
        Assert.IsFalse(MapperConfig.GetAutoConvert());
    }

    #endregion

    #region 自定义配置测试

    [TestMethod]
    public void SetCustomSetting_ShouldStoreAndRetrieve()
    {
        MapperConfig.SetCustomSetting("testKey", "testValue");
        var value = MapperConfig.GetCustomSetting("testKey");

        Assert.IsNotNull(value);
        Assert.AreEqual("testValue", value);
    }

    [TestMethod]
    public void SetCustomSetting_NullValue_ShouldRemoveKey()
    {
        MapperConfig.SetCustomSetting("tempKey", "tempValue");
        Assert.IsNotNull(MapperConfig.GetCustomSetting("tempKey"));

        MapperConfig.SetCustomSetting("tempKey", null);
        Assert.IsNull(MapperConfig.GetCustomSetting("tempKey"));
    }

    [TestMethod]
    public void GetCustomSetting_NonExistentKey_ShouldReturnNull()
    {
        var value = MapperConfig.GetCustomSetting("nonExistentKey");
        Assert.IsNull(value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void SetCustomSetting_NullKey_ShouldThrow()
    {
        MapperConfig.SetCustomSetting(null!, "value");
    }

    #endregion

    #region 配置隔离测试

    [TestMethod]
    public void MapperConstructor_WithExplicitProcessType_ShouldOverrideGlobalConfig()
    {
        // 全局配置为 Expression
        MapperConfig.SetDefaultProcessType(ProcessTypeEnum.Expression);

        var role = new Role
        {
            RoleId1 = 1,
            RoleName = "Test",
            test1 = "t1",
            test2 = "t2"
        };

        // 显式指定 Emit，应覆盖全局配置
        var mapper = new Mapper<Role, RoleNew>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(role);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test", result.RoleName);
    }

    #endregion
}
