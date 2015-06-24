using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAutomation; 

namespace Box.Core.Tests.Pages {
    
    public class HomePage : Box.Tests.BasicPage {

        public HomePage(FluentTest test) : base(test, "/adm/") {
        }
    }
}
