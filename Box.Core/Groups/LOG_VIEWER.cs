using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.Composition;

namespace Box.Core.Groups {
    
    [Export]
    [Export(typeof(IUserGroup))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LOG_VIEWER : IUserGroup {

        public string UserGroupUId {
            get { return "LOG_VIEWER"; }
        }

        public string Name {
            get { return SharedStrings.LogUserGroup_Name; }
        }

        public string Description {
            get { return SharedStrings.LogUserGroup_Description; }
        }
    }
}
