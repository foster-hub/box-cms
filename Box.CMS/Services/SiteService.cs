using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Box.CMS.Services {

    public class SiteService {

        private CMSService cms = null;

        public SiteService() {
            cms = new CMSService();
        }

        public bool DebugCMS {
            get {
                string debugStr = System.Configuration.ConfigurationManager.AppSettings["DEBUG_CMS"] as String;                
                if (debugStr == null)
                    debugStr = System.Configuration.ConfigurationManager.AppSettings["BOX_DEBUG_ON"] as String;
                if (debugStr == null)
                    return false;
                bool debug = false;
                bool.TryParse(debugStr, out debug);
                return debug;
            }
        }

        public bool IgnoreVirtualAppPath {
            get {
                string ignoreStr = System.Configuration.ConfigurationManager.AppSettings["IGNORE_VIRTUAL_APP_PATH"] as String;
                if (ignoreStr == null)
                    return false;
                bool ignore = false;
                bool.TryParse(ignoreStr, out ignore);
                return ignore;
            }
        }

        public ContentHead GetContent(string contentUId) {
            
            ContentHead content = cms.GetContent(contentUId, false);
            if(content==null)
                return null;

            cms.ParseContentData(content);
            return content;
        }

        public IEnumerable<ContentHead> GetContents(string url, string order = "Date", string[] kinds = null, DateTime? createdFrom = null, DateTime? createdTo = null, bool parseContent = false, int skip = 0, int top = 0, string filter = null, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null) {
            bool onlyPublished = !CanSeeUnpublishedContents();
            IEnumerable<ContentHead> contents = cms.GetContents(filter, skip, top, url, kinds, order, createdFrom, createdTo, parseContent, onlyPublished, queryFilter);
            
            if (parseContent) 
                foreach (ContentHead c in contents) 
                    cms.ParseContentData(c);
            
            return contents;
        }

        private bool CanSeeUnpublishedContents() {
            
            var context = System.Web.HttpContext.Current;
            if (context == null)
                return false;
            
            string token = context.Request.QueryString["previewToken"];
            if (String.IsNullOrEmpty(token))
                return false;

            Core.Services.SecurityService security = new Core.Services.SecurityService();
            var admCMS = new Groups.ADM_CMS();
            var admSEC = new Core.Groups.ADM_SEC();

            Core.User u = security.GetUserByAuthToken(token);
            if (u == null)
                return false;

            string[] roles = security.GetUserRoles(u);
            return roles.Contains(admCMS.UserGroupUId) || roles.Contains(admSEC.UserGroupUId);            

        }

        public DateTime? GetLastPublishDate(string location, string[] kinds) {
            return cms.GetLastPublishDate(location, kinds);
        }

        public IEnumerable<ContentHead> GetCrossLinksFrom(string pageArea, string order = "CrossLinkDisplayOrder", int top = 0, string[] kinds = null, bool parseContent = false, string[] pageAreaFallBacks = null) {

            bool onlyPublished = !CanSeeUnpublishedContents();

            IEnumerable<ContentHead> contents = cms.GetCrossLinksFrom(pageArea, order, top, kinds, parseContent, pageAreaFallBacks, onlyPublished);
            
            if (parseContent)
                foreach (ContentHead c in contents)
                    cms.ParseContentData(c);
            
            return contents;
        }

        public IEnumerable<ContentHead> GetRelatedContent(string id, int top, string location, string[] kinds, bool includeData = false) {
            return cms.GetRelatedContent(id, top, location, kinds, includeData);
        }

        public ContentHead GetHotestContent(string[] kinds, string location, ContentRanks rankBy = ContentRanks.PageViews, DateTime? createdFrom = null, DateTime? createdTo = null) {
            return cms.GetHotestContent(kinds, location, rankBy, createdFrom, createdTo);
        }

        public IEnumerable<ContentHead> GetRelatedContent(string[] tags, int top, string location, string[] kinds, bool includeData = false) {
            return cms.GetRelatedContent(tags, top, location, kinds, includeData);
        }


        public void LogPageShare(ContentHead content, string serverHost, bool logShare = true, bool logComments = false) {

            string url = serverHost + content.Location + content.CanonicalName;

            string appDataPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");

            System.Net.Http.HttpClient wc = new System.Net.Http.HttpClient();

            wc.GetAsync("https://graph.facebook.com/?id=" + url)
                .ContinueWith(task => {

                    if (task.IsFaulted && DebugCMS) {
                        string filePath = appDataPath + "\\logShareError" + DateTime.Now.ToString().Replace("/", ".").Replace(":", ".") + ".txt";
                        using (StreamWriter sw = System.IO.File.CreateText(filePath)) {
                            sw.WriteLine("https://graph.facebook.com/?id=" + url);
                            sw.WriteLine("Exception: " + task.Exception.Message);
                            if(task.Exception.InnerException!=null)
                                sw.WriteLine("InnerException: " + task.Exception.InnerException.Message);
                            if (task.Exception.InnerException.InnerException != null)
                                sw.WriteLine("InnerException 2: " + task.Exception.InnerException.InnerException.Message);
                        }
                    }

                    if (task.IsFaulted || task.IsCanceled)
                        return;
                    var msg = task.Result;
                    dynamic data = null;

                    if (msg.StatusCode != System.Net.HttpStatusCode.OK)
                        return;

                    string json = msg.Content.ReadAsStringAsync().Result;
                    data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                    if (logShare) {
                        if (data.shares == null)
                            UpdateShareCount(content.ContentUId, 0);
                        else
                            UpdateShareCount(content.ContentUId, (long)data.shares);
                    }

                    if (logComments) {
                        if (data.comments == null)
                            UpdateCommentCount(content.ContentUId, 0);
                        else
                            UpdateCommentCount(content.ContentUId, (int)data.comments);
                    }

                });               
        }

        private void UpdateShareCount(string id, long count) {

            using (var context = new Data.CMSContext()) {
                ContentShareCount oldCount = context.ContentSharesCounts.SingleOrDefault(c => c.ContentUId == id);
                if (oldCount == null)
                    context.ContentSharesCounts.Add(new ContentShareCount() { ContentUId = id, Count = count });
                else
                    oldCount.Count = count;
                context.SaveChanges();
            }
        }


        public void UpdateCommentCount(string id, int count) {
            using (var context = new Data.CMSContext()) {
                ContentCommentCount oldCount = context.ContentCommentCounts.SingleOrDefault(c => c.ContentUId == id);
                if (oldCount == null)
                    context.ContentCommentCounts.Add(new ContentCommentCount() { ContentUId = id, Count = count });
                else
                    oldCount.Count = count;
                context.SaveChanges();
            }

        }

        public string GetFileName(string fileUId) {
            return cms.GetFile(fileUId, false).FileName;
        }


        public void ResponseFile(System.Web.HttpResponse response, string fileUId) {
            Box.CMS.File file = cms.GetFile(fileUId);
            if (file == null)
                response.End();

            response.ContentType = file.Type;
            response.BinaryWrite(file.Data.StoredData);
            response.End();
        }

        public void LogPageView(string contentUId) {
            
            using (var context = new Data.CMSContext()) {
                ContentPageViewCount pageCount = context.ContentPageViewCounts.SingleOrDefault(c => c.ContentUId == contentUId);
                if (pageCount == null)
                    context.ContentPageViewCounts.Add(pageCount = new ContentPageViewCount() { ContentUId = contentUId, Count = 0 });
                
                pageCount.Count++;

                context.SaveChanges();
            }
        }

        public ContentTagRank[] GetTagCloud(string location, string[] kinds) {

            using (var context = new Data.CMSContext()) {

                IQueryable<ContentTag> tags = context.ContentTags.Where(t => t.Tag!=null && t.Tag!="");


                if (location != null) {
                    location = location.ToLower();
                    if (!location.EndsWith("/"))
                        location = location + "/";
                    tags = tags.Where(t => context.ContentHeads.Any(c => c.ContentUId == t.ContentUId && c.Location == location));
                }

                if (kinds != null)
                    tags = tags.Where(t => context.ContentHeads.Any(c => c.ContentUId == t.ContentUId && kinds.Contains(c.Kind.ToLower())));

                ContentTagRank[] tagTanks = tags
                    .GroupBy(t => t.Tag)
                    .Select(g => new ContentTagRank { Tag = g.Key, Rank = g.Count() })
                    .OrderByDescending(tr => tr.Rank)
                    .ToArray<ContentTagRank>();

                return tagTanks;

            }
        }
    }
}
