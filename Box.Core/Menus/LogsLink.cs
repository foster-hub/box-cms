using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.Composition;

namespace Box.Core.Menus {
    
    [Export(typeof(IMenuActionLink))]
    public class LogsLink : IMenuActionLink {

        [Import]
        private Groups.LOG_VIEWER LOG_VIEWER { get; set; }

        public string Name {
            get { return "Logs"; }
        }

        public string ActionLink {
            get { return "~/core_logs"; }
        }

        public string MenuLocal {
            get { return "SETTINGS"; }
        }


        public string[] Requires {
            get {
                return new string[] { LOG_VIEWER.UserGroupUId };
            }
        }

        public int DisplayOrder {
            get {
                return 11;
            }
        }
    }
}
