using Maple.Core.Data.DbMappers;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Entities;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Data
{
    public class DbMappersTests
    {
        [Fact]
        public void EntityMapperFactory_GetEntityMapper_Configuration()
        {
            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));

            IEntityMapper entityMapper = EntityMapperFactory.Instance.GetEntityMapper(typeof(User));
            Assert.Equal(entityMapper.TableName, "TEST_USER");
            Assert.Equal(entityMapper.OtherProperties.First(f => f.Code == "Name").ColumnName, "USERNAME");
            Assert.Equal(entityMapper.OtherProperties.First(f=>f.Code == "Address_Number").ColumnName, "ADDRESS_NUM");
        }

    }
}
