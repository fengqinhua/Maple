using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Fixture
{
    public class DatabaseFixture : IDisposable
    {
        public string DataSettingName { get; private set; } = "test";
        public IDataProviderFactory DataProviderFactory { get; private set; }

        public DatabaseFixture()
        {
            DbProviderFactories.SetFactory<System.Data.SqlClient.SqlClientFactory>(new Sql2000Translator().ProviderInvariantName);
            DbProviderFactories.SetFactory<MySql.Data.MySqlClient.MySqlClientFactory>(new MySQLTranslator().ProviderInvariantName);
            DbProviderFactories.SetFactory<System.Data.SQLite.SQLiteFactory>(new SqliteTranslator().ProviderInvariantName);

            DataProviderFactory = new DataProviderFactory();
            DataProviderFactory.AddDataSettings(getDefaultDataSetting());
        }

        public void Dispose()
        {
            if (DataProviderFactory != null)
                DataProviderFactory.Dispose();
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

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
