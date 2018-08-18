using Maple.Core.Data.DbMappers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    /// <summary>
    /// 数据库信息翻译器
    /// </summary>
    public interface IDbTranslator
    {
        DataSouceType DataSouceType { get; }
        /// <summary>
        /// 提供程序的固定名称
        /// </summary>
        string ProviderInvariantName { get; }
        /// <summary>
        /// 获取参数连接符
        /// </summary>
        /// <returns></returns>
        char Connector { get; }
        /// <summary>
        /// 引用开始
        /// </summary>
        string OpenQuote { get; }
        /// <summary>
        /// 引用结束
        /// </summary>
        string CloseQuote { get; }
        /// <summary>
        /// 表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        DbProviderFactory GetDbProviderFactory();
    }
}
