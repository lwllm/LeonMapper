using LeonMapper.Demos.Model;

namespace LeonMapper.Demos;

public class Class1
{
    public static UserNew Map(User user)
    {
        UserNew userNew = new UserNew();
        userNew.Name = user.Name;
        userNew.Address = user.Address;
        userNew.Id = user.Id;
        return userNew;
    }
}