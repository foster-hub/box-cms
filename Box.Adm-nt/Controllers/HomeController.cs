using Box.Core;
using Box.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Box.Adm_nt.Controllers {
    public class HomeController : Controller {

        private string NT_CALLBACK_URL {
            get {
                var ntCallback = ConfigurationManager.AppSettings["NT_CALLBACK"];
                if (String.IsNullOrEmpty(ntCallback))
                    ntCallback = "/adm";
                return ntCallback;
            }
        }
        
        //
        // GET: /Home/
        public ActionResult Index() {
            SecurityService securityService = new SecurityService();

            int user = 0;
            string userLogin = "?";
            var identity = HttpContext.User.Identity;
            

            string urlRedirect = NT_CALLBACK_URL + "/Core_Signin/NTcallback/?token=";

            if (identity != null) {
                userLogin = identity.Name;
                user = securityService.SignInUserNT(userLogin);
            }

            if (user == 1) {
                UserToken token = securityService.GetAuthTokenByLoginNT(userLogin);
                return Redirect(urlRedirect + token.Token);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, String.Format(SharedStrings.Unauthorized_UserNT, userLogin));
        }

    }
}
