using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Box.Core.Data;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Box.Core.Migrations {

    internal sealed class Configuration : DbMigrationsConfiguration<Box.Core.Data.CoreContext> {

        public Configuration() {
            AutomaticMigrationsEnabled = true;            
        }

    }
}
