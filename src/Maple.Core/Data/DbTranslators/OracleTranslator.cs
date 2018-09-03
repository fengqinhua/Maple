using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    public class OracleTranslator : DbTranslatorBase
    {
        public override DataSouceType DataSouceType { get { return DataSouceType.Oracle; } }
        public override string ProviderInvariantName { get { return System.Data.Common.DbDriveFactories.OracleProviderInvariantName; } }

        public override char Connector => ':';

        //public override string OpenQuote => "[";

        //public override string CloseQuote => "]";
    }
}
