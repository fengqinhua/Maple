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
        /// 获取数据库连接
        /// </summary>
        IDbConnection DbConnection { get; }
        /// <summary>
        /// 数据库信息翻译器
        /// </summary>
        IDbTranslator DbTranslator { get; }
    }
}
