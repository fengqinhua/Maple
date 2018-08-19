using Maple.Core.Data;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Repositories;
using Maple.Core.Reflection;
using Maple.Data.PerformanceTests.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Maple.Data.PerformanceTests
{
    public class MapleDataTest
    {
        public string DataSettingName { get; private set; } = "test";
        public IDataProviderFactory DataProviderFactory { get; private set; }
        private IRepository<User, long> mapleRepository = null;
        private IDataProvider dataProvider = null;
        private IEntityMapper entityInfo = null;

        public MapleDataTest()
        {
            //DatabaseCommon.DbSqlNeedLog = true;

            DbDriveFactories.SetFactory<MySql.Data.MySqlClient.MySqlClientFactory>(new MySQLTranslator().ProviderInvariantName);
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));

            DataProviderFactory = new DataProviderFactory();
            DataProviderFactory.AddDataSettings(getDefaultDataSetting());

            this.entityInfo = EntityMapperFactory.Instance.GetEntityMapper(typeof(User));
            this.dataProvider = this.DataProviderFactory.CreateProvider(this.DataSettingName);
            this.mapleRepository = new UserRepository(this.DataProviderFactory, this.DataSettingName);
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

        private DataSetting getDefaultDataSetting()
        {
            DataSetting ds = new DataSetting()
            {
                Name = DataSettingName,
                DataConnectionString = "Server=127.0.0.1;port=3306;Database=mapleleaf;Uid=root;Pwd=root;charset=utf8;SslMode=none;",
                DataSouceType = Core.Data.DataSouceType.MySQL
            };
            return ds;
        }
    }
}
