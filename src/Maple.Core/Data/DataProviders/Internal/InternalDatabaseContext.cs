using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Maple.Core.Data.DataSettings;
using Maple.Core.Data.DbTranslators;

namespace Maple.Core.Data.DataProviders.Internal
{
    /// <summary>
    /// 数据库上下文实现类
    /// </summary>
    internal class InternalDatabaseContext : IDatabaseContext
    {
        private readonly DataSetting _dataSetting;
        //是否已经disposed
        private volatile bool _disposed;
        //标识数据库连接是否打开
        private volatile bool _openedConnection;
        //标识数据库连接的请求次数 the number of active requests for an open connection
        private volatile int _connectionRequestCount;
        //数据库连接
        private IDbConnection _dbConnection;
        /// <summary>
        /// 数据库上线文构造函数
        /// </summary>
        /// <param name="dataSetting"></param>
        /// <param name="dbTranslator"></param>
        public InternalDatabaseContext(DataSetting dataSetting, IDbTranslator dbTranslator)
        {
            this._dataSetting = dataSetting;
            this.DbTranslator = dbTranslator;
        }

        /// <summary>
        /// 数据库信息翻译器
        /// </summary>
        public IDbTranslator DbTranslator { get; private set; }
        /// <summary>
        /// 确保数据库连接被打开
        /// </summary>
        public virtual void EnsureConnection()
        {
            if (this._disposed)
                throw new MapleException("IDatabaseContext is Disposed!");

            if (this._dbConnection == null)
                this._dbConnection = creatConnection();

            //与数据源连接断开。只有在连接打开后才有可能发生这种情况。可以关闭处于这种状态下的连接，然后重新打开
            if (this._dbConnection.State == ConnectionState.Broken)
                this._dbConnection.Close();

            if (this._dbConnection.State == ConnectionState.Closed)
            {
                this._dbConnection.Open();
                this._openedConnection = true;
            }

            if (this._openedConnection)
                this._connectionRequestCount++;
        }
        /// <summary>
        /// 释放数据库连接
        /// </summary>
        /// <param name="releaseConnection"></param>
        public virtual void ReleaseConnection(bool releaseConnection = true)
        {
            if (this._disposed)
                throw new MapleException("IDatabaseContext is Disposed!");

            if (this._openedConnection)
            {
                if (this._connectionRequestCount > 0)
                    this._connectionRequestCount--;

                if (releaseConnection && this._connectionRequestCount == 0)
                {
                    this._dbConnection.Close();
                    this._openedConnection = false;
                }
            }
        }
        /// <summary>
        /// 获取IDbCommand 接口
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="dps"></param>
        /// <param name="sqlTimeOut"></param>
        /// <param name="needLog"></param>
        /// <returns></returns>
        public virtual IDbCommand GetDbCommand(CommandType commandType,string commandText, DataParameterCollection dps, int sqlTimeOut = 10, bool needLog=false)
        {
            if (this._disposed)
                throw new MapleException("IDatabaseContext is Disposed!");

            IDbCommand command = this.DbTranslator.GetDbProviderFactory().CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;
            command.Connection = this._dbConnection;
            //if (this.IsInTransaction)
            //    command.Transaction = conn.Transaction;
            if (needLog)
                Console.WriteLine(commandText);
            //设置超时时间
            this.setCommandTimeOut(command, sqlTimeOut);
            //设置参数
            this.fillDbParameters(command, dps);
            return command;
        }
        /// <summary>
        /// 获取IDbDataAdapter接口
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual IDbDataAdapter GetDbAdapter(IDbCommand command)
        {
            if (this._disposed)
                throw new MapleException("IDatabaseContext is Disposed!");

            IDbDataAdapter d = this.DbTranslator.GetDbProviderFactory().CreateDataAdapter();
            d.SelectCommand = command;
            return d;
        }

        /// <summary>
        /// 创建数据库连接IDbConnection
        /// </summary>
        /// <returns></returns>
        protected virtual IDbConnection creatConnection()
        {
            IDbConnection dbConnection = this.DbTranslator.GetDbProviderFactory().CreateConnection();
            dbConnection.ConnectionString = _dataSetting.DataConnectionString;
            return dbConnection;
        }
        /// <summary>
        /// 设置IDbCommand 接口的超时时长
        /// </summary>
        /// <param name="command"></param>
        /// <param name="timeOut"></param>
        protected virtual void setCommandTimeOut(IDbCommand command, int timeOut)
        {
            command.CommandTimeout = timeOut;
        }
        /// <summary>
        /// 填充DataParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dps"></param>
        protected virtual void fillDbParameters(IDbCommand command, DataParameterCollection dps)
        {
            if (dps == null)
                return;

            foreach (DataParameter dp in dps)
            {
                command.Parameters.Add(this.getDbParameter(dp));
            }
        }
        /// <summary>
        /// 创建IDbDataParameter
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        protected virtual IDbDataParameter getDbParameter(DataParameter dp)
        {
            IDbDataParameter odp = this.DbTranslator.GetDbProviderFactory().CreateParameter();
            odp.ParameterName = this.DbTranslator.QuoteParameter(dp.Key);
            odp.Value = this.getDbValue(dp.Value);
            odp.DbType = dp.Type;
            odp.Direction = dp.Direction;
            //odp.SourceColumn = dp.SourceColumn;
            if (dp.Type == DbType.String && dp.Size > 0 && dp.Size < 10000)
                odp.Size = dp.Size;
            return odp;
        }

        protected virtual object getDbValue(object dotNetValue)
        {
            if (dotNetValue == null)
                return DBNull.Value;
            //if (dotNetValue.GetType().IsEnum)
            //    return (int)dotNetValue;
            return dotNetValue;
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
                    if (this._dbConnection != null)
                    {
                        if (this._dbConnection.State != ConnectionState.Closed)
                            this._dbConnection.Close();
                        this._dbConnection.Dispose();
                    }
                    this._dbConnection = null;
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
