using Maple.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Foundation.Setup.Models
{
    public class InstallModel : BaseMapleModel
    {
        /// <summary>
        /// 超级管理员名称
        /// </summary>
        public string AdminName { get; set; }
        /// <summary>
        /// 超级管理密码
        /// </summary>
        public string AdminPassword { get; set; }
        /// <summary>
        /// 超级管理员密码
        /// </summary>
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DatabaseConnectionString { get; set; }
    }
}
