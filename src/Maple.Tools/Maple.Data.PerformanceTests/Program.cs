using BenchmarkDotNet.Running;
using Maple.Core;
using Maple.Data.PerformanceTests.Entities;
using System;
using System.Diagnostics;

namespace Maple.Data.PerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("执行测试前的准备工作...");

            BenchmarkRunner.Run<InsertEntityTests>();
            BenchmarkRunner.Run<SelectEntityTests>();
            BenchmarkRunner.Run<SingleEntityTests>();

            //old();

            Console.WriteLine();
            Console.WriteLine("测试完成...");
            Console.ReadLine();
        }

        private static void old()
        {
            MapleDataTest mapleDataTest = new MapleDataTest();
            DapperDataTest dapperDataTest = new DapperDataTest();
            EFDataTest efDataTest = new EFDataTest();

            var worker = new IdWorker(0);
            int cycle = 10, count = 100;

            #region 测试插入操作

            mapleDataTest.DeleteAll();

            Exce("Maple 执行插入操作", cycle, count, () =>
            {
                User user = creatNewUser(worker.NextId());
                mapleDataTest.Insert(user);
            });

            dapperDataTest.DeleteAll();

            Exce("Dapper 执行插入操作", cycle, count, () =>
            {
                User user = creatNewUser(worker.NextId());
                dapperDataTest.Insert(user);
            });

            efDataTest.DeleteAll();

            Exce("EF CORE 执行插入操作", cycle, count, () =>
            {
                User user = creatNewUser(worker.NextId());
                efDataTest.Insert(user);
            });

            #endregion

            #region 测试查询1000条记录操作

            mapleDataTest.SelectAll();
            dapperDataTest.SelectAll();
            efDataTest.SelectAll();

            Exce("Maple 执行查询1000条记录操作", cycle, count, () =>
            {
                mapleDataTest.SelectAll();
            });

            Exce("Dapper 执行查询1000条记录操作", cycle, count, () =>
            {
                dapperDataTest.SelectAll();
            });

            Exce("EF CORE 执行查询1000条记录操作", cycle, count, () =>
            {
                efDataTest.SelectAll();
            });


            #endregion

            #region 测试基于ID查询

            User x = creatNewUser(worker.NextId());
            mapleDataTest.Insert(x);
            Exce("Maple 执行基于ID查询", cycle, count, () =>
            {
                mapleDataTest.Single(x.Id);
            });

            Exce("Dapper 执行基于ID查询", cycle, count, () =>
            {
                dapperDataTest.Single(x.Id);
            });

            Exce("EF CORE 执行基于ID查询", cycle, count, () =>
            {
                efDataTest.Single(x.Id);
            });

            #endregion
        }

        private static void Exce(string name, int cycle, int count, Action action)
        {
            Stopwatch sw = new Stopwatch();

            Console.WriteLine();
            Console.WriteLine($"{name} 共循环{cycle}次，每次运行 {count} 遍");
            long sum = 0;

            for (int j = 0; j < cycle; j++)
            {
                sw.Restart();
                for (int i = 0; i < count; i++)
                {
                    action();
                }
                sw.Stop();
                sum += sw.ElapsedMilliseconds;

                Console.Write(sw.ElapsedMilliseconds + "ms | ");
            }

            Console.Write((int)(sum / cycle) + "ms | ");

            Console.WriteLine();
            Console.WriteLine("==============================================");


        }

        private static User creatNewUser(long id, bool withValue = true)
        {
            User user = new User();
            //user.Address = new Address();
            user.Id = id;
            user.CreatorUserId = 9527;
            user.DeleterUserId = 9527;
            user.LastModifierUserId = 9527;
            user.DeletionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            user.LastModificationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            user.CreationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            if (withValue)
            {
                //user.Address.CityId = Guid.NewGuid();
                //user.Address.Number = 109;
                //user.Address.Street = "珞狮南路";
                user.Age = 10;
                user.Height = 175;
                user.IsDeleted = true;
                user.Name = "Maple";
                user.OrgId = 307;
                user.Six = Six.Woman;
                user.TenantId = 3306;
            }

            return user;
        }
    }
}
