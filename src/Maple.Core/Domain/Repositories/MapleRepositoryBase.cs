using Maple.Core.Data;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Maple.Core.Domain.Repositories
{
    public abstract class MapleRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>, IAggregateRoot
    {
        protected IDataProvider _dataProvider = null;
        protected DataProviderFactory _dataProviderFactory = null;

        public IEntityMapper EntityInfo { get; protected set; }

        public MapleRepositoryBase(DataProviderFactory dataProviderFactory)
        {
            this._dataProviderFactory = dataProviderFactory;
            this.EntityInfo = EntityMapperFactory.Instance.GetEntityMapper<TEntity, TPrimaryKey>();
        }

        public virtual bool Insert(TEntity entity)
        {
            IDataProvider dataProvider = getDataProvider();
            return execInsert(dataProvider, entity);
        }

        public virtual bool Update(TEntity entity)
        {
            IDataProvider dataProvider = getDataProvider();
            return execUpdate(dataProvider, entity);
        }

        public virtual TEntity Single(TPrimaryKey id)
        {
            return null;
        }



        public abstract bool InsertOrUpdate(TEntity entity);
        public abstract bool Update(TPrimaryKey id, Action<TEntity> updateAction);
        public abstract bool Delete(TEntity entity);
        public abstract bool Delete(TPrimaryKey id);
        public abstract int Delete(Expression<Func<TEntity, bool>> predicate);
        public abstract IEnumerable<TEntity> GetAll();
        public abstract long Count();
        public abstract long Count(Expression<Func<TEntity, bool>> predicate);


        #region IRepository接口具体的执行函数 

        /// <summary>
        /// 获取缺省的数据库连接驱动
        /// </summary>
        /// <returns></returns>
        protected virtual IDataProvider getDataProvider()
        {
            if (this._dataProvider == null)
                this._dataProvider = _dataProviderFactory.CreateProvider(DataSetting.DefalutDataSettingName);
            return this._dataProvider;
        }
        /// <summary>
        /// 执行插入
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool execInsert(IDataProvider dataContext, IEntity<TPrimaryKey> entity)
        {
            if (entity == null)
                return false;

            SqlStatement sqlStatement = this.buildInsertSqlStatement(dataContext, entity);
            return dataContext.ExecuteNonQuery(sqlStatement) > 0;
        }
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool execUpdate(IDataProvider dataContext, IEntity<TPrimaryKey> entity)
        {
            if (entity == null)
                return false;

            SqlStatement sqlStatement = this.buildUpdateSqlStatement(dataContext, entity);
            return dataContext.ExecuteNonQuery(sqlStatement) > 0;
        }


        
        #endregion

        #region SQL相关方法

        /// <summary>
        /// 生成Insert SQL语句
        /// </summary>
        /// <returns></returns>
        protected virtual string buildInsertSQL(IDataProvider dataContext)
        {
            if (this.EntityInfo.PKeyProperties.Count == 0)
                throw new Exception("当前实体中未设置主键");

            StringBuilder sBuilderFiled = new StringBuilder();
            StringBuilder sBuilderValue = new StringBuilder();

            foreach (var propertyMapper in this.EntityInfo.PKeyProperties)
            {
                sBuilderFiled.Append(dataContext.DatabaseContext.DbTranslator.Quote(propertyMapper.ColumnName));
                sBuilderFiled.Append(",");

                sBuilderValue.Append(dataContext.DatabaseContext.DbTranslator.QuoteParameter(propertyMapper.ColumnName));
                sBuilderValue.Append(",");
            }

            foreach (var propertyMapper in this.EntityInfo.OtherProperties)
            {
                sBuilderFiled.Append(dataContext.DatabaseContext.DbTranslator.Quote(propertyMapper.ColumnName));
                sBuilderFiled.Append(",");

                sBuilderValue.Append(dataContext.DatabaseContext.DbTranslator.QuoteParameter(propertyMapper.ColumnName));
                sBuilderValue.Append(",");
            }

            return string.Format("INSERT INTO {0} ({1}) VALUES ({2});\n",
                dataContext.DatabaseContext.DbTranslator.Quote(this.EntityInfo.TableName),
                sBuilderFiled.ToString().TrimEnd(','),
                sBuilderValue.ToString().TrimEnd(','));
        }
        /// <summary>
        /// 生成插入的SQL声明
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual SqlStatement buildInsertSqlStatement(IDataProvider dataContext, IEntity<TPrimaryKey> entity)
        {
            //插入的SQL语句
            string sql = buildInsertSQL(dataContext);
            var dpc = new DataParameterCollection();

            foreach (var propertyMapper in this.EntityInfo.PKeyProperties)
            {
                dpc.Add(this.getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
            }

            foreach (var propertyMapper in this.EntityInfo.OtherProperties)
            {
                dpc.Add(this.getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
            }

            return new SqlStatement(System.Data.CommandType.Text, sql, dpc);
        }
        /// <summary>
        /// 生成Update SQL语句
        /// </summary>
        /// <returns></returns>
        protected virtual string buildUpdateSQL(IDataProvider dataContext)
        {
            if (this.EntityInfo.PKeyProperties.Count == 0)
                throw new Exception("当前实体中未设置主键");
            if (this.EntityInfo.OtherProperties.Count == 0)
                throw new Exception("当前实体中除了主键以外无其他字段");

            //	UPDATE [Test] SET [name] = @name WHERE id=@id 
            StringBuilder sBuilderSet = new StringBuilder();
            StringBuilder sBuilderWhere = new StringBuilder();
            foreach (var item in this.EntityInfo.OtherProperties)
            {
                sBuilderSet.Append(dataContext.DatabaseContext.DbTranslator.Quote(item.ColumnName));
                sBuilderSet.Append(" = ");
                sBuilderSet.Append(dataContext.DatabaseContext.DbTranslator.QuoteParameter(item.ColumnName));
                sBuilderSet.Append(",");
            }

            for (int i = 0; i < this.EntityInfo.PKeyProperties.Count; i++)
            {
                var item = this.EntityInfo.PKeyProperties[i];
                sBuilderWhere.Append(dataContext.DatabaseContext.DbTranslator.Quote(item.ColumnName));
                sBuilderWhere.Append(" = ");
                sBuilderWhere.Append(dataContext.DatabaseContext.DbTranslator.QuoteParameter(item.ColumnName));
                if (i < this.EntityInfo.PKeyProperties.Count - 1)
                    sBuilderWhere.Append(" AND ");
            }

            return string.Format("UPDATE {0} SET {1} WHERE {2};\n",
                dataContext.DatabaseContext.DbTranslator.Quote(this.EntityInfo.TableName), 
                sBuilderSet.ToString().TrimEnd(','), 
                sBuilderWhere.ToString());
        }
        /// <summary>
        /// 生成更新的SQL声明
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual SqlStatement buildUpdateSqlStatement(IDataProvider dataContext, IEntity<TPrimaryKey> entity)
        {
            //插入的SQL语句
            string sql = buildUpdateSQL(dataContext);
            var dpc = new DataParameterCollection();

            foreach (var propertyMapper in this.EntityInfo.PKeyProperties)
            {
                dpc.Add(this.getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
            }

            foreach (var propertyMapper in this.EntityInfo.OtherProperties)
            {
                dpc.Add(this.getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
            }

            return new SqlStatement(System.Data.CommandType.Text, sql, dpc);
        }

        protected virtual DataParameter getDataParameter(IPropertyMapper propertyMapper, object value)
        {
            return getDataParameter(propertyMapper, value, System.Data.ParameterDirection.Input);
        }
        protected virtual DataParameter getDataParameter(IPropertyMapper propertyMapper, object value, System.Data.ParameterDirection pd)
        {
            return new DataParameter(propertyMapper.ColumnName, value, propertyMapper.PropertyInfo.PropertyType, propertyMapper.DbType, propertyMapper.Size, pd);
        }




        #endregion
    }
}
