using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using Box.Composition;

namespace Box.Adm {

    public static class WebApiConfig {

        public static void Register(HttpConfiguration config) {

            config.Routes.MapHttpRoute(
                name: "BoxFileRemove",
                routeTemplate: "api/cms_files/Remove/{folder}/{id}",
                defaults: new { controller = "cms_files", id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "BoxFile",
                routeTemplate: "api/cms_files/{folder}/{id}",
                defaults: new { controller = "cms_files", id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "BoxApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { id = new ParamNotAllowedForMethod("ALL") });

            config.Routes.MapHttpRoute(
                name: "BoxApiAction",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { id = new ParamNotAllowedForMethod("ALL") });

        }

    }

}
