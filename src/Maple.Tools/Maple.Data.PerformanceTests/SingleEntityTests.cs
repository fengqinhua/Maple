using BenchmarkDotNet.Attributes;
using Maple.Core;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Data.PerformanceTests
{    
    //[ClrJob(true), CoreJob, MonoJob, CoreRtJob]
    [ CoreJob]
    [RPlotExporter, RankColumn]
    public class SingleEntityTests
    {
        protected MapleDataTest mapleDataTest = null;
        protected MapleRepositoryDataTest mapleRepositoryDataTest = null;
        protected DapperDataTest dapperDataTest = null;
        protected EFDataTest efDataTest = null;
        protected IdWorker idWorker = null;
        protected User x = null;

        [GlobalSetup]
        public void Setup()
        {
            this.mapleDataTest = new MapleDataTest();
            this.dapperDataTest = new DapperDataTest();
            this.efDataTest = new EFDataTest();
            this.idWorker = new IdWorker(0);

            this.mapleRepositoryDataTest = new MapleRepositoryDataTest();
            this.x = creatNewUser(idWorker.NextId());
            mapleDataTest.Insert(this.x);

            mapleDataTest.Single(this.x.Id);
            dapperDataTest.Single(this.x.Id);
            efDataTest.Single(this.x.Id);
            this.mapleRepositoryDataTest.Single(this.x.Id);
        }


        [Benchmark]
        public void MapleSingle()
        {
            mapleDataTest.Single(x.Id);
        }

        [Benchmark]
        public void MapleRepositorySingle()
        {
            mapleRepositoryDataTest.Single(x.Id);
        }

        [Benchmark]
        public void DapperSingle()
        {
            dapperDataTest.Single(x.Id);
        }

        [Benchmark]
        public void EfSingle()
        {
            efDataTest.Single(x.Id);
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
