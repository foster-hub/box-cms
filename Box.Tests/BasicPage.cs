using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAutomation; 

namespace Box.Tests {

    public class BasicPage : PageObject {

        protected FluentTest test;

        public string HOST {
            get {                
                return Properties.Settings.Default.WEB_HOST;
            }
        }

        public BasicPage(FluentTest test, string relativeUrl) {
            this.test = test;

            Url = HOST + relativeUrl;

            
        }

        public void Open() {
            test.I.Open(Url);
            CheckBasicPage();
        }

        protected void CheckBasicPage() {
            test.I.Assert.Exists("title");
        }


    }
}
