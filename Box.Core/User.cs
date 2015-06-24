using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.Core {


    public class User {

        [Key, MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string LoginNT { get; set; }

        public bool Blocked { get; set; }

        public short LoginErrorsCount { get; set; }
        
        public ICollection<GroupMembership> Memberships { get; set; }

        public ICollection<GroupCollectionMembership> GroupCollectionMemberships { get; set; }

        public UserPassword Password { get; set; }
    }

    public class GroupMembership {

        [Key, Column(Order = 1), MaxLength(255)]
        public string Email { get; set; }

        [Key, Column(Order = 2), MaxLength(15)]
        public string UserGroupUId { get; set; }
    }

    public class UserToken {

        [MaxLength(255)]
        public string Email { get; set; }

        [Key, Column(TypeName = "char"), MaxLength(36), MinLength(36)]
        public string Token { get; set; }

        public DateTime IssueDate { get; set; }
    }

    public class UserPassword {

        [Key, MaxLength(255)]        
        public string Email { get; set; }

        [MaxLength(50)]        
        public string Password { get; set; }
    }

    public class PasswordReset {

        [Key, MaxLength(255)]
        public string Email { get; set; }

        [Column(TypeName = "char"), MaxLength(36), MinLength(36)]
        public string Key { get; set; }

        [MaxLength(50)]    
        public string NewPassword { get; set; }

        public DateTime IssueDate { get; set; }

    }

    public class GroupCollectionMembership {
     
        [Key, Column(Order = 1), MaxLength(36), MinLength(36)]
        public string GroupCollectionUId { get; set; }

        [Key, Column(Order = 2), MaxLength(255)]
        public string Email { get; set; }

        public GroupCollection Collection { get; set; }
    }

}
