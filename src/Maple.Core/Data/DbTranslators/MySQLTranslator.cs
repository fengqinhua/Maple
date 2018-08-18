using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    public class MySQLTranslator : DbTranslatorBase
    {
        public override DataSouceType DataSouceType { get { return DataSouceType.MySQL; } }
        public override string ProviderInvariantName { get { return "MySql.Data.MySqlClient"; } }

        public override char Connector => '@';

        //public override string OpenQuote => "[";

        //public override string CloseQuote => "]";
    }
}
