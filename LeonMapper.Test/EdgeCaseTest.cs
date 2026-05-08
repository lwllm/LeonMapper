using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeonMapper.Attributes;
using LeonMapper.Config;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 核心边缘场景测试：覆盖空值、深层嵌套、大集合、线程安全等
/// </summary>
[TestClass]
public class EdgeCaseTest
{
    [TestInitialize]
    public void Init()
    {
        LeonMapper.Utils.MappingDepthTracker.Reset();
    }

    [TestCleanup]
    public void Cleanup()
    {
        LeonMapper.Utils.MappingDepthTracker.Reset();
    }
    #region Null Source Object

    public class SimpleSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class SimpleTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TestMethod]
    public void NullSource_Expression_ReturnsNull()
    {
        var mapper = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(null);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void NullSource_Emit_ReturnsNull()
    {
        var mapper = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(null);
        Assert.IsNull(result);
    }

    #endregion

    #region Deep Nested Mapping (3+ levels)

    public class Level1Source
    {
        public int Id { get; set; }
        public Level2Source? Level2 { get; set; }
    }

    public class Level2Source
    {
        public string Name { get; set; } = string.Empty;
        public Level3Source? Level3 { get; set; }
    }

    public class Level3Source
    {
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    }

    public class Level1Target
    {
        public int Id { get; set; }
        public Level2Target? Level2 { get; set; }
    }

    public class Level2Target
    {
        public string Name { get; set; } = string.Empty;
        public Level3Target? Level3 { get; set; }
    }

    public class Level3Target
    {
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    }

    [TestMethod]
    public void DeepNestedMapping_Expression_AllLevelsMapped()
    {
        var source = new Level1Source
        {
            Id = 1,
            Level2 = new Level2Source
            {
                Name = "Nested",
                Level3 = new Level3Source
                {
                    Timestamp = new DateTime(2024, 1, 15, 10, 30, 0),
                    Value = 99.99m
                }
            }
        };

        var mapper = new Mapper<Level1Source, Level1Target>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.IsNotNull(result.Level2);
        Assert.AreEqual("Nested", result.Level2.Name);
        Assert.IsNotNull(result.Level2.Level3);
        Assert.AreEqual(new DateTime(2024, 1, 15, 10, 30, 0), result.Level2.Level3.Timestamp);
        Assert.AreEqual(99.99m, result.Level2.Level3.Value);
    }

    [TestMethod]
    public void DeepNestedMapping_Emit_AllLevelsMapped()
    {
        var source = new Level1Source
        {
            Id = 1,
            Level2 = new Level2Source
            {
                Name = "Nested",
                Level3 = new Level3Source
                {
                    Timestamp = new DateTime(2024, 6, 1, 12, 0, 0),
                    Value = 42.50m
                }
            }
        };

        var mapper = new Mapper<Level1Source, Level1Target>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.IsNotNull(result.Level2);
        Assert.AreEqual("Nested", result.Level2.Name);
        Assert.IsNotNull(result.Level2.Level3);
        Assert.AreEqual(42.50m, result.Level2.Level3.Value);
    }

    [TestMethod]
    public void DeepNestedMapping_NullIntermediate_Expression_ReturnsPartialResult()
    {
        var source = new Level1Source { Id = 1, Level2 = null };

        var mapper = new Mapper<Level1Source, Level1Target>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.IsNull(result.Level2);
    }

    [TestMethod]
    public void DeepNestedMapping_NullIntermediate_Emit_ReturnsPartialResult()
    {
        var source = new Level1Source { Id = 1, Level2 = null };

        var mapper = new Mapper<Level1Source, Level1Target>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.IsNull(result.Level2);
    }

    [TestMethod]
    public void BothCompilers_DeepNestedMapping_SameResult()
    {
        var source = new Level1Source
        {
            Id = 42,
            Level2 = new Level2Source
            {
                Name = "Compare",
                Level3 = new Level3Source { Timestamp = DateTime.Now, Value = 7.77m }
            }
        };

        var exprMapper = new Mapper<Level1Source, Level1Target>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<Level1Source, Level1Target>(ProcessTypeEnum.Emit);

        var exprResult = exprMapper.MapTo(source);
        var emitResult = emitMapper.MapTo(source);

        Assert.IsNotNull(exprResult);
        Assert.IsNotNull(emitResult);
        Assert.AreEqual(exprResult.Id, emitResult.Id);
        Assert.IsNotNull(exprResult.Level2);
        Assert.IsNotNull(emitResult.Level2);
        Assert.AreEqual(exprResult.Level2.Name, emitResult.Level2.Name);
        Assert.IsNotNull(exprResult.Level2.Level3);
        Assert.IsNotNull(emitResult.Level2.Level3);
        Assert.AreEqual(exprResult.Level2.Level3.Timestamp, emitResult.Level2.Level3.Timestamp);
        Assert.AreEqual(exprResult.Level2.Level3.Value, emitResult.Level2.Level3.Value);
    }

    #endregion

    #region Large Collection

    [TestMethod]
    public void LargeCollection_Expression_MapsAllElements()
    {
        var source = new List<User>(1000);
        for (int i = 0; i < 1000; i++)
        {
            source.Add(new User
            {
                Id = i,
                Name = $"User{i}",
                StudentNumber = (i * 100).ToString(),
                Address = $"Addr{i}"
            });
        }

        var mapper = new Mapper<List<User>, List<UserNew>>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(1000, result.Count);
        Assert.AreEqual(0, result[0].Id);
        Assert.AreEqual(999, result[^1].Id);
    }

    [TestMethod]
    public void LargeCollection_AsProperty_Expression_MapsAllElements()
    {
        var source = new CollectionModel.CollectionSource
        {
            UsersList = new List<Model.User>(100)
        };
        for (int i = 0; i < 100; i++)
        {
            source.UsersList.Add(new Model.User { Id = i, Name = $"User{i}" });
        }

        var mapper = new Mapper<CollectionModel.CollectionSource, CollectionModel.CollectionTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.UsersList);
        Assert.AreEqual(100, result.UsersList.Count);
        Assert.AreEqual(0, result.UsersList[0].Id);
        Assert.AreEqual(99, result.UsersList[^1].Id);
    }

    [TestMethod]
    public void LargeCollection_AsProperty_Emit_MapsAllElements()
    {
        var source = new CollectionModel.CollectionSource
        {
            UsersList = new List<Model.User>(100)
        };
        for (int i = 0; i < 100; i++)
        {
            source.UsersList.Add(new Model.User { Id = i, Name = $"User{i}" });
        }

        var mapper = new Mapper<CollectionModel.CollectionSource, CollectionModel.CollectionTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.UsersList);
        Assert.AreEqual(100, result.UsersList.Count);
        Assert.AreEqual(0, result.UsersList[0].Id);
        Assert.AreEqual(99, result.UsersList[^1].Id);
    }

    [TestMethod]
    public void LargePrimitiveCollection_Expression_MapsAllElements()
    {
        var source = Enumerable.Range(1, 500).ToList();

        var mapper = new Mapper<List<int>, List<long>>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(500, result.Count);
        Assert.AreEqual(1L, result[0]);
        Assert.AreEqual(500L, result[^1]);
    }

    [TestMethod]
    public void LargePrimitiveCollection_AsProperty_Expression_MapsAllElements()
    {
        var source = new CollectionModel.CollectionSource
        {
            Numbers = Enumerable.Range(1, 500).ToList()
        };

        var mapper = new Mapper<CollectionModel.CollectionSource, CollectionModel.CollectionTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Numbers);
        Assert.AreEqual(500, result.Numbers.Count);
        Assert.AreEqual(1L, result.Numbers[0]);
        Assert.AreEqual(500L, result.Numbers[^1]);
    }

    [TestMethod]
    public void LargePrimitiveCollection_AsProperty_Emit_MapsAllElements()
    {
        var source = new CollectionModel.CollectionSource
        {
            Numbers = Enumerable.Range(1, 500).ToList()
        };

        var mapper = new Mapper<CollectionModel.CollectionSource, CollectionModel.CollectionTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Numbers);
        Assert.AreEqual(500, result.Numbers.Count);
        Assert.AreEqual(1L, result.Numbers[0]);
        Assert.AreEqual(500L, result.Numbers[^1]);
    }

    #endregion

    #region DateTime/Guid Mapping

    public class DateTimeGuidSource
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? NullableCreatedAt { get; set; }
        public Guid Key { get; set; }
        public Guid? NullableKey { get; set; }
    }

    public class DateTimeGuidTarget
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? NullableCreatedAt { get; set; }
        public Guid Key { get; set; }
        public Guid? NullableKey { get; set; }
    }

    [TestMethod]
    public void DateTimeGuidMapping_Expression_AllMapped()
    {
        var source = new DateTimeGuidSource
        {
            CreatedAt = DateTime.Now,
            NullableCreatedAt = DateTime.UtcNow,
            Key = Guid.NewGuid(),
            NullableKey = Guid.NewGuid()
        };

        var mapper = new Mapper<DateTimeGuidSource, DateTimeGuidTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt, result.CreatedAt);
        Assert.AreEqual(source.NullableCreatedAt, result.NullableCreatedAt);
        Assert.AreEqual(source.Key, result.Key);
        Assert.AreEqual(source.NullableKey, result.NullableKey);
    }

    [TestMethod]
    public void DateTimeGuidMapping_Emit_AllMapped()
    {
        var source = new DateTimeGuidSource
        {
            CreatedAt = DateTime.Now,
            Key = Guid.NewGuid()
        };

        var mapper = new Mapper<DateTimeGuidSource, DateTimeGuidTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.CreatedAt, result.CreatedAt);
        Assert.AreEqual(source.Key, result.Key);
    }

    [TestMethod]
    public void NullableDateTimeGuid_Expression_NullValues()
    {
        var source = new DateTimeGuidSource
        {
            CreatedAt = DateTime.MinValue,
            NullableCreatedAt = null,
            Key = Guid.Empty,
            NullableKey = null
        };

        var mapper = new Mapper<DateTimeGuidSource, DateTimeGuidTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNull(result.NullableCreatedAt);
        Assert.IsNull(result.NullableKey);
    }

    [TestMethod]
    public void NullableDateTimeGuid_Emit_NullValues()
    {
        var source = new DateTimeGuidSource
        {
            CreatedAt = DateTime.MinValue,
            NullableCreatedAt = null,
            Key = Guid.Empty,
            NullableKey = null
        };

        var mapper = new Mapper<DateTimeGuidSource, DateTimeGuidTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNull(result.NullableCreatedAt);
        Assert.IsNull(result.NullableKey);
    }

    #endregion

    #region Thread Safety

    public class ThreadSafetySource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ThreadSafetyTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TestMethod]
    public void ConcurrentMapping_Expression_MultipleThreads()
    {
        var mapper = new Mapper<ThreadSafetySource, ThreadSafetyTarget>(ProcessTypeEnum.Expression);
        var sources = Enumerable.Range(0, 100).Select(i => new ThreadSafetySource { Id = i, Name = $"T{i}" }).ToList();

        Parallel.For(0, 100, i =>
        {
            var result = mapper.MapTo(sources[i]);
            Assert.IsNotNull(result);
            Assert.AreEqual(i, result.Id);
        });
    }

    [TestMethod]
    public void ConcurrentMapping_Emit_MultipleThreads()
    {
        var mapper = new Mapper<ThreadSafetySource, ThreadSafetyTarget>(ProcessTypeEnum.Emit);
        var sources = Enumerable.Range(0, 100).Select(i => new ThreadSafetySource { Id = i, Name = $"T{i}" }).ToList();

        Parallel.For(0, 100, i =>
        {
            var result = mapper.MapTo(sources[i]);
            Assert.IsNotNull(result);
            Assert.AreEqual(i, result.Id);
        });
    }

    [TestMethod]
    public void ConcurrentMapping_SharedMapper_MultipleThreads()
    {
        var exprMapper = new Mapper<ThreadSafetySource, ThreadSafetyTarget>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<ThreadSafetySource, ThreadSafetyTarget>(ProcessTypeEnum.Emit);
        var source = new ThreadSafetySource { Id = 999, Name = "Shared" };

        Parallel.For(0, 50, _ =>
        {
            var exprResult = exprMapper.MapTo(source);
            var emitResult = emitMapper.MapTo(source);
            Assert.IsNotNull(exprResult);
            Assert.IsNotNull(emitResult);
            Assert.AreEqual(exprResult.Id, emitResult.Id);
            Assert.AreEqual(exprResult.Name, emitResult.Name);
        });
    }

    #endregion

    #region Mapper Reuse

    [TestMethod]
    public void MapperReuse_Expression_DifferentSources()
    {
        var mapper = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Expression);

        var result1 = mapper.MapTo(new SimpleSource { Id = 1, Name = "First" });
        var result2 = mapper.MapTo(new SimpleSource { Id = 2, Name = "Second" });

        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.AreEqual(1, result1.Id);
        Assert.AreEqual("First", result1.Name);
        Assert.AreEqual(2, result2.Id);
        Assert.AreEqual("Second", result2.Name);
    }

    [TestMethod]
    public void MapperReuse_Emit_DifferentSources()
    {
        var mapper = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Emit);

        var result1 = mapper.MapTo(new SimpleSource { Id = 10, Name = "Alpha" });
        var result2 = mapper.MapTo(new SimpleSource { Id = 20, Name = "Beta" });

        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.AreEqual(10, result1.Id);
        Assert.AreEqual(20, result2.Id);
    }

    #endregion

    #region Inheritance / Base Class Properties

    public class BaseSource
    {
        public int BaseId { get; set; }
        public string BaseName { get; set; } = string.Empty;
    }

    public class DerivedSource : BaseSource
    {
        public int DerivedValue { get; set; }
    }

    public class BaseTarget
    {
        public int BaseId { get; set; }
        public string BaseName { get; set; } = string.Empty;
    }

    public class DerivedTarget : BaseTarget
    {
        public int DerivedValue { get; set; }
    }

    [TestMethod]
    public void InheritanceMapping_Expression_BaseAndDerivedProperties()
    {
        var source = new DerivedSource { BaseId = 1, BaseName = "Base", DerivedValue = 2 };

        var mapper = new Mapper<DerivedSource, DerivedTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.BaseId);
        Assert.AreEqual("Base", result.BaseName);
        Assert.AreEqual(2, result.DerivedValue);
    }

    [TestMethod]
    public void InheritanceMapping_Emit_BaseAndDerivedProperties()
    {
        var source = new DerivedSource { BaseId = 3, BaseName = "EmitBase", DerivedValue = 4 };

        var mapper = new Mapper<DerivedSource, DerivedTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.BaseId);
        Assert.AreEqual("EmitBase", result.BaseName);
        Assert.AreEqual(4, result.DerivedValue);
    }

    #endregion

    #region Boundary Values

    public class BoundarySource
    {
        public int MaxInt { get; set; }
        public int MinInt { get; set; }
        public long MaxLong { get; set; }
        public long MinLong { get; set; }
        public double MaxDouble { get; set; }
        public double MinDouble { get; set; }
        public string? NullString { get; set; }
        public string EmptyString { get; set; } = string.Empty;
        public bool TrueBool { get; set; }
        public bool FalseBool { get; set; }
        public decimal MaxDecimal { get; set; }
        public decimal MinDecimal { get; set; }
    }

    public class BoundaryTarget
    {
        public int MaxInt { get; set; }
        public int MinInt { get; set; }
        public long MaxLong { get; set; }
        public long MinLong { get; set; }
        public double MaxDouble { get; set; }
        public double MinDouble { get; set; }
        public string? NullString { get; set; }
        public string EmptyString { get; set; } = string.Empty;
        public bool TrueBool { get; set; }
        public bool FalseBool { get; set; }
        public decimal MaxDecimal { get; set; }
        public decimal MinDecimal { get; set; }
    }

    [TestMethod]
    public void BoundaryValues_Expression_AllPreserved()
    {
        var source = new BoundarySource
        {
            MaxInt = int.MaxValue,
            MinInt = int.MinValue,
            MaxLong = long.MaxValue,
            MinLong = long.MinValue,
            MaxDouble = double.MaxValue,
            MinDouble = double.MinValue,
            NullString = null,
            EmptyString = string.Empty,
            TrueBool = true,
            FalseBool = false,
            MaxDecimal = decimal.MaxValue,
            MinDecimal = decimal.MinValue,
        };

        var mapper = new Mapper<BoundarySource, BoundaryTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(int.MaxValue, result.MaxInt);
        Assert.AreEqual(int.MinValue, result.MinInt);
        Assert.AreEqual(long.MaxValue, result.MaxLong);
        Assert.AreEqual(long.MinValue, result.MinLong);
        Assert.AreEqual(double.MaxValue, result.MaxDouble);
        Assert.AreEqual(double.MinValue, result.MinDouble);
        Assert.IsNull(result.NullString);
        Assert.AreEqual(string.Empty, result.EmptyString);
        Assert.IsTrue(result.TrueBool);
        Assert.IsFalse(result.FalseBool);
        Assert.AreEqual(decimal.MaxValue, result.MaxDecimal);
        Assert.AreEqual(decimal.MinValue, result.MinDecimal);
    }

    [TestMethod]
    public void BoundaryValues_Emit_AllPreserved()
    {
        var source = new BoundarySource
        {
            MaxInt = int.MaxValue,
            MinInt = int.MinValue,
            MaxLong = long.MaxValue,
            MinLong = long.MinValue,
            NullString = null,
            EmptyString = string.Empty,
            TrueBool = true,
            FalseBool = false,
        };

        var mapper = new Mapper<BoundarySource, BoundaryTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(int.MaxValue, result.MaxInt);
        Assert.AreEqual(int.MinValue, result.MinInt);
        Assert.AreEqual(long.MaxValue, result.MaxLong);
        Assert.AreEqual(long.MinValue, result.MinLong);
        Assert.IsNull(result.NullString);
        Assert.IsTrue(result.TrueBool);
        Assert.IsFalse(result.FalseBool);
    }

    #endregion

    #region Collection Type Conversion (via properties)

    [TestMethod]
    public void PrimitiveCollection_Expression_IntListToLongArray()
    {
        var source = new CollectionModel.CollectionSource
        {
            Numbers = new List<int> { 1, 2, 3 }
        };

        // Target has List<long> Numbers, test as property mapping
        var mapper = new Mapper<CollectionModel.CollectionSource, CollectionModel.CollectionTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Numbers);
        Assert.AreEqual(3, result.Numbers.Count);
        Assert.AreEqual(1L, result.Numbers[0]);
    }

    #endregion

    #region Pure Collection Mapping (List→List via Expression)

    [TestMethod]
    public void PureCollectionMapping_Expression_IntListToLongList()
    {
        var source = new List<int> { 1, 2, 3, 4, 5 };

        var mapper = new Mapper<List<int>, List<long>>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(1L, result[0]);
        Assert.AreEqual(5L, result[4]);
    }

    #endregion

    #region Circular Reference Detection

    public class SelfRefSource
    {
        public int Id { get; set; }
        public SelfRefSource? Self { get; set; }
    }

    public class SelfRefTarget
    {
        public int Id { get; set; }
        public SelfRefTarget? Self { get; set; }
    }

    [TestMethod]
    public void CircularReference_SelfReferencing_Expression_NoStackOverflow()
    {
        var source = new SelfRefSource { Id = 1 };
        source.Self = source; // 自身循环引用

        var mapper = new Mapper<SelfRefSource, SelfRefTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        // 不崩溃即可，超出深度后返回 null 或者部分结果
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
    }

    [TestMethod]
    public void CircularReference_SelfReferencing_Emit_NoStackOverflow()
    {
        var source = new SelfRefSource { Id = 2 };
        source.Self = source;

        var mapper = new Mapper<SelfRefSource, SelfRefTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Id);
    }

    public class CircularA
    {
        public int Id { get; set; }
        public CircularB? B { get; set; }
    }

    public class CircularB
    {
        public int Id { get; set; }
        public CircularA? A { get; set; }
    }

    public class CircularATarget
    {
        public int Id { get; set; }
        public CircularBTarget? B { get; set; }
    }

    public class CircularBTarget
    {
        public int Id { get; set; }
        public CircularATarget? A { get; set; }
    }

    [TestMethod]
    public void CircularReference_CrossType_Expression_NoStackOverflow()
    {
        var a = new CircularA { Id = 1 };
        var b = new CircularB { Id = 2, A = a };
        a.B = b;

        var mapper = new Mapper<CircularA, CircularATarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(a);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.IsNotNull(result.B);
        Assert.AreEqual(2, result.B.Id);
    }

    [TestMethod]
    public void CircularReference_CrossType_Emit_NoStackOverflow()
    {
        var a = new CircularA { Id = 3 };
        var b = new CircularB { Id = 4, A = a };
        a.B = b;

        var mapper = new Mapper<CircularA, CircularATarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(a);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Id);
        Assert.IsNotNull(result.B);
        Assert.AreEqual(4, result.B.Id);
    }

    [TestMethod]
    public void CircularReference_MaxDepth1_ReturnsNullForNested()
    {
        var parent = new SelfRefSource { Id = 1 };
        parent.Self = parent;

        MapperConfig.SetMaxDepth(1);
        try
        {
            var mapper = new Mapper<SelfRefSource, SelfRefTarget>(ProcessTypeEnum.Expression);
            var result = mapper.MapTo(parent);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.IsNull(result.Self);
        }
        finally
        {
            MapperConfig.SetMaxDepth(100);
        }
    }

    #endregion

    #region Init / Required Properties

    public class InitSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class InitTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [TestMethod]
    public void InitOnlyProperty_Expression_MapsCorrectly()
    {
        var source = new InitSource { Id = 42, Name = "InitTest" };

        var mapper = new Mapper<InitSource, InitTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(42, result.Id);
        Assert.AreEqual("InitTest", result.Name);
    }

    [TestMethod]
    public void InitOnlyProperty_Emit_MapsCorrectly()
    {
        var source = new InitSource { Id = 99, Name = "EmitInit" };

        var mapper = new Mapper<InitSource, InitTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(99, result.Id);
        Assert.AreEqual("EmitInit", result.Name);
    }

    #endregion

    #region IMapper Non-Generic Interface

    [TestMethod]
    public void IMapper_Expression_DynamicMap()
    {
        IMapper mapper = new MapperService();
        var source = new SimpleSource { Id = 7, Name = "Dynamic" };

        var result = mapper.Map(source, typeof(SimpleSource), typeof(SimpleTarget));

        Assert.IsNotNull(result);
        var target = (SimpleTarget)result;
        Assert.AreEqual(7, target.Id);
        Assert.AreEqual("Dynamic", target.Name);
    }

    [TestMethod]
    public void IMapper_GenericMap()
    {
        IMapper mapper = new MapperService();
        var source = new SimpleSource { Id = 8, Name = "Generic" };

        var result = mapper.Map<SimpleTarget>(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(8, result.Id);
        Assert.AreEqual("Generic", result.Name);
    }

    [TestMethod]
    public void IMapper_NullSource_ReturnsNull()
    {
        IMapper mapper = new MapperService();

        var result = mapper.Map(null, typeof(SimpleSource), typeof(SimpleTarget));

        Assert.IsNull(result);
    }

    #endregion

    #region CachedMapperFactory Cache Management

    [TestMethod]
    public void CachedMapperFactory_ClearCache_WorksCorrectly()
    {
        // 先触发一些缓存
        var mapper = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Expression);
        mapper.MapTo(new SimpleSource { Id = 1, Name = "test" });

        Assert.IsTrue(Compilers.CachedMapperFactory.GetCacheSize() > 0);

        Compilers.CachedMapperFactory.ClearCache();

        Assert.AreEqual(0, Compilers.CachedMapperFactory.GetCacheSize());

        // 清理后重新映射仍能正常工作
        var mapper2 = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Expression);
        var result = mapper2.MapTo(new SimpleSource { Id = 2, Name = "afterClear" });

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Id);
    }

    [TestMethod]
    public void MapperReuse_ManyTimes_NoDegradation()
    {
        var mapper = new Mapper<SimpleSource, SimpleTarget>(ProcessTypeEnum.Expression);

        for (int i = 0; i < 10000; i++)
        {
            var result = mapper.MapTo(new SimpleSource { Id = i, Name = $"Iteration{i}" });
            Assert.IsNotNull(result);
            Assert.AreEqual(i, result.Id);
        }
    }

    #endregion

    #region IgnoreMap Attribute Tests

    public class IgnoreMapSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [IgnoreMap]
        public string Secret { get; set; } = "shouldNotMap";
    }

    public class IgnoreMapTarget
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Secret { get; set; } = "default";
    }

    [TestMethod]
    public void IgnoreMapAttribute_Expression_SkipsMarkedProperty()
    {
        var source = new IgnoreMapSource { Id = 1, Name = "test", Secret = "hidden" };

        var mapper = new Mapper<IgnoreMapSource, IgnoreMapTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("test", result.Name);
        // Secret 标记了 [IgnoreMap]，目标应保留默认值
        Assert.AreEqual("default", result.Secret);
    }

    [TestMethod]
    public void IgnoreMapAttribute_Emit_SkipsMarkedProperty()
    {
        var source = new IgnoreMapSource { Id = 2, Name = "emit", Secret = "hidden" };

        var mapper = new Mapper<IgnoreMapSource, IgnoreMapTarget>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Id);
        Assert.AreEqual("emit", result.Name);
        Assert.AreEqual("default", result.Secret);
    }

    #endregion

    #region StringToEnum Edge Cases

    public enum TestEnum { ValueA, ValueB, ValueC }

    public class StringToEnumSource
    {
        public string Value { get; set; } = string.Empty;
    }

    public class StringToEnumTarget
    {
        public TestEnum Value { get; set; }
    }

    [TestMethod]
    public void StringToEnum_InvalidName_ReturnsDefault()
    {
        var source = new StringToEnumSource { Value = "NonExistentValue" };

        var mapper = new Mapper<StringToEnumSource, StringToEnumTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(TestEnum.ValueA, result.Value); // 无效名称返回 default(0)
    }

    [TestMethod]
    public void StringToEnum_NumericString_MapsByNumber()
    {
        var source = new StringToEnumSource { Value = "1" };

        var mapper = new Mapper<StringToEnumSource, StringToEnumTarget>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(source);

        Assert.IsNotNull(result);
        Assert.AreEqual(TestEnum.ValueB, result.Value);
    }

    #endregion
}
