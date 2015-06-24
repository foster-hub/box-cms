using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.Composition.Services {

    public interface IMenuService {

        IMenuActionLink[] TopMenuItems { get; }

        IMenuActionLink[] SettingsMenuItems { get; }

        IMenuActionLink[] GetMenuItems(string local);

    }
}
