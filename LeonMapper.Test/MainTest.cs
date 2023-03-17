using System;
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
        public void IsPrimitiveTest()
        {
            // 整数类型：sbyte, byte, short, ushort, int, uint, long, ulong
            // 浮点数类型：float, double, decimal
            // 字符类型：char
            // 布尔类型：bool
            // 指针类型：IntPtr, UIntPtr
            var types = new[]
            {
                typeof(sbyte),
                typeof(byte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong), typeof(float), typeof(double), typeof(decimal),
                typeof(char), typeof(bool), typeof(IntPtr), typeof(UIntPtr),
                typeof(string)
            };
            foreach (var type in types)
            {
                Console.WriteLine($"{type.Name} IsPrimitive : {type.IsPrimitive}");
            }
        }

        [TestMethod]
        public void MapperTest()
        {
            var role = new Role()
            {
                RoleId1 = 11,
                RoleName = "Role11",
                test1 = "ttt1",
                test2 = "ttt2"
            };
            var role2 = new Role()
            {
                RoleId1 = 22,
                RoleName = "Role22",
                test1 = "ttt1_2",
                test2 = "ttt2_2"
            };
            var user = new User
            {
                Id = 11,
                StudentNumber = "123",
                Name = "leon",
                Address = "china",
                Role = role,
                test1 = "t1",
                test2 = 2222,
                role2 = role2
            };

            var userMapper = new Mapper<User, UserNew?>();
            // var roleMapper = new Mapper<Role, RoleNew>();
            var newUser = userMapper.MapTo(user);
            // var newUser2 = userMapper.MapTo(user, ProcessTypeEnum.Emit);
            // var newRole = roleMapper.MapTo(role);
            System.Console.WriteLine(newUser.ToString());
            // System.Console.WriteLine(newUser2.ToString());
            // System.Console.WriteLine(newRole.ToString());

            // var config = new MapperConfiguration(cfg =>
            // {
            //     cfg.CreateMap<User, UserNew>();
            //     cfg.CreateMap<Role, RoleNew>();
            // });
            // var mapper = config.CreateMapper();
            // var newUser2 = mapper.Map<UserNew>(user);
            // System.Console.WriteLine(newUser2.ToString());
        }

        [TestMethod]
        public void PerformanceTest()
        {
            Role role = new Role()
            {
                RoleId1 = 22,
                RoleName = "Role22",
                test1 = "ttt1"
            };
            User user = new User
            {
                Id = 11,
                StudentNumber = "123",
                Name = "leon",
                Address = "china",
                Role = role
            };
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserNew>();
                cfg.CreateMap<Role, RoleNew>();
            });
            var testCount = 10000_0000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < testCount; i++)
            {
                var newUser = new UserNew
                {
                    Id = user.Id,
                    Name = user.Name,
                    Address = user.Address,
                    StudentNumber = System.Convert.ToInt32(user.StudentNumber),
                };
                var newRole = new RoleNew()
                {
                    RoleId2 = role.RoleId1,
                    RoleName = role.RoleName,
                    test1 = role.test1
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