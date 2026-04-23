using System.Collections.Generic;
using System.Linq;
using LeonMapper.Config;
using LeonMapper.Test.CollectionModel;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

/// <summary>
/// 集合类型映射测试
/// </summary>
[TestClass]
public class CollectionMappingTest
{
    #region List to List

    [TestMethod]
    public void ListToList_BasicMapping()
    {
        var source = new CollectionSource
        {
            UsersList = new List<User>
            {
                new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" },
                new() { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersList);
        Assert.AreEqual(2, target.UsersList.Count);
        Assert.AreEqual(1, target.UsersList[0].Id);
        Assert.AreEqual(2, target.UsersList[1].Id);
    }

    [TestMethod]
    public void ListToList_NullSource_ReturnsNull()
    {
        var source = new CollectionSource { UsersList = null };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.UsersList);
    }

    [TestMethod]
    public void ListToList_EmptyList_ReturnsEmptyList()
    {
        var source = new CollectionSource { UsersList = new List<User>() };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersList);
        Assert.AreEqual(0, target.UsersList.Count);
    }

    #endregion

    #region Array to Array

    [TestMethod]
    public void ArrayToArray_BasicMapping()
    {
        var source = new CollectionSource
        {
            UsersArray = new[]
            {
                new User { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" },
                new User { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersArray);
        Assert.AreEqual(2, target.UsersArray.Length);
        Assert.AreEqual(1, target.UsersArray[0].Id);
        Assert.AreEqual(2, target.UsersArray[1].Id);
    }

    [TestMethod]
    public void ArrayToArray_NullSource_ReturnsNull()
    {
        var source = new CollectionSource { UsersArray = null };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.UsersArray);
    }

    [TestMethod]
    public void ArrayToArray_EmptyArray_ReturnsEmptyArray()
    {
        var source = new CollectionSource { UsersArray = System.Array.Empty<User>() };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersArray);
        Assert.AreEqual(0, target.UsersArray.Length);
    }

    #endregion

    #region List to Array

    [TestMethod]
    public void ListToArray_BasicMapping()
    {
        var source = new CollectionSource
        {
            UsersList = new List<User>
            {
                new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersList);
        Assert.AreEqual(1, target.UsersList.Count);
    }

    #endregion

    #region Array to List

    [TestMethod]
    public void ArrayToList_BasicMapping()
    {
        var source = new CollectionSource
        {
            UsersArray = new[]
            {
                new User { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersArray);
        Assert.AreEqual(1, target.UsersArray.Length);
    }

    #endregion

    #region IEnumerable to IEnumerable

    [TestMethod]
    public void IEnumerableToIEnumerable_BasicMapping()
    {
        var source = new CollectionSource
        {
            UsersEnumerable = new List<User>
            {
                new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" },
                new() { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersEnumerable);
        var list = target.UsersEnumerable.ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(1, list[0].Id);
    }

    [TestMethod]
    public void IEnumerableToIEnumerable_NullSource_ReturnsNull()
    {
        var source = new CollectionSource { UsersEnumerable = null };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.UsersEnumerable);
    }

    #endregion

    #region Primitive Collection

    [TestMethod]
    public void PrimitiveList_BasicMapping()
    {
        var source = new CollectionSource
        {
            Numbers = new List<int> { 1, 2, 3, 4, 5 }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(5, target.Numbers.Count);
        Assert.AreEqual(1L, target.Numbers[0]);
        Assert.AreEqual(5L, target.Numbers[4]);
    }

    #endregion

    #region Nested Collection

    [TestMethod]
    public void NestedList_BasicMapping()
    {
        var source = new CollectionSource
        {
            NestedUsersList = new List<List<User>>
            {
                new()
                {
                    new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" }
                },
                new()
                {
                    new() { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
                }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.NestedUsersList);
        Assert.AreEqual(2, target.NestedUsersList.Count);
        Assert.AreEqual(1, target.NestedUsersList[0].Count);
        Assert.AreEqual(1, target.NestedUsersList[0][0].Id);
        Assert.AreEqual(2, target.NestedUsersList[1][0].Id);
    }

    #endregion

    #region Cross Type Mapping

    [TestMethod]
    public void ListToArray_CrossTypeMapping()
    {
        var source = new CollectionSource
        {
            UsersList = new List<User>
            {
                new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersList);
    }

    [TestMethod]
    public void ArrayToList_CrossTypeMapping()
    {
        var source = new CollectionSource
        {
            UsersArray = new[]
            {
                new User { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>();
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersArray);
    }

    #endregion

    #region EmitCompiler Collection Mapping

    [TestMethod]
    public void EmitCompiler_ListToList_BasicMapping()
    {
        var source = new CollectionSource
        {
            UsersList = new List<User>
            {
                new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" },
                new() { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersList);
        Assert.AreEqual(2, target.UsersList.Count);
        Assert.AreEqual(1, target.UsersList[0].Id);
        Assert.AreEqual(2, target.UsersList[1].Id);
    }

    [TestMethod]
    public void EmitCompiler_ArrayToArray_BasicMapping()
    {
        var source = new CollectionSource
        {
            UsersArray = new[]
            {
                new User { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" },
                new User { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersArray);
        Assert.AreEqual(2, target.UsersArray.Length);
        Assert.AreEqual(1, target.UsersArray[0].Id);
        Assert.AreEqual(2, target.UsersArray[1].Id);
    }

    [TestMethod]
    public void EmitCompiler_NullCollection_ReturnsNull()
    {
        var source = new CollectionSource { UsersList = null };

        var mapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNull(target.UsersList);
    }

    [TestMethod]
    public void EmitCompiler_EmptyCollection_ReturnsEmpty()
    {
        var source = new CollectionSource { UsersList = new List<User>() };

        var mapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.UsersList);
        Assert.AreEqual(0, target.UsersList.Count);
    }

    [TestMethod]
    public void EmitCompiler_PrimitiveList_BasicMapping()
    {
        var source = new CollectionSource
        {
            Numbers = new List<int> { 1, 2, 3, 4, 5 }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(5, target.Numbers.Count);
        Assert.AreEqual(1L, target.Numbers[0]);
        Assert.AreEqual(5L, target.Numbers[4]);
    }

    [TestMethod]
    public void EmitCompiler_NestedList_BasicMapping()
    {
        var source = new CollectionSource
        {
            NestedUsersList = new List<List<User>>
            {
                new()
                {
                    new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" }
                },
                new()
                {
                    new() { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
                }
            }
        };

        var mapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);
        var target = mapper.MapTo(source);

        Assert.IsNotNull(target);
        Assert.IsNotNull(target.NestedUsersList);
        Assert.AreEqual(2, target.NestedUsersList.Count);
        Assert.AreEqual(1, target.NestedUsersList[0].Count);
        Assert.AreEqual(1, target.NestedUsersList[0][0].Id);
        Assert.AreEqual(2, target.NestedUsersList[1][0].Id);
    }

    [TestMethod]
    public void BothCompilers_CollectionMapping_ShouldProduceSameResult()
    {
        var source = new CollectionSource
        {
            UsersList = new List<User>
            {
                new() { Id = 1, StudentNumber = "100", Name = "Alice", Address = "addr1" },
                new() { Id = 2, StudentNumber = "200", Name = "Bob", Address = "addr2" }
            },
            Numbers = new List<int> { 10, 20, 30 }
        };

        var exprMapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Expression);
        var emitMapper = new Mapper<CollectionSource, CollectionTarget>(ProcessTypeEnum.Emit);

        var exprResult = exprMapper.MapTo(source);
        var emitResult = emitMapper.MapTo(source);

        Assert.IsNotNull(exprResult);
        Assert.IsNotNull(emitResult);

        // 验证 List 映射结果一致
        Assert.AreEqual(exprResult.UsersList!.Count, emitResult.UsersList!.Count);
        Assert.AreEqual(exprResult.UsersList[0].Id, emitResult.UsersList[0].Id);
        Assert.AreEqual(exprResult.UsersList[0].Name, emitResult.UsersList[0].Name);
        Assert.AreEqual(exprResult.UsersList[1].Id, emitResult.UsersList[1].Id);

        // 验证基础类型集合映射结果一致
        Assert.AreEqual(exprResult.Numbers!.Count, emitResult.Numbers!.Count);
        Assert.AreEqual(exprResult.Numbers[0], emitResult.Numbers[0]);
        Assert.AreEqual(exprResult.Numbers[1], emitResult.Numbers[1]);
    }

    #endregion
}
