using System.Collections.Generic;

namespace Maple.Core.Data.DataSettings
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public partial class MainDataSettings
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainDataSettings()
        {
            RawDataSettings=new Dictionary<string, string>();
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DataProvider { get; set; }

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
            return !string.IsNullOrEmpty(this.DataProvider) && !string.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
