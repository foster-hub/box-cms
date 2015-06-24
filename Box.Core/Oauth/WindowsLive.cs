using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Web;

namespace Box.Core.Oauth {


    [Export]
    public class WindowsLive : Generic {

        protected override string ID {
            get {
                return "WL";
            }            
        }

        protected override string _GetUserEmail(dynamic user) {            
            if (user["emails"] == null)
                return null;
            return user["emails"]["account"];                
        }

  
    }
}
