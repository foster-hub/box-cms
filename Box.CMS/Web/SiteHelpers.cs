using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Policy;
using Box.CMS;
using Box.CMS.Services;
using System.Web;
using System.Web.WebPages;
using Box.CMS.Extensions;
using System.Text.RegularExpressions;

namespace Box.CMS.Web
{

    public class BoxSite
    {

        public static IHtmlString Image(dynamic file, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, string cssClass = "")
        {
            return new HtmlString("<img src=\"" + BoxLib.GetFileUrl((string)file.Folder, (string)file.FileUId, width, height, maxWidth, maxHeight) + "\" alt=\"" + file.Caption + "\" title=\"" + file.Caption + "\" class=\"" + cssClass + "\" />");
        }

        public static IHtmlString Figure(dynamic file, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, string cssClass = "")
        {
            string str = "<figure>{0}{1}</figure>";
            string caption = "";
            if (file.Caption != null && ((string)file.Caption) != "")
            {
                caption = "<figcaption>" + file.Caption + "</figcaption>";
            }
            return new HtmlString(String.Format(str, Image(file, width, height, maxWidth, maxHeight, cssClass), caption));
        }

        public static IHtmlString ContentLink(ContentHead content)
        {
            return new HtmlString("<a href=\"" + BoxLib.GetContentLink(content) + "\" title=\"" + content.Name + "\">" + content.Name + "</a>");
        }

        public static IHtmlString ContentsRelated(string id = null, Func<ContentHead, HelperResult> itemTemplate = null, string headerText = null, string location = null, string[] kinds = null, bool parseContent = false, int top = 0, string navigationId = null)
        {
            if (itemTemplate == null)
                itemTemplate = (head) => { return new HelperResult(w => w.Write("<li>" + ContentLink(head) + "</li>")); };

            string str = "";
            var heads = BoxLib.GetRelatedContents(id, location, kinds, parseContent, top);
            foreach (ContentHead head in heads)
                str = str + itemTemplate(head).ToString();

            if (headerText != null && !String.IsNullOrEmpty(str))
                str = headerText + str;

            if (navigationId != null) {
                BoxLib.SetListIsOver(navigationId, heads.Count() < top);
                BoxLib.SetListCount(navigationId, heads.Count());
            }

            HtmlString html = new HtmlString(str);
            return html;
        }

        public static IHtmlString ContentsRelated(string[] tags, Func<ContentHead, HelperResult> itemTemplate = null, string headerText = null, string location = null, string[] kinds = null, bool parseContent = false, int top = 0, string navigationId = null)
        {
            if (itemTemplate == null)
                itemTemplate = (head) => { return new HelperResult(w => w.Write("<li>" + ContentLink(head) + "</li>")); };

            string str = "";
            var heads = BoxLib.GetRelatedContents(tags, location, kinds, parseContent, top);
            foreach (ContentHead head in heads)
                str = str + itemTemplate(head).ToString();
            if (headerText != null && !String.IsNullOrEmpty(str))
                str = headerText + str;

            if (navigationId != null) {
                BoxLib.SetListIsOver(navigationId, heads.Count() < top);
                BoxLib.SetListCount(navigationId, heads.Count());
            }

            HtmlString html = new HtmlString(str);
            return html;
        }

        public static IHtmlString ContentsRelatedWithHotestThread(Func<ContentHead, HelperResult> itemTemplate = null, string location = null, string[] kinds = null, ContentRanks rankBy = ContentRanks.PageViews, Periods period = Periods.LastDay, int top = 5, string navigationId = null)
        {
            SiteService site = new SiteService();

            DateTime? lastPublished = DateTime.Now;
            if (period != Periods.AnyTime)
                lastPublished = site.GetLastPublishDate(location, kinds);

            ContentHead hotContent = site.GetHotestContent(kinds, location, rankBy, period.StartDate(lastPublished), null);
            if (hotContent == null)
                return new HtmlString("");
            return ContentsRelated(hotContent.TagsToArray(), itemTemplate, null, null, null, false, top, navigationId);
        }

        /// <summary>
        /// Gets Quiz FirstOrDefault() based on pageAREA
        /// </summary>
        /// <param name="pageArea"></param>
        /// <returns></returns>
        public static IHtmlString Quiz(string pageArea)
        {

            Func<ContentHead, HelperResult> template = (quiz) =>
            {

                string templatePath = System.Web.HttpContext.Current.Request.MapPath("~/box_templates/QUIZ.cshtml");
                string schemaTemplate = System.IO.File.ReadAllText(templatePath, Encoding.Default);

                bool alreadyVoted = true;

                if (HttpContext.Current.Request.Cookies["QUIZ_" + quiz.ContentUId] == null)
                    alreadyVoted = false;

                quiz.CONTENT["alreadyVoted"] = alreadyVoted;

                string outputQuiz = RazorEngine.Razor.Parse(schemaTemplate, quiz);

                return new HelperResult(w => w.Write(outputQuiz));
            };

            string str = "";

            SiteService site = new SiteService();
            var quizContent = site.GetCrossLinksFrom(pageArea, null, 1, new string[] { "QUIZ" }, true).FirstOrDefault();
            if (quizContent != null)
                str = template(quizContent).ToString();

            return new HtmlString(str);
        }
        
        public static IHtmlString Contents(string[] kinds = null, Func<ContentHead, HelperResult> itemTemplate = null, string order = "Date", Periods period = Periods.AnyTime, DateTime? createdFrom = null, DateTime? createdTo = null, bool parseContent = false, int top = 0, string navigationId = null, string location = null, string filter = null, IHtmlString noItemMessage = null, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null)
        {
            if (itemTemplate == null)
                itemTemplate = (head) => { return new HelperResult(w => w.Write("<li>" + ContentLink(head) + "</li>")); };
            string str = "";

            int skip = 0;
            if (navigationId != null)
            {
                top = BoxLib.GetListPageSize(navigationId);
                skip = BoxLib.GetPageSkipForList(navigationId) * top;
            }

            SiteService site = new SiteService();

            DateTime? startDate = createdFrom;
            if (period != Periods.AnyTime)
            {
                DateTime? lastPublished = DateTime.Now;
                lastPublished = site.GetLastPublishDate(location, kinds);
                startDate = period.StartDate(lastPublished);
            }


            var contents = BoxLib.GetContents(location, order, kinds, startDate, createdTo, parseContent, skip, top, filter, queryFilter);
            int i = 0;
            foreach (ContentHead head in contents)
            {
                head.OrderIndex = i;
                str = str + itemTemplate(head).ToString();
                i++;
            }

            if (navigationId != null) {
                BoxLib.SetListIsOver(navigationId, contents.Count() < top);
                BoxLib.SetListCount(navigationId, contents.Count());
            }

            if (contents.Count() == 0)
            {
                if (noItemMessage != null)
                    return noItemMessage;
                else
                    return new HtmlString("<li>No items.</li>");
            }

            return new HtmlString(str);
        }
        
        public static IHtmlString ContentsAtUrl(string url = "$currentUrl", Func<ContentHead, HelperResult> itemTemplate = null, string order = "Date", bool parseContent = false, int top = 0, string navigationId = null, IHtmlString noItemMessage = null, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null)
        {
            return Contents(null, itemTemplate, order, Periods.AnyTime, null, null, parseContent, top, navigationId, url, null, noItemMessage, queryFilter);
        }
                
        public static IHtmlString CrossLinksFrom(string pageArea, Func<ContentHead, HelperResult> itemTemplate = null, string order = "DisplayOrder", int top = 0, string[] kinds = null, IHtmlString noItemMessage = null, bool parseContent = false, string navigationId = null)
        {
            if (itemTemplate == null)
                itemTemplate = (head) => { return new HelperResult(w => w.Write("<div style=\"background-image: url(" + BoxLib.GetFileUrl(head.ThumbFilePath, asThumb: true) + ")\">" + ContentLink(head) + "</div>")); };


            var heads = BoxLib.GetCrossLinksFrom(pageArea, order, top, kinds, parseContent);
            string str = "";
            int i = 0;
            foreach (ContentHead head in heads)
            {
                head.OrderIndex = i;
                str = str + itemTemplate(head);
                i++;
            }

            if (heads.Count() == 0)
            {
                if (noItemMessage != null)
                    return noItemMessage;
                else
                    return new HtmlString("<div>No items.</div>");
            }

            if (navigationId != null) {
                BoxLib.SetListIsOver(navigationId, heads.Count() < top);
                BoxLib.SetListCount(navigationId, heads.Count());
            }

            return new HtmlString(str);
        }

        public static string ContentTags(ContentHead content)
        {
            string str = String.Empty;
            foreach (ContentTag tag in content.Tags)
            {
                str = str + ", " + tag.Tag;
            }
            if (str.Length <= 2)
                return str;
            return str.Substring(2);
        }

        public static IHtmlString TagCloud(string tagLink, string location = null, string[] kinds = null)
        {
            string cloud = "<ul class=\"tagCloud\">";
            ContentTagRank[] tags = BoxLib.GetTagCloud(location, kinds);
            foreach (ContentTagRank t in tags.OrderBy(t => t.Tag))
            {
                cloud = cloud + "<li value=\"" + t.Rank + "\"><a href=\"" + tagLink + "\\" + EncodeTag(t.Tag) + "\">" + t.Tag + "</a></li>";
            }
            cloud = cloud + "</ul>";
            return new HtmlString(cloud);
        }

        public static String EncodeTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return tag;

            tag = tag.Replace("&", "-e-");
            tag = tag.Replace("@", "-at-");
            tag = tag.Replace(".", "-dot-");
            tag = tag.Replace("%", "-oo-");
            tag = tag.Replace("#", "%23");
            tag = tag.Replace("*", "-x-");
            tag = tag.Replace("+", "-plus-");
            tag = tag.Replace("$", "%24");

            return tag;
        }

        public static String DecodeTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return tag;

            tag = tag.Replace("-e-", "&");
            tag = tag.Replace("-at-", "@");
            tag = tag.Replace("-dot-", ".");
            tag = tag.Replace("-oo-", "%");
            tag = tag.Replace("-x-", "*");
            tag = tag.Replace("-plus-", "+");
            tag = tag.Replace("%24", "$");
            tag = tag.Replace("%23", "#");

            return tag;
        }

        public static HtmlString TagStyle(string tag)
        {
            return new HtmlString("tagStyle_" + _adjustTagname(tag));
        }

        private static string _adjustTagname(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return null;

            string str = tag.ToLower();
            str = Regex.Replace(str, "[áàâãª]", "a");            
            str = Regex.Replace(str, "[éèêë]", "e");            
            str = Regex.Replace(str, "[íìî]", "i");            
            str = Regex.Replace(str, "[óòôõº]", "o");            
            str = Regex.Replace(str, "[úùû]", "u");            
            str = Regex.Replace(str, "[ç]", "c");            
            str = Regex.Replace(str, "[@#&*\\s\\.]", "_");
            return str;
        }

        public static IHtmlString PageNextButton(string listId, string text = "next", string formId = null)
        {

            if (BoxLib.GetListIsOver(listId))
                return new HtmlString("");

            string html = "<a href=\"{0}\" class=\"listNextButton\">{1}</a>";
            html = String.Format(html, BoxLib.ListNavigatonNextLink(listId, formId), text);

            return new HtmlString(html);
        }

        public static IHtmlString PagePreviousButton(string listId, string text = "previous", string formId = null)
        {

            int page = BoxLib.GetPageSkipForList(listId);
            if (page == 0)
                return new HtmlString("");

            string html = "<a href=\"{0}\" class=\"listPreviousButton\">{1}</a>";
            html = String.Format(html, BoxLib.ListNavigatonPreviousLink(listId, formId), text);

            return new HtmlString(html);
        }

        public static IHtmlString PageFirstButton(string listId, string text = "first", string formId = null)
        {

            int page = BoxLib.GetPageSkipForList(listId);
            if (page == 0)
                return new HtmlString("");

            string html = "<a href=\"{0}\" class=\"listFirstButton\">{1}</a>";
            html = String.Format(html, BoxLib.ListNavigationLink(listId, 0, formId), text);
            return new HtmlString(html);
        }

        public static IHtmlString PageMoreContentButton(string text = "More content...", string cssClass = null) {
            string html = "<a " + (cssClass == null?"":"class=\"" + cssClass +"\"") + " src=\"#\" data-bind=\"click: function(d,e) { d._getData(); }, visible: nextContentButtonVisible()\" class=\"listNextButton\">" + text + "</a>";            
            return new HtmlString(html);
        }

        public static IHtmlString BoxHtmlContent(dynamic content, string html) {
            if (html != null && content != null && content.Images !=null) {
                
                var gallery = IMAGE_GALLERY_TEMPLATE((Newtonsoft.Json.Linq.JArray)content.Images).ToString();
                html = html.Replace("#ImageGallery-Images#", gallery);
            }
            return new HtmlString(html);
        }

        public static string ImageGallery(Newtonsoft.Json.Linq.JArray images) {
            string html = "";
            foreach(var image in images) {
                html = html + BoxSite.Image(file: image);
            }
            return html;
        }


        public static Func<Newtonsoft.Json.Linq.JArray, HelperResult> IMAGE_GALLERY_TEMPLATE;
    }


    

    public class BoxLib
    {

        public static void ResponseFile(string fileUId)
        {
            SiteService site = new SiteService();
            site.ResponseFile(HttpContext.Current.Response, fileUId);
        }

        public static string GetFileName(string fileUId)
        {
            SiteService site = new SiteService();
            return site.GetFileName(fileUId);
        }

        public static string GetFileUrl(dynamic file)
        {
            return GetFileUrl((string)file["Folder"], (string)file["FileUId"]);
        }

        public static string GetFileUrl(string folder, string fileUId, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, bool asThumb = false)
        {
            return GetFileUrl(folder + "/" + fileUId, width, height, maxWidth, maxHeight, asThumb);
        }

        public static string GetFileUrl(string filePath, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, bool asThumb = false)
        {
            SiteService site = new SiteService();
            if (site.IgnoreVirtualAppPath)
                return AppName + "/files/" + filePath + "/?height=" + height + "&maxHeight=" + maxHeight + "&asThumb=" + asThumb.ToString().ToLower() + "&width=" + width + "&maxWidth=" + maxWidth;
            else
                return "/files/" + filePath + "/?height=" + height + "&maxHeight=" + maxHeight + "&asThumb=" + asThumb.ToString().ToLower() + "&width=" + width + "&maxWidth=" + maxWidth;

        }

        public static string GetFileUrl(dynamic file, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, bool asThumb = false) {            
            return GetFileUrl((string)file["Folder"] + "/" + (string)file["FileUId"], width, height, maxWidth, maxHeight, asThumb);
        }

        public static string GetContentLink(ContentHead head)
        {
            SiteService site = new SiteService();

            if (!String.IsNullOrEmpty(head.ExternalLinkUrl))
                return head.ExternalLinkUrl;

            if (site.IgnoreVirtualAppPath)
                return head.Location + head.CanonicalName;
            else
                return AppName + head.Location + head.CanonicalName;
        }

        public static IEnumerable<ContentHead> GetContents(string url, string order = "Date", string[] kinds = null, DateTime? createdFrom = null, DateTime? createdTo = null, bool parseContent = false, int skip = 0, int top = 0, string filter = null, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null)
        {

            if (url != null && url.ToLower() == "$currenturl")
            {
                if (HttpContext.Current == null)
                    return null;

                int qIndex = HttpContext.Current.Request.RawUrl.IndexOf("?", 0);
                if (qIndex > 0)
                    url = HttpContext.Current.Request.RawUrl.Substring(0, qIndex);
                else
                    url = HttpContext.Current.Request.RawUrl;

                url = RemoveAppNameFromUrl(url);

                int slashIndex = url.LastIndexOf("/", 1);
                if (slashIndex > 0)
                    url = url.Substring(0, url.Length - slashIndex);



            }
            SiteService site = new SiteService();
            return site.GetContents(url, order, kinds, createdFrom, createdTo, parseContent, skip, top, filter, queryFilter);
        }

        public static string RemoveAppNameFromUrl(string url)
        {
            SiteService site = new SiteService();
            if (site.IgnoreVirtualAppPath)
                return url;
            if (AppName != "" && url.StartsWith(AppName))
            {
                url = url.Substring(AppName.Length);
            }
            return url;
        }

        public static IEnumerable<ContentHead> GetRelatedContents(string id = null, string location = null, string[] kinds = null, bool parseContent = false, int top = 0)
        {
            if (id == null)
            {
                Box.CMS.Web.ContentRenderView renderView = WebPageContext.Current.Page as Box.CMS.Web.ContentRenderView;
                if (renderView == null)
                    return new List<ContentHead>();
                id = renderView.HEAD.ContentUId;
            }
            SiteService site = new SiteService();
            return site.GetRelatedContent(id, top, location, kinds, parseContent);
        }

        public static IEnumerable<ContentHead> GetRelatedContents(string[] tags, string location = null, string[] kinds = null, bool parseContent = false, int top = 0)
        {
            SiteService site = new SiteService();
            return site.GetRelatedContent(tags, top, location, kinds, parseContent);
        }


        public static IEnumerable<ContentHead> GetCrossLinksFrom(string pageArea, string order = "DisplayOrder", int top = 0, string[] kinds = null, bool parseContent = false)
        {
            SiteService site = new SiteService();
            return site.GetCrossLinksFrom(pageArea, order, top, kinds, parseContent);
        }

        internal static int GetPageSkipForList(string listId)
        {
            int skip = 0;
            if (HttpContext.Current == null)
                return 0;
            string skipStr = HttpContext.Current.Request["_pageSkip_" + listId];
            if (String.IsNullOrEmpty(skipStr))
                return 0;
            int.TryParse(skipStr, out skip);
            return skip;
        }


        public static string ListNavigatonNextLink(string listId, string formId = null)
        {
            if (HttpContext.Current == null)
                return null;

            int actualSkip = GetPageSkipForList(listId);
            actualSkip++;
            return ListNavigationLink(listId, actualSkip, formId);
        }

        public static string ListNavigatonPreviousLink(string listId, string formId = null)
        {
            if (HttpContext.Current == null)
                return null;
            int actualSkip = GetPageSkipForList(listId);
            actualSkip--;
            if (actualSkip < 0)
                actualSkip = 0;
            return ListNavigationLink(listId, actualSkip, formId);
        }



        public static string ListNavigationLink(string listId, int skip, string formId = null)
        {

            if (HttpContext.Current == null)
                return String.Empty;

            HttpRequest request = HttpContext.Current.Request;

            string url = "";
            string query = "?";
            foreach (string key in request.QueryString.Keys)
            {
                if (key != "_pageSkip_" + listId)
                {
                    query = query + key + "=" + request.QueryString[key] + "&";
                }
            }
            query = query + "_pageSkip_" + listId + "=" + skip;
            if (request.Url.Query.Length > 0)
                url = request.RawUrl.Replace(request.Url.Query, "") + query;
            else
                url = request.RawUrl + query;

            // new version - uses POST
            if (!String.IsNullOrEmpty(formId))
            {
                url = String.Format("javascript:{0}.action='{1}';{0}.submit();", formId, url);
            }

            return url;

        }

        internal static void SetListIsOver(string listId, bool isOver)
        {
            var page = WebPageContext.Current.Page;
            if (page == null)
                return;
            page.PageData["__" + listId + "isListOver"] = isOver;            
        }

        internal static void SetListCount(string listId, int count) {
            var page = WebPageContext.Current.Page;
            if (page == null)
                return;
            page.PageData["__" + listId + "Count"] = count;
        }

        public static bool GetListIsOver(string listId)
        {
            var page = WebPageContext.Current.Page;
            if (page == null)
                return false;
            bool? isOver = page.PageData["__" + listId + "isListOver"] as bool?;
            if (!isOver.HasValue)
                isOver = false;
            return isOver.Value;
        }

        public static int GetListCount(string listId) {
            var page = WebPageContext.Current.Page;
            if (page == null)
                return 0;
            int? count = page.PageData["__" + listId + "Count"] as int?;
            if (!count.HasValue)
                count = 0;
            return count.Value;
        }

        public static void LogPageShare()
        {

            Box.CMS.Web.ContentRenderView renderView = WebPageContext.Current.Page as Box.CMS.Web.ContentRenderView;
            if (renderView == null)
                return;

            string serverHost = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;

            SiteService site = new SiteService();
            site.LogPageShare(renderView.HEAD, serverHost);
        }

        public static int GetListPageSize(string listId)
        {
            var page = WebPageContext.Current.Page;
            if (page == null)
                return 0;
            int? size = page.PageData["__" + listId + "pageSize"] as int?;
            if (!size.HasValue)
                size = 20;
            return size.Value;
        }

        public static void SetListPageSize(string listId, int size)
        {
            var page = WebPageContext.Current.Page;
            if (page == null)
                return;
            page.PageData["__" + listId + "pageSize"] = size;
        }


        public static ContentTagRank[] GetTagCloud(string location, string[] kinds)
        {
            SiteService site = new SiteService();
            return site.GetTagCloud(location, kinds);
        }

        public static string AppName
        {
            get
            {
                if (HttpContext.Current == null)
                    return "";
                string appName = HttpContext.Current.Request.ApplicationPath;
                if (appName == "/")
                    appName = "";
                return appName;
            }
        }
    }






}
