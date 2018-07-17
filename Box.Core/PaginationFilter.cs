using System;
using System.Web.Http.Filters;

namespace Box.Core
{
    public class PaginationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                var count = actionExecutedContext.Request.Properties["count"];
                actionExecutedContext.Response.Content.Headers.Add("TotalRecords", count.ToString());
            }
            catch (Exception ex) { }
        }
    }
}
