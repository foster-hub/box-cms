using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.ComponentModel.Composition;
using Box.Core.Services;
using Box.Composition;

namespace Box.Core.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_GroupsCollectionController : ApiController {
        
        [Import]
        private SecurityService securityService { get; set; }

        [Import]
        private LogService log { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        [PaginationFilter]
        public IEnumerable<GroupCollection> Get(string filter = null, int skip = 0, int top = 0, string order = "")
        {
            int totalRecords = 0;
            IEnumerable<GroupCollection> return_ = securityService.GetAllGroupCollection(ref totalRecords, filter);
            Request.Properties["count"] = totalRecords.ToString();
            return return_;
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public IEnumerable<GroupCollection> Get(string fromUserEmail) {
            return securityService.GetGroupCollectionFromUser(fromUserEmail);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public void Put(string id, GroupCollection groupCollection) {

            GroupCollection oldGroupCollection = securityService.GetGroupCollection(groupCollection.GroupCollectionUId);
            if (oldGroupCollection == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            // cant change e-mail
            if (oldGroupCollection.GroupCollectionUId.ToLower() != groupCollection.GroupCollectionUId.ToLower())
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);

            if (groupCollection.CollectionGroups == null)
                groupCollection.CollectionGroups = new GroupCollection_Group[0];

            securityService.SaveGroupCollection(groupCollection);

            log.Log(String.Format(SharedStringsLog.GROUP_UPDATE_0, groupCollection.GroupCollectionUId));
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC"), HttpPost]
        /* Some enviroments does not supports HTTP VERB PUT 
         * Use this workaround */
        public void UPDATE(string id, GroupCollection groupCollection) {
            Put(id, groupCollection);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public void Delete(string id) {
            securityService.RemoveGroupCollection(id);
            log.Log(String.Format(SharedStringsLog.GROUP_REMOVE_0, id));
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC"), HttpPost]
        /* Some enviroments does not supports HTTP VERB DELETE 
         * Use this workaround */
        public void REMOVE(string id) {
            Delete(id);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public GroupCollection Post(GroupCollection groupCollection) {
            groupCollection.GroupCollectionUId = Guid.NewGuid().ToString();
            log.Log(String.Format(SharedStringsLog.GROUP_CREATION_0, groupCollection.GroupCollectionUId));
            return securityService.SaveGroupCollection(groupCollection);
        }
    }
}
