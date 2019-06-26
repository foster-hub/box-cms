using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace Box.CMS.Services
{

    public enum FileStorages
    {
        Database,
        FileSystem
    }



    [Export]
    [Export(typeof(Box.Composition.IAppStart))]
    public partial class CMSService : Box.Composition.IAppStart
    {

        private ContentKind[] kindsCache = null;
        private Box.Composition.IUserGroup[] groupsCache = null;

        [Import]
        private Groups.ADM_CMS ADM_CMS_GROUP { get; set; }

        [Import]
        private Groups.ADM_CMSFILE ADM_CMSFILE_GROUP { get; set; }


        public void ParseContentData(ContentHead content)
        {
            content.CONTENT = JObject.Parse(content.Data.JSON);
        }

        public ContentHead GetContentHead(string id)
        {
            using (var context = new Data.CMSContext())
            {
                return context.ContentHeads.SingleOrDefault(c => c.ContentUId == id);
            }
        }

        public ContentHead GetContent(string id, bool onlyPublished = false)
        {
            using (var context = new Data.CMSContext())
            {
                IQueryable<ContentHead> content = context.ContentHeads.Include("Data").Include("CrossLinks").Include("Tags").Include("CommentsCount").Include("CustomInfo").Where(c => c.ContentUId == id);

                if (onlyPublished)
                    content = OnlyPublishedContents(content);

                return content.SingleOrDefault();
            }
        }

        public List<string> GetTagCloud(string kind)
        {
            using (var context = new Data.CMSContext())
            {
                List<string> kinds = new List<string>();

                IQueryable<ContentTag> tags = context.ContentTags.Where(t => t.Tag != null && t.Tag != "");

                tags = tags.Where(t => context.ContentHeads.Any(c => c.Kind == kind));

                ContentTagRank[] tagTanks = tags
                        .GroupBy(t => t.Tag)
                        .Select(g => new ContentTagRank { Tag = g.Key, Rank = g.Count() })
                        .OrderByDescending(tr => tr.Rank)
                        .ToArray<ContentTagRank>();


                foreach (var item in tagTanks)
                {
                    kinds.Add(item.Tag);
                }
                return kinds.ToList();
            }
        }

        public IEnumerable<ContentHead> GetCrossLinksFrom(string pageArea, string order = "CrossLinkDisplayOrder", int top = 0, string[] kinds = null, bool includeData = false, string[] pageAreaFallbacks = null, bool onlyPublished = true)
        {
            using (var context = new Data.CMSContext())
            {
                IQueryable<ContentHead> contents = null;

                if (!includeData)
                    contents = context.ContentHeads.Include("CommentsCount").Include("ShareCount").Include("PageViewCount").Include("Tags").Include("CustomInfo");
                else
                    contents = context.ContentHeads.Include("CommentsCount").Include("ShareCount").Include("PageViewCount").Include("Tags").Include("CustomInfo").Include("Data");

                contents = contents.Where(c => c.CrossLinks.Any(x => x.PageArea == pageArea));

                if (onlyPublished)
                    contents = OnlyPublishedContents(contents);

                contents = OrderContents(contents, order, pageArea);

                if (kinds != null)
                    contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

                if (top != 0)
                    contents = contents.Take(top);

                var array = contents.ToArray();

                // if ther is no content, and there is any fallback, try it
                if (!array.Any() && pageAreaFallbacks != null && pageAreaFallbacks.Length >= 1)
                {
                    string fallBackArea = pageAreaFallbacks.First();
                    string[] othersFall = pageAreaFallbacks.Where(a => a != fallBackArea).ToArray();
                    return GetCrossLinksFrom(fallBackArea, order, top, kinds, includeData, othersFall);
                }

                return array;
            }
        }

        private IQueryable<ContentHead> OrderContents(IQueryable<ContentHead> contents, string order, string pageArea = null)
        {
            if (order == "Name")
                return contents.OrderBy(c => c.Name).ThenByDescending(c => c.ContentDate).ThenBy(c => c.ContentUId);
            if (order == "Date ASC")
                return contents.OrderBy(c => c.ContentDate);
            if (order == "DisplayOrder")
                return contents.OrderBy(c => c.DisplayOrder).ThenByDescending(c => c.ContentDate).ThenBy(c => c.ContentUId);
            if (order == "Comments")
                return contents.OrderBy(c => c.CommentsCount.Count).ThenBy(c => c.ContentUId);
            if (order == "Comments DESC")
                return contents.OrderByDescending(c => c.CommentsCount.Count).ThenBy(c => c.ContentUId);
            if (order == "Share")
                return contents.OrderBy(c => c.ShareCount.Count).ThenBy(c => c.ContentUId);
            if (order == "Share DESC")
                return contents.OrderByDescending(c => c.ShareCount.Count).ThenBy(c => c.ContentUId);
            if (order == "PageView")
                return contents.OrderBy(c => c.PageViewCount.Count).ThenBy(c => c.ContentUId);
            if (order == "PageView DESC")
                return contents.OrderByDescending(c => c.PageViewCount.Count).ThenBy(c => c.ContentUId);

            if (order == "Random")
            {
                var rand = new Random();
                int r = rand.Next();
                return contents.OrderByDescending(c =>
                    (~(((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) & r))
                    & ((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) | r)));
            }

            if (order == "CrossLinkDisplayOrder" && pageArea != null)
                return contents.OrderByDescending(c => c.CrossLinks.Where(x => x.PageArea == pageArea).FirstOrDefault().DisplayOrder).ThenByDescending(c => c.ContentDate).ThenBy(c => c.ContentUId);

            if (order == "RandomOnDay")
            {

                int dw = (int)System.DateTime.Today.DayOfWeek * 100;
                int dd = System.DateTime.Today.Day * 50;
                int r = (System.DateTime.Today.DayOfYear * 15) + dw + dd;
                return contents.OrderByDescending(c =>
                    (~(((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) & r))
                    & ((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) | r)));

            }

            // CUSTOM ORDERS
            if (order == "CustomNumber1")
            {
                return contents.OrderBy(c => c.CustomInfo.Number1).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomNumber2")
            {
                return contents.OrderBy(c => c.CustomInfo.Number2).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomNumber3")
            {
                return contents.OrderBy(c => c.CustomInfo.Number3).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomNumber4")
            {
                return contents.OrderBy(c => c.CustomInfo.Number4).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomNumber1 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number1).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomNumber2 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number2).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomNumber3 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number3).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomNumber4 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number4).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText1")
            {
                return contents.OrderBy(c => c.CustomInfo.Text1).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText2")
            {
                return contents.OrderBy(c => c.CustomInfo.Text2).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText3")
            {
                return contents.OrderBy(c => c.CustomInfo.Text3).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText4")
            {
                return contents.OrderBy(c => c.CustomInfo.Text4).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText1 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text1).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText2 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text2).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText3 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text3).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomText4 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text4).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate1")
            {
                return contents.OrderBy(c => c.CustomInfo.Date1).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate2")
            {
                return contents.OrderBy(c => c.CustomInfo.Date2).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate3")
            {
                return contents.OrderBy(c => c.CustomInfo.Date3).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate4")
            {
                return contents.OrderBy(c => c.CustomInfo.Date4).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate1 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date1).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate2 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date2).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate3 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date3).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }
            if (order == "CustomDate4 DESC")
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date4).ThenBy(c => c.DisplayOrder).ThenBy(c => c.ContentUId);
            }

            return contents.OrderByDescending(c => c.ContentDate).ThenBy(c => c.ContentUId);
        }



        private IQueryable<ContentHead> OnlyPublishedContents(IQueryable<ContentHead> contents)
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            return contents.Where(c => 
                (c.PublishAfter.HasValue && c.PublishAfter <= now)
                && (!c.PublishUntil.HasValue || c.PublishUntil >= now));
        }

        public DateTime? GetLastPublishDate(string location, string[] kinds)
        {
            using (var context = new Data.CMSContext())
            {

                IQueryable<ContentHead> contents = context.ContentHeads;

                contents = OnlyPublishedContents(contents);

                if (!string.IsNullOrEmpty(location))
                {
                    location = location.ToLower();
                    if (!location.EndsWith("/"))
                        location = location + "/";
                    contents = contents.Where(c => c.Location == location);
                }

                if (kinds != null)
                    contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

                return contents.Max(c => (DateTime?)c.CreateDate);

            }
        }

        public IEnumerable<ContentHead> GetContents(string filter, int skip, int top, string location, string[] kinds, string order = "Date", DateTime? createdFrom = null, DateTime? createdTo = null, bool includeData = false, bool onlyPublished = false, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null, bool showHiddenContents = false)
        {
            using (var context = new Data.CMSContext())
            {
                IQueryable<ContentHead> contents = null;

                if (!includeData)
                    contents = context.ContentHeads.Include("CrossLinks").Include("CommentsCount").Include("ShareCount").Include("PageViewCount").Include("Tags").Include("CustomInfo");
                else
                    contents = context.ContentHeads.Include("CrossLinks").Include("CommentsCount").Include("ShareCount").Include("PageViewCount").Include("Tags").Include("CustomInfo").Include("Data");

                if (createdFrom.HasValue)
                    contents = contents.Where(c => c.ContentDate >= createdFrom.Value);

                if (createdTo.HasValue)
                    contents = contents.Where(c => c.ContentDate <= createdTo.Value);

                if (onlyPublished)
                    contents = OnlyPublishedContents(contents);

                //  if no location was passed and it is not supposed to show hidden contents, hide them
                if (showHiddenContents==false && String.IsNullOrEmpty(location))
                    contents = contents.Where(c => !c.Location.StartsWith("/__"));

                if (queryFilter != null)
                {
                    contents = contents.Where(queryFilter);
                }

                if (!String.IsNullOrEmpty(filter))
                {
                    filter = filter.ToLower();
                    contents = contents.Where(c =>
                        (c.CustomInfo != null && (c.CustomInfo.Text1.ToLower().StartsWith(filter) || c.CustomInfo.Text2.ToLower().StartsWith(filter) || c.CustomInfo.Text3.ToLower().StartsWith(filter) || c.CustomInfo.Text4.ToLower().StartsWith(filter))) ||
                        c.Name.ToLower().Contains(filter) ||
                        c.Abstract.ToLower().Contains(filter) ||
                        c.Tags.Any(t => t.Tag.Contains(filter)) ||
                        c.CrossLinks.Any(x => x.PageArea.Contains(filter)));
                }

                contents = OrderContents(contents, order);

                if (location != null)
                {
                    location = location.ToLower();
                    if (!location.EndsWith("/"))
                        location = location + "/";
                    contents = contents.Where(c => c.Location == location);
                }

                if (kinds != null && kinds.Any(k => k != null))
                    contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

                if (skip != 0)
                    contents = contents.Skip(skip);

                if (top != 0)
                    contents = contents.Take(top);

                return contents.ToArray();
            }
        }

        public ContentHead[] OnlyContentsUserCanEdit(IEnumerable<ContentHead> contents)
        {

            List<ContentHead> allowedContents = new List<ContentHead>();

            ContentKind kind = null;
            foreach (var c in contents)
            {

                if (kind == null || kind.Kind != c.Kind)
                    kind = GetContentKind(c.Kind);

                if (kind == null || CanEditContent(kind))
                    allowedContents.Add(c);
            }
            return allowedContents.ToArray();
        }

        public IEnumerable<ContentHead> GetRelatedContent(string id, int top, string location, string[] kinds, bool includeData = false)
        {
            var content = GetContent(id);
            if (content == null)
                return new List<ContentHead>();
            string[] tags = content.Tags.Select(t => t.Tag).ToArray();
            var contents = GetRelatedContent(tags, top + 1, location, kinds, includeData);

            // remove it selft from related contents
            if (contents != null)
                contents = contents.Where(c => c.ContentUId != id);


            return contents.Take(top);
        }

        public IEnumerable<ContentHead> GetRelatedContent(string[] tags, int top, string location, string[] kinds, bool includeData = false)
        {
            using (var context = new Data.CMSContext())
            {
                IQueryable<ContentHead> contents = null;

                if (!includeData)
                    contents = context.ContentHeads.Include("CrossLinks").Include("CommentsCount").Include("ShareCount").Include("Tags").Include("PageViewCount").Include("CustomInfo");
                else
                    contents = context.ContentHeads.Include("CrossLinks").Include("CommentsCount").Include("ShareCount").Include("Tags").Include("PageViewCount").Include("CustomInfo").Include("Data");

                // only published
                contents = OnlyPublishedContents(contents);

                // remove empty entries, and go lower
                tags = tags.Where(t => !String.IsNullOrEmpty(t)).ToArray();
                for (int i = 0; i < tags.Length; i++)
                    tags[i] = tags[i].ToLower();

                // get related using tags
                contents = contents.Where(c => c.Tags.Any(t => tags.Contains(t.Tag)));

                contents = OrderContents(contents, "Date");

                if (location != null)
                {
                    location = location.ToLower();
                    if (!location.EndsWith("/"))
                        location = location + "/";
                    contents = contents.Where(c => c.Location == location);
                }

                if (kinds != null)
                    contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

                if (top != 0)
                    contents = contents.Take(top);

                return contents.ToArray();
            }
        }

        public ContentHead GetHotestContent(string[] kinds, string location, ContentRanks rankBy = ContentRanks.PageViews, DateTime? createdFrom = null, DateTime? createdTo = null)
        {
            string order = "PageView DESC";
            switch (rankBy)
            {
                case ContentRanks.Comments:
                    order = "Comments DESC";
                    break;
                case ContentRanks.Shares:
                    order = "Share DESC";
                    break;
                case ContentRanks.Date:
                    order = "Date";
                    break;
            }
            return GetContents(null, 0, 1, location, kinds, order, createdFrom, createdTo, false, true).FirstOrDefault();
        }

        public string CanonicalName(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();

            foreach (char letter in arrayText)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(letter) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            string s1 = sbReturn.ToString();

            //apenas letras, números, espaços e hífen

            s1 = s1.Replace("/", "-");

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            s1 = rgx.Replace(s1, "");

            s1 = s1.ToLower().Replace(" ", "-")
                .Replace(".", "")
                .Replace("?", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("&", "")
                .Replace("*", "")
                .Replace(":", "-")
                .Replace("%", "")
                .Replace(",", "-")
                .Replace("ª", "");

            while(s1.IndexOf("--")>0)
            {
                s1 = s1.Replace("--", "-");
            }
            

            return s1;
        }

        public ContentHead SaveContent(ContentHead content)
        {
            using (var context = new Data.CMSContext())
            {



                ContentHead oldContent = GetContent(content.ContentUId);
                List<CrossLink> removedLinks = new List<CrossLink>();
                List<CrossLink> addedLinks = content.CrossLinks.ToList();

                if (oldContent == null)
                {
                    context.ContentHeads.Add(content);
                }
                else
                {
                    context.ContentHeads.Attach(oldContent);
                    context.Entry<ContentHead>(oldContent).CurrentValues.SetValues(content);
                    context.Entry<ContentData>(oldContent.Data).CurrentValues.SetValues(content.Data);
                    if (oldContent.CustomInfo != null)
                        context.Entry<ContentCustomInfo>(oldContent.CustomInfo).CurrentValues.SetValues(content.CustomInfo);
                }

                //context.ApplyCollectionValues<CrossLink>(oldContent != null ? oldContent.CrossLinks : null, content.CrossLinks, (t1, t2) => { return t1.PageArea == t2.PageArea; });
                ApplyCollectionValuesCrossLinks(oldContent != null ? oldContent.CrossLinks : null, content.CrossLinks);

                context.ApplyCollectionValues<ContentTag>(oldContent != null ? oldContent.Tags : null, content.Tags, (t1, t2) => { return t1.Tag == t2.Tag; });


                context.SaveChanges();
            }
            return content;
        }

        public void RemoveContent(string contentUId)
        {
            using (var context = new Data.CMSContext())
            {
                ContentHead content = context.ContentHeads.SingleOrDefault(c => c.ContentUId == contentUId);
                if (content == null)
                    return;
                context.ContentHeads.Remove(content);
                context.SaveChanges();
            }
        }

        private string KindsConfigXMLPath
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return "";
                return System.Web.HttpContext.Current.Server.MapPath("~/App_Data/CMS/Kinds.config");
            }
        }

        private string GroupsConfigXMLPath
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return "";
                return System.Web.HttpContext.Current.Server.MapPath("~/App_Data/CMS/UserGroups.config");
            }
        }

        public ContentHead GetContentHeadByUrlAndKind(string url, string kind, bool onlyPublished)
        {
            url = url.ToLower();
            using (var context = new Data.CMSContext())
            {
                IQueryable<ContentHead> content = context.ContentHeads.Where(c => c.Location + c.CanonicalName == url);
                if (kind != null)
                    content = content.Where(c => c.Kind == kind);
                if (onlyPublished)
                    content = OnlyPublishedContents(content);
                return content.FirstOrDefault();

            }
        }

        public ContentHead GetContentByUrlAndKind(string url, string kind, bool onlyPublished = false)
        {
            using (var context = new Data.CMSContext())
            {
                IQueryable<ContentHead> content = context.ContentHeads.Include("Data").Include("CrossLinks").Include("Tags").Include("CommentsCount").Include("CustomInfo").Where(c => c.Location + c.CanonicalName == url);

                if (kind != null)
                    content = content.Where(c => c.Kind == kind);

                if (onlyPublished)
                    content = OnlyPublishedContents(content);

                return content.FirstOrDefault();
            }
        }


        private void GetContentKinds()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ContentKind[]));
            using (var reader = System.Xml.XmlReader.Create(KindsConfigXMLPath))
            {
                kindsCache = serializer.Deserialize(reader) as ContentKind[];
            }
            if (kindsCache == null)
                kindsCache = new ContentKind[0];

            foreach (ContentKind k in kindsCache)
            {
                if (k.Browsable == null)
                    k.Browsable = true;
                if (k.CaptureController == null)
                    k.CaptureController = "~/cms_contents";
                if (k.FriendlyPluralName == null)
                    k.FriendlyPluralName = k.FriendlyName + "s";
                if (k.CaptureDetailView == null)
                    k.CaptureDetailView = k.Kind;
                if (k.RequiredRolesToEdit == null || k.RequiredRolesToEdit.Length == 0)
                    k.RequiredRolesToEdit = new string[] { ADM_CMS_GROUP.UserGroupUId };
                if (k.Tags == null)
                    k.Tags = new string[0];

            }
        }

        public ContentKind[] ContentKinds
        {
            get
            {
                if (kindsCache == null)
                    GetContentKinds();
                return kindsCache;
            }
        }

        private void GetContentUserGroups()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Groups.ContentUserGroup[]));
            using (var reader = System.Xml.XmlReader.Create(GroupsConfigXMLPath))
            {
                groupsCache = serializer.Deserialize(reader) as Groups.ContentUserGroup[];
            }
            if (groupsCache == null)
                groupsCache = new Groups.ContentUserGroup[0];
        }

        public ContentKind GetContentKind(string kind)
        {
            if (kindsCache == null)
                GetContentKinds();

            var contentKind = kindsCache.SingleOrDefault(k => k.Kind == kind);

            if (contentKind == null)
                return null;

            // adds old locations
            string[] plocations = GetPublishedLocations(kind);
            contentKind.Locations = contentKind.Locations.Union(plocations).ToArray();

            return contentKind;
        }

        public string[] GetPublishedLocations(string kind)
        {
            using (var context = new Data.CMSContext())
            {
                return context.ContentHeads.Where(c => c.Kind == kind).Select(c => c.Location).Distinct().ToArray();
            }
        }

        public void OnStart(System.ComponentModel.Composition.Hosting.CompositionContainer container)
        {
            ExportContentLinks(container);
            ExportContentUserGroups(container);
        }

        private void ExportContentLinks(System.ComponentModel.Composition.Hosting.CompositionContainer container)
        {
            GetContentKinds();
            foreach (ContentKind k in kindsCache)
            {
                Menus.ContentLink link = new Menus.ContentLink(k.FriendlyPluralName, k.CaptureController + "/?kind=" + k.Kind, k.RequiredRolesToEdit);
                container.ComposeExportedValue<Box.Composition.IMenuActionLink>(link);
            }
        }

        private void ExportContentUserGroups(System.ComponentModel.Composition.Hosting.CompositionContainer container)
        {
            GetContentUserGroups();
            foreach (Box.Composition.IUserGroup g in groupsCache)
            {
                container.ComposeExportedValue<Box.Composition.IUserGroup>(g);
            }
        }

        public void VerifyAuthorizationToEditContent(string kind)
        {
            if (!CanEditContent(GetContentKind(kind)))
                throw new System.Security.SecurityException("Not authorized to edit content");
        }

        public bool CanEditContent(ContentKind kind)
        {
            if (kind == null)
                return true;

            // if CMS ADM, can edit anything, get out of here
            if (System.Threading.Thread.CurrentPrincipal.IsInRole(ADM_CMS_GROUP.UserGroupUId))
                return true;

            foreach (string role in kind.RequiredRolesToEdit)
            {
                if (System.Threading.Thread.CurrentPrincipal.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }

        public string SiteHost
        {
            get
            {
                string host = System.Configuration.ConfigurationManager.AppSettings["SITE_HOST"] as String;
                if (host == null)
                    host = "localhost";
                return host;
            }
        }

        public bool CanViewContent(ContentKind kind)
        {
            // if ca edit, can see it
            if (CanEditContent(kind))
                return true;

            if (kind == null)
                throw new System.Security.SecurityException("Content Kind not found");

            foreach (string role in kind.RequiredRolesToView)
            {
                if (System.Threading.Thread.CurrentPrincipal.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }




    }



}
