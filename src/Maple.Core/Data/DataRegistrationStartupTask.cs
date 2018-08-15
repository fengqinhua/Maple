using Maple.Core.Configuration;
using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Entities;
using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data
{
    public class DataRegistrationStartupTask : IStartupTask
    {
        public int Order => 1;

        public void Execute()
        {
            //设置SQL执行的默认值
            var mapleConfig = EngineContext.Current.Resolve<MapleConfig>();
            if(mapleConfig != null)
            {
                DatabaseCommon.DbCommandTimeOut = mapleConfig.DbCommandTimeOut;
                DatabaseCommon.DbSqlNeedLog = mapleConfig.DbSqlNeedLog;
            }

            //设置实体类自定义映射信息
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var types = typeFinder.FindClassesOfType(typeof(IEntityConfiguration), true);
            var entityType = typeof(IEntity);
            var entityConfiguration = typeof(EntityConfiguration<>);

            foreach (var item in types)
            {
                Type entity = typeDerivedFromGenericType(item, entityConfiguration);
                if(entity != null)
                    EntityConfigurationFactory.SetConfiguration(entity, item);
            }
        }

        /// <summary>
        /// 深度查找基类是否派生自某个泛型类
        /// </summary>
        /// <param name="typeToCheck"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        private Type typeDerivedFromGenericType(Type typeToCheck, Type genericType)
        {
            if (typeToCheck == typeof(object))
                return null;
            else if (typeToCheck == null)
                return null;
            else if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                Type[] genericArguments = typeToCheck.GetGenericArguments();
                if (genericArguments.Length == 1)
                    return genericArguments[0];
                else
                    return null;
            }
            else
                return typeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        }

    }
}
