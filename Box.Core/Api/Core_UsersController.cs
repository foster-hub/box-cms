using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using Box.Core.Services;
using System.Net;


namespace Box.Core.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Core_UsersController : ApiController {
        
        [Import]
        private SecurityService service { get; set; }

        [Import]
        private LogService log { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        [PaginationFilter]
        public IEnumerable<User> Get(string filter = null, int skip = 0, int top = 0) {

            int totalRecords = 0;
            IEnumerable<User> return_ = service.GetUsers(ref totalRecords, filter, skip, top);
            Request.Properties["count"] = totalRecords.ToString();
            return return_;
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public User Get(string id) {
            return service.GetUser(id);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public User GetByLoginNT(string loginNT) {            
            return service.GetUserByLoginNT(loginNT);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public User Post(User user) {

            User oldUser = Get(user.Email);

            if(oldUser!=null)
                throw new HttpResponseException(System.Net.HttpStatusCode.Conflict);

            if (user.Password != null && !service.ValidatePassword(user.Password.Password))
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);

            if (user.Memberships==null)
                user.Memberships = new GroupMembership[0];

            service.SaveUser(user);

            log.Log(String.Format(SharedStringsLog.USER_CREATION_0, user.Email), null, false);

            return user;
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public void Put(string id, User user) {

            User oldUser = Get(user.Email);
            
            if (oldUser == null)
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);

            // cant change e-mail
            if (oldUser.Email.ToLower() != user.Email.ToLower())
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
                        
            if (user.Password != null && !service.ValidatePassword(user.Password.Password))
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);

            if (user.Memberships==null)
                user.Memberships = new GroupMembership[0];

            if (user.GroupCollectionMemberships == null)
                user.GroupCollectionMemberships = new GroupCollectionMembership[0];

            log.Log(String.Format(SharedStringsLog.USER_UPDATE_0, user.Email), null, false);

            service.SaveUser(user);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC"), HttpPost]
        /* Some enviroments does not supports HTTP VERB PUT 
         * Use this workaround */
        public void UPDATE(string id, User user) {
            Put(id, user);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC")]
        public void Delete(string id) {
            service.RemoveUser(id);
            log.Log(String.Format(SharedStringsLog.USER_REMOVE_0, id));
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_SEC"), HttpPost]
        /* Some enviroments does not supports HTTP VERB DELETE 
         * Use this workaround */
        public void REMOVE(string id) {
            Delete(id);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize, HttpPost]
        public void Password(string id, [FromBody]string newPassword) {

            if(id!="me")
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);

            User user = service.GetSignedUser();
            if(user==null)
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);

            if(!service.ValidatePassword(newPassword))
                throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);

            user.Password = new UserPassword() { Email = user.Email, Password = newPassword };

            log.Log(String.Format(SharedStringsLog.USER_PASSWORD_UPDATE_0, user.Email), null, false);

            service.SaveUser(user);
        }
    }
}
