//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;

//namespace Maple.Core.Data.Conditions
//{
//    [Serializable]
//    public class ConstCondition : Condition
//    {
//        private readonly string _condition;

//        internal ConstCondition(string condition)
//        {
//            this._condition = condition;
//        }

//        public override bool SubClauseNotEmpty
//        {
//            get { return true; }
//        }

//        public override string ToSqlText(IDataParameterCollection dpc, DbTranslator dd)
//        {
//            return _condition;
//        }
//    }
//}
