﻿using Maple.Core.Data.DbTranslators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public interface IDatabaseContext : IDisposable
    {

        /// <summary>
        /// 数据库信息翻译器
        /// </summary>
        IDbTranslator DbTranslator { get; }
        /// <summary>
        /// 获取数据库连接并保证数据库连接已经打开
        /// </summary>
        void EnsureConnection();
        /// <summary>
        /// 释放数据库连接
        /// </summary>
        /// <param name="releaseConnection"></param>
        void ReleaseConnection(bool releaseConnection = true);

        /// <summary>
        ///  开启数据库事务
        /// </summary>
        void BeginTransaction(IsolationLevel? isolationLevel = null);
        /// <summary>
        /// 提交数据库事务
        /// </summary>
        void Commit();
        /// <summary>
        /// 回滚数据库事务
        /// </summary>
        void Rollback();
        /// <summary>
        /// 回滚数据库事务
        /// </summary>
        /// <param name="RollBackException">异常信息</param>
        void Rollback(Exception RollBackException);
        /// <summary>
        /// 获取IDbCommand 接口
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="dps"></param>
        /// <param name="sqlTimeOut"></param>
        /// <param name="needLog"></param>
        /// <returns></returns>
        IDbCommand GetDbCommand(CommandType commandType, string commandText, DataParameterCollection dps, int sqlTimeOut = 10, bool needLog = false);
        /// <summary>
        /// 获取IDbDataAdapter接口
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        IDbDataAdapter GetDbAdapter(IDbCommand command);
    }
}
