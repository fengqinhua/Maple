using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 工作单元管理器。用于创建一个新的工作单元或获取当前处于激活状态的工作单元
    /// </summary>
    public interface IUnitOfWorkManager
    {
        /// <summary>
        ///  获取当前活动的工作单元（如果不存在，则返回null）
        /// </summary>
        IActiveUnitOfWork Current { get; }

        /// <summary>
        /// 开始了一个新的工作单元
        /// </summary>
        /// <returns></returns>
        IUnitOfWorkCompleteHandle Begin();
        /// <summary>
        /// 开始了一个新的工作单元
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options);
    }
}
