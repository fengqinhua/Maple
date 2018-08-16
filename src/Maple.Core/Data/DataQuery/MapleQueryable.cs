using Maple.Core.Data.Conditions;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Maple.Core.Data.DataQuery
{
    public class MapleQueryable<TEntity, TPrimaryKey> : IMapperQuery<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {

        public double? Average<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public long LongCount()
        {
            throw new NotImplementedException();
        }

        public double? Max<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public DateTime? MaxDate<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public double? Min<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public DateTime? MinDate<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderBy(SortBy sortBy)
        {
            throw new NotImplementedException();
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderBy<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public IMapleSelectable<TEntity, TPrimaryKey> Range(long startIndex, long endIndex)
        {
            throw new NotImplementedException();
        }

        public IMapleSelectable<TEntity, TPrimaryKey> Range(Range range)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> Select()
        {
            throw new NotImplementedException();
        }

        public double? Sum<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> ThenBy<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public IMappeAfterOrderBy<TEntity, TPrimaryKey> ThenByDescending<TKey>(Expression<Func<TEntity, TKey>> expr)
        {
            throw new NotImplementedException();
        }

        public IMapleSelectable<TEntity, TPrimaryKey> Top(long top)
        {
            throw new NotImplementedException();
        }

        public IMappeAfterQuery<TEntity, TPrimaryKey> Where(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
