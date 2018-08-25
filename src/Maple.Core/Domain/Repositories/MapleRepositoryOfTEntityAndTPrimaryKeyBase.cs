using Maple.Core.Data;
using Maple.Core.Data.Conditions;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataQuery;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Domain.Entities;
using Maple.Core.Domain.Uow;
using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic; 
using System.Linq.Expressions; 
using System.Text;

namespace Maple.Core.Domain.Repositories
{
    public  class MapleRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>, IAggregateRoot
    { 
        protected IDataProviderFactory _dataProviderFactory = null;
        public IEntityMapper EntityInfo { get; protected set; }
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        public MapleRepositoryBase(IDataProviderFactory dataProviderFactory, IUnitOfWorkManager unitOfWorkManager)
        {
            this._dataProviderFactory = dataProviderFactory;
            this.EntityInfo = EntityMapperFactory.Instance.GetEntityMapper(typeof(TEntity));
            this.UnitOfWorkManager = unitOfWorkManager;
        }

        #region IRepository<TEntity, TPrimaryKey>

        public virtual bool Insert(TEntity entity)
        {
            if (entity == null)
                return false;

            int result = this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = this._dataProviderFactory.GetDataProvider(this.getDatasettingName());
                SqlStatement sqlStatement = DbSqlFactories.BuildInsertSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, entity);
                return dataProvider.ExecuteNonQuery(sqlStatement);
            });

            return result > 0;
        }
        public virtual bool Update(TEntity entity)
        {
            if (entity == null)
                return false;

            int result = this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = this._dataProviderFactory.GetDataProvider(this.getDatasettingName());
                SqlStatement sqlStatement = DbSqlFactories.BuildUpdateSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, entity);
                return dataProvider.ExecuteNonQuery(sqlStatement) ;
            });

            return result > 0;
        }
        public virtual bool InsertOrUpdate(TEntity entity)
        {
            if (entity == null)
                return false;
            var expr = CreateEqualityExpressionForId(entity.Id);
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                if (this.Count(expr) > 0)
                    return Update(entity);
                else
                    return Insert(entity);
            });
        }
        public virtual bool Delete(TEntity entity)
        {
            if (entity == null)
                return false;

            int result = this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = this._dataProviderFactory.GetDataProvider(this.getDatasettingName());
                SqlStatement sqlStatement = DbSqlFactories.BuildDeleteSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, entity);
                return dataProvider.ExecuteNonQuery(sqlStatement);
            });

            return result > 0;
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
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = this._dataProviderFactory.GetDataProvider(this.getDatasettingName());
                SqlStatement sqlStatement = DbSqlFactories.BuildDeleteSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, predicate);
                return dataProvider.ExecuteNonQuery(sqlStatement);
            });
        }

        public virtual long Count()
        {
            return Count(null);
        }
        public virtual long Count(Expression<Func<TEntity, bool>> predicate)
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = this._dataProviderFactory.GetDataProvider(this.getDatasettingName());
                SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.EntityInfo, predicate, "", FieldFunction.Count);
                object obj = dataProvider.ExecuteScalar(sqlStatement);
                if (obj == null) { return 0; }
                return Convert.ToInt64(obj);
            });
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
             return new MapleUnitOfWorkQueryable<TEntity, TPrimaryKey>(this._dataProviderFactory,this.UnitOfWorkManager, this.EntityInfo, getDatasettingName());
        }

        #endregion

        #region 私有函数

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
