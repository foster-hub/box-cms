using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Box.Core.Data {


    public class CoreContextInitializer : CreateDatabaseIfNotExists<CoreContext>  {

        protected override void Seed(CoreContext context) {

            // creates empty admin group
            GroupCollection adminGroup = new GroupCollection() {
                GroupCollectionUId = Services.SecurityService.ADMIN_GROUP_ID,
                Name = SharedStrings.Admin_group,
                Description = SharedStrings.Admin_group_description
            };
            context.GroupCollections.Add(adminGroup);
            
            // creates a default user for windows nt            
            var principal = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (principal!=null && !principal.IsSystem) {

                try {
                    string email = System.DirectoryServices.AccountManagement.UserPrincipal.Current.EmailAddress;
                    string name = System.DirectoryServices.AccountManagement.UserPrincipal.Current.Name;

                    User userNT = new User() { LoginNT = principal.Name, Email = email, Blocked = false, Name = name };

                    CreateADM(userNT, context);
            
                } catch (Exception) {}
            } 
            
            // creates the default user for forms
            User admin = new User() { Email = "adm@localhost", Blocked = false, Name = "Admin" };
            admin.Password = new UserPassword() { Email = admin.Email, Password = "34be958a921e43d813a2075297d8e862" }; // PASSWORD: box
            CreateADM(admin, context);

            context.SaveChanges();

            context.Database.ExecuteSqlCommand("SELECT * INTO __MigrationHistory_Core FROM __MigrationHistory");
            context.Database.ExecuteSqlCommand("DROP TABLE __MigrationHistory");

        }


        private void CreateADM(User user, CoreContext context) {
            user.GroupCollectionMemberships = new GroupCollectionMembership[1] {
                new GroupCollectionMembership() { Email =  user.Email, GroupCollectionUId = "ADMIN----167e-42a3-abb2-9e3f7ba2074d" }
            };
            context.Users.Add(user);            
        }

        private string DomainToEmail(string login) {
            int idx = login.IndexOf("\\");
            if (idx <= 0)
                return login;
            string domain = login.Substring(0, idx);
            string name = login.Substring(idx+1);
            return (name + "@" + domain).ToLower();
        }
        
    }
}
