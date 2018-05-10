using Maple.Core;
using Maple.Core.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Maple.Web.Framework.Globalization
{

    /// <summary>
    /// 处理多语言的中间件
    /// </summary>
    public class CultureMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="next">Next</param>
        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion
 

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Task</returns>
        public Task Invoke(Microsoft.AspNetCore.Http.HttpContext context)
        {
            CommonHelper.SetTelerikCulture();

            //call the next middleware in the request pipeline
            return _next(context);
        }

        #endregion
    }

    ///// <summary>
    ///// 处理多语言的中间件
    ///// </summary>
    //public class CultureMiddleware
    //{
    //    #region Fields

    //    private readonly RequestDelegate _next;

    //    #endregion

    //    #region Ctor

    //    /// <summary>
    //    /// Ctor
    //    /// </summary>
    //    /// <param name="next">Next</param>
    //    public CultureMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    #endregion

    //    #region Utilities

    //    /// <summary>
    //    /// Set working culture
    //    /// </summary>
    //    /// <param name="webHelper">Web helper</param>
    //    /// <param name="workContext">Work context</param>
    //    protected void SetWorkingCulture(IWebHelper webHelper, IWorkContext workContext)
    //    {
    //        if (!DataSettingsHelper.DatabaseIsInstalled())
    //            return;

    //        var adminAreaUrl = $"{webHelper.GetStoreLocation()}admin";
    //        if (webHelper.GetThisPageUrl(false).StartsWith(adminAreaUrl, StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            //we set culture of admin area to 'en-US' because current implementation of Telerik grid doesn't work well in other cultures
    //            //e.g., editing decimal value in russian culture
    //            CommonHelper.SetTelerikCulture();

    //            //set work context to admin mode
    //            workContext.IsAdmin = true;
    //        }
    //        else
    //        {
    //            //set working language culture
    //            var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
    //            CultureInfo.CurrentCulture = culture;
    //            CultureInfo.CurrentUICulture = culture;
    //        }
    //    }

    //    #endregion

    //    #region Methods

    //    /// <summary>
    //    /// Invoke middleware actions
    //    /// </summary>
    //    /// <param name="context">HTTP context</param>
    //    /// <param name="webHelper">Web helper</param>
    //    /// <param name="workContext">Work context</param>
    //    /// <returns>Task</returns>
    //    public Task Invoke(Microsoft.AspNetCore.Http.HttpContext context, IWebHelper webHelper, IWorkContext workContext)
    //    {
    //        //set culture
    //        SetWorkingCulture(webHelper, workContext);

    //        //call the next middleware in the request pipeline
    //        return _next(context);
    //    }

    //    #endregion
    //}
}
