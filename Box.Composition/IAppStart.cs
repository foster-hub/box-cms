using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.Composition {
    public interface IAppStart {

        void OnStart(System.ComponentModel.Composition.Hosting.CompositionContainer container);
    }
}
