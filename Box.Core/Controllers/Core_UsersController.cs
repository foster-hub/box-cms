using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.Composition.Web;

namespace Box.Core.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_UsersController : Controller {

        [Import]
        private Box.Core.Services.SecurityService service { get; set; }

        [Import]
        private IPageModel PageModel { get; set; }

        [Authorize(Roles="ADM_SEC")]
        public ActionResult Index() {
            ViewData["WindowsAuth"] = service.IsWindowsAuthEnable;
            return View(PageModel);
        }

        [Authorize]
        public ActionResult ChangePassword() {
            return View(PageModel);
        }

       

    }
}
