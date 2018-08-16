using Maple.Core.Data.Conditions;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataQuery
{
    public interface IMappeAfterQuery<TEntity, TPrimaryKey> : IMapleSelectable<TEntity, TPrimaryKey>  where TEntity : class, IEntity<TPrimaryKey>
    {
        IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderBy<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
        IMappeAfterOrderBy<TEntity, TPrimaryKey> OrderByDescending<TKey>(System.Linq.Expressions.Expression<Func<TEntity, TKey>> expr);
    }
}
