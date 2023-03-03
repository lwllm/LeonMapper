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
            var user = new User
            {
                Id = 11,
                Name = "leon",
                Address = "china"
            };
            // Role role = new Role()
            // {
            //     RoleId = 22,
            //     RoleName = "Role22",
            //     test = "ttt"
            // };
            var userMapper = new Mapper<User, UserNew>();
            // var roleMapper = new Mapper<Role, RoleNew>();
            var newUser = userMapper.MapTo(user);
            var newUser2 = userMapper.MapTo(user, ProcessTypeEnum.Emit);
            // var newRole = roleMapper.MapTo(role);
            System.Console.WriteLine(newUser.ToString());
            System.Console.WriteLine(newUser2.ToString());
            // System.Console.WriteLine(newRole.ToString());
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
            Role role = new Role()
            {
                RoleId = 22,
                RoleName = "Role22",
                test = "ttt"
            };
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserNew>();
                cfg.CreateMap<Role, RoleNew>();
            });
            var testCount = 1000_0000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < testCount; i++)
            {
                var newUser = new UserNew
                {
                    Id = user.Id,
                    Name = user.Name,
                    Address = user.Address
                };
                var newRole = new RoleNew()
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    test = role.test
                };
            }

            sw.Stop();
            System.Console.WriteLine($"{sw.ElapsedMilliseconds}");

            //LeonMapper
            sw.Restart();
            var userMapper = new Mapper<User, UserNew?>();
            var roleMapper = new Mapper<Role, RoleNew?>();
            for (int i = 0; i < testCount; i++)
            {
                var newUser = userMapper.MapTo(user);
                var newRole = roleMapper.MapTo(role);
            }

            sw.Stop();
            System.Console.WriteLine($"{sw.ElapsedMilliseconds}");

            var mapper = config.CreateMapper();
            //automapper
            sw.Restart();
            for (int i = 0; i < testCount; i++)
            {
                UserNew newUser = mapper.Map<User, UserNew>(user);
                RoleNew newRole = mapper.Map<Role, RoleNew>(role);
            }

            sw.Stop();
            System.Console.WriteLine($"{sw.ElapsedMilliseconds}");
        }
    }
}