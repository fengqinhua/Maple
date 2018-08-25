using Maple.Core.Data.DbMappers;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Maple.Core.Data.Conditions;

namespace Maple.Core.Data.ModelConfiguration
{ 
    /// <summary>
    /// 实体映射配置
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityConfiguration<TEntity, TPrimaryKey> : IEntityConfiguration where TEntity : class, IEntity<TPrimaryKey>
    {
        private IEntityMapper entityMapper = null;
        private IExpressionParser expressionParser = null;

        public EntityConfiguration() { }

        /// <summary>
        /// 执行映射配置
        /// </summary>
        public void ExceConfiguration(IEntityMapper entityMapper)
        {
            this.entityMapper = entityMapper;
            this.expressionParser = new ExpressionParser(entityMapper);
            this.Configuration();
        }

        /// <summary>
        /// 执行映射配置
        /// </summary>
        public abstract void Configuration();

        public EntityConfiguration<TEntity, TPrimaryKey> ToTable(string tableName)
        {
            return ToTable(tableName, "");
        }

        public EntityConfiguration<TEntity, TPrimaryKey> ToTable(string tableName, string schemaName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                this.entityMapper.TableName = tableName;
                this.entityMapper.SchemaName = schemaName;
            }

            return this;
        }



        public PropertyConfiguration Property<TKey>(Expression<Func<TEntity, TKey>> propertyExpression)
        {
            IPropertyMapper propertyMapper = this.expressionParser.GetPropertyMapper(propertyExpression);
            if (propertyMapper == null)
                throw new MapleException("Unable to get property information.");

            return new PropertyConfiguration() { PropertyMapper = propertyMapper as PropertyMapper };
        }

    }
    /// <summary>
    /// 实体映射配置
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityConfiguration<TEntity> : EntityConfiguration<TEntity, long>, IEntityConfiguration where TEntity : class, IEntity<long>
    {

    }
}
