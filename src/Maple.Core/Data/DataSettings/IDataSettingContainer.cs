using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataSettings
{
    public interface IDataSettingContainer : IDisposable
    {
        /// <summary>
        /// 添加或更新数据库配置信息
        /// </summary>
        /// <param name="driver"></param>
        void AddOrUpdate(IDataSetting dataSetting);
        /// <summary>
        /// 获取数据库配置信息
        /// </summary>
        /// <param name="dataSettingName"></param>
        /// <returns></returns>
        IDataSetting GetDataSetting(string dataSettingName);
        /// <summary>
        /// 清理数据库配置（始终保持缺省配置）
        /// </summary>
        void Clear();
    }
}
