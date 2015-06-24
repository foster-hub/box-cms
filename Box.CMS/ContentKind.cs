using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.CMS {

    public class ContentKind {

        public string Kind { get; set; }

        public string FriendlyName { get; set; }

        public string FriendlyPluralName { get; set; }

        public string CaptureController { get; set; }

        public string CaptureDetailView { get; set; }

        public bool AnyTimeDefaultFilter { get; set; }

        public bool NameDefaultFilter { get; set; }
        
        public string RenderView { get; set; }

        public int DisplayOrder { get; set; }

        public bool? Browsable { get; set; }

        public string[] Locations { get; set; }

        public string[] Tags { get; set; }

        public string[] RequiredRolesToEdit { get; set; }

        public string[] RequiredRolesToView { get; set; }

    }

}
