using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.Composition.Web;


namespace Box.CMS.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_CrossLinksController : Controller {

        [Import]
        private IPageModel PageModel { get; set; }

        [Import]
        private CMS.Services.CMSService cms { get; set; }

        [Import]
        private Core.Services.SecurityService security { get; set; }

        [Authorize(Roles="ADM_CMS")]
        public ActionResult Index() {

            return View(PageModel);
        }

    }
}
