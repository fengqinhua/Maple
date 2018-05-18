using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    /// <summary>
    /// 数据库信息翻译器
    /// </summary>
    public interface IDbTranslator
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        DataSouceType DataSouceType { get; }
        /// <summary>
        /// 获取数据库对象工厂名称
        /// </summary>
        /// <returns></returns>
        string ProviderFactoryName { get; }
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
    }
}
