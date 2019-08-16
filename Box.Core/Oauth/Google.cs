using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Web;
using Box.Core.Services;

namespace Box.Core.Oauth {


    [Export]
    public class Google : Generic {

        [Import]
        protected override LogService log { get; set; }

        protected override string ID {
            get {
                return "G";
            }
        }

        protected override string _GetUserEmail(dynamic user) {         
            return user["email"];                
        }

        protected override string CALLBACK_URL {
            get {                
                return HOST_NAME + "/core_signin/" + ID + "callback";
            }
        }

        
    }
}
