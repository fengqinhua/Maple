using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 工作单元中仓储接口CRUD操作
    /// 需要使用工作单元的仓储，需要实现本接口
    /// </summary>
    public interface IUnitOfWorkRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        void PersistCreationOf(IEntity<TPrimaryKey> entity);
        void PersistUpdateOf(IEntity<TPrimaryKey> entity);
        void PersistDeletionOf(IEntity<TPrimaryKey> entity);
    }
}
