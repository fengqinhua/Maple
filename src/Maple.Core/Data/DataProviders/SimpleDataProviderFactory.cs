using Maple.Core.Data.DataProviders.Internal;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    public class SimpleDataProviderFactory : IDataProviderFactory 
    {
        protected readonly IDataSettingContainer dataSettingContainer = null;

        public SimpleDataProviderFactory(IDataSettingContainer dataSettingContainer)
        {
            this.dataSettingContainer = dataSettingContainer;
        }

        public IDataProvider GetDataProvider()
        {
            return GetDataProvider(string.Empty);
        }

        public IDataProvider GetDataProvider(string dataSettingName)
        {
            IDataSetting dataSetting = this.dataSettingContainer.GetDataSetting(dataSettingName);
            if (dataSetting == null)
                throw new MapleException($"未设置[{dataSettingName}]对应的数据库配置信息");

            IDbTranslator dbTranslator = getDbTranslator(dataSetting);
            var databaseContext = new InternalDatabaseContext(dataSetting, dbTranslator);
            return new InternalDataProvider(databaseContext);
        }

        private IDbTranslator getDbTranslator(IDataSetting dataSetting)
        {
            IDbTranslator dbTranslator;
            switch (dataSetting.DataSouceType)
            {
                case DataSouceType.MySQL:
                    dbTranslator = new MySQLTranslator();
                    break;
                case DataSouceType.Oracle:
                    dbTranslator = new OracleTranslator();
                    break;
                case DataSouceType.Sql2000:
                    dbTranslator = new Sql2000Translator();
                    break;
                case DataSouceType.Sql2005:
                    dbTranslator = new Sql2005Translator();
                    break;
                case DataSouceType.Sql2008:
                    dbTranslator = new Sql2008Translator();
                    break;
                case DataSouceType.Sqlite:
                    dbTranslator = new SqliteTranslator();
                    break;
                default:
                    throw new MapleException(string.Format("未知的数据源类型“{0}”。请核实数据源类型配置是否正确。", dataSetting.DataSouceType));
            }

            return dbTranslator;
        }
    }
}
