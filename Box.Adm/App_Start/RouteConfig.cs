using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Box.Composition;

namespace Box.Adm {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                 name: "AsyncTaskresult",
                 url: "core_taskresult/{id}",
                 defaults: new { controller = "core_taskresult", action = "Index" }
            );


            routes.MapRoute(
                name: "File",
                url: "files/{folder}/{id}",
                defaults: new { controller = "cms_files", action = "Index" }
            );

            routes.MapRoute(
                name: "Signin",
                url: "signin",
                namespaces: new string[] { "Box.Core.Controllers" }, 
                defaults: new { controller = "core_signin", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Signin-nt",
                url: "signin-nt",
                namespaces: new string[] { "Box.Core.Controllers" },
                defaults: new { controller = "core_signin", action = "nt", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ResetPassword",
                url: "applyNewPassword/{id}",
                namespaces: new string[] { "Box.Core.Controllers" }, 
                defaults: new { controller = "core_signin", action = "ApplyNewPassword" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { id = new ParamNotAllowedForMethod("ALL"), folder = new ParamNotAllowedForMethod("ALL") }
            );

            

        }
    }
}