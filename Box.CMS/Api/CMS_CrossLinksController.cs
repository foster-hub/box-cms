using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using Box.CMS.Services;
using Box.Core.Services;

namespace Box.CMS.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_CrossLinksController : ApiController {

        [Import]
        private CMSService cms { get; set; }

        [Import]
        private LogService log { get; set; }

        
        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles="ADM_CMS"), HttpPost]
        /* Some enviroments does not supports HTTP VERB DELETE 
         * Use this workaround */
        public void Remove(string id, string area) {
            Delete(id, area);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_CMS")]
        public void Delete(string id, string area) {

            ContentHead head = cms.GetContentHead(id);
            cms.VerifyAuthorizationToEditContent(head.Kind);

            cms.RemoveCrossLink(id, area);

            log.Log(String.Format(SharedStringsLog.CROSSLINK_0_REMOVE_1, head.Kind, head.Name));
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_CMS")]
        public bool Put(string id, [FromBody] string area) {

            ContentHead head = cms.GetContentHead(id);
            cms.VerifyAuthorizationToEditContent(head.Kind);
            
            cms.VerifyAuthorizationToEditContent(head.Kind);

            return cms.AddCrossLink(id, area);
            
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_CMS"), HttpPost]
        /* Some enviroments does not supports HTTP VERB PUT 
         * Use this workaround */
        public bool UPDATE(string id, [FromBody] string area) {
            return Put(id, area);
        }


    }
}
