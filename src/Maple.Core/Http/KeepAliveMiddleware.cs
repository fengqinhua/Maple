using Maple.Core.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Maple.Core.Http
{
    /// <summary>
    /// 表示检查请求是否保留的中间件
    /// </summary>
    public class KeepAliveMiddleware
    {
        #region 属性字段

        private readonly RequestDelegate _next;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next">Next</param>
        public KeepAliveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region 共有方法


        public async Task Invoke(HttpContext context, IWebHelper webHelper)
        {
            //TODO test. ensure that no guest record is created

            //判断数据库是否已经完成安装与配置
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //未完成则继续等待
                var keepAliveUrl = $"{webHelper.GetStoreLocation()}keepalive/index";
                if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                    return;
            }
            //进入下一个中间件
            await _next(context);
        }

        #endregion
    }
}
