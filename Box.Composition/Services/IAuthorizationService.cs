using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.Composition.Services {

    public interface IAuthorizationService {

        void AuthenticateRequestPrincipal();
    }
}
