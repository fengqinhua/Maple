using System;

namespace Maple.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 如果一个实体实现该接口，那么需存储实体的创建时间
    /// </summary>
    public interface IHasCreationTime
    {
        /// <summary>
        /// 实体的创建时间
        /// </summary>
        DateTime CreationTime { get; set; }
    }
}