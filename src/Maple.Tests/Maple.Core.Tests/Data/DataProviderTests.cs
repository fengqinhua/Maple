using Maple.Core.Data;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
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
        
      
        private SqlStatement getDefaultSqlStatement()
        {
            string sql = "select 1;";
            return new SqlStatement(System.Data.CommandType.Text, sql, null);
        }
    }
}
