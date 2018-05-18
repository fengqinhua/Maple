using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Transactions;

namespace Maple.Core.Domain.Repositories
{
    /// <summary>
    /// 表示实现该接口的类型是仓储上下文
    /// </summary>
    public class MapleRepositoryContext : IRepositoryContext
    {
        private readonly object syncObj = new object();
        private readonly ThreadLocal<Dictionary<IAggregateRoot, IUnitOfWorkRepository>> addedEntities
            = new ThreadLocal<Dictionary<IAggregateRoot, IUnitOfWorkRepository>>(() => new Dictionary<IAggregateRoot, IUnitOfWorkRepository>());
        private readonly ThreadLocal<Dictionary<IAggregateRoot, IUnitOfWorkRepository>> changedEntities
            = new ThreadLocal<Dictionary<IAggregateRoot, IUnitOfWorkRepository>>(() => new Dictionary<IAggregateRoot, IUnitOfWorkRepository>());
        private readonly ThreadLocal<Dictionary<IAggregateRoot, IUnitOfWorkRepository>> deletedEntities
            = new ThreadLocal<Dictionary<IAggregateRoot, IUnitOfWorkRepository>>(() => new Dictionary<IAggregateRoot, IUnitOfWorkRepository>());

        public MapleRepositoryContext() { }

        public virtual void Commit()
        {
            lock (syncObj)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var addDic = this.addedEntities.Value;
                    foreach (IAggregateRoot entity in addDic.Keys)
                    {
                        addDic[entity].PersistCreationOf(entity);
                    }

                    var changeDic = this.changedEntities.Value;
                    foreach (IAggregateRoot entity in changeDic.Keys)
                    {
                        changeDic[entity].PersistUpdateOf(entity);
                    }

                    var delDic = this.deletedEntities.Value;
                    foreach (IAggregateRoot entity in delDic.Keys)
                    {
                        delDic[entity].PersistDeletionOf(entity);
                    }
                    scope.Complete();

                    this.ClearRegistrations();
                }
            }
        }

        public virtual void Dispose()
        {
            addedEntities.Dispose();
            changedEntities.Dispose();
            deletedEntities.Dispose();
        }

        public virtual void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            //if (obj.ID.Equals(Guid.Empty))
            //    throw new ArgumentException("The ID of the object is empty.", "obj");

            if (this.deletedEntities.Value.ContainsKey(entity))
                throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");

            if (!this.changedEntities.Value.ContainsKey(entity) && !this.addedEntities.Value.ContainsKey(entity))
                this.changedEntities.Value.Add(entity, unitofWorkRepository);
        }

        public virtual void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            //if (obj.ID.Equals(Guid.Empty))
            //    throw new ArgumentException("The ID of the object is empty.", "obj");

            if (this.changedEntities.Value.ContainsKey(entity))
                throw new InvalidOperationException("The object cannot be registered as a new object since it was marked as modified.");

            if (!this.addedEntities.Value.ContainsKey(entity))
                this.addedEntities.Value.Add(entity, unitofWorkRepository);
        }

        public virtual void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository)
        {
            //if (obj.ID.Equals(Guid.Empty))
            //    throw new ArgumentException("The ID of the object is empty.", "obj");

            if (this.addedEntities.Value.ContainsKey(entity))
            {
                if (this.addedEntities.Value.Remove(entity))
                    return;
            }

            if (this.changedEntities.Value.ContainsKey(entity))
            {
                if (this.changedEntities.Value.Remove(entity))
                    return;
            }

            if (!this.deletedEntities.Value.ContainsKey(entity))
            {
                this.deletedEntities.Value.Add(entity, unitofWorkRepository);
            }
        }

        /// <summary>
        /// 清除UOW中记录的IAggregateRoot变化信息
        /// </summary>
        protected void ClearRegistrations()
        {
            this.addedEntities.Value.Clear();
            this.changedEntities.Value.Clear();
            this.deletedEntities.Value.Clear();
        }
    }
}
