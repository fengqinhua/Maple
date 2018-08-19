using Maple.Core.Data.DataSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maple.Core.Data.DataProviders.Internal
{
    internal class InternalDataProvider : IDataProvider
    {
        private volatile bool _disposed = false;


        public InternalDataProvider(IDatabaseContext databaseContext)
        {
            this.DatabaseContext = databaseContext;
        }

        public IDatabaseContext DatabaseContext { get; private set; }


        #region Dispose

        public void Dispose()
        {
            if (this._disposed)
            {
                this._disposed = true;
                this.DatabaseContext.Dispose();
            }
        }

        protected virtual bool CheckDisposed()
        {
            return this._disposed;
        }

        #endregion

        /// <summary>
        /// 检测是否可以连接
        /// </summary>
        /// <returns></returns>
        public bool IsCanConnection()
        {
            bool result = false;
            try
            {
                this.DatabaseContext.EnsureConnection();
                result = true;
            }
            finally
            {
                this.DatabaseContext.ReleaseConnection();
            }
            return result;
        }

        /// <summary>
        /// 执行数据查询的R命令，返回数据集DataTable
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlStatement sqlStatement)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("InternalDataProvider");

            return Execute(() => InternalExecuteDataTable(sqlStatement), true);
        }
        /// <summary>
        /// 执行数据CUD命令，返回影响的行数 (立即执行)
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(SqlStatement sqlStatement)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("InternalDataProvider");

            return Execute(() => InternalExecuteNonQuery(sqlStatement), true);
        }
        /// <summary>
        /// 执行数据查询R命令，返回IDataReader
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public bool ExecuteReader(SqlStatement sqlStatement, CallbackObjectHandler<IDataReader> callback)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("InternalDataProvider");

            return Execute(() => InternalExecuteReader(sqlStatement, callback), true);
        }
        /// <summary>
        /// 执行数据查询的R命令，返回第一行第一列的值
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public object ExecuteScalar(SqlStatement sqlStatement)
        {
            if (this.CheckDisposed())
                throw new ObjectDisposedException("InternalDataProvider");

            return Execute(() => InternalExecuteScalar(sqlStatement), true);
        }


        /// <summary>
        /// 处理返回值
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <param name="cmd"></param>
        protected virtual void PopulateOutParams(DataParameterCollection dps, IDbCommand cmd)
        {
            if (dps.HasReturnValue)
            {
                for (int i = 0; i < dps.Count; i++)
                {
                    DataParameter p = dps[i];
                    if (p.Direction != ParameterDirection.Input)
                    {
                        p.Value = ((IDbDataParameter)cmd.Parameters[i]).Value;
                    }
                }
            }
        }
        protected virtual bool InternalExecuteReader(SqlStatement sqlStatement, CallbackObjectHandler<IDataReader> callback)
        {
            using (IDbCommand cmd = this.DatabaseContext.GetDbCommand(sqlStatement.SqlCommandType, sqlStatement.SqlCommandText, sqlStatement.CommandParameters, sqlStatement.SqlTimeOut, sqlStatement.NeedLog))
            {
                //IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //using (IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                //using (IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    callback(rdr);
                }
                cmd.Parameters.Clear();
            }
            return true;
        }
        protected virtual int InternalExecuteNonQuery(SqlStatement sqlStatement)
        {
            using (IDbCommand cmd = this.DatabaseContext.GetDbCommand(sqlStatement.SqlCommandType, sqlStatement.SqlCommandText, sqlStatement.CommandParameters, sqlStatement.SqlTimeOut, sqlStatement.NeedLog))
            {
                int result = cmd.ExecuteNonQuery();
                //处理返回值
                PopulateOutParams(sqlStatement.CommandParameters, cmd);
                cmd.Parameters.Clear();
                return result;
            }
        }
        protected virtual object InternalExecuteScalar(SqlStatement sqlStatement)
        {
            using (IDbCommand cmd = this.DatabaseContext.GetDbCommand(sqlStatement.SqlCommandType, sqlStatement.SqlCommandText, sqlStatement.CommandParameters, sqlStatement.SqlTimeOut, sqlStatement.NeedLog))
            {
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj;
            }
        }
        protected virtual DataTable InternalExecuteDataTable(SqlStatement sqlStatement)
        {
            DataTable dt = null;
            using (IDbCommand cmd = this.DatabaseContext.GetDbCommand(sqlStatement.SqlCommandType, sqlStatement.SqlCommandText, sqlStatement.CommandParameters, sqlStatement.SqlTimeOut, sqlStatement.NeedLog))
            {
                IDbDataAdapter da = this.DatabaseContext.GetDbAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                ds.Dispose();
                cmd.Parameters.Clear();
            }
            return dt;
        }
        protected virtual T Execute<T>(Func<T> func, bool releaseConnectionOnSuccess = true)
        {
            this.DatabaseContext.EnsureConnection();//确保数据库连接成功
            try
            {
                //执行命令
                T result = func();
                if (releaseConnectionOnSuccess)
                    this.DatabaseContext.ReleaseConnection(releaseConnectionOnSuccess);
                return result;
            }
            catch (Exception ex)
            {
                //释放数据库连接
                this.DatabaseContext.ReleaseConnection(true);
                throw ex;
            }
        }
    }
}
