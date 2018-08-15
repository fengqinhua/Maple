using Maple.Core.Data.DbMappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maple.Core.Data.ModelConfiguration
{
    public class PropertyConfiguration
    {
        internal PropertyMapper PropertyMapper { get; set; }

        public PropertyConfiguration() { }

        public PropertyConfiguration ToColumnName(string columnName)
        {
            if (!string.IsNullOrEmpty(columnName))
                PropertyMapper.ColumnName = columnName;

            return this;
        }

        public PropertyConfiguration ToAllowsNulls(bool allowsNulls)
        {
            PropertyMapper.AllowsNulls = allowsNulls;
            return this;
        }

        public PropertyConfiguration ToDbSize(int size)
        {
            if (size > 0)
                PropertyMapper.Size = size;

            return this;
        }

        public PropertyConfiguration ToDbType(DbType dbType)
        {
            PropertyMapper.DbType = dbType;
            return this;
        }

    }
}
