
using Maple.Core.Domain.Auditing;
using Maple.Core.Extensions;

namespace Maple.Core.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// 校验实体是否被软删除或者为NULL
        /// </summary>
        public static bool IsNullOrDeleted(this ISoftDelete entity)
        {
            return entity == null || entity.IsDeleted;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UnDelete(this ISoftDelete entity)
        {
            entity.IsDeleted = false;
            if (entity is IDeletionAudited)
            {
                var deletionAuditedEntity = entity.As<IDeletionAudited>();
                deletionAuditedEntity.DeletionTime = null;
                deletionAuditedEntity.DeleterUserId = null;
            }
        }
    }
}