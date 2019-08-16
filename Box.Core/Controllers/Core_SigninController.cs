using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using System.Security.Principal;
using System.Net;
using System.Web.Http;
using Box.Core.Oauth;

namespace Box.Core.Controllers {


    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_SigninController : Controller {

        [Import]
        private Box.Core.Services.SecurityService service { get; set; }

        [Import]
        private Box.Core.Oauth.WindowsLive windowLive { get; set; }

        [Import]
        private Box.Core.Oauth.Azure azure { get; set; }

        [Import]
        private Box.Core.Oauth.Google google { get; set; }

        [Import]
        private Facebook Facebook { get; set; }

        [Import]
        private Services.LogService log { get; set; }
        
        [System.Web.Mvc.AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Index() {

            ViewData["WINDOWS_LIVE_LOGIN_URL"] = windowLive.LOGIN_URL;
            ViewData["AZURE_LOGIN_URL"] = azure.LOGIN_URL;
            ViewData["GOOGLE_LOGIN_URL"] = google.LOGIN_URL;
            ViewData["FACEBOOK_LOGIN_URL"] = Facebook.LOGIN_URL;

            ViewData["WINDOWS_AUTH_ENABLE"] = service.IsWindowsAuthEnable;
            ViewData["FORMS_AUTH_ENABLE"] = service.IsFormsAuthEnable;

            ViewData["WINDOWS_AUTH_URL"] = service.WindowsAuthUrl;

            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Out()
        {

            service.SignOutUser();
            return new RedirectResult("~/signin");
        }

        public ActionResult NTcallback(string token) {
            User user = service.GetUserByAuthToken(token);

            log.Log("Valid token for user " + user.LoginNT + " (" + user.Email + ") signing via network.");

            if (user == null || String.IsNullOrEmpty(user.Email))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, SharedStrings.Unauthorized_UserNT);


            return CallBack(user.Email);
        }

        public ActionResult WLcallback() {
            string email = windowLive.GetUserEmail(Request.QueryString["code"]);
            return CallBack(email);
        }

        public ActionResult AZUREcallback()
        {
            string email = azure.GetUserEmail(Request.QueryString["code"]);
            return CallBack(email);
        }

        public ActionResult Gcallback() {
            string email = google.GetUserEmail(Request.QueryString["code"]);
            return CallBack(email);
        }

        public ActionResult FBcallback()
        {
            string email = Facebook.GetUserEmail(Request.QueryString["code"]);
            return CallBack(email);
        }

        private ActionResult CallBack(string email) {
            string callBackMessage = null;

            if (email == null)
                callBackMessage = SharedStrings.Error_sigin;
            
            int signedin = service.SignInUser(email);
            
            if (signedin != 1)
                callBackMessage = SharedStrings.You_do_not_have_an_active_account_at_this_system;

            ViewData["callBackMessage"] = callBackMessage;

            if (signedin == 1) {

                log.Log("User " + email + " signed via callback.");

                string url = Request["ReturnUrl"];
                if (!String.IsNullOrEmpty(url))
                    Response.Redirect(url);

                return RedirectToAction("Index", "Home");
            }

            return View("CallBack");
        }


        public ActionResult ResetPassword() {
            return View();
        }

        public ActionResult ApplyNewPassword(string id) {

            bool applied = service.ApplyResetPassword(id);
            if (applied)
                ViewData["RESULT"] = SharedStrings.Your_password_was_changed;
            else
                ViewData["RESULT"] = SharedStrings.Could_not_change_your_password;

            return View();
        }
    }
}
