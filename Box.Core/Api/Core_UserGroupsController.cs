using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using Box.Core.Services;

namespace Box.Core.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_UserGroupsController : ApiController {

        [Import]
        private SecurityService securityService { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public IEnumerable<IUserGroup> Get(string filter = null, string fromUser = null, string groupCollectionId = null, int skip = 0, int top = 0) {
            if (!String.IsNullOrEmpty(fromUser))
                return securityService.GetGroupsFromUser(fromUser);

            return securityService.GetAllGroups();
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public IEnumerable<IUserGroup> Get(string fromGroupCollection) {
            return securityService.GetGroupsFromGroupCollection(fromGroupCollection);
        }
    }
}
