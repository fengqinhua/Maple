using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.Conditions
{
    public interface IExpressionParser
    {
        /// <summary>
        /// 将用于查询的拉曼达表达式转换为SQL查询片段
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="dbTranslator"></param>
        /// <param name="dpc"></param>
        /// <returns></returns>
        string ToSQL(System.Linq.Expressions.Expression expr, IDbTranslator dbTranslator, DataParameterCollection dpc);
        /// <summary>
        /// 获取IPropertyMapper（用于排序等场景）
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        IPropertyMapper GetPropertyMapper(System.Linq.Expressions.Expression expr);
    }
}
