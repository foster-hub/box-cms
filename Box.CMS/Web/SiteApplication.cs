using System.Web.Http;
using System.Web.Mvc;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Box.Composition;

namespace Box.CMS.Web {

    public static class SiteApplication {

        private static CompositionContainer mefContainer;

        public static void RegisterWebApisRoutes(HttpConfiguration config) {

            config.Routes.MapHttpRoute(
                    name: "BoxApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = System.Web.Http.RouteParameter.Optional },
                    constraints: new { id = new ParamNotAllowedForMethod("ALL"), folder = new ParamNotAllowedForMethod("ALL") });

            config.Routes.MapHttpRoute(
                name: "BoxApiAction",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional },
                constraints: new { id = new ParamNotAllowedForMethod("ALL"), folder = new ParamNotAllowedForMethod("ALL") });

        }

        public static void RegisterMVCRoutes() {
            RouteTable.Routes.MapRoute(
                    name: "File",
                    url: "files/{folder}/{id}",
                    defaults: new { controller = "cms_filesReadOnly", action = "Index" }
                );

            RouteTable.Routes.MapRoute(
                    name: "FileCache",
                    url: "files-c/{folder}/{id}",
                    defaults: new { controller = "cms_filesReadOnly", action = "Cache" }
                );

            RouteTable.Routes.MapRoute(
                    name: "Captcha",
                    url: "captcha",
                    defaults: new { controller = "cms_Captcha", action = "Index" }
                );
        }

        public static void RegisterMefControllers(string pluginPath, HttpConfiguration config) {

            var pluginsCatalog = new DirectoryCatalog(pluginPath, "Box.*.dll");
            mefContainer = new CompositionContainer(pluginsCatalog, true);

            // FOR MVC CONTROLLERS
            System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(new Box.Composition.Web.MefControllerFactory(mefContainer));

            // FOR WEB API CONTROLLERS
            config.DependencyResolver = new Box.Composition.Web.MefDependencyResolver(mefContainer);

        }

        public static void AuthorizeRequest() {
            if (mefContainer == null)
                return;
            var authrorizator = mefContainer.GetExportedValue<Box.Composition.Services.IAuthorizationService>();
            authrorizator.AuthenticateRequestPrincipal();
        }

        public static void SetUserCulture() {

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
