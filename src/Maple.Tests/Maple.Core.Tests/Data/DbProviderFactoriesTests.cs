﻿using Maple.Core.Configuration;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Data
{
    [Collection("Database connection")]
    public class DbProviderFactoriesTests 
    {
        private Fixture.DatabaseFixture fixture;
        public DbProviderFactoriesTests(Fixture.DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }



        [Fact]
        public void GetDbProviderFactory_MYSQL()
        {
            string name = new MySQLTranslator().ProviderInvariantName;
            DbProviderFactory factory = DbDriveFactories.GetFactory(name);

            Assert.True(factory != null);
        }

        [Fact]
        public void GetDbProviderFactory_SQLITE()
        {
            string name = new SqliteTranslator().ProviderInvariantName;
            DbProviderFactory factory = DbDriveFactories.GetFactory(name);

            Assert.True(factory != null);
        }
    }
}
