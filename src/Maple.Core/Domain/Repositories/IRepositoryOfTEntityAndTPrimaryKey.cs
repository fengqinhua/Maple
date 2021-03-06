﻿using Maple.Core.Data.DataQuery;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Maple.Core.Domain.Repositories
{
    /// <summary>
    /// 仓储接口
    /// <para>协调领域和数据映射层，利用类似于集合的接口来访问领域对象</para>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : class, IEntity<TPrimaryKey>, IAggregateRoot
    {
        bool Insert(TEntity entity);
        bool Update(TEntity entity);
        bool InsertOrUpdate(TEntity entity);

        bool Delete(TEntity entity);
        bool Delete(TPrimaryKey id);
        int Delete(Expression<Func<TEntity, bool>> predicate);

        long Count();
        long Count(Expression<Func<TEntity, bool>> predicate);

        TEntity Single(TPrimaryKey id);
        IEnumerable<TEntity> GetAllList();

        IMapleQueryable<TEntity, TPrimaryKey> GetAll();
    }

}
