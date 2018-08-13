using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    public class SqliteTranslator : DbTranslatorBase
    {
        public override string ProviderInvariantName { get { return "System.Data.SQLite"; } }

        public override char Connector => '@';

        //public override string OpenQuote => "[";

        //public override string CloseQuote => "]";

    }
}
