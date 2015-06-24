using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Box.CMS {

    public class CrossLink {

        [Key, Column(Order = 1, TypeName="char"), MaxLength(36)]
        public string ContentUId { get; set; }

        [Key, Column(Order = 2), MaxLength(50)]
        public string PageArea { get; set; }

        public short DisplayOrder { get; set; }

    }

    public class CrossLinkArea {
        public string Area { get; set; }
        public int MaxLinks { get; set; }
    }

}
