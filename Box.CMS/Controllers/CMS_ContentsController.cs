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
    public class CMS_ContentsController : Controller {

        [Import]
        private IPageModel PageModel { get; set; }

        [Import]
        private CMS.Services.CMSService cms { get; set; }

        [Import]
        private Core.Services.SecurityService security { get; set; }

        [Authorize]
        public ActionResult Index(string kind) {
            
            ContentKind ckind = cms.GetContentKind(kind);
            cms.VerifyAuthorizationToEditContent(ckind.Kind);

            Web.ContentPageModel model = new Web.ContentPageModel(PageModel, ckind, cms.CrossLinkAreas);

            //model.PreviewToken = security.GetSignedUserToken();

            if(!String.IsNullOrEmpty(ckind.CaptureListView))
                return View(ckind.CaptureListView, model);
            
            return View(model);
        }

        [Authorize]
        public RedirectResult Preview(string id)
        {
            string siteHost = cms.SiteHost;
            string token = security.GetSignedUserToken();
            ContentHead content = cms.GetContentHead(id);
            if (content == null)
                throw new System.Exception("Colud not preview content. Content '" + id + "' not found.");

            return new RedirectResult("http://"+siteHost + content.Location + content.CanonicalName + "?previewToken=" + token);
        }

    }
}
