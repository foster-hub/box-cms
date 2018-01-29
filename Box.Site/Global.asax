<%@ Import namespace="System.Web.Http" %>
<%@ Import namespace="System.Web.Mvc" %>
<%@ Import namespace="Box.Composition" %>
<%@ Import namespace="Box.CMS.Web" %>

<%@ Application Language="C#" %>

<script runat="server">

    protected void Application_Start() {

        HttpConfiguration config = GlobalConfiguration.Configuration;
        
        // register the MEF controllers
        SiteApplication.RegisterMefControllers("bin", config);

        // register the default api routes
        SiteApplication.RegisterWebApisRoutes(config);

        // register MV routes
        SiteApplication.RegisterMVCRoutes();
                
    }

    protected void Application_Error(Object sender, EventArgs e) {
        Exception exception = Server.GetLastError();
        Response.Clear();

        var SQLexception = Box.Core.Services.SecurityService.GetSqlException(exception);

        if (SQLexception != null && !Box.Core.Services.SecurityService.IsDebug)
        {
            Response.Redirect("~/where-is-my-db.htm#" + SQLexception.Number);
            Response.End();
        }
        
        HttpException validationException = exception.InnerException as System.Web.HttpRequestValidationException;
        if (validationException != null)
            Response.Redirect("~/invalid-post");
    }
    
    protected void Application_AcquireRequestState(object sender, EventArgs e) {
        SiteApplication.SetUserCulture();
    }

     protected void Application_BeginRequest(object sender, EventArgs e) {
        HttpContext.Current.Response.AddHeader("x-frame-options", "SAMEORIGIN");
    }


    
</script>
