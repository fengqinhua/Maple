using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    public class UnitOfWorkOptions
    {
        /// <summary>
        /// 是否启用数据库事务 （如果未设置，则使用默认值:启用事务）
        /// </summary>
        public bool? IsTransactional { get; set; }
        /// <summary>
        /// 如果此工作单元是事务性的，则此选项指示事务的隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }
    }
}
