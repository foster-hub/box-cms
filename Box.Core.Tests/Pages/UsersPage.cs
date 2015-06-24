using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAutomation; 

namespace Box.Core.Tests.Pages {

    public class UsersPage : Box.Tests.BasicPage {

        public UsersPage(FluentTest test)
            : base(test, "/adm/core_users") {
        }


        public string AddNewButton = "button[onclick^=\"pageVM.setAddingItem({ Name: ''\"]";
        public string ApplyButton = "button[onclick=\"if (!$('#editForm').valid()) return; pageVM.syncPassword(); pageVM.applyItemChanges();\"]";

        public string EditForm = "#editForm";

        public string UserEmail = "#userEmail";


        public string RequiredEmailAlert = "label[for=userEmail].error";

    }
}
