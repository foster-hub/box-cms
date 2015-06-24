using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.CMS;

namespace Box.CMS.Groups {

    
    public class ContentUserGroup : IUserGroup {

        public string UserGroupUId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


}
