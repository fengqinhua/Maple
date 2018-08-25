using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataQuery;
using Maple.Core.Data.DbMappers;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
   public class MapleUnitOfWorkQueryable<TEntity, TPrimaryKey> : MapleQueryable<TEntity, TPrimaryKey>, IMapleQueryable<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        public MapleUnitOfWorkQueryable(IDataProviderFactory dataProviderFactory, IUnitOfWorkManager unitOfWorkManager, IEntityMapper entityInfo = null, string datasetingName = "")
            :base(dataProviderFactory, entityInfo, datasetingName)
        {
            Check.NotNull(unitOfWorkManager, nameof(unitOfWorkManager));
            this.UnitOfWorkManager = unitOfWorkManager;
        }

        #region 重写数据操作部分，嵌入工作单元

        public override double? Average<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr)
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedAverage(expr, dataProvider);

            }, this.getDefaultUnitOfWorkOptions());
        }
        public override int Count()
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedCount(dataProvider);

            }, this.getDefaultUnitOfWorkOptions());
        }
        public override long LongCount()
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedLongCount(dataProvider);

            }, this.getDefaultUnitOfWorkOptions());
        }
        public override double? Max<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr)
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedMax(expr, dataProvider);

            }, this.getDefaultUnitOfWorkOptions());
        }
        public override DateTime? MaxDate<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr)
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedMaxDate(expr, dataProvider);

            }, this.getDefaultUnitOfWorkOptions());
        }
        public override double? Min<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr)
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedMin(expr, dataProvider);

            }, this.getDefaultUnitOfWorkOptions());
        }
        public override DateTime? MinDate<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr)
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedMinDate(expr, dataProvider);

            }, this.getDefaultUnitOfWorkOptions());
        }
        public override double? Sum<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr)
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedSum(expr, dataProvider);
            }, this.getDefaultUnitOfWorkOptions());
        }
        public override TEntity FirstOrDefault()
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedFirstOrDefault(dataProvider);
            }, this.getDefaultUnitOfWorkOptions());
        }
        public override IList<TEntity> Select()
        {
            return this.UnitOfWorkManager.ExecuteWithUOW(() =>
            {
                IDataProvider dataProvider = base.getDataProvider();
                return base.protectedSelect(dataProvider);
            },this.getDefaultUnitOfWorkOptions());
        }

        #endregion


        protected virtual UnitOfWorkOptions getDefaultUnitOfWorkOptions()
        {
            return new UnitOfWorkOptions() { IsTransactional = false };
        }

    }
}
