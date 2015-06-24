using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAutomation; 

namespace Box.Tests {

    public abstract class BasicTest : FluentTest {

        public BasicTest(SeleniumWebDriver.Browser browser) {
            SeleniumWebDriver.Bootstrap(browser);
        }

        
    }
}
