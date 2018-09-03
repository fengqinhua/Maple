using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    public class SqliteTranslator : DbTranslatorBase
    {
        public override DataSouceType DataSouceType { get { return DataSouceType.Sqlite; } }
        public override string ProviderInvariantName { get { return System.Data.Common.DbDriveFactories.SQLiteProviderInvariantName; } }

        public override char Connector => '@';

        //public override string OpenQuote => "[";

        //public override string CloseQuote => "]";

    }
}
