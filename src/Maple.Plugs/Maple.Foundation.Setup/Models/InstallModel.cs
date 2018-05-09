using Maple.Web.Framework.Mvc.ModelBinding;
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
        [MapleResourceDisplayName("Maple.Foundation.Setup.Models.InstallModel.AdminName")]
        public string AdminName { get; set; }
        /// <summary>
        /// 超级管理密码
        /// </summary>
        [MapleResourceDisplayName("Maple.Foundation.Setup.Models.InstallModel.AdminPassword")]
        public string AdminPassword { get; set; }
        /// <summary>
        /// 超级管理员密码
        /// </summary>
        [MapleResourceDisplayName("Maple.Foundation.Setup.Models.InstallModel.ConfirmPassword")]
        [NoTrim]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [MapleResourceDisplayName("Maple.Foundation.Setup.Models.InstallModel.DatabaseConnectionString")]
        public string DatabaseConnectionString { get; set; }
    }
}
