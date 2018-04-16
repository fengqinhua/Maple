using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Web.Framework.Mvc.Models
{
    /// <summary>
    /// MVC中Model的基类
    /// </summary>
    public class BaseMapleModel
    {
        public BaseMapleModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            this.PostInitialize();
        }
         
        /// <summary>
        /// 使用Key Value键值对存储任意自定义数据
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }

        #region 方法

        /// <summary>
        /// 执行绑定模型Model的附加操作
        /// </summary>
        /// <param name="bindingContext">Model binding context</param>
        /// <remarks></remarks>
        public virtual void BindModel(ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// 模型Model初始化的附加操作
        /// </summary>
        /// <remarks></remarks>
        protected virtual void PostInitialize()
        {
        }

        #endregion
    }
}
