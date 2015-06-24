using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Box.ContactForms.Data {


    public class ContactFormsContextInitializer : CreateDatabaseIfNotExists<ContactFormsContext> {

        protected override void Seed(ContactFormsContext context) {
           
            context.Database.ExecuteSqlCommand("SELECT * INTO __MigrationHistory_ContactForms FROM __MigrationHistory");
            context.Database.ExecuteSqlCommand("DROP TABLE __MigrationHistory");

        }

        
    }
}
