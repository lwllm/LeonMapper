# LeonMapper

---

示例：
```c#
var user = new User
{
    Id = 11,
    Name = "leon",
    Address = "china"
};
var userMapper = new Mapper<User, UserNew>();

var newUser = userMapper.MapTo(user);
System.Console.WriteLine(newUser.ToString());

var newUser2 = userMapper.MapTo(user, ProcessTypeEnum.Emit);
System.Console.WriteLine(newUser2.ToString());
```

暂时只支持同类型的转换，提供 **Expression** 和 **Emit** 两套实现