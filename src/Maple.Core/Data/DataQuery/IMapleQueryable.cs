using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataQuery
{
    public interface IMapleQueryable<TEntity, TPrimaryKey> : IMappeAfterQuery<TEntity, TPrimaryKey>, IMappeAfterOrderBy<TEntity, TPrimaryKey>, IMapleSelectable<TEntity, TPrimaryKey>  where TEntity : class, IEntity<TPrimaryKey>
    {
        IMappeAfterQuery<TEntity, TPrimaryKey> Where(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
    }
}
