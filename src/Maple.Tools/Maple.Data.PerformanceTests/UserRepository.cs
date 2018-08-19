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



        protected override IDataProvider getDataProvider()
        {
            if (this._dataProvider == null)
                this._dataProvider = _dataProviderFactory.CreateProvider(this.database);
            return this._dataProvider;
        }
    }
}
