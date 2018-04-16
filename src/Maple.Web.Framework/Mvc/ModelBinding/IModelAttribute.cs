using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Web.Framework.Mvc.ModelBinding
{
    /// <summary>
    /// 自定义的属性标签，用于MVC中Model类（处理多语言）
    /// </summary>
    public interface IModelAttribute
    {
        /// <summary>
        /// 获取名称
        /// </summary>
        string Name { get; }
    }
}
