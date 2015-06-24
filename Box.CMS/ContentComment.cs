using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Box.CMS {

    public class ContentComment {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string CommentUId { get; set; }

        [Column(TypeName = "char"), MaxLength(36)]
        public string ContentUId { get; set; }

        [Column(TypeName = "char"), MaxLength(36)]
        public string ParentCommentUId { get; set; }

        public DateTime CommentDate { get; set; }

        [MaxLength(50)]
        public string Author { get; set; }

        public string Comment { get; set; }

        
        public short Status { get; set; }

        public short StartRank { get; set; }

        public int Position { get; set; }
        
    }
}
