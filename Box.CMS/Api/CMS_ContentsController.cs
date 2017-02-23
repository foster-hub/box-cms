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
    public class CMS_ContentsController : ApiController {

        [Import]
        private CMSService cms { get; set; }

        [Import]
        private LogService log { get; set; }


        [Box.Core.Web.WebApiAntiForgery]
        [Authorize, HttpGet]
        public List<string> SuggestedTags(string kind)
        {
            List<string> tags = cms.GetTagCloud(kind);
            
            cms.VerifyAuthorizationToEditContent(kind);

            return tags;
        }


        [Box.Core.Web.WebApiAntiForgery]
        [Authorize, HttpGet]        
        public ContentHead WithData(string id) {
            ContentHead head = cms.GetContent(id);
            cms.VerifyAuthorizationToEditContent(head.Kind);
            return head;
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public IEnumerable<ContentHead> Get(string filter = null, int skip = 0, int top = 0, string location = null, string kind = null, string order = "Date", DateTime? createdFrom = null, DateTime? createdTo = null, bool onlyPublished = false, string area = null) {

            IEnumerable<ContentHead> contents = null;

            if(!string.IsNullOrEmpty(area))
                contents = cms.GetCrossLinksFrom(area, top: top, order: order);
            else
                contents = cms.GetContents(filter, skip, top, location, new string[] { kind }, order, createdFrom, createdTo, false, onlyPublished);
            
            return cms.OnlyContentsUserCanEdit(contents);

        }
                
        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public ContentHead Post(ContentHead content, [FromUri] int publishNow = 0) {

            cms.VerifyAuthorizationToEditContent(content.Kind);

            content.ContentUId = Guid.NewGuid().ToString();
            content.CreateDate = DateTime.Now.ToUniversalTime();
            content.Data.ContentUId = content.ContentUId;
            FormatContentTags(content);
            FormatContentCrossLinks(content);
            FormatLocation(content);
            if (content.ContentDate == DateTime.MinValue)
                content.ContentDate = DateTime.Now.ToUniversalTime();

            if (publishNow > 0) {
                DateTime now = DateTime.Now.ToUniversalTime();
                content.PublishAfter = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            }

            SetCustomInfo(content);

            log.Log(String.Format(SharedStringsLog.CONTENT_0_CREATION_1, content.Kind, content.Name));

            return cms.SaveContent(content);
        }

        private void FormatLocation(ContentHead content) {
            content.Name = content.Name.Trim();
            content.CanonicalName = cms.CanonicalName(content.Name);
            if (!content.Location.EndsWith("/"))
                content.Location = content.Location + "/";
            content.Location = content.Location.ToLower();

            ContentHead contentWithSameUrl = cms.GetContentHeadByUrlAndKind(content.Location + content.CanonicalName, null, false);
            if (contentWithSameUrl == null || contentWithSameUrl.ContentUId == content.ContentUId)
                return;

            // if already exists a different content with the same url and browsable, throws an error
            ContentKind ckind = cms.GetContentKind(content.Kind);
            if (ckind.Browsable.HasValue && (bool)ckind.Browsable)
                content.CanonicalName = MakeCanonicalNameUnique(content);
        }

        private string MakeCanonicalNameUnique(ContentHead content) {
            string currentcanonicalname = content.CanonicalName += "/" + content.ContentDate.ToString("dd/MMM/yyyy");

            ContentHead contentWithSameUrl = cms.GetContentHeadByUrlAndKind(content.Location + content.CanonicalName, null, false);
            if (contentWithSameUrl == null || contentWithSameUrl.ContentUId == content.ContentUId)
                return currentcanonicalname;

            throw new HttpResponseException(System.Net.HttpStatusCode.Conflict);
        }

        private void FormatContentCrossLinks(ContentHead content) {
            foreach (CrossLink x in content.CrossLinks)
                x.ContentUId = content.ContentUId;
        }
        
        private void FormatContentTags(ContentHead content) {
            foreach (ContentTag t in content.Tags) {
                t.ContentUId = content.ContentUId;
                t.Tag = t.Tag.ToLower().Trim();
            }
            content.Tags =  content.Tags.Distinct().ToArray();
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public ContentHead Put(string id, ContentHead content, [FromUri] int publishNow = 0) {

            cms.VerifyAuthorizationToEditContent(content.Kind);
            content.Data.ContentUId = content.ContentUId;
            FormatContentCrossLinks(content);
            FormatContentTags(content);
            FormatLocation(content);

            if (publishNow > 0) {
                DateTime now = DateTime.Now.ToUniversalTime();
                content.PublishAfter = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            }

            SetCustomInfo(content);

            log.Log(String.Format(SharedStringsLog.CONTENT_0_UPDATE_1, content.Kind, content.Name));

            return cms.SaveContent(content);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize, HttpPost]
        /* Some enviroments does not supports HTTP VERB PUT 
         * Use this workaround */
        public ContentHead UPDATE(string id, ContentHead content, [FromUri] int publishNow = 0) {
            return Put(id, content, publishNow);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize, HttpPost]
        /* Some enviroments does not supports HTTP VERB DELETE 
         * Use this workaround */
        public void Remove(string id) {
            Delete(id);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public void Delete(string id) {

            ContentHead head = cms.GetContentHead(id);

            cms.VerifyAuthorizationToEditContent(head.Kind);

            cms.RemoveContent(id);

            log.Log(String.Format(SharedStringsLog.CONTENT_0_REMOVE_1, head.Kind, head.Name));
        }

        private void SetCustomInfo(ContentHead head) {
            if (head.CustomInfo == null)
                return;
            head.CustomInfo.ContentUId = head.ContentUId;
        }

    }
}
