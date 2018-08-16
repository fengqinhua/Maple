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
            return null;
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
            return null;
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
            return null;
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
    }
}
