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
        private DbProviderFactory dbProviderFactory = null;

        public abstract string ProviderInvariantName { get; }
        public abstract char Connector { get; }
        public virtual string OpenQuote { get { return string.Empty; } }
        public virtual string CloseQuote { get { return string.Empty; } }

        public virtual DbProviderFactory GetDbProviderFactory()
        {
            if(dbProviderFactory == null)
                dbProviderFactory = DbProviderFactories.GetFactory(this.ProviderInvariantName);
            return dbProviderFactory;
        }
    }
}
