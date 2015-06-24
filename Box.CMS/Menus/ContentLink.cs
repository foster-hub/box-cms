using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Box.CMS.Menus {

    
    public class ContentLink : IMenuActionLink {


        private Groups.ADM_CMS ADM_CMS_GROUP = new Groups.ADM_CMS();

        public ContentLink() {
            this.MenuLocal = "TOP";
        }

        public ContentLink(string name, string link, string[] requires) {
            this.Name = name;
            this.ActionLink = link;
            this.MenuLocal = "TOP";
            this.Requires = requires;

            if (!Requires.Contains(ADM_CMS_GROUP.UserGroupUId)) {
                List<string> groups = new List<string>(Requires);
                groups.Add(ADM_CMS_GROUP.UserGroupUId);
                Requires = groups.ToArray();
            }
                
        }

        public string Name { get; set; }

        public string ActionLink { get; set; }

        public string MenuLocal {
            get;
            set;
        }

        public string[] Requires {
            get;
            set;
        }

        public int DisplayOrder {
            get;
            set;
        }

    }

}
