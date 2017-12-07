using System;

namespace Maple.Core.Domain.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需存储实体的删除时间
    /// </summary>
    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        /// 实体的删除时间.
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }
}