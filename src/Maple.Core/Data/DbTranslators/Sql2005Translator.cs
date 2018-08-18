using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    public class Sql2005Translator : Sql2000Translator
    {
        public override DataSouceType DataSouceType { get { return DataSouceType.Sql2005; } }
    }
}
