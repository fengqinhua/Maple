using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Configuration
{
    /// <summary>
    /// 定义宿主程序运行环境所需的配置参数
    /// </summary>
    public partial class HostingConfig
    {
        /// <summary>
        /// 获取或设置自定义获取客户端IP的HTTP头 (例如. CF-Connecting-IP, X-FORWARDED-PROTO, 等等)
        /// </summary>
        public string ForwardedHttpHeader { get; set; }

        /// <summary>
        /// 获取或设置是否启用HTTPS
        /// </summary>
        public bool UseHttpClusterHttps { get; set; }

        /// <summary>
        /// 获取或设置 是否使用了HTTP_X_FORWARDED_PROTO
        /// <para>记录一个请求一个请求最初从浏览器发出时候，是使用什么协议。因为有可能当一个请求最初和反向代理通信时，是使用https，但反向代理和服务器通信时改变成http协议，这个时候，X-Forwarded-Proto的值应该是https</para>
        /// </summary>
        public bool UseHttpXForwardedProto { get; set; }
    }
}
