using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition.Web;

namespace Box.People.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class People_GroupController : Controller {
        
        [Import]
        private IPageModel PageModel { get; set; }

        [Authorize(Roles = "ADM_PEOPLE")]
        public ActionResult Index() {
            return View(PageModel);
        }
    }
}
