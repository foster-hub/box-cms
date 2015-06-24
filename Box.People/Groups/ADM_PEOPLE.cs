using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.Composition;

namespace Box.People.Groups {

    [Export]
    [Export(typeof(IUserGroup))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ADM_PEOPLE : IUserGroup {

        public string UserGroupUId {
            get { return "ADM_PEOPLE"; }
        }

        public string Name {
            get { return SharedStrings.SecurityPeople_Name; }
        }

        public string Description {
            get { return SharedStrings.SecurityPeople_Description; }
        }
    }
}