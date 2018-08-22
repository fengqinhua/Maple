using Maple.Core.Data;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataQuery;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Repositories;
using Maple.Core.Tests.Domain;
using Maple.Core.Tests.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Data
{
    [Collection("Database collection")]
    public class DataProviderTests
    {

        private Fixture.DatabaseFixture fixture;
        public DataProviderTests(Fixture.DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public void CreateProvider_ExecuteScalar()
        {
            IDataProvider dataProvider = this.fixture.DataProviderFactory.CreateProvider(this.fixture.DataSettingName);
            object obj = dataProvider.ExecuteScalar(getDefaultSqlStatement());
            Assert.Equal(string.Format("{0}", obj), "1");
        }




        [Fact]
        public void CreateProvider_MapleQueryable()
        {
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));
            IMapleQueryable<User, long> mapleQueryable = new MapleQueryable<User, long>(this.fixture.DataProviderFactory, this.fixture.DataSettingName);
            mapleQueryable.Count();

            Assert.Equal(0, 1);
        }

        [Fact]
        public void CreateProvider_Repository()
        {
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));
            IDataProvider dataProvider = this.fixture.DataProviderFactory.CreateProvider(this.fixture.DataSettingName);
 
            IRepository<User, long> mapleQueryable = new UserRepository(this.fixture.DataProviderFactory, this.fixture.DataSettingName);

            IdWorker worker = new IdWorker(0);

            long id = worker.NextId();
            User user = new User();
            user.Id = id;
            user.Address = new Address();
            user.Address.CityId = Guid.NewGuid();
            user.Address.Number = 109;
            user.Address.Street = "珞狮南路";

            user.Age = 10;
            user.CreationTime = DateTime.Now;
            user.CreatorUserId = 9527;
            user.DeleterUserId = 9527;
            user.DeletionTime = DateTime.Now;
            user.Height = 175;
            user.IsDeleted = true;
            user.LastModificationTime = DateTime.Now;
            user.LastModifierUserId = 9527;
            user.Name = "Maple";
            user.OrgId = 307;
            user.Six = Six.Woman;
            user.TenantId = 3306;

            bool a = mapleQueryable.Insert(user);
            User temp = mapleQueryable.Single(id);
            a = mapleQueryable.Update(temp);
            temp.Id = temp.Id + 1;
            a = mapleQueryable.InsertOrUpdate(temp);

            var xx = mapleQueryable.GetAllList();

            long c = mapleQueryable.Count();
            c = mapleQueryable.Delete(f => f.Id == id);
            a = mapleQueryable.Delete(id);
            a = mapleQueryable.Delete(temp);


            Assert.Equal(0, 1);
        }


        private SqlStatement getDefaultSqlStatement()
        {
            string sql = "select 1;";
            return new SqlStatement(System.Data.CommandType.Text, sql, null);
        }
    }
}
