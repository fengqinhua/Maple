using Maple.Core.Data.DataSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maple.Core.Data.DataProviders.Internal
{
    internal class InternalDataProvider : IDataProvider
    {
        public InternalDataProvider(IDatabaseContext databaseContext)
        {
            this.DatabaseContext = databaseContext;
        }

        public IDatabaseContext DatabaseContext { get; private set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteDataTable(SqlStatement sqlStatement)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(SqlStatement sqlStatement)
        {
            throw new NotImplementedException();
        }

        public bool ExecuteReader(SqlStatement sqlStatement, CallbackObjectHandler<IDataReader> callback)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(SqlStatement sqlStatement)
        {
            throw new NotImplementedException();
        }
    }
}
