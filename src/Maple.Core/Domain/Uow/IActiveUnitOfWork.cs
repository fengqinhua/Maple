using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 该接口用于处理活动的工作单元
    /// </summary>
    public interface IActiveUnitOfWork
    {
        /// <summary>
        /// 工作单位提交成功的后处理事件
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// 工作单位提交失败的后处理事件
        /// </summary>
        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <summary>
        /// 工作单元注销的后处理事件
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        /// 当前工作单元是否已Disposed
        /// </summary>
        bool IsDisposed { get; }
    }
}
