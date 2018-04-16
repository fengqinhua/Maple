using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// 自定义属性，表示字符型字段赋值前不要执行Trim操作
    /// </summary>
    public class NoTrimAttribute : Attribute
    {
    }
}
