using Maple.Core.Data.DataProviders;
using Maple.Core.Domain.Repositories;
using Maple.Core.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class RoleRepository : MapleRepositoryBase<Role>, IRepository<Role>
    {
        public RoleRepository(IDataProviderFactory dataProviderFactory, IUnitOfWorkManager unitOfWorkManager) : base(dataProviderFactory, unitOfWorkManager)
        {
        }
    }
}
