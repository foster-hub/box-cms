using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.Core.Web {

    public class WebApiAntiForgeryAttribute : System.Web.Http.Filters.ActionFilterAttribute {

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            
            string cookieToken = "", formToken = "";

            IEnumerable<string> headers;
            actionContext.Request.Headers.TryGetValues("RequestVerificationToken", out headers);
            if (headers!=null && headers.Count() >= 0) {
                string[] tokens = headers.First().Split(':');
                if (tokens.Length == 2) {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }

            System.Web.Helpers.AntiForgery.Validate(cookieToken, formToken);

            base.OnActionExecuting(actionContext);

        }

    }
}
