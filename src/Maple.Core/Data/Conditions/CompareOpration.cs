using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/**
 * 创建人员：ANTHONY-PC.Anthony
 * 创建时间：2014/3/28 11:20:39
 * 功能说明： 
 * 
 * */

namespace Maple.Core.Data.Conditions
{
    [Serializable]
    public enum CompareOpration
    {
        //[ShowString(">")]
        GreatThan,
        //[ShowString("<")]
        LessThan,
        //[ShowString("=")]
        Equal,
        //[ShowString(">=")]
        GreatOrEqual,
        //[ShowString("<=")]
        LessOrEqual,
        //[ShowString("<>")]
        NotEqual,
        //[ShowString("LIKE")]
        Like,
        //[ShowString("IS")]
        Is,
        //[ShowString("IS NOT")]
        IsNot,
        //[ShowString("StartsWith")]
        StartsWith,
        //[ShowString("EndsWith")]
        EndsWith
    }
}
