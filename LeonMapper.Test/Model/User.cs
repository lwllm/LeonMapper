using LeonMapper.Attributes;

namespace LeonMapper.Test.Model
{
    public class User
    {
        public User()
        {
        }

        public int Id { get; set; }
        public string StudentNumber { get; set; }
        public string Name { get; set; }

        [IgnoreMap] public string Address { get; set; }
        public Role Role { get; set; }

        public string test1;
        public int test2;

        public Role role2;

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
        [IgnoreMapTo] public string Name { get; set; }
        public string Address { get; set; }
        public RoleNew Role { get; set; }

        public string test1;
        public string test2;
        
        public RoleNew role2;

        public override string ToString()
        {
            return
                $"{{{nameof(Id)}={Id.ToString()}, {nameof(StudentNumber)}={StudentNumber.ToString()}, {nameof(Name)}={Name}, {nameof(Address)}={Address}, {nameof(Role)}={Role}, {nameof(test1)}={test1}, {nameof(test2)}={test2}, {nameof(role2)}={role2}}}";
        }
    }
}