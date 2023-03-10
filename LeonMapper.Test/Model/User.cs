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
        public string Address { get; set; }
        public Role Role { get; set; }

        public override string ToString()
        {
            return
                $"{{{nameof(Id)}={Id.ToString()}, {nameof(StudentNumber)}={StudentNumber.ToString()}, {nameof(Name)}={Name}, {nameof(Address)}={Address}, {nameof(Role)}={Role}}}";
        }
    }

    public class UserNew
    {
        public UserNew()
        {
        }

        public int Id { get; set; }
        public int StudentNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public RoleNew Role { get; set; }

        public override string ToString()
        {
            return
                $"{{{nameof(Id)}={Id.ToString()}, {nameof(StudentNumber)}={StudentNumber.ToString()}, {nameof(Name)}={Name}, {nameof(Address)}={Address}, {nameof(Role)}={Role}}}";
        }
    }
}