using Maple.Core.Data.DataQuery;
using Maple.Core.Domain.Repositories;
using Maple.Core.Infrastructure;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Repositories
{
    [Collection("Database connection")]
    public class RepositoryTests
    {
        private Fixture.DatabaseFixture fixture;
        public RepositoryTests(Fixture.DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Repository_NEW()
        {
            var repository1 = EngineContext.Current.Resolve<IRepository<User>>();
            var repository2 = EngineContext.Current.Resolve<IRepository<User>>();
            Assert.NotEqual(repository1, null);
            Assert.NotEqual(repository2, null);
            Assert.Equal(repository1, repository2);
        }


        [Fact]
        public void Repository_CURD()
        {
            var repository1 = EngineContext.Current.Resolve<IRepository<User>>();

            long id = EntityBuilder.worker.NextId();
            User user = EntityBuilder.CreatNewUser(id, false);
            //插入
            Assert.Equal(repository1.Insert(user), true);
            //读取1
            User userRead1 = repository1.Single(id);
            Assert.NotEqual(userRead1, null);            //读取值不为空
            Assert.True(objectIsSame(user, userRead1));

            //修改1
            userRead1.Height = 1000;
            Assert.Equal(repository1.Update(userRead1), true);

            //读取2
            User userRead2 = repository1.GetAll().Where(f => f.Id == id).FirstOrDefault();
            Assert.NotEqual(userRead2, null);            //读取值不为空
            Assert.True(objectIsSame(userRead1, userRead2));

            //修改2
            userRead2.Address.Number = 1000;
            Assert.Equal(repository1.Update(userRead2), true);

            //删除1
            Assert.Equal(repository1.Delete(userRead2), true);

            //新增或修改
            Assert.Equal(repository1.InsertOrUpdate(userRead2), true);

            //读取3
            User userRead3 = repository1.GetAllList().FirstOrDefault(f => f.Id == id);               //不推荐使用
            Assert.NotEqual(userRead3, null);                                                           //读取值不为空
            Assert.True(objectIsSame(userRead3, userRead2));

            //删除2
            Assert.Equal(repository1.Delete(f => f.Id == id) == 1, true);

            //新增或修改
            Assert.Equal(repository1.InsertOrUpdate(userRead2), true);

            //查询
            User userRead4 = repository1.GetAll().Where(f => f.IsDeleted == userRead2.IsDeleted && f.Id == userRead2.Id).FirstOrDefault();
            Assert.NotEqual(userRead4, null);                                                           //读取值不为空


            //删除3
            Assert.Equal(repository1.Delete(id), true);


        }

        [Fact]
        public void Repository_GetAll()
        {
            var repository1 = EngineContext.Current.Resolve<IRepository<User>>();
            var data = repository1.GetAllList();
            Assert.NotEqual(data, null);
        }

        [Fact]
        public void Repository_Query()
        {
            var mapleQueryable = EngineContext.Current.Resolve<IRepository<User>>();
            int allCount = 1000;

            mapleQueryable.Delete(f => 1 == 1);
            for (int i = 0; i < allCount; i++)
            {
                mapleQueryable.Insert(EntityBuilder.CreatNewUser(i, true));
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

            IPagedList<User> paged = new MaplePagedList<User, long>(mapleQueryable.GetAll().OrderBy(f => f.Id), 5, 10);
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
    }
}
