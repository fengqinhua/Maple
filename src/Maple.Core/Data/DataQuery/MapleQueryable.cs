using Maple.Core.Data.Conditions;
using Maple.Core.Data.DataProviders;
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
    public class MapleQueryable<TEntity, TPrimaryKey> : IMapperQuery<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        private Range range = null;
        private Expression whereExpr = null;
        private Dictionary<string, FieldSearchOrder> order = null;

        private IExpressionParser expressionParser = null;
        private IDataProvider dataProvider = null;
        private IEntityMapper entityInfo = null;

        #region IMapleQueryable

        public IMappeAfterQuery<TEntity, TPrimaryKey> Where(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate != null)
                this.whereExpr = predicate;
            return this;
        }

        #endregion

        #region IMappeAfterQuery

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expr)
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

        public IMapleSelectable<TEntity, TPrimaryKey> Top(long top)
        {
            return Range(0, top);
        }

        public IMapleSelectable<TEntity, TPrimaryKey> Range(long startIndex, long endIndex)
        {
            return Range(new Range(startIndex, endIndex));
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

        public double? Average<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Average);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }
        public int Count()
        {
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, "", FieldFunction.Count);
            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return 0; }
            return Convert.ToInt32(obj);
        }
        public long LongCount()
        {
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, "", FieldFunction.Count);
            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return 0; }
            return Convert.ToInt64(obj);
        }
        public double? Max<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Max);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }
        public DateTime? MaxDate<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Max);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDateTime(obj);
        }
        public double? Min<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Min);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }
        public DateTime? MinDate<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Min);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDateTime(obj);
        }
        public double? Sum<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            string columnName = GetColumnName<TKey>(expr);
            SqlStatement sqlStatement = DbSqlFactories.BuildFunctionSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
                this.entityInfo, whereExpr, columnName, FieldFunction.Sum);

            object obj = dataProvider.ExecuteScalar(sqlStatement);
            if (obj == null) { return null; }
            return Convert.ToDouble(obj);
        }

        public TEntity FirstOrDefault()
        {
            TEntity obj = null;
            SqlStatement sqlStatement = DbSqlFactories.BuildSelectSqlStatement(this.dataProvider.DatabaseContext.DbTranslator, this.entityInfo, whereExpr, order);
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
        public IList<TEntity> Select()
        {
            var ret = new System.ComponentModel.BindingList<TEntity>();
            SqlStatement sqlStatement = DbSqlFactories.BuildSelectSqlStatement(this.dataProvider.DatabaseContext.DbTranslator,
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

    }
}
