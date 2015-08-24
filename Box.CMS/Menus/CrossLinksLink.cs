using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;
using System.ComponentModel.Composition;

namespace Box.CMS.Menus {

    [Export(typeof(IMenuActionLink))]
    public class CrossLinksLink : IMenuActionLink {

        [Import]
        private Groups.ADM_CMS ADM_CMS { get; set; }

        public string Name {
            get { return SharedStrings.Cross_Links; }
        }

        public string ActionLink {
            get { return "~/cms_crosslinks"; }
        }

        public string MenuLocal {
            get { return "TOP"; }
        }


        public string[] Requires {
            get {
                return new string[] { ADM_CMS.UserGroupUId };
            }
        }

        public int DisplayOrder {
            get {
                return 1;
            }
        }

    }

}
