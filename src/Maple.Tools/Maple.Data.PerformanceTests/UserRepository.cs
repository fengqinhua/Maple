using Maple.Core.Data.DataProviders;
using Maple.Core.Domain.Repositories;
using Maple.Data.PerformanceTests.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Data.PerformanceTests
{
    public class UserRepository : MapleRepositoryBase<User, long>
    {
        private string database = "";
        public UserRepository(IDataProviderFactory dataProviderFactory, string db) : base(dataProviderFactory)
        {
            this.database = db;
        }

        protected override string getDatasettingName()
        {
            return this.database;
        }

    }
}
