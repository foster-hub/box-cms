namespace Box.People.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Box.People.Data.PeopleContext> {

        public Configuration() {
            AutomaticMigrationsEnabled = true;            
        }

        
    }
}
