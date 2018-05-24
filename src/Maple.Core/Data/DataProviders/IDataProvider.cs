using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DataProviders
{
    /// <summary>
    /// IDataProvider 数据库访问驱动
    /// </summary>
    public interface IDataProvider : IDisposable
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        IDatabaseContext DatabaseContext { get; }

        /// <summary>
        /// 执行数据CUD命令，返回影响的行数 (立即执行)
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        int ExecuteNonQuery(SqlStatement sqlStatement);
        /// <summary>
        /// 执行数据查询R命令，返回IDataReader
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        bool ExecuteReader(SqlStatement sqlStatement, CallbackObjectHandler<System.Data.IDataReader> callback);
        /// <summary>
        /// 执行数据查询的R命令，返回第一行第一列的值
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        object ExecuteScalar(SqlStatement sqlStatement);
        /// <summary>
        /// 执行数据查询的R命令，返回数据集DataTable
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        System.Data.DataTable ExecuteDataTable(SqlStatement sqlStatement);
    }
}
