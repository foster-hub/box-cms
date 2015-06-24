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
    public class ADM_SEC : IUserGroup {

        public string UserGroupUId {
            get { return "ADM_SEC"; }
        }

        public string Name {
            get { return SharedStrings.SecurityUserGroup_Name; }
        }

        public string Description {
            get { return SharedStrings.SecurityUserGroup_Description; ; }
        }
    }


}
