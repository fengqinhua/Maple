using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataQuery;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Repositories;
using Maple.Core.Tests.Domain;
using Maple.Core.Tests.Fixture;
using Maple.Core.Tests.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Data
{


    [Collection("Database collection")]
    public class UnitOfWorkTest
    {
        private IdWorker worker = new IdWorker(0);
        private MapleRepositoryBase<User, long> mapleQueryable = null;
        private IRepositoryContext repositoryContext = null; 
        private DatabaseFixture fixture;


        public UnitOfWorkTest(Fixture.DatabaseFixture fixture)
        {
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));

            this.fixture = fixture;

            this.repositoryContext = new MapleRepositoryContext();
            this.mapleQueryable = new UserRepository(this.fixture.DataProviderFactory, this.fixture.DataSettingName);
        }

        [Fact]
        public void Repository_Commit_Success()
        {
            long key = worker.NextId();

            repositoryContext.RegisterNew(creatNewUser(key, false), this.mapleQueryable);
            repositoryContext.RegisterNew(creatNewUser(worker.NextId(), false), this.mapleQueryable);
            repositoryContext.RegisterNew(creatNewUser(worker.NextId(), false), this.mapleQueryable);


            repositoryContext.Commit();

            var user = this.mapleQueryable.Single(key);
            Assert.NotEqual(user, null);
            Assert.Equal(user.Id, key);
        }

        [Fact]
        public void Repository_Commit_Faile()
        {
            long key = worker.NextId();
            try
            {
                repositoryContext.RegisterNew(creatNewUser(key, true), this.mapleQueryable);
                repositoryContext.RegisterNew(creatNewUser(worker.NextId(), true), this.mapleQueryable);
                var test = creatNewUser(worker.NextId(), true);
                test.Name = "一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十一二三四五六七八九十";
                repositoryContext.RegisterNew(test, this.mapleQueryable);
                repositoryContext.Commit();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var user = this.mapleQueryable.Single(key);
            Assert.Equal(user, null);
        }



        private User creatNewUser(long id, bool withValue = true)
        {
            User user = new User();
            user.Address = new Address();
            user.Id = id;
            user.CreatorUserId = 9527;
            user.DeleterUserId = 9527;
            user.LastModifierUserId = 9527;
            user.DeletionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            user.LastModificationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            user.CreationTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            if (withValue)
            {
                user.Address.CityId = Guid.NewGuid();
                user.Address.Number = 109;
                user.Address.Street = "珞狮南路";
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
