using System;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需存储实体最近一次的修改时间.
    /// </summary>
    public interface IHasModificationTime
    {
        /// <summary>
        /// 实体的最近一次的修改时间.
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }
}