using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataSettings
{
    public class DefaultDataSettingContainer : IDataSettingContainer
    {
        private volatile bool _disposed = false;
        private readonly object _sync = new object();
        private readonly Dictionary<string, IDataSetting> _dataProviders = new Dictionary<string, IDataSetting>();

        public DefaultDataSettingContainer()
        {
            if (MainDataSettingsHelper.DatabaseIsInstalled())
                AddOrUpdate(Singleton<IDataSetting>.Instance);
        }

        public void AddOrUpdate(IDataSetting dataSetting)
        {
            Check.NotNull(dataSetting, nameof(dataSetting));

            if (string.IsNullOrEmpty(dataSetting.Name))
                throw new MapleException("未设置DataSetting名称");

            if (this.CheckDisposed())
                throw new ObjectDisposedException("DataProviderFactory");

            object sync = this._sync;
            lock (sync)
            {
                if (!_dataProviders.ContainsKey(dataSetting.Name))
                    _dataProviders.Add(dataSetting.Name, dataSetting);
                else
                    _dataProviders[dataSetting.Name] = dataSetting;
            }
        }

        public IDataSetting GetDataSetting(string dataSettingName)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("DataProviderFactory");

            if (!_dataProviders.ContainsKey(dataSettingName))
                throw new MapleException(string.Format("未知的数据源“{0}”", dataSettingName));
            else
            {
                return _dataProviders[dataSettingName];
            }
        }

        public void Clear()
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("DataProviderFactory");

            this._dataProviders.Clear();
        }

        #region IDispose

        public void Dispose()
        {
            if (this._disposed)
            {
                this._disposed = true;
                //foreach(var item in this._dataProviders)
                //{
                //    item.Value.Dispose();
                //}
                this._dataProviders.Clear();
            }
        }
        protected virtual bool CheckDisposed()
        {
            return this._disposed;
        }
        #endregion

    }
}
