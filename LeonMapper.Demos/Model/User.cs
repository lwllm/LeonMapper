namespace LeonMapper.Demos.Model
{
    public class User
    {
        public User()
        {
            
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(Name)}={Name}, {nameof(Address)}={Address}}}";
        }
    }

    public class UserNew
    {
        public UserNew()
        {
            
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(Name)}={Name}, {nameof(Address)}={Address}}}";
        }
    }
}
