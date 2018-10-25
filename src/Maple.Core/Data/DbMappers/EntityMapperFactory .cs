using Maple.Core.Data.ModelConfiguration;
using Maple.Core.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbMappers
{
    public class EntityMapperFactory
    {
        private readonly ConcurrentDictionary<Type, IEntityMapper> _classMaps = new ConcurrentDictionary<Type, IEntityMapper>();

        private EntityMapperFactory() { }
        public static EntityMapperFactory Instance { get { return Nested.instance; } }

        /// <summary>
        /// 通过嵌套类实现单利模式
        /// </summary>
        class Nested
        {
            static Nested() { }
            internal static readonly EntityMapperFactory instance = new EntityMapperFactory();
        }

        public IEntityMapper GetEntityMapper(Type entityType)
        {
            return this._classMaps.GetOrAdd(entityType, x => { return creatEntityMapper(x); });
        }

        private IEntityMapper creatEntityMapper(Type entityType)
        {
            IEntityMapper map = new AutoEntityMapper(entityType);
            //按照用户自定义配置执行
            IEntityConfiguration entityConfiguration = EntityConfigurationFactory.GetEntityConfiguration(entityType);
            if (entityConfiguration != null)
                entityConfiguration.ExceConfiguration(map);

            return map;
        }
    }
}
