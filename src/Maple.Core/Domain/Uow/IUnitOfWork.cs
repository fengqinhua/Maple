using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 工作单元
    /// <para>维护受业务事务影响的对象列表，并协调变化的写入和并发问题的解决</para>
    /// </summary>
    public interface IUnitOfWork
    {

        void RegisterAmended<TEntity, TPrimaryKey>(IEntity<TPrimaryKey> entity, IUnitOfWorkRepository<TEntity, TPrimaryKey> unitofWorkRepository) where TEntity : class, IEntity<TPrimaryKey>;
        void RegisterNew<TEntity, TPrimaryKey>(IEntity<TPrimaryKey> entity, IUnitOfWorkRepository<TEntity, TPrimaryKey> unitofWorkRepository) where TEntity : class, IEntity<TPrimaryKey>;
        void RegisterRemoved<TEntity, TPrimaryKey>(IEntity<TPrimaryKey> entity, IUnitOfWorkRepository<TEntity, TPrimaryKey> unitofWorkRepository) where TEntity : class, IEntity<TPrimaryKey>;

        void Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<object, Action<object>> addedEntities;
        private Dictionary<object, Action<object>> changedEntities;
        private Dictionary<object, Action<object>> deletedEntities;


        public void Commit()
        {
            throw new NotImplementedException();
        }

        void IUnitOfWork.RegisterAmended<TEntity, TPrimaryKey>(IEntity<TPrimaryKey> entity, IUnitOfWorkRepository<TEntity, TPrimaryKey> unitofWorkRepository)
        {
            if (!changedEntities.ContainsKey(entity))
            {
                changedEntities.Add(entity, (obj) =>
                {
                    IEntity<TPrimaryKey> temp = obj.As<IEntity<TPrimaryKey>>();
                    unitofWorkRepository.PersistUpdateOf(temp);
                });
            }
        }

        void IUnitOfWork.RegisterNew<TEntity, TPrimaryKey>(IEntity<TPrimaryKey> entity, IUnitOfWorkRepository<TEntity, TPrimaryKey> unitofWorkRepository)
        {
            throw new NotImplementedException();
        }

        void IUnitOfWork.RegisterRemoved<TEntity, TPrimaryKey>(IEntity<TPrimaryKey> entity, IUnitOfWorkRepository<TEntity, TPrimaryKey> unitofWorkRepository)
        {
            throw new NotImplementedException();
        }
    }
}
