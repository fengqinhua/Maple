using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    /// <summary>
    /// IDataProvider 数据库访问驱动工厂
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public interface IDataProviderFactory
    {
        /// <summary>
        /// 获得数据库访问驱动
        /// </summary>
        /// <returns></returns>
        IDataProvider GetDataProvider();
        /// <summary>
        /// 获得数据库访问驱动
        /// </summary>
        /// <param name="dataSettingName"></param>
        /// <returns></returns>
        IDataProvider GetDataProvider(string dataSettingName);
    }
}
