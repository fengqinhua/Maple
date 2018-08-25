using BenchmarkDotNet.Attributes;
using Maple.Core;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Data.PerformanceTests
{
    //[ClrJob(true), CoreJob, MonoJob, CoreRtJob]
    [CoreJob]
    [RPlotExporter, RankColumn]
    public class InsertEntityTests
    {
        protected MapleDataTest mapleDataTest = null;
        protected MapleRepositoryDataTest mapleRepositoryDataTest = null;
        protected DapperDataTest dapperDataTest = null;
        protected EFDataTest efDataTest = null;
        protected IdWorker idWorker = null;

        [GlobalSetup]
        public void Setup()
        {
            this.mapleDataTest = new MapleDataTest();
            this.dapperDataTest = new DapperDataTest();
            this.efDataTest = new EFDataTest();
            this.idWorker = new IdWorker(0);
            this.mapleRepositoryDataTest = new MapleRepositoryDataTest();
        }

        [Benchmark]
        public void MapleInsert()
        {
            User user = this.creatNewUser(this.idWorker.NextId());
            this.mapleDataTest.Insert(user);
        }

        [Benchmark]
        public void MapleRepositoryInsert()
        {
            User user = this.creatNewUser(this.idWorker.NextId());
            this.mapleRepositoryDataTest.Insert(user);
        }

        [Benchmark]
        public void DapperInsert()
        {
            User user = this.creatNewUser(this.idWorker.NextId());
            this.dapperDataTest.Insert(user);
        }

        [Benchmark]
        public void EfInsert()
        {
            User user = this.creatNewUser(this.idWorker.NextId());
            this.efDataTest.Insert(user);
        }

        protected User creatNewUser(long id, bool withValue = true)
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
