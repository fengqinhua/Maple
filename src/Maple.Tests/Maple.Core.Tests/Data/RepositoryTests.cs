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
    public class RepositoryTests
    {
        private IdWorker worker = new IdWorker(0);
        private IRepository<User, long> mapleQueryable = null;
        private IDataProvider dataProvider = null;
        private DatabaseFixture fixture;

        public RepositoryTests(Fixture.DatabaseFixture fixture)
        {
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));

            this.fixture = fixture;
            this.dataProvider = this.fixture.DataProviderFactory.CreateProvider(this.fixture.DataSettingName);
            this.mapleQueryable = new UserRepository(this.fixture.DataProviderFactory, this.fixture.DataSettingName);
        }

        [Fact]
        public void Repository_CURD()
        {
            long id = worker.NextId();
            User user = creatNewUser(id, false);
            //插入
            Assert.Equal(mapleQueryable.Insert(user), true);
            //读取1
            User userRead1 = mapleQueryable.Single(id);
            Assert.NotEqual(userRead1, null);            //读取值不为空
            Assert.True(objectIsSame(user, userRead1));

            //修改1
            userRead1.Height = 1000;
            Assert.Equal(mapleQueryable.Update(userRead1), true);

            //读取2
            User userRead2 = mapleQueryable.GetAll().Where(f => f.Id == id).FirstOrDefault();
            Assert.NotEqual(userRead2, null);            //读取值不为空
            Assert.True(objectIsSame(userRead1, userRead2));

            //修改2
            userRead2.Address.Number = 1000;
            Assert.Equal(mapleQueryable.Update(userRead2), true);

            //删除1
            Assert.Equal(mapleQueryable.Delete(userRead2), true);

            //新增或修改
            Assert.Equal(mapleQueryable.InsertOrUpdate(userRead2), true);

            //读取3
            User userRead3 = mapleQueryable.GetAllList().FirstOrDefault(f => f.Id == id);               //不推荐使用
            Assert.NotEqual(userRead3, null);            //读取值不为空
            Assert.True(objectIsSame(userRead3, userRead2));

            //删除2
            Assert.Equal(mapleQueryable.Delete(f => f.Id == id) == 1, true);

            //新增或修改
            Assert.Equal(mapleQueryable.InsertOrUpdate(userRead2), true);

            //删除3
            Assert.Equal(mapleQueryable.Delete(id), true);


        }

        [Fact]
        public void Repository_Query()
        {
            int allCount = 1000;

            mapleQueryable.Delete(f => 1 == 1);
            for (int i = 0; i < allCount; i++)
            {
                mapleQueryable.Insert(creatNewUser(i, true));
            }

            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).Select().Count(), allCount);
            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.CreationTime).Count(), allCount);
            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.CreationTime).LongCount(), allCount);
            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.CreationTime).ThenBy(f => f.Id).Max(f => f.Id), allCount - 1);
            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.CreationTime).ThenBy(f => f.Id).Min(f => f.Id), 0);
            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.CreationTime).ThenBy(f => f.Id).Sum(f => f.Id), (0 + allCount - 1) * allCount / 2);
            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.CreationTime).ThenBy(f => f.Id).Average(f => f.Id), ((double)(0 + allCount - 1)) / 2);

            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.Id).Top(10).Select().Count, 10);

            Assert.Equal(mapleQueryable.GetAll().Where(f => f.CreationTime < DateTime.Now).OrderBy(f => f.Id).Range(10, 20).Select().Count, 10);

            IPagedList<User> paged = new MaplePagedList<User, long>(mapleQueryable.GetAll().OrderBy(f => f.Id),5, 10);
            Assert.Equal(paged.Count, 10);
            Assert.Equal(paged.First().Id, 40);
            Assert.Equal(paged.Last().Id, 49);

        }

        private bool objectIsSame(object obj1, object obj2)
        {
            string x1 = SerializeHelper.ConvertToString(SerializeHelper.SerializeToXml(obj1));
            string x2 = SerializeHelper.ConvertToString(SerializeHelper.SerializeToXml(obj2));
            return x1 == x2;
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
