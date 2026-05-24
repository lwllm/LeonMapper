using LeonMapper.Plan;
using System;
using System.Linq;
using LeonMapper.Compilers;
using LeonMapper.Config;
using LeonMapper.Convert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 缓存管理和 ConvertFactory 扩展 API 测试
/// </summary>
[TestClass]
public class CacheManagementTest
{    [TestInitialize]
    public void Setup()
    {
        CachedMapperFactory.ClearCache();
    }

    [TestCleanup]
    public void Cleanup()
    {
        CachedMapperFactory.ClearCache();
        MapperConfig.SetConfiguration(ProcessTypeEnum.Expression, ConverterScope.Common, true);
    }

    #region CacheStatistics 测试

    public class StatSource { public int Id { get; set; } }
    public class StatTarget { public int Id { get; set; } }

    [TestMethod]
    public void GetCacheStatistics_Initial_ShouldHaveZeroCounts()
    {
        CachedMapperFactory.ClearCache();
        var stats = CachedMapperFactory.GetCacheStatistics();

        Assert.AreEqual(0, stats.MapperCount);
        Assert.AreEqual(0, stats.MapFuncCount);
        Assert.AreEqual(0, stats.EmptyMapperCount);
        Assert.AreEqual(0, stats.PlanCacheCount);
    }

    [TestMethod]
    public void GetCacheStatistics_AfterMapping_ShouldIncreaseCounts()
    {
        CachedMapperFactory.ClearCache();
        var mapper = new Mapper<StatSource, StatTarget>();
        mapper.MapTo(new StatSource { Id = 1 });

        var stats = CachedMapperFactory.GetCacheStatistics();

        // MapperCount is 0 for simple direct Mapper usage (CachedMapperFactory only used for nested complex types)
        Assert.IsTrue(stats.PlanCacheCount > 0, "PlanCacheCount should be > 0 after mapping");
    }

    [TestMethod]
    public void GetCacheStatistics_AfterClearCache_ShouldResetToZero()
    {
        var mapper = new Mapper<StatSource, StatTarget>();
        mapper.MapTo(new StatSource { Id = 1 });

        CachedMapperFactory.ClearCache();
        var stats = CachedMapperFactory.GetCacheStatistics();

        Assert.AreEqual(0, stats.MapperCount);
        Assert.AreEqual(0, stats.MapFuncCount);
    }

    #endregion

    #region RemoveMapper 测试

    public class RemoveSource { public string Value { get; set; } = string.Empty; }
    public class RemoveTarget { public string Value { get; set; } = string.Empty; }

    [TestMethod]
    public void RemoveMapper_ExistingType_ShouldRemove()
    {
        // Use MapperService which goes through CachedMapperFactory
        IMapper mapperService = new MapperService();
        var result = mapperService.Map(new StatSource { Id = 1 }, typeof(StatSource), typeof(StatTarget));
        Assert.IsNotNull(result);

        // MapperService creates entries in CachedMapperFactory
        Assert.IsTrue(CachedMapperFactory.GetCacheSize() > 0, "Should have cached mappers via MapperService");

        var removed = CachedMapperFactory.RemoveMapper(typeof(StatSource), typeof(StatTarget));
        Assert.IsTrue(removed, "Should successfully remove existing mapper");
    }

    [TestMethod]
    public void RemoveMapper_NonExistingType_ShouldReturnFalse()
    {
        CachedMapperFactory.ClearCache();
        var removed = CachedMapperFactory.RemoveMapper(typeof(RemoveSource), typeof(RemoveTarget));
        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void RemoveMapper_AfterRemove_NewMapperStillWorks()
    {
        CachedMapperFactory.ClearCache();
        var mapper1 = new Mapper<RemoveSource, RemoveTarget>();
        mapper1.MapTo(new RemoveSource { Value = "first" });

        CachedMapperFactory.RemoveMapper(typeof(RemoveSource), typeof(RemoveTarget));
        Assert.AreEqual(0, CachedMapperFactory.GetCacheSize());

        // 重新创建 mapper 应该能正常工作
        var mapper2 = new Mapper<RemoveSource, RemoveTarget>();
        var result = mapper2.MapTo(new RemoveSource { Value = "second" });

        Assert.IsNotNull(result);
        Assert.AreEqual("second", result.Value);
    }

    #endregion

    #region ConvertFactory 注册 API 测试

    public class ExternalSource { public int Value { get; set; } }
    public class ExternalTarget { public string Value { get; set; } = string.Empty; }

    // 自定义外部转换器（不应在默认扫描中发现）
    public class IntToHexConverter : IConverter<int, string>
    {
        public string Convert(int input) => $"0x{input:X}";
    }

    [TestMethod]
    public void RegisterConverter_Generic_ShouldRegisterConverter()
    {
        // int->string 已有内置转换器，注册 IntToHexConverter 后应覆盖它
        ConvertFactory.RegisterConverter<IntToHexConverter>();

        var converter = ConvertFactory.GetConverter(typeof(int), typeof(string), ConverterScope.All);
        Assert.IsNotNull(converter);
        Assert.IsInstanceOfType(converter, typeof(IntToHexConverter));

        var result = ((IConverter<int, string>)converter).Convert(255);
        Assert.AreEqual("0xFF", result);
    }

    [TestMethod]
    public void RegisterConverter_DuplicateRegistration_ShouldOverwrite()
    {
        ConvertFactory.RegisterConverter<IntToHexConverter>();
        ConvertFactory.RegisterConverter<IntToHexConverter>();

        var converter = ConvertFactory.GetConverter(typeof(int), typeof(string), ConverterScope.All);
        Assert.IsNotNull(converter);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegisterAssembly_NullAssembly_ShouldThrow()
    {
        ConvertFactory.RegisterAssembly(null!);
    }

    [TestMethod]
    public void RegisterAssembly_WithCurrentAssembly_ShouldNotThrow()
    {
        // 当前程序集的转换器应该已经注册过了，但不应抛出异常
        ConvertFactory.RegisterAssembly(typeof(ConvertFactory).Assembly);
    }

    [TestMethod]
    public void GetConverterCount_CommonAndAll_ShouldBeConsistent()
    {
        var commonCount = ConvertFactory.GetConverterCount(ConverterScope.Common);
        var allCount = ConvertFactory.GetConverterCount(ConverterScope.All);

        Assert.IsTrue(commonCount > 0, "Common converters should exist");
        Assert.IsTrue(allCount >= commonCount, "All converters should be >= Common");
    }

    #endregion

    #region DelegateInvoker 缓存清理测试

    public class InvokerSource { public double Price { get; set; } }
    public class InvokerTarget { public int Price { get; set; } }

    [TestMethod]
    public void ClearCache_ShouldSyncDelegateInvokerConverters()
    {
        var mapper = new Mapper<InvokerSource, InvokerTarget>(ProcessTypeEnum.Emit);
        var result1 = mapper.MapTo(new InvokerSource { Price = 42.5 });

        Assert.IsNotNull(result1);
        Assert.AreEqual(42, result1.Price);

        // 清理缓存后重新映射
        CachedMapperFactory.ClearCache();

        var mapper2 = new Mapper<InvokerSource, InvokerTarget>(ProcessTypeEnum.Emit);
        var result2 = mapper2.MapTo(new InvokerSource { Price = 99.9 });

        Assert.IsNotNull(result2);
        Assert.AreEqual(99, result2.Price);
    }

    #endregion

    #region MapperService 缓存清理测试

    [TestMethod]
    public void MapperService_AfterClearCache_StillWorks()
    {
        CachedMapperFactory.ClearCache();
        IMapper mapperService = new MapperService();

        var result1 = mapperService.Map(
            new StatSource { Id = 42 },
            typeof(StatSource),
            typeof(StatTarget));

        Assert.IsNotNull(result1);

        CachedMapperFactory.ClearCache();

        var result2 = mapperService.Map(
            new StatSource { Id = 99 },
            typeof(StatSource),
            typeof(StatTarget));

        Assert.IsNotNull(result2);
    }

    #endregion
}
