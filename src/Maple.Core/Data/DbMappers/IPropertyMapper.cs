using System.Reflection;

namespace Maple.Core.Data.DbMappers
{
    /// <summary>
    /// 实体类IEntity属性与数据库字段的对应信息
    /// </summary>
    public interface IPropertyMapper
    {
        /// <summary>
        /// 获取属性信息
        /// </summary>
        /// <returns></returns>
        PropertyInfo PropertyInfo { get; }
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        string ColumnName { get; }
        /// <summary>
        /// 是否主键标识
        /// </summary>
        bool IsPrimaryKey { get; }
        /// <summary>
        /// 是否可空
        /// </summary>
        bool AllowsNulls { get; }
        /// <summary>
        /// 数据库字段类型
        /// </summary>
        System.Data.DbType DbType { get; }
        /// <summary>
        /// 长度
        /// </summary>
        int Size { get; }
        /// <summary>
        /// 是否为值对象中的属性
        /// </summary>
        bool IsDataObjectProperty { get; }
        /// <summary>
        /// 读取属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        object FastGetValue(object entity);
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        void FastSetValue(object entity, object value);

    }
}
