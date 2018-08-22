using Maple.Core.Data;
using Maple.Core.Data.Conditions;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataQuery;
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
    public abstract class MapleRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>, IUnitOfWorkRepository where TEntity : class, IEntity<TPrimaryKey>, IAggregateRoot
    {
        protected IDataProviderFactory _dataProviderFactory = null;
        public IEntityMapper EntityInfo { get; protected set; }

        public MapleRepositoryBase(IDataProviderFactory dataProviderFactory)
        {
            this._dataProviderFactory = dataProviderFactory;
            this.EntityInfo = EntityMapperFactory.Instance.GetEntityMapper(typeof(TEntity));
        }

        #region IRepository<TEntity, TPrimaryKey>

        public virtual bool Insert(TEntity entity)
        {
            if (entity == null)
                return false;

            using (IDataProvider dataProvider = getDataProvider())
            {
                SqlStatement sqlStatement = DbSqlFactories.BuildInsertSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, entity);
                return dataProvider.ExecuteNonQuery(sqlStatement) > 0;
            }
        }

        public virtual bool Update(TEntity entity)
        {
            if (entity == null)
                return false;

            using (IDataProvider dataProvider = getDataProvider())
            {
                SqlStatement sqlStatement = DbSqlFactories.BuildUpdateSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, entity);
                return dataProvider.ExecuteNonQuery(sqlStatement) > 0;
            }
        }
        public virtual bool InsertOrUpdate(TEntity entity)
        {
            if (entity == null)
                return false;

            var expr = CreateEqualityExpressionForId(entity.Id);
            if (this.Count(expr) > 0)
                return Update(entity);
            else
                return Insert(entity);
        }
        public virtual bool Delete(TEntity entity)
        {
            if (entity == null)
                return false;

            using (IDataProvider dataProvider = getDataProvider())
            {
                SqlStatement sqlStatement = DbSqlFactories.BuildDeleteSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, entity);
                return dataProvider.ExecuteNonQuery(sqlStatement) > 0;
            }
        }
        public virtual bool Delete(TPrimaryKey id)
        {
            var expr = CreateEqualityExpressionForId(id);
            return Delete(expr) == 1;
        }
        public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return 0;

            using (IDataProvider dataProvider = getDataProvider())
            {
                SqlStatement sqlStatement = DbSqlFactories.BuildDeleteSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, predicate);
                return dataProvider.ExecuteNonQuery(sqlStatement);
            }
        }

        public virtual long Count()
        {
            return Count(null);
        }
        public virtual long Count(Expression<Func<TEntity, bool>> predicate)
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, predicate, "", FieldFunction.Count);
                object obj = dataProvider.ExecuteScalar(sqlStatement);
                if (obj == null) { return 0; }
                return Convert.ToInt64(obj);
            }
        }
        public virtual TEntity Single(TPrimaryKey id)
        {
            var expr = CreateEqualityExpressionForId(id);
            return GetAll().Where(expr).FirstOrDefault();
        }
        public virtual IEnumerable<TEntity> GetAllList()
        {
            return GetAll().Select();
        }
        public virtual IMapleQueryable<TEntity, TPrimaryKey> GetAll()
        {
            return new MapleQueryable<TEntity, TPrimaryKey>(this._dataProviderFactory, this.EntityInfo,);
        }

        #endregion

        #region IUnitOfWorkRepository

        public void PersistCreationOf(IAggregateRoot entity)
        {
            TEntity myEntity = entity as TEntity;
            if (myEntity == null)
                throw new MapleException("entity must inherit IEntity<TPrimaryKey>");
            this.Insert(myEntity);
        }

        public void PersistUpdateOf(IAggregateRoot entity)
        {
            TEntity myEntity = entity as TEntity;
            if (myEntity == null)
                throw new MapleException("entity must inherit IEntity<TPrimaryKey>");
            this.InsertOrUpdate(myEntity);
        }

        public void PersistDeletionOf(IAggregateRoot entity)
        {
            TEntity myEntity = entity as TEntity;
            if (myEntity == null)
                throw new MapleException("entity must inherit IEntity<TPrimaryKey>");
            this.Delete(myEntity);
        }

        #endregion

        #region 私有函数

        /// <summary>
        /// 获取缺省的数据库连接驱动
        /// </summary>
        /// <returns></returns>
        protected virtual IDataProvider getDataProvider()
        {
            return _dataProviderFactory.CreateProvider(this.getDatasettingName());
            //if (this._dataProvider == null)
            //    this._dataProvider = _dataProviderFactory.CreateProvider(DataSetting.DefalutDataSettingName);
            //return this._dataProvider;
        }

        protected virtual string getDatasettingName()
        {
            return DataSetting.DefalutDataSettingName;
        }

        /// <summary>
        /// 创建基于主键ID查询的Lambda Expression
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
         
        #endregion

    }
}
