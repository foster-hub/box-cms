using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Box.Core;


namespace Box.CMS.Web {

    public class ContentUrlModule : System.Web.IHttpModule {

        private readonly ConcurrentDictionary<string, string> redirectCache = new ConcurrentDictionary<string, string>();

        private Box.CMS.Groups.ADM_CMS ADM_CMS_group = new Groups.ADM_CMS();
        private Box.Core.Groups.ADM_SEC AMD_SEC_group = new Core.Groups.ADM_SEC();

        void context_AuthorizeRequest(object sender, EventArgs e) {

            System.Web.HttpApplication app = sender as System.Web.HttpApplication;
            if (app == null)
                return;

            string fullUrl = BoxLib.RemoveAppNameFromUrl(app.Context.Request.RawUrl);
            string redirectUrl = null;

            if (fullUrl.StartsWith("/where-is-my-db.htm"))
                return;

            // if true, can see not published contents
            bool canSeeOnlyPublished = GetShowOnlyPublished(app, fullUrl);

            string url = BoxLib.RemoveAppNameFromUrl(app.Request.FilePath);
                        
            redirectCache.TryGetValue(url, out redirectUrl);

            if (redirectUrl == null) {
                Services.CMSService cms = new Services.CMSService();

                ContentHead c = null;
                try
                {
                    c = cms.GetContentHeadByUrlAndKind(url, null, canSeeOnlyPublished);                    
                }                
                catch (Exception ex)
                {
                    var SQLexception = Box.Core.Services.SecurityService.GetSqlException(ex);
                    if (SQLexception != null && !Box.Core.Services.SecurityService.IsDebug)
                        app.Context.Response.Redirect("~/where-is-my-db.htm#" + SQLexception.Number);
                    else
                        throw ex;
                }

                if (c == null)
                    return;
                
                redirectUrl = "~/box_templates/" + c.Kind + "/" + c.ContentUId;

                // only add at cache published urls
                if(canSeeOnlyPublished)
                    redirectCache.TryAdd(url, redirectUrl);
            }

            app.Context.RewritePath(redirectUrl + "?" + app.Context.Request.QueryString);
        }

        private bool GetShowOnlyPublished(System.Web.HttpApplication app, string url)
        {
            if (app.Request.QueryString["previewToken"] == null)
                return true;
            
            //Remove token from url
            string token = app.Request.QueryString["previewToken"].ToString();

            Core.Services.SecurityService security = new Core.Services.SecurityService();
            
            User u = security.GetUserByAuthToken(token);
            
            string[] roles = security.GetUserRoles(u);
            return !roles.Contains(ADM_CMS_group.UserGroupUId) && !roles.Contains(AMD_SEC_group.UserGroupUId);            
        }
        
        public void Dispose() {            
        }

        public void Init(System.Web.HttpApplication context) {           
            context.AuthorizeRequest += new EventHandler(context_AuthorizeRequest);
        }

    }
}
