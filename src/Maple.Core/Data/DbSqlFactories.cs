using Maple.Core.Data.Conditions;
using Maple.Core.Data.DataProviders;
using Maple.Core.Data.DbMappers;
using Maple.Core.Data.DbTranslators;
using Maple.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Maple.Core.Data
{
    public static class DbSqlFactories
    {
        /// <summary>
        /// 生成插入的SQL声明
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static SqlStatement BuildInsertSqlStatement(IDbTranslator dbTranslator, IEntityMapper entityInfo,object entity)
        {
            if (entityInfo.PKeyProperties.Count == 0)
                throw new MapleException("当前实体中未设置主键");

            //插入的SQL语句
            StringBuilder sBuilderFiled = new StringBuilder();
            StringBuilder sBuilderValue = new StringBuilder();
            var dpc = new DataParameterCollection();

            foreach (var propertyMapper in entityInfo.PKeyProperties)
            {
                sBuilderFiled.Append(dbTranslator.Quote(propertyMapper.ColumnName));
                sBuilderFiled.Append(",");

                sBuilderValue.Append(dbTranslator.QuoteParameter(propertyMapper.ColumnName));
                sBuilderValue.Append(",");

                dpc.Add(getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
            }

            foreach (var propertyMapper in entityInfo.OtherProperties)
            {
                sBuilderFiled.Append(dbTranslator.Quote(propertyMapper.ColumnName));
                sBuilderFiled.Append(",");

                sBuilderValue.Append(dbTranslator.QuoteParameter(propertyMapper.ColumnName));
                sBuilderValue.Append(",");

                dpc.Add(getDataParameter(propertyMapper, propertyMapper.FastGetValue(entity)));
            }

            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2});\n",
                                        dbTranslator.Quote(entityInfo.TableName),
                                        sBuilderFiled.ToString().TrimEnd(','),
                                        sBuilderValue.ToString().TrimEnd(','));
            return new SqlStatement(System.Data.CommandType.Text, sql, dpc);
        }
        /// <summary>
        /// 生成更新的SQL声明
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static SqlStatement BuildUpdateSqlStatement(IDbTranslator dbTranslator, IEntityMapper entityInfo, object entity)
        {
            if (entityInfo.PKeyProperties.Count == 0)
                throw new MapleException("当前实体中未设置主键");
            if (entityInfo.OtherProperties.Count == 0)
                throw new MapleException("当前实体中除了主键以外无其他字段");

            //	UPDATE [Test] SET [name] = @name WHERE id=@id 
            StringBuilder sBuilderSet = new StringBuilder();
            StringBuilder sBuilderWhere = new StringBuilder();
            var dpc = new DataParameterCollection();

            foreach (var item in entityInfo.OtherProperties)
            {
                sBuilderSet.Append(dbTranslator.Quote(item.ColumnName));
                sBuilderSet.Append(" = ");
                sBuilderSet.Append(dbTranslator.QuoteParameter(item.ColumnName));
                sBuilderSet.Append(",");

                dpc.Add(getDataParameter(item, item.FastGetValue(entity)));
            }

            for (int i = 0; i <entityInfo.PKeyProperties.Count; i++)
            {
                var item =entityInfo.PKeyProperties[i];
                sBuilderWhere.Append(dbTranslator.Quote(item.ColumnName));
                sBuilderWhere.Append(" = ");
                sBuilderWhere.Append(dbTranslator.QuoteParameter(item.ColumnName));
                if (i <entityInfo.PKeyProperties.Count - 1)
                    sBuilderWhere.Append(" AND ");

                dpc.Add(getDataParameter(item, item.FastGetValue(entity)));
            }

            string sql = string.Format("UPDATE {0} SET {1} WHERE {2};\n",
                                        dbTranslator.Quote(entityInfo.TableName),
                                        sBuilderSet.ToString().TrimEnd(','),
                                        sBuilderWhere.ToString());

            return new SqlStatement(System.Data.CommandType.Text, sql, dpc);
        }
        /// <summary>
        /// 生成删除的SQL声明
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static SqlStatement BuildDeleteSqlStatement(IDbTranslator dbTranslator, IEntityMapper entityInfo, object entity)
        {
            if (entityInfo.PKeyProperties.Count == 0)
                throw new MapleException("当前实体中未设置主键");
            //DELETE [Customers] WHERE CustomerID=@CustomerID and City=@City 
            StringBuilder sBuilderWhere = new StringBuilder();
            var dpc = new DataParameterCollection();

            for (int i = 0; i < entityInfo.PKeyProperties.Count; i++)
            {
                var item = entityInfo.PKeyProperties[i];
                sBuilderWhere.Append(dbTranslator.Quote(item.ColumnName));
                sBuilderWhere.Append(" = ");
                sBuilderWhere.Append(dbTranslator.QuoteParameter(item.ColumnName));

                if (i < entityInfo.PKeyProperties.Count - 1)
                {
                    sBuilderWhere.Append(" AND ");
                }

                dpc.Add(getDataParameter(item, item.FastGetValue(entity)));
            }
            string sql = string.Format("DELETE FROM {0} WHERE {1};\n",
                                        dbTranslator.Quote(entityInfo.TableName),
                                        sBuilderWhere.ToString());
            return new SqlStatement(System.Data.CommandType.Text, sql, dpc);
        }
        /// <summary>
        /// 生成删除的SQL声明
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static SqlStatement BuildDeleteSqlStatement(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr)
        {
            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);

            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            string sqlString = string.Format("DELETE FROM {0} {1}",
                dbTranslator.Quote(entityInfo.TableName),
                strWhere);
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);
        }
        /// <summary>
        /// 生成Function的SQL声明
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static SqlStatement BuildFunctionSqlStatement(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, string fieldName, FieldFunction function)
        {
            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);
            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            //设置Function
            string functionText = getFunctionSelectText(dbTranslator, fieldName, function);
            //生成SQL
            string sqlString = string.Format("SELECT {0} FROM {1} {2}",
                                            functionText,
                                            dbTranslator.Quote(entityInfo.TableName),
                                            strWhere);
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);
        }

        /// <summary>
        /// 生成查询的SQL声明
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="whereExpr"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static SqlStatement BuildSelectSqlStatement(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, Dictionary<string, FieldSearchOrder> order = null)
        {
            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);
            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            //查询字段
            string strFieldInfo = getSelectText(dbTranslator, entityInfo);
            //排序规则
            string strOrder = getOrderInfo(dbTranslator, entityInfo, order, "", true);
            //SQL语句
            string sqlString = string.Format("SELECT {0} FROM {1} {2} {3}",
                strFieldInfo,
                dbTranslator.Quote(entityInfo.TableName),
                strWhere,
                strOrder);
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);
        }

        /// <summary>
        /// 生成查询的SQL声明
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="whereExpr"></param>
        /// <param name="range"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static SqlStatement BuildSelectSqlStatement(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, Range range, Dictionary<string, FieldSearchOrder> order = null)
        {
            if (range == null)
                return BuildSelectSqlStatement(dbTranslator, entityInfo, whereExpr, order);
            else
            {
                if (dbTranslator.DataSouceType == DataSouceType.Sql2005
                    || dbTranslator.DataSouceType == DataSouceType.Sql2008)
                    return rangeSelectSql2005(dbTranslator, entityInfo, whereExpr, range, order);
                else if (dbTranslator.DataSouceType == DataSouceType.MySQL)
                    return rangeSelectMySQL(dbTranslator, entityInfo, whereExpr, range, order);
                else if (dbTranslator.DataSouceType == DataSouceType.Sqlite)
                    return rangeSelectSqlite(dbTranslator, entityInfo, whereExpr, range, order);
                else if (dbTranslator.DataSouceType == DataSouceType.Oracle)
                    return rangeSelectOracle(dbTranslator, entityInfo, whereExpr, range, order);
                else if (dbTranslator.DataSouceType == DataSouceType.Sql2000)
                    return rangeSelectSql2000(dbTranslator, entityInfo, whereExpr, range, order);
                else
                    return null;
            }
        }

        /// <summary>
        /// 分页查询（Sqlserver2000）
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="whereExpr"></param>
        /// <param name="range"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static SqlStatement rangeSelectSql2000(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, Range range, Dictionary<string, FieldSearchOrder> order = null)
        {
            //采用改良的方法三

            //SELECT * FROM (
            //  SELECT TOP 页长 * FROM (
            //    SELECT TOP 页长*页数 * FROM {表名}
            //  ) ORDER BY {排序字段} ASC
            //) ORDER BY {排序字段} DESC

            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);
            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            //查询字段
            string strFieldInfo = getSelectText(dbTranslator, entityInfo);
            //排序规则
            string strOrder = getOrderInfo(dbTranslator, entityInfo, order, "", true);
            string strOrder2 = getOrderInfo(dbTranslator, entityInfo, order, "", true, false);

            string temp = "SELECT {8} FROM (SELECT TOP {4} {9} FROM (SELECT TOP {5} {0} FROM {1} {2} {3}) A {6}) B {7}";
            string sqlString = string.Format(temp,
                strFieldInfo,
                dbTranslator.Quote(entityInfo.TableName),
                strWhere,
                strOrder, range.Rows, range.EndIndex, strOrder2, strOrder, strFieldInfo, strFieldInfo);
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);

            //[SQLServer]SQL Server 2000的分页方法(SQL篇)
            //不像SQL Server 2005提供ROW_NUMBER()和Oracle的ROWNUM伪列，SQL Server 2000本身无提供方便的分页方法，但可用select top的混合SQL语句实现。
            //方法A：结合not in和select top

            //SELECT TOP 页长 * FROM {表名}
            //WHERE {字段} NOT IN (
            //  SELECT TOP 页长*(页数-1) {字段} FROM {表名} ORDER BY {排序字段}
            //)

            //ORDER BY {排序字段}
            //点评：语句简单，但not in的字段不能有重复数据
            //性能：页数越靠后越慢
            //限制：只适合使用主键字段not in

            //--------------------------------------------------
            //方法B：结合>和select top

            //SELECT TOP 页长 * FROM {表名}
            //WHERE {字段} > (
            //  SELECT MAX({字段}) FROM (
            //    SELECT TOP 页长*(页数-1) {字段} FROM {表名} ORDER BY {排序字段}
            //  )
            //)
            //ORDER BY {排序字段}
            //点评：跟方法A类似，但语句比方法A多一层查询。
            //性能：页数越靠后越慢，但由于使用数字型字段，比方法A快
            //限制：只适合主键字段是数字型并且最好是自增的
            //--------------------------------------------------

            //方法C：结合双select top和相反排序
            //SELECT * FROM (
            //  SELECT TOP 页长 * FROM (
            //    SELECT TOP 页长*页数 * FROM {表名}
            //  ) ORDER BY {排序字段} ASC
            //) ORDER BY {排序字段} DESC

            //点评：与上面两种方法不一样，此方法对字段类型无任何要求，且最里层语句结构可随意(需添加top 页长*页数)，此方法依赖互斥的排序
            //性能：页数越靠后越慢，但可改进为，当查询数据位置位于数据总数后半部分时，前一个排序改为倒序，后一个排序改为顺序
            //限制：必须排序，且需要特别处理最后一页情况(最后一页时最里层是top 记录总数%页长)
        }

        /// <summary>
        /// 分页查询（Sqlserver2005 以上版本）
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="whereExpr"></param>
        /// <param name="range"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static SqlStatement rangeSelectSql2005(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, Range range, Dictionary<string, FieldSearchOrder> order = null)
        {
            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);
            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            //查询字段
            string strFieldInfo = getSelectText(dbTranslator, entityInfo);
            //排序规则
            string strOrder = getOrderInfo(dbTranslator, entityInfo, order, "", true);

            string temp = "SELECT * FROM (SELECT * FROM (SELECT {0},ROW_NUMBER() OVER({3}) AS [row_number] FROM {1} {2}) p WHERE p.[row_number] >= {4}) q WHERE q.[row_number] <= {5}";
            string sqlString = string.Format(temp,
                strFieldInfo,
                dbTranslator.Quote(entityInfo.TableName),
                strWhere,
                strOrder, range.StartIndex, range.EndIndex);
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);

            //Linq提供了Skip和Take的API可以用于分页，由于使用的是Entity Framework，在好奇的驱使下用EFProfiler查看生成的SQL，才知道这样以下分页更好。 
            //主要就是使用了row_numer()over()这样的分析函数，可以直接找到那第5000行开始的地方，然后在取出30行就行了。
            // select *
            //   from (select *
            //           from (select t.*, row_number() OVER(ORDER BY null) AS "row_number"
            //                   from yz_bingrenyz t) p
            //          where p."row_number" > 5000) q
            //  where rownum <= 300

        }

        /// <summary>
        /// 分页查询（Oracle）
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="whereExpr"></param>
        /// <param name="range"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static SqlStatement rangeSelectOracle(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, Range range, Dictionary<string, FieldSearchOrder> order = null)
        {
            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);
            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            //查询字段
            string strFieldInfo = getSelectText(dbTranslator, entityInfo);
            //排序规则
            string strOrder = getOrderInfo(dbTranslator, entityInfo, order, "", true);

            string temp = "SELECT * FROM (SELECT * FROM (SELECT {0},ROWNUM AS ROW_NUMBER FROM {1} {2} {3}) p WHERE p.ROW_NUMBER >= {4}) q WHERE q.ROW_NUMBER <= {5}";
            string sqlString = string.Format(temp,
                strFieldInfo,
                dbTranslator.Quote(entityInfo.TableName),
                strWhere,
                strOrder, range.StartIndex, range.EndIndex);
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);
            //具体原理同sql2005
        }

        /// <summary>
        /// 分页查询（Sqlite）
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="whereExpr"></param>
        /// <param name="range"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static SqlStatement rangeSelectSqlite(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, Range range, Dictionary<string, FieldSearchOrder> order = null)
        {
            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);
            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            //查询字段
            string strFieldInfo = getSelectText(dbTranslator, entityInfo);
            //排序规则
            string strOrder = getOrderInfo(dbTranslator, entityInfo, order, "", true);

            //select * from users order by id limit 10 offset 0;//offset代表从第几条记录“之后“开始查询，limit表明查询多少条结果
            //运用：
            //sqlitecmd.CommandText = string.Format("select * from GuestInfo order by GuestId limit {0} offset {0}*{1}", size, index-1);//size:每页显示条数，index页码

            string temp = "SELECT {0} FROM {1} {2} {3} limit {4} offset {5}";
            string sqlString = string.Format(temp,
                strFieldInfo,
                dbTranslator.Quote(entityInfo.TableName),
                strWhere,
                strOrder,
                (range.EndIndex - range.StartIndex + 1)
                , range.StartIndex - 1);
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);
        }

        /// <summary>
        /// 分页查询（MySQL）
        /// </summary>
        /// <param name="dbTranslator"></param>
        /// <param name="entityInfo"></param>
        /// <param name="whereExpr"></param>
        /// <param name="range"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static SqlStatement rangeSelectMySQL(IDbTranslator dbTranslator, IEntityMapper entityInfo, Expression whereExpr, Range range, Dictionary<string, FieldSearchOrder> order = null)
        {
            IExpressionParser parser = new ExpressionParser(entityInfo);
            //设置查询条件
            var dpc = new DataParameterCollection();
            string strWhere = "";
            if (whereExpr != null)
                strWhere = parser.ToSQL(whereExpr, dbTranslator, dpc);
            if (strWhere.Length > 0)
                strWhere = "WHERE " + strWhere;
            //查询字段
            string strFieldInfo = getSelectText(dbTranslator, entityInfo);
            //排序规则
            string strOrder = getOrderInfo(dbTranslator, entityInfo, order, "", true);

            //SELECT * FROM table LIMIT [offset,] rows | rows OFFSET offset  
            //LIMIT 子句可以被用于强制 SELECT 语句返回指定的记录数。LIMIT 接受一个或两个数字参数。
            //参数必须是一个整数常量。如果给定两个参数，第一个参数指定第一个返回记录行的偏移量，第二个参数指定返回记录行的最大数目。
            //初始记录行的偏移量是 0(而不是 1)： 为了与 PostgreSQL 兼容，MySQL 也支持句法： LIMIT # OFFSET #。

            //mysql> SELECT * FROM table LIMIT 5,10; // 检索记录行 6-15  

            string temp = "SELECT {0} FROM {1} {2} {3} LIMIT {4},{5}";
            string sqlString = string.Format(temp,
                strFieldInfo,
                dbTranslator.Quote(entityInfo.TableName),
                strWhere,
                strOrder,
                range.StartIndex - 1,
                (range.EndIndex - range.StartIndex + 1));
            return new SqlStatement(System.Data.CommandType.Text, sqlString, dpc);
        }

        private static DataParameter getDataParameter(IPropertyMapper propertyMapper, object value)
        {
            return getDataParameter(propertyMapper, value, System.Data.ParameterDirection.Input);
        }
        private static DataParameter getDataParameter(IPropertyMapper propertyMapper, object value, System.Data.ParameterDirection pd)
        {
            return new DataParameter(propertyMapper.ColumnName, value, propertyMapper.DbType, propertyMapper.Size, pd);
        }
        private static string getFunctionSelectText(IDbTranslator dbTranslator, string fieldName, FieldFunction function)
        {
            string fileInfo = "";
            switch (function)
            {
                case FieldFunction.Average:
                    fileInfo = string.Format("AVG({0}{1}) AS MP", fieldName, dbTranslator.Quote(fieldName));
                    break;
                case FieldFunction.Sum:
                    fileInfo = string.Format("SUM({0}{1}) AS MP", fieldName, dbTranslator.Quote(fieldName));
                    break;
                case FieldFunction.Max:
                    fileInfo = string.Format("MAX({0}{1}) AS MP", fieldName, dbTranslator.Quote(fieldName));
                    break;
                case FieldFunction.Min:
                    fileInfo = string.Format("MIN({0}{1}) AS MP", fieldName, dbTranslator.Quote(fieldName));
                    break;
                case FieldFunction.Count:
                    fileInfo = "COUNT(1) AS MP";
                    break;
                default:
                    break;
            }
            return fileInfo;
        }
        /// <summary>
        /// 获得查询字段信息
        /// <para>1、如果有函数计算这返回函数计算命令，否则查询所有的字段</para>
        /// </summary>
        ///  <param name="dbTranslator"></param>
        ///  <param name="entityInfo"></param>
        ///  <param name="aliases">数据表的别名</param>
        /// <returns></returns>
        private static string getSelectText(IDbTranslator dbTranslator, IEntityMapper entityInfo, string aliases = "")
        {
            StringBuilder sBuilder = new StringBuilder();
            foreach (var item in entityInfo.PKeyProperties)
            {
                sBuilder.Append(aliases);
                sBuilder.Append(dbTranslator.Quote(item.ColumnName));
                sBuilder.Append(",");
            }
            foreach (var item in entityInfo.OtherProperties)
            {
                sBuilder.Append(aliases);
                sBuilder.Append(dbTranslator.Quote(item.ColumnName));
                sBuilder.Append(",");
            }
            return  sBuilder.ToString().TrimEnd(','); 
        }

        /// <summary>
        /// 获取排序信息
        /// </summary>
        /// <param name="aliases">数据表的别名</param>
        /// <param name="order"></param>
        /// <param name="setDefault">当未设置排序时，是否使用主键进行排序</param>
        /// <param name="reversion">是否反转排序状态</param>
        /// <returns></returns>
        private static string getOrderInfo(IDbTranslator dbTranslator, IEntityMapper entityInfo, Dictionary<string, FieldSearchOrder> order = null, string aliases = "", bool setDefault = false, bool reversion = false)
        {
            StringBuilder sBuilder = new StringBuilder();
            if (order != null)
            {
                foreach (var item in order)
                {
                    if (reversion)
                        sBuilder.Append(string.Format("{0}{1} {2}", aliases, dbTranslator.Quote(item.Key), item.Value == FieldSearchOrder.Ascending ? "ASC" : "DESC"));
                    else
                        sBuilder.Append(string.Format("{0}{1} {2}", aliases, dbTranslator.Quote(item.Key), item.Value == FieldSearchOrder.Ascending ? "DESC" : "ASC"));
                    sBuilder.Append(",");
                }
            }
            else
            {
                if (setDefault)
                {
                    //如果未设置排序信息，则默认按照主键排序
                    foreach (var item in entityInfo.PKeyProperties)
                    {
                        sBuilder.Append(string.Format("{0}{1} {2}", aliases, dbTranslator.Quote(item.ColumnName), reversion ? "DESC" : "ASC"));
                        sBuilder.Append(",");
                    }
                }
            }
            string txt = sBuilder.ToString();
            if (txt.Length > 0)
                txt = "ORDER BY " + txt.TrimEnd(',');
            return txt;
        }
    }
}
