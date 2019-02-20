using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using Box.CMS.Services;
using Box.Core.Services;

namespace Box.CMS.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_PublishedContentsController : ApiController {

        [Import]
        private CMSService cms { get; set; }
        
        [Box.Core.Web.WebApiAntiForgery]        
        public IEnumerable<ContentHead> Get(string filter = null, int skip = 0, int top = 0, string location = null, [FromUri] string[] kinds = null, string order = "Date", DateTime? createdFrom = null, DateTime? createdTo = null, string area = null, [FromUri] string[] tags = null, bool includeData = false, string fromUrl = null) {

            IEnumerable<ContentHead> contents = null;
            Expression<Func<ContentHead, bool>> queryFilter = null;

            if (!String.IsNullOrEmpty(fromUrl))
            {
                var content = FromUrl(fromUrl, kinds!=null ? kinds.FirstOrDefault() : null);
                if (content != null)                
                    return new ContentHead[1] { content };                
                else
                    return new ContentHead[0];
            }

            if (tags[0] != null && tags.Length > 0)
                queryFilter = c => c.Tags.Any(t => tags.Contains(t.Tag));

            if (!string.IsNullOrEmpty(area))
                contents = cms.GetCrossLinksFrom(area, top: top, order: order, includeData: includeData);
            else
                contents = cms.GetContents(filter, skip, top, location, kinds, order, createdFrom, createdTo, includeData, true, queryFilter);

            return contents;
        }

        [Box.Core.Web.WebApiAntiForgery]
        public ContentHead Get(string id)
        {            
            return cms.GetContent(id, true);            
        }


        private ContentHead FromUrl(string url, string kind)
        {
            if (String.IsNullOrEmpty(url))
                return null;
            if (!url.StartsWith("/"))
                url = "/" + url;
            return cms.GetContentByUrlAndKind(url, kind, true);
        }
    }
}
