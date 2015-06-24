using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.Composition.Web;
using System.Web.Mvc;

namespace Box.Core.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_LogsController : Controller {

        [Import]
        private IPageModel PageModel { get; set; }

        [Authorize]
        public ActionResult Index() {
            return View(PageModel);
        }
    }
}
