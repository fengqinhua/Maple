﻿//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Maple.Core.Data.Conditions
//{
//    [Serializable]
//    public abstract class Condition
//    {

//        public static readonly Condition Empty = new EmptyCondition();
//        public static readonly ConstCondition True = new ConstCondition("(1=1)");
//        public static readonly ConstCondition False = new ConstCondition("(1<>1)");

//        internal Condition() { }

//        public abstract bool SubClauseNotEmpty { get; }

//        public static bool operator true(Condition kv)
//        {
//            return false;
//        }

//        public static bool operator false(Condition kv)
//        {
//            return false;
//        }

//        public static Condition operator &(Condition condition1, Condition condition2)
//        {
//            return GetConditionClause(condition1, condition2, new AndClause(condition1, condition2));
//        }

//        public Condition And(Condition condition2)
//        {
//            return GetConditionClause(this, condition2, new AndClause(this, condition2));
//        }

//        public static Condition operator |(Condition condition1, Condition condition2)
//        {
//            return GetConditionClause(condition1, condition2, new OrClause(condition1, condition2));
//        }

//        public Condition Or(Condition condition2)
//        {
//            return GetConditionClause(this, condition2, new OrClause(this, condition2));
//        }

//        private static Condition GetConditionClause(Condition condition1, Condition condition2, Condition notNullCondition)
//        {
//            if (IsNullOrEmpty(condition1) && IsNullOrEmpty(condition2))
//            {
//                return Empty;
//            }
//            if (!IsNullOrEmpty(condition1) && !IsNullOrEmpty(condition2))
//            {
//                return notNullCondition;
//            }
//            if (!IsNullOrEmpty(condition1))
//            {
//                return condition1;
//            }
//            return condition2;
//        }

//        public static Condition operator !(Condition condition)
//        {
//            if (IsNullOrEmpty(condition))
//            {
//                return Empty;
//            }
//            return new NotClause(condition);
//        }

//        public Condition Not()
//        {
//            if (IsNullOrEmpty(this))
//            {
//                return this;
//            }
//            return new NotClause(this);
//        }

//        private static bool IsNullOrEmpty(Condition condition)
//        {
//            return (condition == null || (condition is EmptyCondition));
//        }

//        public abstract string ToSqlText(DataParameterCollection dpc, DbTranslator dd);

//        protected virtual string GetValueString(DataParameterCollection dpc, DbTranslator dd, KeyValue kv)
//        {
//            if (kv.Value == null)
//                return "NULL";
//            string dpStr = string.Format("{0}_{1}", kv.Key, dpc.Count);
//            var dp = new DataParameter(dpStr, kv.NullableValue, kv.ValueType);
//            dpc.Add(dp);

//            return dd.QuoteParameter(dpStr);
//        }
//    }
//}
