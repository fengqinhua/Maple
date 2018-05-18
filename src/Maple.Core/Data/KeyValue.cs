using System;

namespace Maple.Core.Data
{
    /// <summary>
    /// 键对值：存储Sql语句中的参数信息
    /// </summary>
    public class KeyValue
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
        /// 值类型
        /// </summary>
        public Type ValueType { get; set; }

        public object NullableValue
        {
            get
            {
                return Value ?? DBNull.Value;
            }
        }

        protected KeyValue() { }

        protected KeyValue(object Value) : this(null, Value) { }

        public KeyValue(string Key, object Value)
            : this(Key, Value, (Value == null) ? typeof(DBNull) : Value.GetType()) { }

        public KeyValue(string Key, object Value, Type ValueType)
        {
            this.Key = Key;
            this.Value = Value;
            this.ValueType = ValueType;
        }
    }
}
