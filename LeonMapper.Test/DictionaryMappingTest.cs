using System.Collections.Generic;
using LeonMapper.Config;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// Dictionary 类型映射测试
/// </summary>
[TestClass]
public class DictionaryMappingTest
{
    #region 测试模型

    public class DictSource
    {
        public Dictionary<string, User>? UserDict { get; set; }
        public Dictionary<string, int>? ScoreDict { get; set; }
        public Dictionary<int, string>? IdNameDict { get; set; }
    }

    public class DictTarget
    {
        public Dictionary<string, UserNew>? UserDict { get; set; }
        public Dictionary<string, long>? ScoreDict { get; set; }
        public Dictionary<long, string>? IdNameDict { get; set; }
    }

    public class StringToIntSource
    {
        public Dictionary<string, int>? Values { get; set; }
    }

    public class StringToLongTarget
    {
        public Dictionary<string, long>? Values { get; set; }
    }

    #endregion

    #region Expression 编译器（默认）

    [TestMethod]
    public void StringToIntDict_BasicMapping()
    {
        var source = new DictSource
        {
            ScoreDict = new Dictionary<string, int>
            {
                { "Alice", 95 },
                { "Bob", 87 },
                { "Charlie", 92 }
            }
        };

        var mapper = new Mapper<DictSource, DictTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.ScoreDict);
        Assert.AreEqual(3, target.ScoreDict.Count);
        Assert.AreEqual(95L, target.ScoreDict["Alice"]);
        Assert.AreEqual(87L, target.ScoreDict["Bob"]);
        Assert.AreEqual(92L, target.ScoreDict["Charlie"]);
    }

    [TestMethod]
    public void ComplexValueDict_BasicMapping()
    {
        var source = new DictSource
        {
            UserDict = new Dictionary<string, User>
            {
                { "admin", new User { Id = 1, Name = "Admin", Role = new Role { RoleId1 = 1, RoleName = "AdminRole" } } },
                { "user", new User { Id = 2, Name = "User", Role = new Role { RoleId1 = 2, RoleName = "UserRole" } } }
            }
        };

        var mapper = new Mapper<DictSource, DictTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UserDict);
        Assert.AreEqual(2, target.UserDict.Count);
        Assert.AreEqual(1, target.UserDict["admin"].Id);
        Assert.AreEqual("AdminRole", target.UserDict["admin"].Role.RoleName);
        Assert.AreEqual(2, target.UserDict["user"].Id);
        Assert.AreEqual("UserRole", target.UserDict["user"].Role.RoleName);
    }

    [TestMethod]
    public void KeyTypeConversionDict_BasicMapping()
    {
        var source = new DictSource
        {
            IdNameDict = new Dictionary<int, string>
            {
                { 1, "One" },
                { 2, "Two" },
                { 3, "Three" }
            }
        };

        var mapper = new Mapper<DictSource, DictTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.IdNameDict);
        Assert.AreEqual(3, target.IdNameDict.Count);
        Assert.AreEqual("One", target.IdNameDict[1L]);
        Assert.AreEqual("Two", target.IdNameDict[2L]);
        Assert.AreEqual("Three", target.IdNameDict[3L]);
    }

    [TestMethod]
    public void NullDict_ReturnsNull()
    {
        var source = new DictSource { UserDict = null };

        var mapper = new Mapper<DictSource, DictTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.UserDict);
    }

    [TestMethod]
    public void EmptyDict_ReturnsEmpty()
    {
        var source = new DictSource { ScoreDict = new Dictionary<string, int>() };

        var mapper = new Mapper<DictSource, DictTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.ScoreDict);
        Assert.AreEqual(0, target.ScoreDict.Count);
    }

    #endregion

    #region Emit 编译器

    [TestMethod]
    public void Emit_StringToIntDict_BasicMapping()
    {
        var source = new DictSource
        {
            ScoreDict = new Dictionary<string, int>
            {
                { "Alice", 95 },
                { "Bob", 87 }
            }
        };

        var mapper = new Mapper<DictSource, DictTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.ScoreDict);
        Assert.AreEqual(2, target.ScoreDict.Count);
        Assert.AreEqual(95L, target.ScoreDict["Alice"]);
        Assert.AreEqual(87L, target.ScoreDict["Bob"]);
    }

    [TestMethod]
    public void Emit_ComplexValueDict_BasicMapping()
    {
        var source = new DictSource
        {
            UserDict = new Dictionary<string, User>
            {
                { "admin", new User { Id = 1, Name = "Admin", Role = new Role { RoleId1 = 1, RoleName = "AdminRole" } } }
            }
        };

        var mapper = new Mapper<DictSource, DictTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UserDict);
        Assert.AreEqual(1, target.UserDict.Count);
        Assert.AreEqual(1, target.UserDict["admin"].Id);
        Assert.AreEqual("AdminRole", target.UserDict["admin"].Role.RoleName);
    }

    [TestMethod]
    public void Emit_KeyTypeConversionDict_BasicMapping()
    {
        var source = new DictSource
        {
            IdNameDict = new Dictionary<int, string>
            {
                { 1, "One" },
                { 2, "Two" }
            }
        };

        var mapper = new Mapper<DictSource, DictTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.IdNameDict);
        Assert.AreEqual(2, target.IdNameDict.Count);
        Assert.AreEqual("One", target.IdNameDict[1L]);
        Assert.AreEqual("Two", target.IdNameDict[2L]);
    }

    [TestMethod]
    public void Emit_NullDict_ReturnsNull()
    {
        var source = new DictSource { UserDict = null };

        var mapper = new Mapper<DictSource, DictTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.UserDict);
    }

    [TestMethod]
    public void Emit_EmptyDict_ReturnsEmpty()
    {
        var source = new DictSource { ScoreDict = new Dictionary<string, int>() };

        var mapper = new Mapper<DictSource, DictTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.ScoreDict);
        Assert.AreEqual(0, target.ScoreDict.Count);
    }

    #endregion

    #region 编译器一致性

    [TestMethod]
    public void BothCompilers_DictMapping_ShouldProduceSameResult()
    {
        var source = new DictSource
        {
            ScoreDict = new Dictionary<string, int>
            {
                { "Alice", 95 },
                { "Bob", 87 }
            },
            UserDict = new Dictionary<string, User>
            {
                { "admin", new User { Id = 1, Name = "Admin", Role = new Role { RoleId1 = 1, RoleName = "AdminRole" } } }
            }
        };

        var exprMapper = new Mapper<DictSource, DictTarget>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<DictSource, DictTarget>(ProcessTypeEnum.Emit);

        var exprResult = exprMapper.MapTo(source);
        var emitResult = emitMapper.MapTo(source);

        Assert.IsNotNull(exprResult);
        Assert.IsNotNull(emitResult);

        // 验证基础类型 Value 映射结果一致
        Assert.AreEqual(exprResult.ScoreDict!.Count, emitResult.ScoreDict!.Count);
        Assert.AreEqual(exprResult.ScoreDict["Alice"], emitResult.ScoreDict["Alice"]);
        Assert.AreEqual(exprResult.ScoreDict["Bob"], emitResult.ScoreDict["Bob"]);

        // 验证复杂类型 Value 映射结果一致
        Assert.AreEqual(exprResult.UserDict!.Count, emitResult.UserDict!.Count);
        Assert.AreEqual(exprResult.UserDict["admin"].Id, emitResult.UserDict["admin"].Id);
        Assert.AreEqual(exprResult.UserDict["admin"].Role.RoleName, emitResult.UserDict["admin"].Role.RoleName);
    }

    #endregion

    #region GetPlan / Validate

    [TestMethod]
    public void GetPlan_ShouldShowDictionaryMapping()
    {
        var plan = Mapper<DictSource, DictTarget>.GetPlan();

        Assert.IsNotNull(plan);
        var planStr = plan.ToString();
        Assert.IsTrue(planStr.Contains("Dictionary"), $"映射计划应包含 Dictionary 策略信息，实际内容: {planStr}");
    }

    [TestMethod]
    public void Validate_ValidDictionaryMapping_ShouldBeValid()
    {
        var validation = Mapper<DictSource, DictTarget>.Validate();

        Assert.IsTrue(validation.IsValid);
    }

    [TestMethod]
    public void Validate_StringToLongDictMapping_ShouldBeValid()
    {
        var validation = Mapper<StringToIntSource, StringToLongTarget>.Validate();

        Assert.IsTrue(validation.IsValid);
    }

    #endregion

    #region IDictionary / IReadOnlyDictionary 目标类型

    public class DictIDictSource
    {
        public Dictionary<string, int>? Values { get; set; }
    }

    public class DictIDictTarget
    {
        public IDictionary<string, long>? Values { get; set; }
    }

    public class DictReadOnlyTarget
    {
        public IReadOnlyDictionary<string, long>? Values { get; set; }
    }

    [TestMethod]
    public void IDictionaryTarget_Expression_BasicMapping()
    {
        var source = new DictIDictSource
        {
            Values = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
        };

        var mapper = new Mapper<DictIDictSource, DictIDictTarget>(ProcessTypeEnum.Expression);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Values);
        Assert.AreEqual(2, target.Values.Count);
        Assert.AreEqual(1L, target.Values["a"]);
        Assert.AreEqual(2L, target.Values["b"]);
    }

    [TestMethod]
    public void IDictionaryTarget_Emit_BasicMapping()
    {
        var source = new DictIDictSource
        {
            Values = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
        };

        var mapper = new Mapper<DictIDictSource, DictIDictTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Values);
        Assert.AreEqual(2, target.Values.Count);
        Assert.AreEqual(1L, target.Values["a"]);
        Assert.AreEqual(2L, target.Values["b"]);
    }

    [TestMethod]
    public void IReadOnlyDictionaryTarget_Expression_BasicMapping()
    {
        var source = new DictIDictSource
        {
            Values = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
        };

        var mapper = new Mapper<DictIDictSource, DictReadOnlyTarget>(ProcessTypeEnum.Expression);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Values);
        Assert.AreEqual(2, target.Values.Count);
        Assert.AreEqual(1L, target.Values["a"]);
        Assert.AreEqual(2L, target.Values["b"]);
    }

    [TestMethod]
    public void IReadOnlyDictionaryTarget_Emit_BasicMapping()
    {
        var source = new DictIDictSource
        {
            Values = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
        };

        var mapper = new Mapper<DictIDictSource, DictReadOnlyTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Values);
        Assert.AreEqual(2, target.Values.Count);
        Assert.AreEqual(1L, target.Values["a"]);
        Assert.AreEqual(2L, target.Values["b"]);
    }

    #endregion

    #region 字段映射

    public class DictFieldSource
    {
        public Dictionary<string, int>? ScoreField;
    }

    public class DictFieldTarget
    {
        public Dictionary<string, long>? ScoreField;
    }

    [TestMethod]
    public void DictField_Expression_BasicMapping()
    {
        var source = new DictFieldSource
        {
            ScoreField = new Dictionary<string, int> { { "Alice", 95 }, { "Bob", 87 } }
        };

        var mapper = new Mapper<DictFieldSource, DictFieldTarget>(ProcessTypeEnum.Expression);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.ScoreField);
        Assert.AreEqual(2, target.ScoreField.Count);
        Assert.AreEqual(95L, target.ScoreField["Alice"]);
        Assert.AreEqual(87L, target.ScoreField["Bob"]);
    }

    [TestMethod]
    public void DictField_Emit_BasicMapping()
    {
        var source = new DictFieldSource
        {
            ScoreField = new Dictionary<string, int> { { "Alice", 95 }, { "Bob", 87 } }
        };

        var mapper = new Mapper<DictFieldSource, DictFieldTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.ScoreField);
        Assert.AreEqual(2, target.ScoreField.Count);
        Assert.AreEqual(95L, target.ScoreField["Alice"]);
        Assert.AreEqual(87L, target.ScoreField["Bob"]);
    }

    #endregion

    #region 验证 — 不兼容类型

    /// <summary>
    /// String 不是基元类型，但 IsBaseType 认为它是基础类型，
    /// 此测试验证 Dictionary&lt;int, string&gt; -> Dictionary&lt;int, UserNew&gt; 应报告错误
    /// </summary>
    public class DictInvalidValueSource
    {
        public Dictionary<string, int>? Values { get; set; }
    }

    public class DictInvalidValueTarget
    {
        public Dictionary<string, UserNew>? Values { get; set; }
    }

    [TestMethod]
    public void Validate_IncompatibleValueType_ShouldHaveErrors()
    {
        var validation = Mapper<DictInvalidValueSource, DictInvalidValueTarget>.Validate();

        Assert.IsFalse(validation.IsValid);
        Assert.IsTrue(validation.Errors.Count > 0, $"应报告 Value 类型不兼容错误，实际: {string.Join(", ", validation.Errors)}");
    }

    #endregion
}
