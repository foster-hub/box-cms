using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;
using System.ComponentModel.Composition;

namespace Box.ContactForms.Menus {

    [Export(typeof(IMenuActionLink))]
    public class ContactFormsLink : IMenuActionLink {

        [Import]
        private Groups.ADM_FORMS ADM_FORMS { get; set; }

        public string Name {
            get { return SharedStrings.Contact_Forms; }
        }

        public string ActionLink {
            get { return "~/contactforms_forms"; }
        }

        public string MenuLocal {
            get { return "TOP"; }
        }


        public string[] Requires {
            get {
                return new string[] { ADM_FORMS.UserGroupUId };
            }
        }

        public int DisplayOrder {
            get {
                return 1;
            }
        }

    }

}
