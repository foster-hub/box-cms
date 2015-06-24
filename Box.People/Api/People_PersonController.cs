using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;


namespace Box.People.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class People_PersonController : ApiController{
        
        [Import]
        private Services.PeopleService peopleService { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_PEOPLE")]
        public IEnumerable<Person> Get(string filter = null, int skip = 0, int top = 0, string optin = "All", string group = "All") {
            return peopleService.GetPeople(filter, skip, top, optin, group);
        }
    }
}
