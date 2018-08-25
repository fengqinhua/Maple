using Maple.Core.Data.Conditions;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbMappers;
using Maple.Core.Domain.Entities;
using Maple.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Maple.Core.Data.DataQuery
{
    public class MapleQueryable<TEntity, TPrimaryKey> : IMapleQueryable<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        private Range range = null;
        private Expression whereExpr = null;
        private Dictionary<string, FieldSearchOrder> order = null;

        protected IExpressionParser expressionParser = null;
        protected IEntityMapper entityInfo = null;
        protected IDataProviderFactory dataProviderFactory = null;
        protected string datasetingName = "";

        public MapleQueryable(IDataProviderFactory dataProviderFactory, IEntityMapper entityInfo = null, string datasetingName = "")
        {
            Check.NotNull(dataProviderFactory, nameof(dataProviderFactory));

            this.dataProviderFactory = dataProviderFactory;
            if (entityInfo == null)
                this.entityInfo = EntityMapperFactory.Instance.GetEntityMapper(typeof(TEntity));
            else
                this.entityInfo = entityInfo;

            this.expressionParser = new ExpressionParser(this.entityInfo);
            if (string.IsNullOrEmpty(datasetingName))
                this.datasetingName = DataSetting.DefalutDataSettingName;
            else
                this.datasetingName = datasetingName;
        }

        //internal MapleQueryable(IDataProviderFactory<IDataProvider> dataProviderFactory, IEntityMapper entityInfo, string datasetingName = "")
        //{
        //    Check.NotNull(dataProviderFactory, nameof(dataProviderFactory));
        //    Check.NotNull(entityInfo, nameof(entityInfo));

        //    this.dataProviderFactory = dataProviderFactory;
        //    this.entityInfo = entityInfo;
        //    this.expressionParser = new ExpressionParser(entityInfo);

        //    if (string.IsNullOrEmpty(datasetingName))
        //        this.datasetingName = DataSetting.DefalutDataSettingName;
        //    else
        //        this.datasetingName = datasetingName;
        //}

        #region IMapleQueryable

        public virtual IMappeAfterQuery<TEntity, TPrimaryKey> Where(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate != null)
                this.whereExpr = predicate;
            return this;
        }

        #endregion

        #region IMappeAfterQuery

        public virtual IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            this.AddOrderBy<TKey>(expr, FieldSearchOrder.Ascending);
            return this;
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            this.AddOrderBy<TKey>(expr, FieldSearchOrder.Descending);
            return this;
        }

        #endregion

        #region IMappeAfterOrderBy

        public virtual IMapleSelectable<TEntity, TPrimaryKey> Top(long top)
        {
            return Range(0, top);
        }

        public IMapleSelectable<TEntity, TPrimaryKey> Range(long startIndex, long endIndex)
        {
            return Range(new Range(startIndex + 1, endIndex));
        }

        public IMapleSelectable<TEntity, TPrimaryKey> Range(Range range)
        {
            if (range != null)
                this.range = range;
            return this;
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> ThenBy<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            this.AddOrderBy<TKey>(expr, FieldSearchOrder.Ascending);
            return this;
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            this.AddOrderBy<TKey>(expr, FieldSearchOrder.Descending);
            return this;
        }

        #endregion

        #region IMapleSelectable

        public virtual double? Average<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedAverage(expr, dataProvider);
            }
        }
        public virtual int Count()
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedCount(dataProvider);
            }

        }
        public virtual long LongCount()
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedLongCount(dataProvider);
            }
        }
        public virtual double? Max<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedMax(expr, dataProvider);
            }
        }
        public virtual DateTime? MaxDate<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedMaxDate(expr, dataProvider);
            }
        }
        public virtual double? Min<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedMin(expr, dataProvider);
            }
        }
        public virtual DateTime? MinDate<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedMinDate(expr, dataProvider);
            }
        }
        public virtual double? Sum<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedSum(expr, dataProvider);
            }
        }
        public virtual TEntity FirstOrDefault()
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedFirstOrDefault(dataProvider);
            }
        }
        public virtual IList<TEntity> Select()
        {
            using (IDataProvider dataProvider = getDataProvider())
            {
                return protectedSelect(dataProvider);
            }
        } 

        #endregion

        protected virtual void AddOrderBy<TKey>(Expression<Func<TEntity, TKey>> expr, FieldSearchOrder searchOrder)
        {
            string columnName = GetColumnName<TKey>(expr);
            if (this.order == null)
                this.order = new Dictionary<string, FieldSearchOrder>();
            this.order.Add(columnName, searchOrder);
        }
        protected virtual string GetColumnName<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            IPropertyMapper propertyMapper = expressionParser.GetPropertyMapper(expr);
            if (propertyMapper == null)
                throw new MapleException("Unable to get property information.");

            return propertyMapper.ColumnName;
        }

        protected double? protectedAverage<TKey>(Expression<Func<TEntity, TKey>> expr, IDataProvider dataProvider)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Average);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }
        protected int protectedCount(IDataProvider dataProvider)
        {
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.entityInfo, whereExpr, "", FieldFunction.Count);
            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return 0; }
            return Convert.ToInt32(obj);
        }
        protected long protectedLongCount(IDataProvider dataProvider)
        {
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator,
this.entityInfo, whereExpr, "", FieldFunction.Count);
            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return 0; }
            return Convert.ToInt64(obj);
        }
        protected double? protectedMax<TKey>(Expression<Func<TEntity, TKey>> expr, IDataProvider dataProvider)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Max);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }
        protected DateTime? protectedMaxDate<TKey>(Expression<Func<TEntity, TKey>> expr, IDataProvider dataProvider)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Max);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDateTime(obj);
        }
        protected double? protectedMin<TKey>(Expression<Func<TEntity, TKey>> expr, IDataProvider dataProvider)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Min);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }
        protected DateTime? protectedMinDate<TKey>(Expression<Func<TEntity, TKey>> expr, IDataProvider dataProvider)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Min);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDateTime(obj);
        }
        protected double? protectedSum<TKey>(Expression<Func<TEntity, TKey>> expr, IDataProvider dataProvider)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Sum);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }
        protected TEntity protectedFirstOrDefault(IDataProvider dataProvider)
        {
            TEntity obj = null;
            SqlStatement sqlStatement = DbSqlFactories.BuildSelectSqlStatement(dataProvider.DatabaseContext.DbTranslator, this.entityInfo, whereExpr, order);
            dataProvider.ExecuteReader(sqlStatement, delegate (IDataReader dr)
            {
                while (dr.Read())
                {
                    DataReaderDeserializer deserializer = entityInfo.GetDataReaderDeserializer(dr);
                    obj = (TEntity)deserializer(dr);
                    break;
                }
            });
            return obj;
        }
        protected IList<TEntity> protectedSelect(IDataProvider dataProvider)
        {
            var ret = new System.ComponentModel.BindingList<TEntity>();
            SqlStatement sqlStatement = DbSqlFactories.BuildSelectSqlStatement(dataProvider.DatabaseContext.DbTranslator,
                            this.entityInfo, whereExpr, range, order);
            dataProvider.ExecuteReader(sqlStatement, delegate (IDataReader dr)
            {
                while (dr.Read())
                {
                    DataReaderDeserializer deserializer = entityInfo.GetDataReaderDeserializer(dr);
                    do
                    {
                        ret.Add((TEntity)deserializer(dr));
                    }
                    while (dr.Read());
                }
            });
            return ret;
        }

        /// <summary>
        /// 获取缺省的数据库连接驱动
        /// </summary>
        /// <returns></returns>
        protected virtual IDataProvider getDataProvider()
        {
            return this.dataProviderFactory.GetDataProvider(this.datasetingName);
            //if (this._dataProvider == null)
            //    this._dataProvider = _dataProviderFactory.CreateProvider(DataSetting.DefalutDataSettingName);
            //return this._dataProvider;
        }
    }
}
