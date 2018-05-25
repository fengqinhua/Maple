using Maple.Core.Data.DataSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    /// <summary>
    /// IDataProvider 数据库访问驱动工厂
    /// </summary>
    public interface IDataProviderFactory : IDisposable
    {
        /// <summary>
        /// 添加数据库配置信息
        /// </summary>
        /// <param name="driver"></param>
        void AddDataSettings(DataSetting driver);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSettingName"></param>
        /// <returns></returns>
        IDataProvider CreateProvider(string dataSettingName);
    }
}
