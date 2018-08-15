//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;

//namespace Maple.Core.Data.Conditions
//{
//    public class LambdaToSqlHelper
//    {
//        /// <summary>
//        /// NodeType枚举
//        /// </summary>
//        private enum EnumNodeType
//        {
//            /// <summary>
//            /// 二元运算符
//            /// </summary>
//            [Description("二元运算符")]
//            BinaryOperator = 1,

//            /// <summary>
//            /// 一元运算符
//            /// </summary>
//            [Description("一元运算符")]
//            UndryOperator = 2,

//            /// <summary>
//            /// 常量表达式
//            /// </summary>
//            [Description("常量表达式")]
//            Constant = 3,

//            /// <summary>
//            /// 成员（变量）
//            /// </summary>
//            [Description("成员（变量）")]
//            MemberAccess = 4,

//            /// <summary>
//            /// 函数
//            /// </summary>
//            [Description("函数")]
//            Call = 5,

//            /// <summary>
//            /// 未知
//            /// </summary>
//            [Description("未知")]
//            Unknown = -99,

//            /// <summary>
//            /// 不支持
//            /// </summary>
//            [Description("不支持")]
//            NotSupported = -98
//        }

//        /// <summary>
//        /// 判断表达式类型
//        /// </summary>
//        /// <param name="exp">lambda表达式</param>
//        /// <returns></returns>
//        private static EnumNodeType CheckExpressionType(Expression exp)
//        {
//            switch (exp.NodeType)
//            {
//                case ExpressionType.AndAlso:
//                case ExpressionType.OrElse:
//                case ExpressionType.Equal:
//                case ExpressionType.GreaterThanOrEqual:
//                case ExpressionType.LessThanOrEqual:
//                case ExpressionType.GreaterThan:
//                case ExpressionType.LessThan:
//                case ExpressionType.NotEqual:
//                    return EnumNodeType.BinaryOperator;
//                case ExpressionType.Constant:
//                    return EnumNodeType.Constant;
//                case ExpressionType.MemberAccess:
//                    return EnumNodeType.MemberAccess;
//                case ExpressionType.Call:
//                    return EnumNodeType.Call;
//                case ExpressionType.Not:
//                case ExpressionType.Convert:
//                    return EnumNodeType.UndryOperator;
//                default:
//                    return EnumNodeType.Unknown;
//            }
//        }

//        /// <summary>
//        /// 表达式类型转换
//        /// </summary>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        private static string ExpressionTypeCast(ExpressionType type)
//        {
//            switch (type)
//            {
//                case ExpressionType.And:
//                case ExpressionType.AndAlso:
//                    return " and ";
//                case ExpressionType.Equal:
//                    return " = ";
//                case ExpressionType.GreaterThan:
//                    return " > ";
//                case ExpressionType.GreaterThanOrEqual:
//                    return " >= ";
//                case ExpressionType.LessThan:
//                    return " < ";
//                case ExpressionType.LessThanOrEqual:
//                    return " <= ";
//                case ExpressionType.NotEqual:
//                    return " <> ";
//                case ExpressionType.Or:
//                case ExpressionType.OrElse:
//                    return " or ";
//                case ExpressionType.Add:
//                case ExpressionType.AddChecked:
//                    return " + ";
//                case ExpressionType.Subtract:
//                case ExpressionType.SubtractChecked:
//                    return " - ";
//                case ExpressionType.Divide:
//                    return " / ";
//                case ExpressionType.Multiply:
//                case ExpressionType.MultiplyChecked:
//                    return " * ";
//                default:
//                    return null;
//            }
//        }

//        private static string BinarExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            BinaryExpression be = exp as BinaryExpression;
//            Expression left = be.Left;
//            Expression right = be.Right;
//            ExpressionType type = be.NodeType;
//            string sb = "(";
//            //先处理左边
//            sb += ExpressionRouter(left, listSqlParaModel);
//            sb += ExpressionTypeCast(type);
//            //再处理右边
//            string sbTmp = ExpressionRouter(right, listSqlParaModel);
//            if (sbTmp == "null")
//            {
//                if (sb.EndsWith(" = "))
//                    sb = sb.Substring(0, sb.Length - 2) + " is null";
//                else if (sb.EndsWith(" <> "))
//                    sb = sb.Substring(0, sb.Length - 2) + " is not null";
//            }
//            else
//                sb += sbTmp;
//            return sb += ")";
//        }

//        private static string ConstantExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            ConstantExpression ce = exp as ConstantExpression;
//            if (ce.Value == null)
//            {
//                return "null";
//            }
//            else if (ce.Value is ValueType)
//            {
//                GetSqlParaModel(listSqlParaModel, GetValueType(ce.Value));
//                return "@para" + listSqlParaModel.Count;
//            }
//            else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
//            {
//                GetSqlParaModel(listSqlParaModel, GetValueType(ce.Value));
//                return "@para" + listSqlParaModel.Count;
//            }
//            return "";
//        }

//        private static string LambdaExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            LambdaExpression le = exp as LambdaExpression;
//            return ExpressionRouter(le.Body, listSqlParaModel);
//        }

//        private static string MemberExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            if (!exp.ToString().StartsWith("value"))
//            {
//                MemberExpression me = exp as MemberExpression;
//                if (me.Member.Name == "Now")
//                {
//                    GetSqlParaModel(listSqlParaModel, DateTime.Now);
//                    return "@para" + listSqlParaModel.Count;
//                }
//                return me.Member.Name;
//            }
//            else
//            {
//                var result = Expression.Lambda(exp).Compile().DynamicInvoke();
//                if (result == null)
//                {
//                    return "null";
//                }
//                else if (result is ValueType)
//                {
//                    GetSqlParaModel(listSqlParaModel, GetValueType(result));
//                    return "@para" + listSqlParaModel.Count;
//                }
//                else if (result is string || result is DateTime || result is char)
//                {
//                    GetSqlParaModel(listSqlParaModel, GetValueType(result));
//                    return "@para" + listSqlParaModel.Count;
//                }
//                else if (result is int[])
//                {
//                    var rl = result as int[];
//                    StringBuilder sbTmp = new StringBuilder();
//                    foreach (var r in rl)
//                    {
//                        GetSqlParaModel(listSqlParaModel, r.ToString().ToInt32());
//                        sbTmp.Append("@para" + listSqlParaModel.Count + ",");
//                    }
//                    return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
//                }
//                else if (result is string[])
//                {
//                    var rl = result as string[];
//                    StringBuilder sbTmp = new StringBuilder();
//                    foreach (var r in rl)
//                    {
//                        GetSqlParaModel(listSqlParaModel, r.ToString());
//                        sbTmp.Append("@para" + listSqlParaModel.Count + ",");
//                    }
//                    return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
//                }
//            }
//            return "";
//        }

//        private static string MethodCallExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            MethodCallExpression mce = exp as MethodCallExpression;
//            if (mce.Method.Name == "Contains")
//            {
//                if (mce.Object == null)
//                {
//                    return string.Format("{0} in ({1})", ExpressionRouter(mce.Arguments[1], listSqlParaModel), ExpressionRouter(mce.Arguments[0], listSqlParaModel));
//                }
//                else
//                {
//                    if (mce.Object.NodeType == ExpressionType.MemberAccess)
//                    {
//                        //w => w.name.Contains("1")
//                        var _name = ExpressionRouter(mce.Object, listSqlParaModel);
//                        var _value = ExpressionRouter(mce.Arguments[0], listSqlParaModel);
//                        var index = _value.RetainNumber().ToInt32() - 1;
//                        listSqlParaModel[index].value = "%{0}%".FormatWith(listSqlParaModel[index].value);
//                        return string.Format("{0} like {1}", _name, _value);
//                    }
//                }
//            }
//            else if (mce.Method.Name == "OrderBy")
//            {
//                return string.Format("{0} asc", ExpressionRouter(mce.Arguments[1], listSqlParaModel));
//            }
//            else if (mce.Method.Name == "OrderByDescending")
//            {
//                return string.Format("{0} desc", ExpressionRouter(mce.Arguments[1], listSqlParaModel));
//            }
//            else if (mce.Method.Name == "ThenBy")
//            {
//                return string.Format("{0},{1} asc", MethodCallExpressionProvider(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
//            }
//            else if (mce.Method.Name == "ThenByDescending")
//            {
//                return string.Format("{0},{1} desc", MethodCallExpressionProvider(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
//            }
//            else if (mce.Method.Name == "Like")
//            {
//                return string.Format("({0} like {1})", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel).Replace("'", ""));
//            }
//            else if (mce.Method.Name == "NotLike")
//            {
//                return string.Format("({0} not like '%{1}%')", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel).Replace("'", ""));
//            }
//            else if (mce.Method.Name == "In")
//            {
//                return string.Format("{0} in ({1})", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
//            }
//            else if (mce.Method.Name == "NotIn")
//            {
//                return string.Format("{0} not in ({1})", ExpressionRouter(mce.Arguments[0], listSqlParaModel), ExpressionRouter(mce.Arguments[1], listSqlParaModel));
//            }
//            return "";
//        }

//        private static string NewArrayExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            NewArrayExpression ae = exp as NewArrayExpression;
//            StringBuilder sbTmp = new StringBuilder();
//            foreach (Expression ex in ae.Expressions)
//            {
//                sbTmp.Append(ExpressionRouter(ex, listSqlParaModel));
//                sbTmp.Append(",");
//            }
//            return sbTmp.ToString(0, sbTmp.Length - 1);
//        }

//        private static string ParameterExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            ParameterExpression pe = exp as ParameterExpression;
//            return pe.Type.Name;
//        }

//        private static string UnaryExpressionProvider(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            UnaryExpression ue = exp as UnaryExpression;
//            var result = ExpressionRouter(ue.Operand, listSqlParaModel);
//            ExpressionType type = exp.NodeType;
//            if (type == ExpressionType.Not)
//            {
//                if (result.Contains(" in "))
//                {
//                    result = result.Replace(" in ", " not in ");
//                }
//                if (result.Contains(" like "))
//                {
//                    result = result.Replace(" like ", " not like ");
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// 路由计算
//        /// </summary>
//        /// <param name="exp"></param>
//        /// <param name="listSqlParaModel"></param>
//        /// <returns></returns>
//        private static string ExpressionRouter(Expression exp, List<SqlParaModel> listSqlParaModel)
//        {
//            var nodeType = exp.NodeType;
//            if (exp is BinaryExpression)    //表示具有二进制运算符的表达式
//            {
//                return BinarExpressionProvider(exp, listSqlParaModel);
//            }
//            else if (exp is ConstantExpression) //表示具有常数值的表达式
//            {
//                return ConstantExpressionProvider(exp, listSqlParaModel);
//            }
//            else if (exp is LambdaExpression)   //介绍 lambda 表达式。 它捕获一个类似于 .NET 方法主体的代码块
//            {
//                return LambdaExpressionProvider(exp, listSqlParaModel);
//            }
//            else if (exp is MemberExpression)   //表示访问字段或属性
//            {
//                return MemberExpressionProvider(exp, listSqlParaModel);
//            }
//            else if (exp is MethodCallExpression)   //表示对静态方法或实例方法的调用
//            {
//                return MethodCallExpressionProvider(exp, listSqlParaModel);
//            }
//            else if (exp is NewArrayExpression) //表示创建一个新数组，并可能初始化该新数组的元素
//            {
//                return NewArrayExpressionProvider(exp, listSqlParaModel);
//            }
//            else if (exp is ParameterExpression)    //表示一个命名的参数表达式。
//            {
//                return ParameterExpressionProvider(exp, listSqlParaModel);
//            }
//            else if (exp is UnaryExpression)    //表示具有一元运算符的表达式
//            {
//                return UnaryExpressionProvider(exp, listSqlParaModel);
//            }
//            return null;
//        }

//        /// <summary>
//        /// 值类型转换
//        /// </summary>
//        /// <param name="_value"></param>
//        /// <returns></returns>
//        private static object GetValueType(object _value)
//        {
//            var _type = _value.GetType().Name;
//            switch (_type)
//            {
//                case "Decimal ": return _value.ToDecimal();
//                case "Int32": return _value.ToInt32();
//                case "DateTime": return _value.ToDateTime();
//                case "String": return _value.ToString();
//                case "Char": return _value.ToChar();
//                case "Boolean": return _value.ToBoolean();
//                default: return _value;
//            }
//        }

//        /// <summary>
//        /// sql参数
//        /// </summary>
//        /// <param name="listSqlParaModel"></param>
//        /// <param name="val"></param>
//        private static void GetSqlParaModel(List<SqlParaModel> listSqlParaModel, object val)
//        {
//            SqlParaModel p = new SqlParaModel();
//            p.name = "para" + (listSqlParaModel.Count + 1);
//            p.value = val;
//            listSqlParaModel.Add(p);
//        }

//        /// <summary>
//        /// lambda表达式转换sql
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="listSqlParaModel"></param>
//        /// <returns></returns>
//        public static string GetWhereSql<T>(Expression<Func<T, bool>> where, List<SqlParaModel> listSqlParaModel) where T : class
//        {
//            string result = string.Empty;
//            if (where != null)
//            {
//                Expression exp = where.Body as Expression;
//                result = ExpressionRouter(exp, listSqlParaModel);
//            }
//            if (result != string.Empty)
//            {
//                result = " where " + result;
//            }
//            return result;
//        }

//        /// <summary>
//        /// lambda表达式转换sql
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="orderBy"></param>
//        /// <returns></returns>
//        public static string GetOrderBySql<T>(Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderBy) where T : class
//        {
//            string result = string.Empty;
//            if (orderBy != null && orderBy.Body is MethodCallExpression)
//            {
//                MethodCallExpression exp = orderBy.Body as MethodCallExpression;
//                List<SqlParaModel> listSqlParaModel = new List<SqlParaModel>();
//                result = MethodCallExpressionProvider(exp, listSqlParaModel);
//            }
//            if (result != string.Empty)
//            {
//                result = " order by " + result;
//            }
//            return result;
//        }

//        /// <summary>
//        /// lambda表达式转换sql
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="fields"></param>
//        /// <returns></returns>
//        public static string GetQueryField<T>(Expression<Func<T, object>> fields)
//        {
//            StringBuilder sbSelectFields = new StringBuilder();
//            if (fields.Body is NewExpression)
//            {
//                NewExpression ne = fields.Body as NewExpression;
//                for (var i = 0; i < ne.Members.Count; i++)
//                {
//                    sbSelectFields.Append(ne.Members[i].Name + ",");
//                }
//            }
//            else if (fields.Body is ParameterExpression)
//            {
//                sbSelectFields.Append("*");
//            }
//            else
//            {
//                sbSelectFields.Append("*");
//            }
//            if (sbSelectFields.Length > 1)
//            {
//                sbSelectFields = sbSelectFields.Remove(sbSelectFields.Length - 1, 1);
//            }
//            return sbSelectFields.ToString();
//        }

//    }
//}
