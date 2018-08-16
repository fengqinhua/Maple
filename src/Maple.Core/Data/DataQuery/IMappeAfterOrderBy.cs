using Maple.Core.Data.Conditions;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataQuery
{
    public interface IMappeAfterOrderBy<TEntity, TPrimaryKey> : IMappeAfterQuery<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        IMapleSelectable<TEntity, TPrimaryKey> Top(long top);
        IMapleSelectable<TEntity, TPrimaryKey> Range(long startIndex, long endIndex);
        IMapleSelectable<TEntity, TPrimaryKey> Range(Range range);
         
        IMappeAfterOrderBy<TEntity, TPrimaryKey> ThenBy<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
        IMappeAfterOrderBy<TEntity, TPrimaryKey> ThenByDescending<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
    }
}
