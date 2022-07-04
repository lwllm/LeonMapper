using AutoMapper;
using LeonMapper.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace LeonMapper.Test
{
    [TestClass]
    public class MainTest
    {
        [TestMethod]
        public void MapperTest()
        {
            User user = new User
            {
                Id = 11,
                Name = "leon",
                Address = "china"
            };
            var newUser = Mapper.Map<User, UserNew>(user);
            System.Console.WriteLine(user.ToString());
            System.Console.WriteLine(newUser.ToString());
        }

        [TestMethod]
        public void PerformanceTest()
        {
            User user = new User
            {
                Id = 11,
                Name = "leon",
                Address = "china"
            };
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserNew>());
            var mapper = config.CreateMapper();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1_0000_0000; i++)
            {
                var newUser = new UserNew
                {
                    Id = user.Id,
                    Name = user.Name,
                    Address = user.Address
                };
            }
            sw.Stop();
            System.Console.WriteLine($"{sw.ElapsedMilliseconds}");

            sw.Restart();
            for (int i = 0; i < 1_0000_0000; i++)
            {
                var newUser = Mapper.Map<User, UserNew>(user);
            }
            sw.Stop();
            System.Console.WriteLine($"{sw.ElapsedMilliseconds}");

            sw.Restart();
            for (int i = 0; i < 1_0000_0000; i++)
            {
                UserNew newUser = mapper.Map<User, UserNew>(user);
            }
            sw.Stop();
            System.Console.WriteLine($"{sw.ElapsedMilliseconds}");
        }
    }
}