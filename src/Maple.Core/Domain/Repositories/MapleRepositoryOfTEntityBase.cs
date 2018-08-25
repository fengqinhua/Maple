using Maple.Core.Data.DataProviders;
using Maple.Core.Domain.Entities;
using Maple.Core.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Repositories
{
    public class MapleRepositoryBase<TEntity> : MapleRepositoryBase<TEntity, long>, IRepository<TEntity>
        where TEntity : class, IEntity<long>, IAggregateRoot
    {
        public MapleRepositoryBase(IDataProviderFactory dataProviderFactory, IUnitOfWorkManager unitOfWorkManager) : base(dataProviderFactory, unitOfWorkManager)
        {
        }
    }
}
