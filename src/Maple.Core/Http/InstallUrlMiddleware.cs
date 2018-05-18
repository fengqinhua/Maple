using Maple.Core.Data.DataSettings;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Maple.Core.Http
{
    /// <summary>
    /// 用于检测是否已经初始化系统的中间件，如果未初始化将请求重定向至初始化页面
    /// </summary>
    public class InstallUrlMiddleware
    {
        #region 字段

        private readonly RequestDelegate _next;

        #endregion

        #region 构造函数

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="next">Next</param>
        public InstallUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region 共有方法

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <returns>Task</returns>
        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context, IWebHelper webHelper)
        {
            //判断数据库是否已安装和配置，如果没有，那么跳转至Install页面中
            if (!MainDataSettingsHelper.DatabaseIsInstalled())
            {
                var installUrl = $"{webHelper.GetTenantLocation()}install";
                if (!webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    //重定向
                    context.Response.Redirect(installUrl);
                    return;
                }
            }

            //如果已安装则进入下一个中间件
            await _next(context);
        }

        #endregion
    }
}
