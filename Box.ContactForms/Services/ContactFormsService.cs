using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.Composition;
using System.ComponentModel.Composition;


namespace Box.ContactForms.Services {

    [Export]
    public class ContentFormsService {

        public ContactForm GetForm(string formId) {
            using (var context = new Data.ContactFormsContext()) {
                return context.ContactForms.Include("Data").SingleOrDefault(f => f.ContactFormUId == formId);
            }
        }

        public IEnumerable<ContactForm> GetForms(string filter, int skip, int top, string order = "Date", DateTime? createdFrom = null, DateTime? createdTo = null) {

            using (var context = new Data.ContactFormsContext()) {
                IQueryable<ContactForm> forms = null;

                forms = context.ContactForms;

                if (createdFrom.HasValue)
                    forms = forms.Where(f => f.CreateDate >= createdFrom.Value);

                if (createdTo.HasValue)
                    forms = forms.Where(f => f.CreateDate <= createdTo.Value);

                if (!String.IsNullOrEmpty(filter)) {
                    filter = filter.ToLower();
                    forms = forms.Where(f =>
                        f.ContactName.ToLower().Contains(filter) ||
                        f.ContactEmail.ToLower().Contains(filter) ||
                        f.ContactPersonalId.ToLower().Contains(filter) ||
                        f.ContactPersonalId2.ToLower().Contains(filter) ||
                        f.Subject.ToLower().Contains(filter) ||
                        f.ShortMessage.ToLower().Contains(filter) ||
                        f.ContactCountry.ToLower().Equals(filter) ||
                        f.ContactCity.ToLower().Equals(filter) ||
                        f.ContactState.ToLower().Equals(filter));
                }

                forms = OrderForms(forms, order);
              
                if (skip != 0)
                    forms = forms.Skip(skip);

                if (top != 0)
                    forms = forms.Take(top);

                return forms.ToArray();
            }
        }


        private IQueryable<ContactForm> OrderForms(IQueryable<ContactForm> forms, string order) {            
            if (order == "Date ASC")
                return forms.OrderBy(f => f.CreateDate);
            return forms.OrderByDescending(d => d.CreateDate);
        }


        public ContactForm SaveContactForm(ContactForm form) {
            using (var context = new Data.ContactFormsContext()) {
                context.ContactForms.Add(form);
                
                context.SaveChanges();
            }
            return form;
        }
    }

    
}
