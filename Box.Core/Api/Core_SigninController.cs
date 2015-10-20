using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using Box.Core.Services;

namespace Box.Core.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_SigninController : ApiController {

        [Import]
        private SecurityService service { get; set; }

        [System.Web.Http.AllowAnonymous]
        
        [Box.Core.Web.WebApiAntiForgery]
        public int Post(User user) {
            if (user == null)
                return 0;
            if (user.Password == null)
                return 0;
            return service.SignInUser(user.Email, user.Password.Password);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [HttpPost, ActionName("ResetPassword")]
        public bool ResetPassword(string id) {
            return service.ResetPassword(id);
        }

    }
}
