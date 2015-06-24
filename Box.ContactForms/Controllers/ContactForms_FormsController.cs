using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.Composition.Web;

namespace Box.ContactForms.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContactForms_FormsController : Controller {

        [Import]
        private IPageModel PageModel { get; set; }

        [Authorize(Roles = "ADM_FORMS")]
        public ActionResult Index() {            
            return View(PageModel);
        }

    }
}
