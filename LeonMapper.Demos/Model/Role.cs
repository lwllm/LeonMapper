namespace LeonMapper.Demos.Model;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string test;
}

public class RoleNew
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string test;
    public override string ToString()
    {
        return $"{{{nameof(RoleId)}={RoleId.ToString()}, {nameof(RoleName)}={RoleName}, {nameof(test)}={test}}}";
    }
}