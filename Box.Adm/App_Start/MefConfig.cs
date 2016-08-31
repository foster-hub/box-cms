using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Web.Mvc;
using System.Configuration;

namespace Box.Adm {

    // see http://kennytordeur.blogspot.com.br/2012/08/mef-in-aspnet-mvc-4-and-webapi.html

    public static class MefConfig {

        public static CompositionContainer Container { get; private set; }

        public static void RegisterMef(string pluginPath = "bin-plugins") {

            ConfigureContainer(pluginPath);

            ControllerBuilder.Current.SetControllerFactory(new Box.Composition.Web.MefControllerFactory(Container));
                
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Box.Composition.Web.MefDependencyResolver(Container);

            IEnumerable<Box.Composition.IAppStart> starts = Container.GetExportedValues<Box.Composition.IAppStart>();
            foreach (Box.Composition.IAppStart s in starts)
                s.OnStart(Container);
        }

        private static void ConfigureContainer(string pluginPath) {

            // plugins at different folder (medium trust IIs)
            if (PluginFolder) {
                var pluginsCatalog = new DirectoryCatalog(pluginPath, "*.dll");
                var boxCatalog = new DirectoryCatalog("bin", "Box*.dll");                
                var catalog = new AggregateCatalog(boxCatalog, pluginsCatalog);
                Container = new CompositionContainer(catalog, true);
            }

            // plugins at bin folder (may have some problems at medium trust IIs)
            else {
                var pluginsCatalog = new DirectoryCatalog("bin", "*.dll");
                Container = new CompositionContainer(pluginsCatalog, true);
            }
        }


        public static bool PluginFolder {
            get {
                object isOn = ConfigurationManager.AppSettings["PLUGIN_FOLDER"];
                if (isOn == null)
                    return false;

                bool isOnBool = false;
                Boolean.TryParse(isOn.ToString(), out isOnBool);

                return isOnBool;
            }
        }
    }
}