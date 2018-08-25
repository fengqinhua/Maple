using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 此接口用于定义用于完成一个工作单元
    /// </summary>
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        /// <summary>
        /// 完成这个工作单元。保存所有更改并提交事务（如果存在）
        /// </summary>
        void Complete();
    }
}
