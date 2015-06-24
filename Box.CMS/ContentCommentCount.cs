using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Box.CMS {

    
    public class ContentCommentCount {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string ContentUId { get; set; }

        public int Count { get; set; }
        
    }
}
