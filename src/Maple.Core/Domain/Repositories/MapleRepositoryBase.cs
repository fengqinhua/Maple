using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Maple.Core.Domain.Repositories
{
    public abstract class MapleRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>, IAggregateRoot
    {
        public abstract void Delete(TEntity entity);
        public abstract void Delete(TPrimaryKey id);
        public abstract void Delete(Expression<Func<TEntity, bool>> predicate);
        public abstract IEnumerable<TEntity> GetAll();
        public abstract TEntity Insert(TEntity entity);
        public abstract TEntity InsertOrUpdate(TEntity entity);
        public abstract TEntity Single(TPrimaryKey id);
        public abstract TEntity Update(TEntity entity);
        public abstract TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);
    }
}
