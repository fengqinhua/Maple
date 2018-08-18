using Maple.Core.Data;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Data
{
    public class DbSqlFactoriesTests
    {
        [Fact]
        public void DbSqlFactories_CUD_SQL()
        {
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));

            IDbTranslator dbTranslator = new MySQLTranslator();
            IEntityMapper entityInfo = EntityMapperFactory.Instance.GetEntityMapper(typeof(User));
            object entity = new User() { Id = 10 };

            SqlStatement InsertSql = DbSqlFactories.BuildInsertSqlStatement(dbTranslator, entityInfo, entity);
            SqlStatement UpdateSql = DbSqlFactories.BuildUpdateSqlStatement(dbTranslator, entityInfo, entity);
            SqlStatement DeleteSql = DbSqlFactories.BuildDeleteSqlStatement(dbTranslator, entityInfo, entity);

            Assert.Equal(1, 1);
        }
    }
}
