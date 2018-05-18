using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Maple.Core.Domain
{
    /// <summary>
    /// 工作单元
    /// <para>维护受业务事务影响的对象列表，并协调变化的写入和并发问题的解决</para>
    /// </summary>
    public interface IUnitOfWork 
    {
        void Commit();
    }
}
