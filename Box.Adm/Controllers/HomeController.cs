using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Box.Composition;
using Box.Composition.Web;
using Box.Composition.Services;
using System.ComponentModel.Composition;


namespace Box.Adm.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : Controller {

        [Import]
        private IPageModel PageModel { get; set; }
        
        [Authorize]
        public ActionResult Index() {
            return View(PageModel);
        }

    }
}
