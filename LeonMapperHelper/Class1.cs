namespace LeonMapperHelper;

public class Class1
{
    public static TestUser2 Test(TestUser1 testUser1)
    {
        // testUser2.Id = testUser.Id;
        // testUser2.Name = testUser.Name;
        var func = new Func<TestUser1, TestUser2>(t1 => new TestUser2
        {
            Id = t1.Id,
            Name = t1.Name
        });
        return func(testUser1);
    }
}

public class TestUser1
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class TestUser2
{
    public int Id { get; set; }
    public string Name { get; set; }
}