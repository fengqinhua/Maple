using Maple.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core
{
    /// <summary>
    /// 子站点上下文接口
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// 获取子站点上下文
        /// </summary>
        Store CurrentStore { get; }
    }
}
