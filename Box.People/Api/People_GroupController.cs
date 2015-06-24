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
    public class People_GroupController : ApiController {

        [Import]
        private Services.PeopleService peopleService { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_PEOPLE")]
        public IEnumerable<string> Get(int skip = 0, int top = 0) {
            return peopleService.GetGroups(skip, top);
        }
    }
}
