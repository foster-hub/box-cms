using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.Composition {

    public interface IMenuActionLink {

        string Name { get; }        
        string ActionLink { get; }
        string MenuLocal { get; }

        string[] Requires { get; }

        int DisplayOrder { get; }
    }
}
