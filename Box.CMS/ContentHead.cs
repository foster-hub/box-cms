using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.CMS {

    public enum ContentRanks {
        PageViews,
        Shares,
        Comments,
        Date
    }

    public enum Periods {
        AnyTime,
        LastHour,
        LastDay,
        Last5Days,
        Last30Days,
        Last150Days,
        Last360Days
    }

    public class ContentHead {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string ContentUId { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string CanonicalName { get; set; }       

        [MaxLength(25)]
        public string Kind { get; set; }

        [MaxLength(300)]
        public string Abstract { get; set; }

        [MaxLength(87)]
        public string ThumbFilePath { get; set; }

        [MaxLength(500)]
        public string Location { get; set; }

        public ContentData Data { get; set; }


        public DateTime CreateDate { get; set; }
        public DateTime ContentDate { get; set; }

        public short DisplayOrder { get; set; }

        public DateTime? PublishUntil { get; set; }
        public DateTime? PublishAfter { get; set; }

        public int Version { get; set; }

        public ICollection<ContentTag> Tags { get; set; }

        public ICollection<CrossLink> CrossLinks { get; set; }

        public ContentCommentCount CommentsCount { get; set; }

        public ContentShareCount ShareCount { get; set; }

        public ContentPageViewCount PageViewCount { get; set; }

        public ContentCustomInfo CustomInfo { get; set; }

        [MaxLength(500)]
        public string ExternalLinkUrl { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public dynamic CONTENT { get; set; }

        public string[] TagsToArray() {
            if(Tags==null)
                return new string[0];
            return Tags.Select(t => t.Tag).ToArray<string>();
        }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public int OrderIndex { get; set; }
    }

    public class ContentData {
        
        [Key, MaxLength(36)]
        public string ContentUId { get; set; }

        public string JSON { get; set; }
    }

}
