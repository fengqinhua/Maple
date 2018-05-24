using System;
using System.IO;
using Newtonsoft.Json;
using Maple.Core.Infrastructure;

namespace Maple.Core.Data.DataSettings
{
    /// <summary>
    /// 主库连接信息管理
    /// </summary>
    public partial class MainDataSettingsManager
    {
        #region 构造函数

        /// <summary>
        /// 数据配置信息文件
        /// </summary>
        private const string DataSettingsFilePath_ = "~/App_Data/mainDataSettings.json";

        #endregion

        #region 属性字段

        /// <summary>
        /// 数据配置信息文件
        /// </summary>
        public static string DataSettingsFilePath => DataSettingsFilePath_;

        #endregion

        #region 公用方法

        /// <summary>
        /// 加载数据库配置信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="reloadSettings"></param>
        /// <returns></returns>
        public virtual DataSetting LoadSettings(string filePath = null, bool reloadSettings = false)
        {
            if (!reloadSettings && Singleton<DataSetting>.Instance != null)
                return Singleton<DataSetting>.Instance;

            filePath = filePath ?? CommonHelper.MapPath(DataSettingsFilePath);

            //检查文件是否存在，如果不存在，则返回空
            if (!File.Exists(filePath))
                return new DataSetting();
            //读取配置信息文本内容
            var text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
                return new DataSetting();
            //获取配置信息对象实例
            Singleton<DataSetting>.Instance = JsonConvert.DeserializeObject<DataSetting>(text);
            return Singleton<DataSetting>.Instance;
        }

        /// <summary>
        /// 保持配置信息
        /// </summary>
        /// <param name="settings">Data settings</param>
        public virtual void SaveSettings(DataSetting settings)
        {
            Singleton<DataSetting>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            var filePath = CommonHelper.MapPath(DataSettingsFilePath);

            //如果文件不存在则创建之
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
            }

            //保存信息
            var text = JsonConvert.SerializeObject(Singleton<DataSetting>.Instance, Formatting.Indented);
            File.WriteAllText(filePath, text);
        }

        #endregion
    }
}
