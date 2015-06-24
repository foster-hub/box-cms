using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.Composition.Web {

    public interface IPageModel {


        ICollection<IMenuActionLink> SettingsMenuItems { get; }
        ICollection<IMenuActionLink> TopMenuItems { get; }

        string UserDisplayName { get; }
        string UserEmail { get; }
    }
}
