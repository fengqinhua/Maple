﻿namespace Maple.Core.Data.DataSettings
{
    /// <summary>
    /// 数据库设置帮助类
    /// </summary>
    public partial class MainDataSettingsHelper
    {
        private static bool? _databaseIsInstalled;

        /// <summary>
        /// 判断数据库是否已安装配置完成
        /// </summary>
        /// <returns></returns>
        public static bool DatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var manager = new MainDataSettingsManager();
                var settings = manager.LoadSettings(reloadSettings:true);
                _databaseIsInstalled = settings != null && !string.IsNullOrEmpty(settings.DataConnectionString);
            }
            return _databaseIsInstalled.Value;
        }

        /// <summary>
        /// 重置数据库是否安装配置完成的状态
        /// </summary>
        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }
    }
}
