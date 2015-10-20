using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.ComponentModel.Composition;
using System.Threading;
using System.Web.Security;
using Box.Composition;
using Box.Core;


namespace Box.Adm {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {

        
     
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MefConfig.RegisterMef("bin");

            // puts all roles at the admin group
            var security = MefConfig.Container.GetExportedValue<Box.Core.Services.SecurityService>();
            security.UpdateAdminGroup();
            

        }

        protected void Application_BeginRequest(Object source, EventArgs e) {

            // detect if DB was not configured
            Box.Core.Services.SecurityService.DetectNotInstalled();

        }


        protected void Application_AuthorizeRequest(Object sender, EventArgs e) {
            var authorizator = MefConfig.Container.GetExportedValue<Box.Composition.Services.IAuthorizationService>();
            authorizator.AuthenticateRequestPrincipal();            
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e) {

            if (System.Web.HttpContext.Current == null)
                return;

            string[] lang = System.Web.HttpContext.Current.Request.UserLanguages;
            if (lang==null || lang.Length == 0)
                return;

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang[0]);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang[0]);
        }

    }
}