using System.Reflection;

namespace Maple.Core.Data.DbMappers
{
    /// <summary>
    /// 实体类IEntity属性与数据库字段的对应信息
    /// </summary>
    public interface IPropertyMapper
    {
        /// <summary>
        /// 获取熟悉信息
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
    }
}
