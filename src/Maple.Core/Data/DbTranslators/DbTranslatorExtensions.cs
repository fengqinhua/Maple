using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.DbTranslators
{
    public static class DbTranslatorExtensions
    {
        /// <summary>
        /// 包装表名称或字段
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="tableOrColumnName"></param>
        /// <returns></returns>
        public static string Quote(this IDbTranslator dbTranslator, string tableOrColumnName)
        {
            return string.Format("{0}{2}{1}", dbTranslator.OpenQuote, dbTranslator.CloseQuote, tableOrColumnName);
        }
        /// <summary>
        /// 包装参数
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string QuoteParameter(this IDbTranslator dbTranslator, string parameterName)
        {
            return string.Format("{0}{1}", dbTranslator.Connector, parameterName);
        }

    }
}
