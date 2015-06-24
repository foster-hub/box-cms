using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;
using System.ComponentModel.Composition;

namespace Box.People.Menus {

    [Export(typeof(IMenuActionLink))]
    public class PeopleLink : IMenuActionLink {

        [Import]
        private Groups.ADM_PEOPLE ADM_PEOPLE { get; set; }

        public string Name
        {
            get { return SharedStrings.People; }
        }

        public string ActionLink
        {
            get { return "~/people_group"; }
        }

        public string MenuLocal
        {
            get { return "TOP"; }
        }

        public string[] Requires
        {
            get
            {
                return new string[] { ADM_PEOPLE.UserGroupUId };
            }
        }

        public int DisplayOrder
        {
            get
            {
                return 1;
            }
        }
    }
}
