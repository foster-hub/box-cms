using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.CMS;
using Box.CMS.Services;

namespace Box.CMS.Web {
    public abstract class ContentRenderMVCView : System.Web.Mvc.WebViewPage {

        public ContentHead HEAD = null;
        public string CONTENT_URL = null;
        public dynamic CONTENT = null;

        protected override void InitializePage() {
            base.InitializePage();
            string id = this.TempData["id"] as string;

            SiteService site = new Box.CMS.Services.SiteService();
            HEAD = site.GetContent(id);
            if (HEAD == null)
                throw new System.Web.HttpException(404, "page not found");

            site.LogPageView(id);

            CONTENT_URL = Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.Port!=80?":" + Request.Url.Port:"") + HEAD.Location+HEAD.CanonicalName;
            CONTENT = HEAD.CONTENT;
        }

    }
}
