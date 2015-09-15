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
    public class CMS_PublishedContentsController : ApiController {

        [Import]
        private CMSService cms { get; set; }
        
        [Box.Core.Web.WebApiAntiForgery]        
        public IEnumerable<ContentHead> Get(string filter = null, int skip = 0, int top = 0, string location = null, [FromUri] string[] kinds = null, string order = "Date", DateTime? createdFrom = null, DateTime? createdTo = null, string area = null) {

            IEnumerable<ContentHead> contents = null;

            if(!string.IsNullOrEmpty(area))
                contents = cms.GetCrossLinksFrom(area, top: top);
            else
                contents = cms.GetContents(filter, skip, top, location, kinds, order, createdFrom, createdTo, false, true);

            return contents;
        }
    }
}
