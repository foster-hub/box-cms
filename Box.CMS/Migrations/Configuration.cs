namespace Box.CMS.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Box.CMS.Data.CMSContext> {

        public Configuration() {
            AutomaticMigrationsEnabled = true;            
        }

        
    }
}
