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
        ///// <summary>
        ///// 深度查找基类是否派生自某个泛型类
        ///// </summary>
        ///// <param name="typeToCheck"></param>
        ///// <param name="genericType"></param>
        ///// <returns></returns>
        //private Type typeDerivedFromGenericType(Type typeToCheck, Type genericType)
        //{
        //    if (typeToCheck == typeof(object))
        //        return null;
        //    else if (typeToCheck == null)
        //        return null;
        //    else if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
        //    {
        //        Type[] genericArguments = typeToCheck.GetGenericArguments();
        //        if (genericArguments.Length == 1)
        //            return genericArguments[0];
        //        else
        //            return null;
        //    }
        //    else
        //        return typeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        //}

        [Fact]
        public void EntityMapperFactory_GetEntityMapper_Configuration()
        {
            //var entityType = typeof(IEntity);
            //var entityConfiguration = typeof(EntityConfiguration<>);
            //var item = typeof(UserEntityConfiguration);
            //Type entity = typeDerivedFromGenericType(item, entityConfiguration);
            //if (entity != null)
            //    EntityConfigurationFactory.SetConfiguration(entity, item);

            EntityConfigurationFactory.SetConfiguration(typeof(User), typeof(UserEntityConfiguration));

            IEntityMapper entityMapper = EntityMapperFactory.Instance.GetEntityMapper(typeof(User));
            Assert.Equal(entityMapper.TableName, "TEST_USER");
            Assert.Equal(entityMapper.OtherProperties.First(f => f.Code == "Name").ColumnName, "USERNAME");
            Assert.Equal(entityMapper.OtherProperties.First(f=>f.Code == "Address_Number").ColumnName, "ADDRESS_NUM");
        }

    }
}
