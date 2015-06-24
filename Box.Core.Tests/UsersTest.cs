using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAutomation; 

//
// http://fluent.stirno.com/the-journey-to-3-0/
//

namespace Box.Core.Tests {

    /// <summary>
    /// Sign tests.
    /// </summary>
    public abstract class UsersTest : Box.Tests.BasicTest {

        public UsersTest(SeleniumWebDriver.Browser browser) : base(browser) { }

        /// <summary>
        /// Tests the a valid sign in.
        /// </summary>
        [TestMethod]
        public void ADD_NEW_USER() {

            var signinPage = new Pages.SigninPage(this);
            signinPage.Open();
            signinPage.Signin("adm@localhost", "box");

            var page = new Pages.UsersPage(this);
            page.Open();

            // press the ADD BUTTON should show the edit form
            I.Assert.Not.Exists(page.EditForm);
            I.Click(page.AddNewButton);
            I.Assert.Exists(page.EditForm);
            
            // try to save without filling the form 
            I.Click(page.ApplyButton);
            I.Assert.Exists(page.RequiredEmailAlert);

            // try to use an invalid e-mail
            I.Enter("teste").In(page.UserEmail);
            I.Assert.Exists(page.RequiredEmailAlert);

            // try to use an valid e-mail
            I.Enter("teste@teste.com").In(page.UserEmail);
            I.Assert.Not.Visible(page.RequiredEmailAlert);

        }

        
    }


    [TestClass]
    public class UsersTest_CHROME : UsersTest {
        public UsersTest_CHROME() : base(SeleniumWebDriver.Browser.Chrome) { }
    }

    [TestClass]
    public class UsersTest_IE : UsersTest {
        public UsersTest_IE() : base(SeleniumWebDriver.Browser.InternetExplorer) { }
    }
}
