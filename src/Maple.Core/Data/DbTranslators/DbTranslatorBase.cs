using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Maple.Core.Data.DbMappers;

namespace Maple.Core.Data.DbTranslators
{
    public abstract class DbTranslatorBase : IDbTranslator
    {
        public abstract char Connector { get; }
        public virtual string OpenQuote { get { return string.Empty; } }
        public virtual string CloseQuote { get { return string.Empty; } }

        public DbProviderFactory GetDbProviderFactory()
        {
            throw new NotImplementedException();
        }
    }
}
