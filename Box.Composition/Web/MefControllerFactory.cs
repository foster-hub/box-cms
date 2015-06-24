using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Web.Mvc;

namespace Box.Composition.Web {

    public class MefControllerFactory : DefaultControllerFactory {

        private readonly CompositionContainer _compositionContainer;

        public MefControllerFactory(CompositionContainer compositionContainer) {
            _compositionContainer = compositionContainer;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType) {
            IController controller = CreateMEFController(requestContext, controllerType);

            if(controller==null && controllerType!=null)
                controller = base.GetControllerInstance(requestContext, controllerType);

            return controller;            
        }

        public IController CreateMEFController(System.Web.Routing.RequestContext requestContext, Type controllerType) {
            // tryes to get it from MEF
            if (controllerType == null)
                return null;
            var export = _compositionContainer.GetExports(controllerType, null, null).FirstOrDefault();
            if (export == null)
                return null;

            return export.Value as IController;
        }

    }
}