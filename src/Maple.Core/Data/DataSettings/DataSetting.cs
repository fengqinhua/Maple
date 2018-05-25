using System.Collections.Generic;

namespace Maple.Core.Data.DataSettings
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public partial class DataSetting
    {
        public const string DefalutDataSettingName = "Maple.Defalut";

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataSetting()
        {
            RawDataSettings = new Dictionary<string, string>();
        }
        /// <summary>
        /// 数据库配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataSouceType DataSouceType { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// 数据库连接配置项
        /// </summary>
        public IDictionary<string, string> RawDataSettings { get; }

        /// <summary>
        /// 校验数据是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return DataSouceType != DataSouceType.Unknown && !string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
