using Maple.Core.Data.DataProviders.Internal;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    public class DataProviderFactory : IDataProviderFactory
    {
        private volatile bool _disposed = false;
        private readonly object _sync = new object();
        private readonly Dictionary<string, IDataProvider> _dataProviders = new Dictionary<string, IDataProvider>();

        public DataProviderFactory()
        {
            if (MainDataSettingsHelper.DatabaseIsInstalled())
                AddDataSettings(Singleton<DataSetting>.Instance);
        }

        public void AddDataSettings(DataSetting dataSetting)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("DataProviderFactory");

            object sync = this._sync;
            lock (sync)
            {
                if (!_dataProviders.ContainsKey(dataSetting.Name))
                {
                    IDbTranslator dbTranslator = getDbTranslator(dataSetting);
                    IDatabaseContext databaseContext = new InternalDatabaseContext(dataSetting, dbTranslator);
                    IDataProvider dataProvider = new InternalDataProvider(databaseContext);

                    _dataProviders.Add(dataSetting.Name, dataProvider);
                }
            }
        }

        public IDataProvider CreateProvider(string dataSettingName)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("DataProviderFactory");



            if (!_dataProviders.ContainsKey(dataSettingName))
                throw new Exception(string.Format("未知的数据源“{0}”", dataSettingName));
            else
                return _dataProviders[dataSettingName];
        }


        private IDbTranslator getDbTranslator(DataSetting dataSetting)
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
                    throw new Exception(string.Format("未知的数据源类型“{0}”。请核实数据源类型配置是否正确。", dataSetting.DataSouceType));
            }

            return dbTranslator;
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                this._disposed = true;
                foreach(var item in this._dataProviders)
                {
                    item.Value.Dispose();
                }
                this._dataProviders.Clear();
            }
        }

        protected virtual bool CheckDisposed()
        {
            return this._disposed;
        }
    }
}
