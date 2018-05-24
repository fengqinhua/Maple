using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;

namespace Maple.Core.Data.DataProviders.Internal
{
    internal class InternalDatabaseContext : IDatabaseContext
    {
        private readonly DataSetting _dataSetting;
        //是否已经disposed
        private volatile bool _disposed;
        //标识数据库连接是否打开
        private volatile bool _openedConnection;
        //标识数据库连接的请求次数 the number of active requests for an open connection
        private volatile int _connectionRequestCount;

        public InternalDatabaseContext(DataSetting dataSetting)
        {
            this._dataSetting = dataSetting;
        }

        /// <summary>
        /// 数据库信息翻译器
        /// </summary>
        public IDbTranslator DbTranslator { get; private set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection { get; private set; }

        /// <summary>
        /// 确保数据库连接被打开
        /// </summary>
        internal void EnsureConnection()
        {
            if (this._disposed)
                throw new Exception("IDatabaseContext is Disposed!");

            if (this.DbConnection == null)
                this.DbConnection = creatConnection();

            //与数据源连接断开。只有在连接打开后才有可能发生这种情况。可以关闭处于这种状态下的连接，然后重新打开
            if (this.DbConnection.State == ConnectionState.Broken)
                this.DbConnection.Close();

            if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.DbConnection.Open();
                this._openedConnection = true;
            }

            if (this._openedConnection)
                this._connectionRequestCount++;
        }
        /// <summary>
        /// 释放数据库连接
        /// </summary>
        /// <param name="releaseConnection"></param>
        internal void ReleaseConnection(bool releaseConnection = true)
        {
            if (this._disposed)
                throw new Exception("IDatabaseContext is Disposed!");

            if (this._openedConnection)
            {
                if (this._connectionRequestCount > 0)
                    this._connectionRequestCount--;

                if (releaseConnection && this._connectionRequestCount == 0)
                {
                    this.DbConnection.Close();
                    this._openedConnection = false;
                }
            }
        }

        private IDbConnection creatConnection()
        {
            IDbConnection dbConnection = this.DbTranslator.GetDbProviderFactory().CreateConnection();
            dbConnection.ConnectionString = _dataSetting.DataConnectionString;
            return dbConnection;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                this._disposed = true;
                if (disposing)
                {
                    if (this.DbConnection != null)
                    {
                        if (this.DbConnection.State != ConnectionState.Closed)
                            this.DbConnection.Close();
                        this.DbConnection.Dispose();
                    }
                    this.DbConnection = null;
                }
            }
        }

        internal bool IsDisposed
        {
            get { return _disposed; }
        }

        #endregion
    }
}
