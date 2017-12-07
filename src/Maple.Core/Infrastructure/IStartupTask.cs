using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Infrastructure
{
    /// <summary>
    /// 接口：在Startup启动是需要执行的任务
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        void Execute();
        /// <summary>
        /// 执行任务的顺序
        /// </summary>
        int Order { get; }
    }
}
