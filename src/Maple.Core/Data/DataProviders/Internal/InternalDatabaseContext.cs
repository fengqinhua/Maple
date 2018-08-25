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
        private readonly IDataSetting _dataSetting;
        //是否已经disposed
        private volatile bool _disposed;
        //标识数据库连接是否打开
        private volatile bool _openedConnection;
        //标识数据库连接的请求次数 the number of active requests for an open connection
        private volatile int _connectionRequestCount;
        //数据库连接
        private IDbConnection _dbConnection;
        //数据库事务
        private IDbTransaction m_Transaction = null;
        //数据库事务启动次数
        private int m_TransactionCount = 0;

        /// <summary>
        /// 数据库上线文构造函数
        /// </summary>
        /// <param name="dataSetting"></param>
        /// <param name="dbTranslator"></param>
        public InternalDatabaseContext(IDataSetting dataSetting, IDbTranslator dbTranslator)
        {
            this._dataSetting = dataSetting;
            this.DbTranslator = dbTranslator;
        }

        /// <summary>
        /// 数据库信息翻译器
        /// </summary>
        public IDbTranslator DbTranslator { get; private set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection Connection
        {
            get { return this._dbConnection; }
        }
        /// <summary>
        /// 数据库事务
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return this.m_Transaction; }
        }
        /// <summary>
        /// 是否处于事务中
        /// </summary>
        public bool IsInTransaction
        {
            get { return this.m_Transaction != null; }
        }
        /// <summary>
        ///  开启数据库事务
        /// </summary>
        public void BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            this.EnsureConnection();
            if (this.m_Transaction == null)
            {
                if (isolationLevel == null)
                    this.m_Transaction = this._dbConnection.BeginTransaction();
                else
                    this.m_Transaction = this._dbConnection.BeginTransaction(isolationLevel.Value);
            }
            this.m_TransactionCount++;
        }

        /// <summary>
        /// 提交数据库事务
        /// </summary>
        public void Commit()
        {
            if (this.m_Transaction != null)
            {
                this.m_TransactionCount = this.m_TransactionCount - 1;
                if (this.m_TransactionCount == 0)
                {
                    this.m_Transaction.Commit(); 
                    this._dbConnection.Close();

                    this.m_Transaction.Dispose();
                    this.m_Transaction = null;
                }
            }
        }
        /// <summary>
        /// 回滚数据库事务
        /// </summary>
        public void Rollback()
        {
            Rollback(null);
        }
        /// <summary>
        /// 回滚数据库事务
        /// </summary>
        /// <param name="RollBackException">异常信息</param>
        public void Rollback(Exception RollBackException)
        {
            if (this.m_Transaction != null)
            {
                this.m_TransactionCount = this.m_TransactionCount - 1;
                if (this.m_TransactionCount != 0)
                {
                    if (RollBackException == null)
                        RollBackException = new Exception("数据库访问错误,原因未知!");
                    throw RollBackException;
                }
                else
                {
                    this.m_Transaction.Rollback();
                    if (this._dbConnection.State != ConnectionState.Closed)
                        this._dbConnection.Close();

                    this.m_Transaction.Dispose();
                    this.m_Transaction = null;
                }
            }
        }

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
            if (this.IsInTransaction)
                command.Transaction = this.Transaction;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dotNetValue"></param>
        /// <returns></returns>
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
                    if (this.m_Transaction != null)
                    {
                        this.m_Transaction.Rollback();
                        this.m_Transaction.Dispose();
                    }

                    if (this._dbConnection != null)
                    {
                        if (this._dbConnection.State != ConnectionState.Closed)
                            this._dbConnection.Close();
                        this._dbConnection.Dispose();
                    }

                    this._dbConnection = null;
                    this.m_Transaction = null;
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
