using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Box.Core.Data;

namespace Box.Core.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Box.Core.Data.CoreContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(Box.Core.Data.CoreContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //creates empty admin group
            GroupCollection adminGroup = new GroupCollection()
            {
                GroupCollectionUId = Services.SecurityService.ADMIN_GROUP_ID,
                Name = SharedStrings.Admin_group,
                Description = SharedStrings.Admin_group_description
            };
            context.GroupCollections.AddOrUpdate(adminGroup);

            // creates a default user for windows nt            
            var principal = WindowsIdentity.GetCurrent();
            if (principal != null && !principal.IsSystem)
            {

                try
                {
                    string email = UserPrincipal.Current.EmailAddress;
                    string name = UserPrincipal.Current.Name;

                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(name))
                    {
                        User userNT = new User() { LoginNT = principal.Name, Email = email, Blocked = false, Name = name };
                        context.Users.AddOrUpdate(userNT);
                    }

                }
                catch (Exception) { }
            }


            // creates the default user for forms
            User admin = new User() { Email = "adm@localhost", Blocked = false, Name = "Admin" };
            admin.Password = new UserPassword() { Email = admin.Email, Password = "34be958a921e43d813a2075297d8e862" }; // PASSWORD: box
            admin.GroupCollectionMemberships = new GroupCollectionMembership[1] {
                new GroupCollectionMembership() { Email =  admin.Email, GroupCollectionUId = "ADMIN----167e-42a3-abb2-9e3f7ba2074d" }
            };
            context.Users.AddOrUpdate(admin);
        }
    }
}
