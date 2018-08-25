using Maple.Core.Data;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataQuery;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Repositories;
using Maple.Core.Infrastructure;
using Maple.Core.Reflection;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Maple.Data.PerformanceTests
{
    public class MapleRepositoryDataTest
    {
        private IRepository<User> mapleRepository = null;
        public MapleRepositoryDataTest()
        {
            Maple.Core.Tests.Fixture.DatabaseFixture DatabaseFixture = new Maple.Core.Tests.Fixture.DatabaseFixture();

           this.mapleRepository = EngineContext.Current.Resolve<IRepository<User>>();
        }

        public void Insert(User user)
        {
            this.mapleRepository.Insert(user);
        }

        public void Update(User user)
        {
            this.mapleRepository.Update(user);
        }

        public void Delete(User user)
        {
            this.mapleRepository.Delete(user);
        }

        public void DeleteAll()
        {
            this.mapleRepository.Delete(f => true);
        }

        public void SelectAll()
        {
            //string query = "SELECT Id,USERNAME,Age,Height,Six,ExtensionData,OrgId,TenantId,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId FROM TEST_USER  ORDER BY Id ASC";

            //var ret = new System.ComponentModel.BindingList<User>();
            //this.dataProvider.ExecuteReader(new Core.Data.SqlStatement(System.Data.CommandType.Text, query), (P_0) =>
            //{
            //    while (P_0.Read())
            //    {
            //        do
            //        {
            //            User user = new User();
            //            user.Id = P_0.GetInt64(0);
            //            if (!P_0.IsDBNull(1))
            //            {
            //                user.Name = P_0.GetString(1);
            //            }
            //            user.Age = P_0.GetInt32(2);
            //            user.Height = P_0.GetDouble(3);
            //            user.Six = (Six)P_0.GetInt32(4);
            //            if (!P_0.IsDBNull(5))
            //            {
            //                user.ExtensionData = P_0.GetString(5);
            //            }
            //            user.OrgId = P_0.GetInt64(6);
            //            user.TenantId = P_0.GetInt64(7);

            //            user.IsDeleted = ((P_0.GetInt16(8) != 0) ? true : false);
            //            if (!P_0.IsDBNull(9))
            //            {
            //                user.DeleterUserId = P_0.GetInt64(9);
            //            }
            //            if (!P_0.IsDBNull(10))
            //            {
            //                user.DeletionTime = P_0.GetDateTime(10);
            //            }
            //            if (!P_0.IsDBNull(11))
            //            {
            //                user.LastModificationTime = P_0.GetDateTime(11);
            //            }
            //            if (!P_0.IsDBNull(12))
            //            {
            //                user.LastModifierUserId = P_0.GetInt64(12);
            //            }
            //            user.CreationTime = P_0.GetDateTime(13);
            //            if (!P_0.IsDBNull(14))
            //            {
            //                user.CreatorUserId = P_0.GetInt64(14);
            //            }

            //            ret.Add(user);
            //        }
            //        while (P_0.Read());
            //    }
            //});

            var data = this.mapleRepository.GetAllList();
        }

        public void Single(long key)
        {
            var data = this.mapleRepository.Single(key);
        }

    }

    public class MapleDataTest
    {
        private IDataProvider dataProvider = null;
        public IEntityMapper EntityInfo { get; protected set; }
        private IDataProviderFactory dataProviderFactory = null;
        public MapleDataTest()
        {
            Maple.Core.Tests.Fixture.DatabaseFixture DatabaseFixture = new Maple.Core.Tests.Fixture.DatabaseFixture();
            this.dataProviderFactory = new SimpleDataProviderFactory(EngineContext.Current.Resolve<IDataSettingContainer>());
            this.dataProvider = this.dataProviderFactory.GetDataProvider(DataSetting.DefalutDataSettingName);
            this.EntityInfo = EntityMapperFactory.Instance.GetEntityMapper(typeof(User));
        }

        public void Insert(User user)
        {
            SqlStatement sqlStatement = DbSqlFactories.BuildInsertSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, user);
            dataProvider.ExecuteNonQuery(sqlStatement);
        }

        public void Update(User user)
        {
            SqlStatement sqlStatement = DbSqlFactories.BuildUpdateSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, user);
             dataProvider.ExecuteNonQuery(sqlStatement);
        }

        public void Delete(User user)
        {
            SqlStatement sqlStatement = DbSqlFactories.BuildDeleteSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, user);
             dataProvider.ExecuteNonQuery(sqlStatement);
        }

        public void DeleteAll()
        {

        }

        public void SelectAll()
        {
            //string query = "SELECT Id,USERNAME,Age,Height,Six,ExtensionData,OrgId,TenantId,IsDeleted,DeleterUserId,DeletionTime,LastModificationTime,LastModifierUserId,CreationTime,CreatorUserId FROM TEST_USER  ORDER BY Id ASC";

            //var ret = new System.ComponentModel.BindingList<User>();
            //this.dataProvider.ExecuteReader(new Core.Data.SqlStatement(System.Data.CommandType.Text, query), (P_0) =>
            //{
            //    while (P_0.Read())
            //    {
            //        do
            //        {
            //            User user = new User();
            //            user.Id = P_0.GetInt64(0);
            //            if (!P_0.IsDBNull(1))
            //            {
            //                user.Name = P_0.GetString(1);
            //            }
            //            user.Age = P_0.GetInt32(2);
            //            user.Height = P_0.GetDouble(3);
            //            user.Six = (Six)P_0.GetInt32(4);
            //            if (!P_0.IsDBNull(5))
            //            {
            //                user.ExtensionData = P_0.GetString(5);
            //            }
            //            user.OrgId = P_0.GetInt64(6);
            //            user.TenantId = P_0.GetInt64(7);

            //            user.IsDeleted = ((P_0.GetInt16(8) != 0) ? true : false);
            //            if (!P_0.IsDBNull(9))
            //            {
            //                user.DeleterUserId = P_0.GetInt64(9);
            //            }
            //            if (!P_0.IsDBNull(10))
            //            {
            //                user.DeletionTime = P_0.GetDateTime(10);
            //            }
            //            if (!P_0.IsDBNull(11))
            //            {
            //                user.LastModificationTime = P_0.GetDateTime(11);
            //            }
            //            if (!P_0.IsDBNull(12))
            //            {
            //                user.LastModifierUserId = P_0.GetInt64(12);
            //            }
            //            user.CreationTime = P_0.GetDateTime(13);
            //            if (!P_0.IsDBNull(14))
            //            {
            //                user.CreatorUserId = P_0.GetInt64(14);
            //            }

            //            ret.Add(user);
            //        }
            //        while (P_0.Read());
            //    }
            //});

            var data = new MapleQueryable<User,long>(this.dataProviderFactory,this.EntityInfo, "").Select();
        }

        public void Single(long key)
        {
            var data = new MapleQueryable<User, long>(this.dataProviderFactory, this.EntityInfo, "").Where(f => f.Id == key).FirstOrDefault();
        }

    }
}
