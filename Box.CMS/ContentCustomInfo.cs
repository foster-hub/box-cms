using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Box.CMS {

    
    [Table("ContentCustomInfos")]
    public class ContentCustomInfo {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string ContentUId { get; set; }

        [MaxLength(2000)]
        public string Text1 { get; set; }
        [MaxLength(2000)]
        public string Text2 { get; set; }
        [MaxLength(2000)]
        public string Text3 { get; set; }
        [MaxLength(2000)]
        public string Text4 { get; set; }

        public double? Number1 { get; set; }
        public double? Number2 { get; set; }
        public double? Number3 { get; set; }
        public double? Number4 { get; set; }

        public DateTime? Date1 { get; set; }
        public DateTime? Date2 { get; set; }
        public DateTime? Date3 { get; set; }
        public DateTime? Date4 { get; set; }

        
    }
}
