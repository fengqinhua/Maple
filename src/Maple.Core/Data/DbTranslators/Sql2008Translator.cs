using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    public class Sql2008Translator : Sql2005Translator
    {
        public override DataSouceType DataSouceType { get { return DataSouceType.Sql2008; } }
    }
}
