using LeonMapper.Attributes;

namespace LeonMapper.Test.Model
{
    public class User
    {
        public User()
        {
        }

        public int Id { get; set; }
        
        [MapTo(nameof(UserNew.Name))] 
        public string StudentNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [IgnoreMap] public string Address { get; set; } = string.Empty;
        public Role Role { get; set; } = null!;

        public string test1 = string.Empty;
        public int test2;

        public Role role2 = null!;

        public override string ToString()
        {
            return
                $"{{{nameof(Id)}={Id.ToString()}, {nameof(StudentNumber)}={StudentNumber.ToString()}, {nameof(Name)}={Name}, {nameof(Address)}={Address}, {nameof(Role)}={Role}, {nameof(test1)}={test1}, {nameof(test2)}={test2},{nameof(role2)}={role2}}}";
        }
    }

    public class UserNew
    {
        public UserNew()
        {
        }

        public int Id { get; set; }
        public int StudentNumber { get; set; }
        [IgnoreMapTo] public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public RoleNew Role { get; set; } = null!;

        public string test1 = string.Empty;
        public string test2 = string.Empty;

        public RoleNew role2 = null!;

        public override string ToString()
        {
            return
                $"{{{nameof(Id)}={Id.ToString()}, {nameof(StudentNumber)}={StudentNumber.ToString()}, {nameof(Name)}={Name}, {nameof(Address)}={Address}, {nameof(Role)}={Role}, {nameof(test1)}={test1}, {nameof(test2)}={test2}, {nameof(role2)}={role2}}}";
        }
    }
}