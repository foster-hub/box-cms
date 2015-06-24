using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAutomation; 

namespace Box.Core.Tests.Pages {

    public class SigninPage : Box.Tests.BasicPage {

        public SigninPage(FluentTest test) : base(test, "/adm/signin") {

            At = () => {                
                test.I.Expect.Exists(Email);
                test.I.Expect.Exists(Password);
                test.I.Expect.Exists(SigninButton);
                test.I.Expect.Exists(InvalidUserAlert);
            };
        }

        public void Signin(string email, string password) {
            test.I.Enter(email).In(Email);
            test.I.Enter(password).In(Password);
            test.I.Click(SigninButton);
        }

        
        public string Email = "input[data-bind='value: signedUser().Email']";
        public string Password = "input[data-bind^='value: signedUser().Password.Password']";
        public string SigninButton = "button[onclick='pageVM.signin()']";

        public string InvalidUserAlert = "span[data-bind=\"visible: loginResult()=='INVALID'\"]";
    }
}
