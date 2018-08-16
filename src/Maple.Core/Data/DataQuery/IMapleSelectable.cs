using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataQuery
{
    public interface IMapleSelectable<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        int Count();
        long LongCount();
        double? Sum<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
        double? Average<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
        double? Max<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
        double? Min<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
        DateTime? MaxDate<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
        DateTime? MinDate<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);

        TEntity FirstOrDefault();
        IList<TEntity> Select();
    }
}
