using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;

namespace Maple.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal static class PropertyTypeToDbTypeTranslator
    {
        public static DbType Translation(Type t)
        {
            if (t.IsEnum)
                return DbType.Int32;
            else if (t == typeof(string))
                return DbType.String;
            else if (t == typeof(byte[]))
                return DbType.Binary;
            else if (t == typeof(bool) || t == typeof(Nullable<bool>))
                return DbType.Boolean;
            else if (t == typeof(byte) || t == typeof(Nullable<byte>))
                return DbType.Byte;
            else if (t == typeof(DateTime) || t == typeof(Nullable<DateTime>))
                return DbType.DateTime;
            else if (t == typeof(short) || t == typeof(Nullable<short>))
                return DbType.Int16;
            else if (t == typeof(decimal) || t == typeof(Nullable<decimal>))
                return DbType.Decimal;
            else if (t == typeof(double) || t == typeof(Nullable<double>))
                return DbType.Double;
            else if (t == typeof(float) || t == typeof(Nullable<float>))
                return DbType.Single;
            else if (t == typeof(int) || t == typeof(Nullable<int>))
                return DbType.Int32;
            else if (t == typeof(long) || t == typeof(Nullable<long>))
                return DbType.Int64;
            else if (t == typeof(sbyte) || t == typeof(Nullable<sbyte>))
                return DbType.SByte;
            else if (t == typeof(uint) || t == typeof(Nullable<uint>))
                return DbType.UInt32;
            else if (t == typeof(ulong) || t == typeof(Nullable<ulong>))
                return DbType.UInt64;
            else if (t == typeof(ushort) || t == typeof(Nullable<ushort>))
                return DbType.UInt16;
            else if (t == typeof(Guid) || t == typeof(Nullable<Guid>))
                return DbType.Guid;
            else
                throw new ArgumentOutOfRangeException(t.ToString());
        }
    }
}
