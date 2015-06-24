using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.ContactForms {

    public class ContactForm {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string ContactFormUId { get; set; }

        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ContactFormNumber { get; set; }

        [MaxLength(255)]
        public string ContactName { get; set; }

        [MaxLength(255)]
        public string ContactEmail { get; set; }

        [MaxLength(255)]
        public string Subject { get; set; }

        [MaxLength(255)]
        public string TargetEmail { get; set; }

        [MaxLength(255)]
        public string ContactPersonalId { get; set; }

        [MaxLength(255)]
        public string ContactPersonalId2 { get; set; }

        [MaxLength(50)]
        public string ContactCountry { get; set; }

        [MaxLength(10)]
        public string ContactState { get; set; }

        [MaxLength(150)]
        public string ContactCity { get; set; }

        [MaxLength(150)]
        public string ShortMessage { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public string Message { get; set; }

        public ContactFormData Data { get; set; }

        public DateTime CreateDate { get; set; }

    }


    public class ContactFormData {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string ContactFormId { get; set; }

        public string Data { get; set; }

    }
}
