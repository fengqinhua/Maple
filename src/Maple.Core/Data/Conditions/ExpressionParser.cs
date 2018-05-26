﻿using Maple.Core.Data.DbMappers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using Maple.Core.Data.DbTranslators;
using System.Data;
using System.Collections;
using System.Reflection;

namespace Maple.Core.Data.Conditions
{
    public class ExpressionParser : IExpressionParser
    {
        public IEntityMapper _entityInfo;

        public ExpressionParser(IEntityMapper entityInfo)
        {
            this._entityInfo = entityInfo;
        }

        /// <summary>
        /// 将用于查询的拉曼达表达式转换为SQL查询片段
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="dbTranslator"></param>
        /// <param name="dpc"></param>
        /// <returns></returns>
        public string ToSQL(Expression expr, IDbTranslator dbTranslator, DataParameterCollection dpc)
        {
            if (dpc == null)
                dpc = new DataParameterCollection();

            StringBuilder sBuilder = new StringBuilder();
            this.parse(expr, dbTranslator, sBuilder, dpc);
            return sBuilder.ToString();
        }


        /// <summary>
        /// 执行翻译工作
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="dbTranslator"></param>
        /// <param name="sBuilder"></param>
        /// <param name="dpc"></param>
        private void parse(Expression expr, IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            if (expr is BinaryExpression)
                this.parseBinaryExpression((BinaryExpression)expr, dbTranslator, sBuilder, dpc);
            else if (expr is MethodCallExpression)
                this.parseMethodCallExpression((MethodCallExpression)expr, dbTranslator, sBuilder, dpc);
            else if (expr is UnaryExpression)
                this.parseUnaryExpression((UnaryExpression)expr, dbTranslator, sBuilder, dpc);
            else if (expr is System.Linq.Expressions.ConstantExpression)
            {
                System.Linq.Expressions.ConstantExpression cExp = expr as System.Linq.Expressions.ConstantExpression;

                bool cExpValue = false;
                if (cExp.Value != null)
                    if (!bool.TryParse(cExp.Value.ToString(), out cExpValue))
                        cExpValue = true;
                if (cExpValue)
                    sBuilder.Append("(1=1)");
                else
                    sBuilder.Append("(1<>1)");
            }
            else
            {
                if (isBooleanFieldOrProperty(expr))
                {
                    IPropertyMapper propertyMapper = this.getPropertyMapper(((MemberExpression)expr).Member.Name);
                    //生成SQL片段和参数
                    this.buildSqlAndDataParameter(propertyMapper, ColumnFunction.None, CompareOpration.Equal, true, dbTranslator, sBuilder, dpc);
                }
                else
                    throw new Exception("该操作不支持！" + expr.Type.FullName);
            }
        }

        /// <summary>
        /// 翻译一元运算符的表达式
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="dbTranslator"></param>
        /// <param name="sBuilder"></param>
        /// <param name="dpc"></param>
        private void parseUnaryExpression(UnaryExpression expr, IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Not:
                    {
                        sBuilder.Append("( NOT (");
                        this.parse(expr.Operand, dbTranslator, sBuilder, dpc);
                        sBuilder.Append(") )");
                        break;
                    }
                default:
                    throw new Exception("该操作不支持！" + expr.Type.FullName);
            }
        }
        /// <summary>
        /// 翻译二元运算符表达式
        /// </summary>
        /// <param name="e"></param>
        /// <param name="dbTranslator"></param>
        /// <param name="sBuilder"></param>
        /// <param name="dpc"></param>
        private void parseBinaryExpression(BinaryExpression e, IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                    this.parseClause(e, CompareOpration.Equal, dbTranslator, sBuilder, dpc);
                    break;
                case ExpressionType.GreaterThan:
                    this.parseClause(e, CompareOpration.GreatThan, dbTranslator, sBuilder, dpc);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.parseClause(e, CompareOpration.GreatOrEqual, dbTranslator, sBuilder, dpc);
                    break;
                case ExpressionType.LessThan:
                    this.parseClause(e, CompareOpration.LessThan, dbTranslator, sBuilder, dpc);
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.parseClause(e, CompareOpration.LessOrEqual, dbTranslator, sBuilder, dpc);
                    break;
                case ExpressionType.NotEqual:
                    this.parseClause(e, CompareOpration.NotEqual, dbTranslator, sBuilder, dpc);
                    break;
                case ExpressionType.AndAlso:
                    {
                        sBuilder.Append("(");
                        this.parse(e.Left, dbTranslator, sBuilder, dpc);
                        sBuilder.Append(") AND (");
                        this.parse(e.Right, dbTranslator, sBuilder, dpc);
                        sBuilder.Append(")");
                        break;
                    }
                case ExpressionType.OrElse:
                    {
                        sBuilder.Append("(");
                        this.parse(e.Left, dbTranslator, sBuilder, dpc);
                        sBuilder.Append(") OR (");
                        this.parse(e.Right, dbTranslator, sBuilder, dpc);
                        sBuilder.Append(")");
                        break;
                    }
                default:
                    throw new Exception("该操作不支持！" + e.NodeType);
            }
        }
        /// <summary>
        /// 翻译对静态方法或实例方法的调用
        /// </summary>
        /// <param name="e"></param>
        /// <param name="dbTranslator"></param>
        /// <param name="sBuilder"></param>
        /// <param name="dpc"></param>
        private void parseMethodCallExpression(MethodCallExpression e, IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            switch (e.Method.Name)
            {
                case "StartsWith":
                     this.parseLikeCall(e, CompareOpration.StartsWith, dbTranslator, sBuilder, dpc);
                    break;
                case "EndsWith":
                    this.parseLikeCall(e, CompareOpration.EndsWith, dbTranslator, sBuilder, dpc);
                    break;
                case "Contains":
                    this.parseLikeCall(e, CompareOpration.Like, dbTranslator, sBuilder, dpc);
                    break;
                case "In":
                case "InStatement":
                    this.parseInCall(e, false, dbTranslator, sBuilder, dpc);
                    break;
                case "NotIn":
                case "NotInStatement":
                    this.parseInCall(e, true, dbTranslator, sBuilder, dpc);
                    break;
                case "IsNull":
                    this.parseNull(e, true, dbTranslator, sBuilder, dpc);
                    break;
                case "IsNotNull":
                    this.parseNull(e, false, dbTranslator, sBuilder, dpc);
                    break;
                default:
                    throw new Exception("无法处理的函数：" + e.Method.Name);
            }
        }


        private void parseNull(MethodCallExpression e, bool isNull, IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            IPropertyMapper propertyMapper = this.getPropertyMapper(e.Arguments[0], out _, out _);

            CompareOpration co = isNull ? CompareOpration.Equal : CompareOpration.NotEqual;

            this.buildSqlAndDataParameter(propertyMapper, ColumnFunction.None, co, null, dbTranslator, sBuilder, dpc);
        }
        private void parseInCall(MethodCallExpression e, bool notIn , IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            ColumnFunction function;
            MemberExpression member;
            IPropertyMapper propertyMapper = this.getPropertyMapper(e.Arguments[0], out function, out member);
            
            var ie = this.getRightValue(e.Arguments[1]);
            if (ie is IEnumerable)
            {
                sBuilder.Append(propertyMapper.ColumnName);
                if (notIn)
                    sBuilder.Append(" NOT");
                sBuilder.Append(" IN (");

                IEnumerable list = (IEnumerable)ie;
                string prefix = "";
                foreach (var obj in (IEnumerable)ie)
                {
                    if (prefix.Length == 0)
                        prefix = ",";
                    else
                        sBuilder.Append(prefix);
                    sBuilder.Append(obj);
                }
                sBuilder.Append(")");
            }
            else
            {
                CompareOpration co = notIn ? CompareOpration.NotEqual : CompareOpration.Equal;
                this.buildSqlAndDataParameter(propertyMapper, ColumnFunction.None, co, ie, dbTranslator, sBuilder, dpc);
            }

            //foreach (var o in _args)
            //{
            //    var v = GetValueString(dpc, dd, new KeyValue("in", o));
            //    sb.Append(v);
            //    sb.Append(",");
            //}
            //if (_args.Length > 0)
            //{
            //    sb.Length--;
            //}


     

            //var list = new List<object>();
            
   
            //return new InClause(key, list.ToArray(), notIn);
        }
        private void parseLikeCall(MethodCallExpression e, CompareOpration co, IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            ColumnFunction function;
            MemberExpression member;
            IPropertyMapper propertyMapper = this.getPropertyMapper(e.Object, out function, out member);

            if (e.Arguments.Count == 1)
            {
                object value = this.getRightValue(e.Arguments[0]);
                if (value != null && value is string)
                {
                    this.buildSqlAndDataParameter(propertyMapper, function, co, value, dbTranslator, sBuilder, dpc);
                }
            }
            throw new Exception("'Like' clause only supported one Parameter and the Parameter should be string and not allow NULL.！");
        }
        private void parseClause(BinaryExpression e, CompareOpration co, IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            if (e.Right.NodeType == ExpressionType.MemberAccess)
                throw new Exception("该操作不支持！Right.NodeType == ExpressionType.MemberAccess");

            ColumnFunction function;
            MemberExpression left;
            //获得左侧的属性字段信息
            IPropertyMapper propertyMapper = this.getPropertyMapper(e.Left, out function, out left);
             //获取右侧的值
            object value = this.getRightValue(e.Right);
            //生成SQL片段和参数
            this.buildSqlAndDataParameter(propertyMapper, function, co, value, dbTranslator, sBuilder, dpc);
        }
        /// <summary>
        /// 生成SQL片段和参数
        /// </summary>
        /// <param name="propertyMapper"></param>
        /// <param name="function"></param>
        /// <param name="co"></param>
        /// <param name="value"></param>
        /// <param name="dbTranslator"></param>
        /// <param name="sBuilder"></param>
        /// <param name="dpc"></param>
        private void buildSqlAndDataParameter(IPropertyMapper propertyMapper, ColumnFunction function, CompareOpration co, object value,
            IDbTranslator dbTranslator, StringBuilder sBuilder, DataParameterCollection dpc)
        {
            //获取参数名称
            string strKey = dbTranslator.Quote(string.Format("{0}_{1}", propertyMapper.ColumnName, dpc.Count));
            //拼接SQL 1
            switch (function)
            {
                //处理函数
                case ColumnFunction.ToLower:
                    sBuilder.Append(string.Format("LOWER({0})", propertyMapper.ColumnName));
                    break;
                case ColumnFunction.ToUpper:
                    sBuilder.Append(string.Format("UPPER({0})", propertyMapper.ColumnName));
                    break;
                default:
                    sBuilder.Append(propertyMapper.ColumnName);
                    break;
            }
            //拼接SQL 2
            if (value == null)
            {
                if (co == CompareOpration.Equal)
                    sBuilder.Append(" IS NULL ");
                else if (co == CompareOpration.NotEqual)
                    sBuilder.Append(" IS NOT NULL ");
                else
                    throw new Exception("该操作不支持！NULL value only supported Equal and NotEqual!");
            }
            else
            {
                switch (co)
                {
                    case CompareOpration.Equal:
                        sBuilder.Append(" = ");
                        sBuilder.Append(strKey);
                        break;
                    case CompareOpration.GreatOrEqual:
                        sBuilder.Append(" >= ");
                        sBuilder.Append(strKey);
                        break;
                    case CompareOpration.GreatThan:
                        sBuilder.Append(" > ");
                        sBuilder.Append(strKey);
                        break;
                    case CompareOpration.LessOrEqual:
                        sBuilder.Append(" <= ");
                        sBuilder.Append(strKey);
                        break;
                    case CompareOpration.LessThan:
                        sBuilder.Append(" < ");
                        sBuilder.Append(strKey);
                        break;
                    case CompareOpration.Like:
                        sBuilder.Append(" LIKE ");
                        sBuilder.Append(strKey);
                        break;
                    case CompareOpration.StartsWith:
                        sBuilder.Append(" LIKE ");
                        sBuilder.Append(strKey);
                        value = string.Format("{0}%", value);
                        break;
                    case CompareOpration.EndsWith:
                        sBuilder.Append(" LIKE ");
                        sBuilder.Append(strKey);
                        value = string.Format("%{0}", value);
                        break;
                    case CompareOpration.NotEqual:
                        sBuilder.Append(" <> ");
                        sBuilder.Append(strKey);
                        value = string.Format("%{0}%", value);
                        break;
                    default:
                        throw new Exception("该操作不支持！CompareOpration = " + co);
                }

                //添加查询参数
                dpc.Add(new DataParameter(propertyMapper.ColumnName, value, propertyMapper.PropertyInfo.PropertyType,
                    propertyMapper.DbType, propertyMapper.Size, ParameterDirection.Input));
            }

        }
        private IPropertyMapper getPropertyMapper(Expression expr, out ColumnFunction function, out MemberExpression obj)
        {
            if (expr.NodeType == ExpressionType.Convert)
                expr = ((UnaryExpression)expr).Operand;

            if (expr is MemberExpression)
            {
                function = ColumnFunction.None;
                obj = (MemberExpression)expr;
                return getPropertyMapper(obj);
            }

            if (expr is MethodCallExpression)
            {
                var e = (MethodCallExpression)expr;
                if (e.Method.Name == "ToLower" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToLower;
                    obj = (MemberExpression)e.Object;
                    return getPropertyMapper(obj);
                }
                if (e.Method.Name == "ToUpper" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToUpper;
                    obj = (MemberExpression)e.Object;
                    return getPropertyMapper(obj);
                }
            }
            throw new Exception("The expression must be 'Column op const' or 'Column op Column'");
        }
        private IPropertyMapper getPropertyMapper(MemberExpression expr)
        {
            string mn = expr.Member.Name;
            if (expr.Expression is MemberExpression && mn == "Id")
                mn = ((MemberExpression)expr.Expression).Member.Name;
            return getPropertyMapper(mn);
        }
        private IPropertyMapper getPropertyMapper(string columnName)
        {
            IPropertyMapper propertyMapper = this._entityInfo.OtherProperties.FirstOrDefault(f => f.PropertyInfo.Name == columnName);
            if (propertyMapper == null)
                propertyMapper = this._entityInfo.PKeyProperties.FirstOrDefault(f => f.PropertyInfo.Name == columnName);
            if (propertyMapper == null)
                throw new Exception(string.Format("对象{0}中字段{1}未查找到数据库映射的字段！", this._entityInfo.TableName, columnName));
            else
                return propertyMapper;

        }
        private object getRightValue(Expression right)
        {
            object value
                = right.NodeType == ExpressionType.Constant
                      ? ((ConstantExpression)right).Value
                      : Expression.Lambda(right).Compile().DynamicInvoke();

            //else if (Right.NodeType == ExpressionType.Convert
            //    || Right.NodeType == ExpressionType.MemberAccess)
            //{
            //    value = Expression.Lambda(Right).Compile().DynamicInvoke();
            //}
            //else
            //{
            //    throw new LinqException("Unsupported expression.");
            //}

            return value;
        }
        private bool isBooleanFieldOrProperty(Expression expr)
        {
            if (expr is MemberExpression)
            {
                var member = ((MemberExpression)expr);
                if (member.Member.MemberType == MemberTypes.Field || member.Member.MemberType == MemberTypes.Property)
                {
                    if (member.Type == typeof(bool))
                        return true;
                }
            }
            return false;
        }

    }
}
