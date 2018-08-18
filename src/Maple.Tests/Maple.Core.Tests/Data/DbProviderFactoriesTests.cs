using Maple.Core.Configuration;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Data
{
    [Collection("Database collection")]
    public class DbProviderFactoriesTests 
    {
        private Fixture.DatabaseFixture fixture;
        public DbProviderFactoriesTests(Fixture.DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void GetDbProviderFactory_SQL()
        {
            var mapleConfig = EngineContext.Current.Resolve<MapleConfig>();


            string name = new Sql2000Translator().ProviderInvariantName;
            DbProviderFactory factory = DbProviderFactories.GetFactory(name);

            Assert.NotEqual(factory, null);
        }

        [Fact]
        public void GetDbProviderFactory_MYSQL()
        {
            string name = new MySQLTranslator().ProviderInvariantName;
            DbProviderFactory factory = DbProviderFactories.GetFactory(name);

            Assert.NotEqual(factory, null);
        }

        [Fact]
        public void GetDbProviderFactory_SQLITE()
        {
            string name = new SqliteTranslator().ProviderInvariantName;
            DbProviderFactory factory = DbProviderFactories.GetFactory(name);

            Assert.NotEqual(factory, null);
        }
    }
}
