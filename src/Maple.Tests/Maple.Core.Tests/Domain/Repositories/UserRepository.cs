using Maple.Core.Data.DataProviders;
using Maple.Core.Domain.Repositories;
using Maple.Core.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Domain
{
    public class UserRepository : MapleRepositoryBase<User>, IRepository<User>
    {
        public UserRepository(IDataProviderFactory dataProviderFactory, IUnitOfWorkManager unitOfWorkManager) : base(dataProviderFactory, unitOfWorkManager)
        {
        }
    }
}
