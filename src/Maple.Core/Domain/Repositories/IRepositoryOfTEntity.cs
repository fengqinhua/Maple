﻿using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Repositories
{
    /// <summary>
    /// 仓储接口
    /// <para>协调领域和数据映射层，利用类似于集合的接口来访问领域对象</para>
    /// </summary>
    public interface IRepository<TEntity> : IRepository<TEntity, long> where TEntity : class, IEntity<long>, IAggregateRoot { }
}
