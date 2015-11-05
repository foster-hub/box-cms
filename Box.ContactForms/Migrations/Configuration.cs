namespace Box.ContactForms.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Box.ContactForms.Data.ContactFormsContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true;         
        }
    }
}
