using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data
{
    internal static class DatabaseCommon
    {
        /// <summary>
        /// 获取或设置 执行DbCommand超时时间（秒）
        /// </summary>
        public static int DbCommandTimeOut = 10;
        /// <summary>
        /// 获取或设置 是否需要记录SQL运行日志
        /// </summary>
        public static bool DbSqlNeedLog = false;
        ///// <summary>
        ///// 获取或设置 数据表名称或字段名称大小写
        ///// <para>0:默认</para>
        ///// <para>1:大写</para>
        ///// <para>2:小写</para>
        ///// </summary>
        //public static int DbCapitalization = 0;

        ///// <summary>
        ///// 执行大小写
        ///// </summary>
        ///// <param name="txt"></param>
        ///// <returns></returns>
        //public static string Capitalization(string txt)
        //{
        //    switch (DbCapitalization)
        //    {
        //        case 1:
        //            txt = txt.ToUpper();
        //            break;
        //        case 2:
        //            txt = txt.ToLower();
        //            break;
        //        default:
        //            break;
        //    }

        //    return txt;
        //}

    }
}
