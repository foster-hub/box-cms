using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.Composition;

namespace Box.ContactForms.Groups {

    [Export]
    [Export(typeof(IUserGroup))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ADM_FORMS : IUserGroup {

        public string UserGroupUId {
            get { return "ADM_FORMS"; }
        }

        public string Name {
            get { return SharedStrings.ContactForms_UserGroupName; }
        }

        public string Description {
            get { return SharedStrings.ContactForms_UserGroupDescription; }
        }
    }


}
