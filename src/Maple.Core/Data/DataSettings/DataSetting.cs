using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Maple.Core.Data.DataSettings
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public class DataSetting : IDataSetting
    {
        public const string DefalutDataSettingName = "Maple.Defalut";

        public DataSetting(string name, DataSouceType dataSouceType,string dataConnectionString, IDictionary<string, string> rawDataSettings =null)
        {
            this.Name = name;
            this.DataSouceType = dataSouceType;
            this.DataConnectionString = dataConnectionString;

            if (rawDataSettings == null)
                RawDataSettings = new Dictionary<string, string>();
            else
                RawDataSettings = new ReadOnlyDictionary<string, string>(rawDataSettings);

        }
        /// <summary>
        /// 数据库配置名称
        /// </summary>
        public string Name { get;private set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataSouceType DataSouceType { get; private set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DataConnectionString { get; private set; }

        /// <summary>
        /// 数据库连接配置项
        /// </summary>
        public IReadOnlyDictionary<string, string> RawDataSettings { get; private set; }
        
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
