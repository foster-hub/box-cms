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

    protected void Application_BeginRequest(Object source, EventArgs e) {

        // detect if DB was not configured
        Box.Core.Services.SecurityService.DetectNotInstalled();

    }

    protected void Application_Error(Object sender, EventArgs e) {
        Exception exception = Server.GetLastError();
        Response.Clear();
        HttpException validationException = exception.InnerException as System.Web.HttpRequestValidationException;
        if (validationException != null)
            Response.Redirect("/invalid-post");
    }
    
    protected void Application_AcquireRequestState(object sender, EventArgs e) {
        SiteApplication.SetUserCulture();
    }

    
</script>
