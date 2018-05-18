using System;
using System.Data;
using System.Collections.Specialized;

namespace Maple.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal static class DataTypeParser
    {
        private static readonly HybridDictionary Types;

        static DataTypeParser()
        {
            Types = new HybridDictionary();
            Types[typeof(string)] = DbType.String;
            Types[typeof(DateTime)] = DbType.DateTime;
            Types[typeof(bool)] = DbType.Boolean;

            Types[typeof(byte)] = DbType.Byte;
            Types[typeof(sbyte)] = DbType.SByte;
            Types[typeof(decimal)] = DbType.Decimal;
            Types[typeof(double)] = DbType.Double;
            Types[typeof(float)] = DbType.Single;

            Types[typeof(int)] = DbType.Int32;
            Types[typeof(uint)] = DbType.UInt32;
            Types[typeof(long)] = DbType.Int64;
            Types[typeof(ulong)] = DbType.UInt64;
            Types[typeof(short)] = DbType.Int16;
            Types[typeof(ushort)] = DbType.UInt16;

            Types[typeof(Guid)] = DbType.Guid;
            Types[typeof(byte[])] = DbType.Binary;
            Types[typeof(Enum)] = DbType.Int32;

            Types[typeof(DBNull)] = DbType.Single; // is that right?
        }

        public static DbType Parse(object o)
        {
            return Parse(o.GetType());
        }

        public static DbType Parse(Type t)
        {
            if (t.IsEnum)
            {
                t = typeof(Enum);
            }
            if (Types.Contains(t))
            {
                return (DbType)Types[t];
            }
            if (NullableTool.IsNullableType(t))
            {
                return NullableTool.GetDataType(t);
            }
            throw new ArgumentOutOfRangeException(t.ToString());
        }

        //public static string ParseToString(object o, Dialect.DbDialect dd)
        //{
        //    if (o == null)
        //    {
        //        return "NULL";
        //    }
        //    var ot = o.GetType();
        //    if (typeof(bool) == ot)
        //    {
        //        return Convert.ToInt32(o).ToString();
        //    }
        //    if (typeof(string) == ot)
        //    {
        //        string s = o.ToString();
        //        s = s.Replace("'", "''");
        //        return string.Format("N'{0}'", s);
        //    }
        //    if (typeof(DateTime) == ot || typeof(Date) == ot || typeof(Time) == ot)
        //    {
        //        return dd.QuoteDateTimeValue(o.ToString());
        //    }
        //    if (ot.IsEnum)
        //    {
        //        return Convert.ToInt32(o).ToString();
        //    }
        //    if (typeof(byte[]) == ot)
        //    {
        //        throw new ApplicationException("Sql without Parameter can not support blob, please using Parameter mode.");
        //    }
        //    return o.ToString();
        //}
    }

}
