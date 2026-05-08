using LeonMapper.Attributes;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeonMapper.Test;

[TestClass]
public class MainTest
{
    [TestMethod]
    public void Mapper_BasicMapping_AllPropertiesMapped()
    {
        var role = new Role()
        {
            RoleId1 = 11,
            RoleName = "Role11",
            test1 = "ttt1",
            test2 = "ttt2"
        };
        var role2 = new Role()
        {
            RoleId1 = 22,
            RoleName = "Role22",
            test1 = "ttt1_2",
            test2 = "ttt2_2"
        };
        var user = new User
        {
            Id = 11,
            StudentNumber = "123",
            Name = "leon",
            Address = "china",
            Role = role,
            test1 = "t1",
            test2 = 2222,
            role2 = role2
        };

        var userMapper = new Mapper<User, UserNew>();
        var newUser = userMapper.MapTo(user);

        Assert.IsNotNull(newUser);
        // 同名属性映射
        Assert.AreEqual(11, newUser.Id);
        // MapTo: StudentNumber → Name
        Assert.AreEqual("123", newUser.Name);
        // IgnoreMap: Address 应被忽略
        Assert.IsNotNull(newUser.Address);
        // 复杂类型映射
        Assert.IsNotNull(newUser.Role);
        Assert.AreEqual("Role11", newUser.Role.RoleName);
        // 字段映射
        Assert.AreEqual("t1", newUser.test1);
        // 类型转换: int → string
        Assert.AreEqual("2222", newUser.test2);
        // 嵌套字段映射
        Assert.IsNotNull(newUser.role2);
        Assert.AreEqual("Role22", newUser.role2.RoleName);
    }
}
