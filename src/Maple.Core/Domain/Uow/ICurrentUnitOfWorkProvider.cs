using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 该接口用于获取当前的工作单元，确保一个线程共用一个工作单元
    /// </summary>
    public interface ICurrentUnitOfWorkProvider
    {
        /// <summary>
        /// 获取当前的工作单元
        /// </summary>
        IUnitOfWork Current { get; set; }
    }
}
