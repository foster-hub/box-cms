using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.Composition {

    public class BoxPrincipal : System.Security.Principal.GenericPrincipal {

        public BoxPrincipal(System.Security.Principal.IIdentity identity, string[] roles, string displayName, string email) : base(identity, roles) {
            DisplayName = displayName;
            Email = email;
        }

        public string DisplayName { get; private set; }
        public string Email { get; private set; }
    }
}
