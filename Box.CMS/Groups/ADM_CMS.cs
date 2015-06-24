using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.CMS;

namespace Box.CMS.Groups {

    [Export]
    [Export(typeof(IUserGroup))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ADM_CMS : IUserGroup {

        public string UserGroupUId {
            get { return "ADM_CMS"; }
        }

        public string Name {
            get { return SharedStrings.CMSUserGroup_Name; }
        }

        public string Description {
            get { return SharedStrings.CMSUserGroup_Description; }
        }
    }


}
