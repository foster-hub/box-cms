using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Box.Composition {

    public interface IUserGroup {
        
        string UserGroupUId { get; }

        string Name { get; }

        string Description { get;  }
    }
}
