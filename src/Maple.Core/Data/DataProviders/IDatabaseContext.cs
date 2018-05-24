using Maple.Core.Data.DbTranslators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public interface IDatabaseContext : IDisposable
    {
        /// <summary>
        /// 数据库信息翻译器
        /// </summary>
        IDbTranslator DbTranslator { get; }

        /// <summary>
        /// 获取数据库连接并保证数据库连接已经打开
        /// </summary>
        void EnsureConnection();
        /// <summary>
        /// 释放数据库连接
        /// </summary>
        /// <param name="releaseConnection"></param>
        void ReleaseConnection(bool releaseConnection = true);
    }
}
