using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;

namespace Box.ContactForms.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContactForms_FormsController : ApiController{

        [Import]
        private Services.ContentFormsService formsService { get; set; }
        
        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_FORMS")]
        public ContactForm Get(string id) {
            return formsService.GetForm(id);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_FORMS")]
        public IEnumerable<ContactForm> Get(string filter = null, int skip = 0, int top = 0, string order = "Date", DateTime? createdFrom = null, DateTime? createdTo = null) {
            return formsService.GetForms(filter, skip, top, order, createdFrom, createdTo);
        }



    }
}
