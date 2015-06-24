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
    public abstract class SigninTest : Box.Tests.BasicTest {

        public SigninTest(SeleniumWebDriver.Browser browser) : base(browser) {}

        /// <summary>
        /// Tests the a valid sign in.
        /// </summary>
        [TestMethod]
        public void VALID_SIGN() {

            var page = new Pages.SigninPage(this);

            page.Open();
            page.Signin("adm@localhost", "box");
            I.Assert.Url(new Pages.HomePage(this).Url);
        }

        /// <summary>
        /// Tests an invalid sign in.
        /// </summary>
        [TestMethod]
        public void INVALID_SIGN() {

            var page = new Pages.SigninPage(this);

            page.Open();
            I.Assert.Not.Visible(page.InvalidUserAlert);
            page.Signin("xxx@xxxxx", "xxxxx");
            I.Assert.Visible(page.InvalidUserAlert);

        }
    }


    [TestClass]
    public class SigninTest_CHROME : SigninTest {
        public SigninTest_CHROME() : base(SeleniumWebDriver.Browser.Chrome) { }
    }

    [TestClass]
    public class SigninTest_IE : SigninTest {
        public SigninTest_IE() : base(SeleniumWebDriver.Browser.InternetExplorer) { }
    }
}
