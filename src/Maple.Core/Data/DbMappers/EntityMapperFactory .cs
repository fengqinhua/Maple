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
            static Nested()
            {
            }
            internal static readonly EntityMapperFactory instance = new EntityMapperFactory();
        }

        public IEntityMapper GetEntityMapper<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            IEntityMapper map;
            Type entityType = typeof(IEntity<TPrimaryKey>);

            if (!_classMaps.TryGetValue(entityType, out map))
            {
                map = new AutoEntityMapper<TEntity, TPrimaryKey>();
                _classMaps[entityType] = map;
            }

            return map;
        }

    } 
}
