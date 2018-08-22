using Maple.Core.Data.DataProviders;
using Maple.Core.Domain.Repositories;
using Maple.Core.Tests.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Tests.Repositories
{
    public class UserRepository : MapleRepositoryBase<User, long>, IRepository<User>
    {
        private string database = "";
        public UserRepository(IDataProviderFactory dataProviderFactory,string db) : base(dataProviderFactory)
        {
            this.database = db;
        }

        protected override string getDatasettingName()
        {
            return this.database;
        }
    }
}
