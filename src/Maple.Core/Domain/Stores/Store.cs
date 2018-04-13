using Maple.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Stores
{
    /// <summary>
    /// 聚合跟：站点信息 
    /// </summary>
    public partial class Store : Entity, ILocalizedEntity
    {
        /// <summary>
        /// 站点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 站点地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否启动了SSL
        /// </summary>
        public bool SslEnabled { get; set; }

        /// <summary>
        /// 站点地址 (HTTPS)
        /// </summary>
        public string SecureUrl { get; set; }

        /// <summary>
        /// 主机列表（多个用逗号 "," 隔开）
        /// </summary>
        public string Hosts { get; set; }

        /// <summary>
        /// 站点默认的语言标识
        /// </summary>
        public int DefaultLanguageId { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string CompanyAddress { get; set; }

        /// <summary>
        /// 公司联系电话
        /// </summary>
        public string CompanyPhoneNumber { get; set; }
    }
}
