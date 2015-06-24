using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;
using Box.Composition.Services;
using System.ComponentModel.Composition;

namespace Box.Core.Services {

    [Export(typeof(IMenuService))]
    public class MenuService : IMenuService {

        [ImportMany]
        private IEnumerable<IMenuActionLink> actionLinks { get; set; }

        public MenuService() { }

        public MenuService(IEnumerable<IMenuActionLink> actionLinks) {
            this.actionLinks = actionLinks;
        }

        public IMenuActionLink[] SettingsMenuItems {
            get {
                return GetMenuItems("SETTINGS");
            }
        }

        public IMenuActionLink[] GetMenuItems(string local) {
            System.Security.Principal.IPrincipal principal = System.Threading.Thread.CurrentPrincipal;
            return actionLinks.OrderBy(l => l.DisplayOrder).Where(l => l.MenuLocal == local && (l.Requires==null || l.Requires.Any(r => principal.IsInRole(r)))).ToArray();
        }


        public IMenuActionLink[] TopMenuItems {
            get {
                return GetMenuItems("TOP");
            }
        }
    }

}
