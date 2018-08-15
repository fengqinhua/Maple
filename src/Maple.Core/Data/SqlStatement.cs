using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Maple.Core.Data
{
    /// <summary>
    /// SQL声明
    /// </summary>
    [Serializable]
    public class SqlStatement
    {
        /// <summary>
        /// 命令文本（SQL语句或者存储过程名称）
        /// </summary>
        public string SqlCommandText { get; set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType SqlCommandType { get; set; }
        /// <summary>
        /// 命令参数集合
        /// </summary>
        public DataParameterCollection CommandParameters { get; set; }
        /// <summary>
        /// 命令超时时间
        /// </summary>
        public int SqlTimeOut { get; set; }
        /// <summary>
        /// 是否需要写日志
        /// </summary>
        public bool NeedLog { get; set; }

        public SqlStatement(CommandType sqlCommandType, string sqlCommandText)
            : this(sqlCommandType, sqlCommandText, new DataParameterCollection())
        { }

        /// <summary>
        /// SQL语句
        /// </summary>
        /// <param name="sqlCommandText">CommandType</param>
        /// <param name="sqlCommandText">执行文本（SQL语句或者存储过程名称）(默认是SQL语句)</param>
        /// <param name="commandParameters">参数集合</param>
        public SqlStatement(CommandType sqlCommandType, string sqlCommandText, DataParameterCollection commandParameters)
            : this(sqlCommandType, sqlCommandText, commandParameters, DatabaseCommon.DbCommandTimeOut, DatabaseCommon.DbSqlNeedLog)
        {
        }

        /// <summary>
        /// SQL语句
        /// </summary>
        /// <param name="sqlCommandText">CommandType</param>
        /// <param name="sqlCommandText">执行文本（SQL语句或者存储过程名称）(默认是SQL语句)</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="sqlTimeOut">命令超时时间</param>
        /// <param name="needLog">是否需要写日志</param>
        public SqlStatement(CommandType sqlCommandType, string sqlCommandText, DataParameterCollection commandParameters, int sqlTimeOut , bool needLog)
        {
            SqlCommandType = sqlCommandType;
            SqlCommandText = sqlCommandText;
            CommandParameters = commandParameters;
            SqlTimeOut = sqlTimeOut;
            NeedLog = needLog;
        }


        public override bool Equals(object obj)
        {
            var sql = (SqlStatement)obj;
            var b = (this.SqlCommandText == sql.SqlCommandText)
                && (this.SqlTimeOut == sql.SqlTimeOut)
                && (this.CommandParameters.Equals(sql.CommandParameters)
                && (this.SqlCommandType == sql.SqlCommandType));
            return b;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("\n SqlCommandText:{0} \n SqlCommandType:{1} \n SqlTimeOut:{2} \n CommandParameters:{3}", SqlCommandText, SqlCommandType, SqlTimeOut, CommandParameters);
        }

    }
}
