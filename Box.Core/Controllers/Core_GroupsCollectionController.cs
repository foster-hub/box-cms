using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition.Web;

namespace Box.Core.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_GroupsCollectionController : Controller {

        [Import]
        private Box.Core.Services.SecurityService service { get; set; }

        [Import]
        private IPageModel PageModel { get; set; }

        [Authorize(Roles = "ADM_SEC")]
        public ActionResult Index() {
            return View(PageModel);
        }
    }
}
