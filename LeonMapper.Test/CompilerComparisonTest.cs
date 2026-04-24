using System;
using System.Collections.Generic;
using System.Diagnostics;
using LeonMapper.Config;
using LeonMapper.Test.CollectionModel;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

[TestClass]
public class CompilerComparisonTest
{
    private Role _role = null!;
    private User _user = null!;

    [TestInitialize]
    public void Setup()
    {
        _role = new Role
        {
            RoleId1 = 22,
            RoleName = "Role22",
            test1 = "ttt1",
            test2 = "ttt2"
        };
        _user = new User
        {
            Id = 11,
            StudentNumber = "123",
            Name = "leon",
            Address = "china",
            Role = _role,
            test1 = "t1",
            test2 = 2222,
            role2 = new Role
            {
                RoleId1 = 33,
                RoleName = "Role33",
                test1 = "r2t1",
                test2 = "r2t2"
            }
        };
    }

    #region 正确性测试

    [TestMethod]
    public void ExpressionCompiler_SimplePropertyMapping_ShouldBeCorrect()
    {
        var mapper = new Mapper<Role, RoleNew>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(_role);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.RoleId2);
        Assert.AreEqual("Role22", result.RoleName);
    }

    [TestMethod]
    public void EmitCompiler_SimplePropertyMapping_ShouldBeCorrect()
    {
        var mapper = new Mapper<Role, RoleNew>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(_role);

        Assert.IsNotNull(result);
        // RoleId1 和 RoleId2 名字不同，默认不映射
        Assert.AreEqual(0, result.RoleId2);
        // RoleName 同名，会映射
        Assert.AreEqual("Role22", result.RoleName);
    }

    [TestMethod]
    public void ExpressionCompiler_ComplexTypeMapping_ShouldBeCorrect()
    {
        var mapper = new Mapper<User, UserNew>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(_user);

        Assert.IsNotNull(result);
        Assert.AreEqual(11, result.Id);
        Assert.IsNotNull(result.Role);
        Assert.AreEqual("Role22", result.Role.RoleName);
    }

    [TestMethod]
    public void EmitCompiler_ComplexTypeMapping_ShouldBeCorrect()
    {
        var mapper = new Mapper<User, UserNew>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(_user);

        Assert.IsNotNull(result);
        Assert.AreEqual(11, result.Id);
        Assert.IsNotNull(result.Role);
        Assert.AreEqual("Role22", result.Role.RoleName);
    }

    [TestMethod]
    public void ExpressionCompiler_FieldMapping_ShouldBeCorrect()
    {
        var mapper = new Mapper<Role, RoleNew>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(_role);

        Assert.IsNotNull(result);
        Assert.AreEqual("ttt1", result.test1);
        Assert.AreEqual("ttt2", result.test2);
    }

    [TestMethod]
    public void EmitCompiler_FieldMapping_ShouldBeCorrect()
    {
        var mapper = new Mapper<Role, RoleNew>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(_role);

        Assert.IsNotNull(result);
        Assert.AreEqual("ttt1", result.test1);
        Assert.AreEqual("ttt2", result.test2);
    }

    [TestMethod]
    public void ExpressionCompiler_TypeConversion_ShouldBeCorrect()
    {
        var mapper = new Mapper<User, UserNew>(ProcessTypeEnum.Expression);
        var result = mapper.MapTo(_user);

        Assert.IsNotNull(result);
        // test2 是 int → string 转换
        Assert.AreEqual("2222", result.test2);
    }

    [TestMethod]
    public void EmitCompiler_TypeConversion_ShouldBeCorrect()
    {
        var mapper = new Mapper<User, UserNew>(ProcessTypeEnum.Emit);
        var result = mapper.MapTo(_user);

        Assert.IsNotNull(result);
        // test2 是 int → string 转换
        Assert.AreEqual("2222", result.test2);
    }

    [TestMethod]
    public void BothCompilers_ShouldProduceSameResult()
    {
        var exprMapper = new Mapper<User, UserNew>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<User, UserNew>(ProcessTypeEnum.Emit);

        var exprResult = exprMapper.MapTo(_user);
        var emitResult = emitMapper.MapTo(_user);

        Assert.IsNotNull(exprResult);
        Assert.IsNotNull(emitResult);

        // 所有映射字段值应该完全一致
        Assert.AreEqual(exprResult.Id, emitResult.Id);
        Assert.AreEqual(exprResult.Name, emitResult.Name);
        Assert.AreEqual(exprResult.Address, emitResult.Address);
        Assert.AreEqual(exprResult.StudentNumber, emitResult.StudentNumber);
        Assert.AreEqual(exprResult.test1, emitResult.test1);
        Assert.AreEqual(exprResult.test2, emitResult.test2);

        // 复杂类型
        Assert.IsNotNull(exprResult.Role);
        Assert.IsNotNull(emitResult.Role);
        Assert.AreEqual(exprResult.Role.RoleId2, emitResult.Role.RoleId2);
        Assert.AreEqual(exprResult.Role.RoleName, emitResult.Role.RoleName);
        Assert.AreEqual(exprResult.Role.test1, emitResult.Role.test1);
        Assert.AreEqual(exprResult.Role.test2, emitResult.Role.test2);

        // 复杂字段
        Assert.IsNotNull(exprResult.role2);
        Assert.IsNotNull(emitResult.role2);
        Assert.AreEqual(exprResult.role2.RoleId2, emitResult.role2.RoleId2);
        Assert.AreEqual(exprResult.role2.RoleName, emitResult.role2.RoleName);
    }

    #endregion

    #region 性能对比测试

    [TestMethod]
    public void PerformanceComparison_Expression_vs_Emit()
    {
        const int iterations = 100_000_000; // 1亿
        var role = new Role { RoleId1 = 1, RoleName = "R", test1 = "t1", test2 = "t2" };

        // 预热
        var exprMapper = new Mapper<Role, RoleNew>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<Role, RoleNew>(ProcessTypeEnum.Emit);
        exprMapper.MapTo(role);
        emitMapper.MapTo(role);

        // ExpressionCompiler
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            exprMapper.MapTo(role);
        }

        var expressionMs = sw.ElapsedMilliseconds;

        // EmitCompiler
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            emitMapper.MapTo(role);
        }

        var emitMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"=== 编译器性能对比 ({iterations:N0} 次迭代) ===");
        Console.WriteLine($"ExpressionCompiler: {expressionMs} ms");
        Console.WriteLine($"EmitCompiler:       {emitMs} ms");
        Console.WriteLine($"差异:               {Math.Abs(expressionMs - emitMs)} ms ({(double)Math.Abs(expressionMs - emitMs) / Math.Max(expressionMs, emitMs) * 100:F1}%)");
    }

    #endregion

    #region 集合映射性能对比

    [TestMethod]
    public void PerformanceComparison_CollectionMapping_Expression_vs_Emit()
    {
        const int iterations = 1_000_000; // 100万
        var source = new CollectionSource
        {
            UsersList = new List<User>
            {
                new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" },
                new() { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" },
                new() { Id = 3, StudentNumber = "300", Name = "Charlie", Address = "addr3" }
            }
        };

        // 预热
        var exprMapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        exprMapper.MapTo(source);
        emitMapper.MapTo(source);

        // ExpressionCompiler
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            exprMapper.MapTo(source);
        }

        var expressionMs = sw.ElapsedMilliseconds;

        // EmitCompiler
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            emitMapper.MapTo(source);
        }

        var emitMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"=== 集合映射性能对比 ({iterations:N0} 次迭代) ===");
        Console.WriteLine($"ExpressionCompiler: {expressionMs} ms");
        Console.WriteLine($"EmitCompiler:       {emitMs} ms");
        Console.WriteLine($"差异:               {Math.Abs(expressionMs - emitMs)} ms ({(double)Math.Abs(expressionMs - emitMs) / Math.Max(expressionMs, emitMs) * 100:F1}%)");
    }

    [TestMethod]
    public void PerformanceComparison_PrimitiveCollectionMapping_Expression_vs_Emit()
    {
        const int iterations = 1_000_000; // 100万
        var source = new CollectionSource
        {
            Numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };

        // 预热
        var exprMapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        exprMapper.MapTo(source);
        emitMapper.MapTo(source);

        // ExpressionCompiler
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            exprMapper.MapTo(source);
        }

        var expressionMs = sw.ElapsedMilliseconds;

        // EmitCompiler
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            emitMapper.MapTo(source);
        }

        var emitMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"=== 基础类型集合映射性能对比 ({iterations:N0} 次迭代) ===");
        Console.WriteLine($"ExpressionCompiler: {expressionMs} ms");
        Console.WriteLine($"EmitCompiler:       {emitMs} ms");
        Console.WriteLine($"差异:               {Math.Abs(expressionMs - emitMs)} ms ({(double)Math.Abs(expressionMs - emitMs) / Math.Max(expressionMs, emitMs) * 100:F1}%)");
    }

    #endregion
}
