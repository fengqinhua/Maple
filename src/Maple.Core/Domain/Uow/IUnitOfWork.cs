using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 工作单元
    /// <para>维护受业务事务影响的对象列表，并协调变化的写入和并发问题的解决</para>
    /// <para>使用<see cref =“IUnitOfWorkManager.Begin（）”/>开始一个新的工作单元</para>
    /// </summary>
    public interface IUnitOfWork : IActiveUnitOfWork, IUnitOfWorkCompleteHandle
    {
        /// <summary>
        /// 工作单元标识
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 如果存在，则引用外部UOW（工作单元链 ）
        /// </summary>
        IUnitOfWork Outer { get; set; }
        /// <summary>
        /// 启动工作单元
        /// </summary>
        /// <param name="options"></param>
        void Begin(UnitOfWorkOptions options);
    }
}
