using Maple.Core.Configuration;
using Maple.Core.Data;
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
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            var model = new InstallModel
            {
                AdminName = "admin@maple.com"
            };

            return View(model);
        }
    }
}
