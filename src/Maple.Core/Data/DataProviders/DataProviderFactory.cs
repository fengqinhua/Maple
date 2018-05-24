using Maple.Core.Data.DataSettings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    public class DataProviderFactory : IDataProviderFactory
    {
        private volatile bool _disposed = false;
        private readonly object _sync = new object();
        private readonly Dictionary<string, DataSetting> _dataSettings = new Dictionary<string, DataSetting>();


        public DataProviderFactory() { }

        public void AddDataSettings(DataSetting dataSetting)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("DataProviderFactory");

            object sync = this._sync;
            lock (sync)
            {
                if (_dataSettings.ContainsKey(dataSetting.Name))
                    _dataSettings[dataSetting.Name] = dataSetting;
                else
                    _dataSettings.Add(dataSetting.Name, dataSetting);
            }
        }

        public IDataProvider CreateProvider(string dataSettingName)
        {
            throw new NotImplementedException();
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
