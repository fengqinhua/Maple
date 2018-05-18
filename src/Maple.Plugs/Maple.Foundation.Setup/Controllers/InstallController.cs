using Maple.Core.Configuration;
using Maple.Core.Data.DataSettings;
using Maple.Foundation.Setup.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Foundation.Setup.Controllers
{
    public partial class InstallController : Controller
    {
        private readonly MapleConfig _config;

        public InstallController(MapleConfig config)
        {
            this._config = config;
        }

        public virtual IActionResult Index()
        {
            if (MainDataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");
            
            //string host = Core.Infrastructure.EngineContext.Current
            //    .Resolve<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
            //    .HttpContext?
            //    .Request?
            //    .Headers[Microsoft.Net.Http.Headers.HeaderNames.Host];

            var model = new InstallModel
            {
                AdminName = "admin@maple.com"
            };

            return View("~/Plugins/Maple.Foundation.Setup/Views/Install/Index.cshtml", model);
        }
    }
}
