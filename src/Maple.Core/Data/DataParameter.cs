using System;
using System.Data;

namespace Maple.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DataParameter
    {
        /// <summary>
        /// 键（字段名称）
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值（字段值）
        /// </summary>
        public object Value { get; set; }

 
        /// <summary>
        /// 如果该项为字符型，则一定要指定Size大小。
        /// 如果未指定大小对性能会有影响
        /// <para>具体参见：http://blog.csdn.net/hy6688_/article/details/11409857 </para>
        /// </summary>
        public int Size { get; set; }
        public DbType Type { get; set; }
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public DataParameter(string key, object value, DbType type, int size, ParameterDirection direction)
        {
            this.Key = key;
            this.Value = value;

            this.Type = type;
            this.Direction = direction;
            this.Size = size;
        }

        public override bool Equals(object obj)
        {
            var dp = (DataParameter)obj;
            bool b = (Key == dp.Key)
                && (Value == dp.Value)
                && (Type == dp.Type);
            return b;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}={1}:{2}", Key, Value == DBNull.Value ? "<NULL>" : Value, Type);
        }
    }
}
