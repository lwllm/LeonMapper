namespace LeonMapper.Test.Model;

public class Role
{
    public int RoleId1 { get; set; }
    public string RoleName { get; set; }
    public string test1;
    public string test2;
    
    public override string ToString()
    {
        return $"{{{nameof(RoleId1)}={RoleId1.ToString()}, {nameof(RoleName)}={RoleName}, {nameof(test1)}={test1}, {nameof(test2)}={test2}}}";
    }
}

public class RoleNew
{
    public int RoleId2 { get; set; }
    public string RoleName { get; set; }
    public string test1;
    public string test2;
    public override string ToString()
    {
        return $"{{{nameof(RoleId2)}={RoleId2.ToString()}, {nameof(RoleName)}={RoleName}, {nameof(test1)}={test1}, {nameof(test2)}={test2}}}";
    }
}