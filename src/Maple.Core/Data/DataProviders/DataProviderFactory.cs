using Maple.Core.Data.DataProviders.Internal;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;
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
        protected readonly Dictionary<string, IDataProvider> _dataProvider = new Dictionary<string, IDataProvider>();


        public void AddDataSettings(DataSetting dataSetting)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("DataProviderFactory");

            object sync = this._sync;
            lock (sync)
            {
                if (!_dataProvider.ContainsKey(dataSetting.Name))
                {
                    IDbTranslator dbTranslator = getDbTranslator(dataSetting);
                    IDatabaseContext databaseContext = new InternalDatabaseContext(dataSetting, dbTranslator);
                    IDataProvider dataProvider = new InternalDataProvider(databaseContext);

                    _dataProvider.Add(dataSetting.Name, dataProvider);
                }
            }
        }

        public IDataProvider CreateProvider(string dataSettingName)
        {
            if (!_dataProvider.ContainsKey(dataSettingName))
                throw new Exception(string.Format("未知的数据源“{0}”", dataSettingName));
            else
                return _dataProvider[dataSettingName];
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
                _dataSettings.Clear();
            }
        }

        protected virtual bool CheckDisposed()
        {
            return this._disposed;
        }
    }
}
