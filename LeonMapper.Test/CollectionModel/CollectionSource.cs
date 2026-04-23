using System.Collections.Generic;
using LeonMapper.Test.Model;

namespace LeonMapper.Test.CollectionModel;

/// <summary>
/// 包含各种集合类型的源模型
/// </summary>
public class CollectionSource
{
    /// <summary>
    /// List 类型
    /// </summary>
    public List<User>? UsersList { get; set; }

    /// <summary>
    /// 数组类型
    /// </summary>
    public User[]? UsersArray { get; set; }

    /// <summary>
    /// IEnumerable 类型
    /// </summary>
    public IEnumerable<User>? UsersEnumerable { get; set; }

    /// <summary>
    /// 基础类型集合
    /// </summary>
    public List<int>? Numbers { get; set; }

    /// <summary>
    /// 嵌套集合（集合的集合）
    /// </summary>
    public List<List<User>>? NestedUsersList { get; set; }
}

/// <summary>
/// 包含各种集合类型的目标模型
/// </summary>
public class CollectionTarget
{
    /// <summary>
    /// List 类型
    /// </summary>
    public List<UserNew>? UsersList { get; set; }

    /// <summary>
    /// 数组类型
    /// </summary>
    public UserNew[]? UsersArray { get; set; }

    /// <summary>
    /// IEnumerable 类型
    /// </summary>
    public IEnumerable<UserNew>? UsersEnumerable { get; set; }

    /// <summary>
    /// 基础类型集合
    /// </summary>
    public List<long>? Numbers { get; set; }

    /// <summary>
    /// 嵌套集合（集合的集合）
    /// </summary>
    public List<List<UserNew>>? NestedUsersList { get; set; }
}
