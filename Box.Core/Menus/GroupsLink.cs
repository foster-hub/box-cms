using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;
using System.ComponentModel.Composition;

namespace Box.Core.Menus {

    [Export(typeof(IMenuActionLink))]
    public class GroupsLink : IMenuActionLink {

        [Import]
        private Groups.ADM_SEC ADM_SEC { get; set; }

        public string Name {
            get { return SharedStrings.Manage_groups; }
        }

        public string ActionLink {
            get { return "~/core_groupscollection"; }
        }

        public string MenuLocal {
            get { return "SETTINGS"; }
        }


        public string[] Requires {
            get {
                return new string[] { ADM_SEC.UserGroupUId };
            }
        }

        public int DisplayOrder {
            get {
                return 2;
            }
        }

    }

}
