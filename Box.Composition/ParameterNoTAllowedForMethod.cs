using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Box.Composition {
    public class ParamNotAllowedForMethod : IRouteConstraint {
        string method;

        public ParamNotAllowedForMethod(string method) {
            this.method = method;
        }

        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection) {

                if (routeDirection == RouteDirection.IncomingRequest &&
                    (httpContext.Request.HttpMethod == method || method=="ALL") &&
                    httpContext.Request.QueryString[parameterName]!=null) {
                    return false;
                }

            return true;
        }
    }
}
