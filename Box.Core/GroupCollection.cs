using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.Core {
    
    public class GroupCollection {

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Key, MaxLength(36), MinLength(36)]
        public string GroupCollectionUId { get; set; }

        public ICollection<GroupCollection_Group> CollectionGroups { get; set; }
    }

    public class GroupCollection_Group {
        [Key, Column(Order = 1), MaxLength(36), MinLength(36)]
        public string GroupCollectionUId { get; set; }

        [Key, Column(Order = 2), MaxLength(15)]
        public string UserGroupUId { get; set; }

    }
}
