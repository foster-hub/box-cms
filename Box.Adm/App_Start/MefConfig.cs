using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Web.Mvc;

namespace Box.Adm {

    // see http://kennytordeur.blogspot.com.br/2012/08/mef-in-aspnet-mvc-4-and-webapi.html

    public static class MefConfig {

        public static CompositionContainer Container { get; private set; }

        public static void RegisterMef(string pluginPath) {

            ConfigureContainer(pluginPath);

            ControllerBuilder.Current.SetControllerFactory(new Box.Composition.Web.MefControllerFactory(Container));
                
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Box.Composition.Web.MefDependencyResolver(Container);

            IEnumerable<Box.Composition.IAppStart> starts = Container.GetExportedValues<Box.Composition.IAppStart>();
            foreach (Box.Composition.IAppStart s in starts)
                s.OnStart(Container);
        }

        private static void ConfigureContainer(string pluginPath) {
            var pluginsCatalog = new DirectoryCatalog(pluginPath, "*.dll");            
            Container = new CompositionContainer(pluginsCatalog, true);
        }
    }
}