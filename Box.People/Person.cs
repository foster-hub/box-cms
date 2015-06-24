using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.People {

    public class Person {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string PersonUId { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }
        
        [MaxLength(10)]
        public string ZipCode { get; set; }

        [MaxLength(15)]
        public string Phone1 { get; set; }

        [MaxLength(15)]
        public string Phone2 { get; set; }

        [MaxLength(15)]
        public string Phone3 { get; set; }

        [MaxLength(35)]
        public string Job { get; set; }

        [MaxLength(35)]
        public string Role { get; set; }

        [MaxLength(35)]
        public string MarketField { get; set; }

        [MaxLength(50)]
        public string PersonalId1 { get; set; }

        [MaxLength(50)]
        public string PersonalId2 { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Surename { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Address1 { get; set; }

        [MaxLength(200)]
        public string Address2 { get; set; }

        [MaxLength(20)]
        public string Title { get; set; }

        [MaxLength(50)]
        public string Company { get; set; }

        [MaxLength(100)]
        public string RegisterSource { get; set; }

        [MaxLength(100)]
        public string GroupName { get; set; }

        public bool EmailOptIn { get; set; }

        public bool SMSOptIn { get; set; }

        public bool MailOptIn { get; set; }

        public DateTime RegisterDate { get; set; }
        
        public DateTime? Birthday { get; set; }
        
        public DateTime? EmailOptOutDate { get; set; }

        public DateTime? SMSOptOutDate { get; set; }

        public DateTime? MailOptOutDate { get; set; }


        public override string ToString() {
            return
                "\"" + this.PersonUId + "\";" +
                "\"" + this.Name + "\";" +
                "\"" + this.Surename + "\";" +
                "\"" + this.Title + "\";" +
                "\"" + this.Email + "\";" +
                "\"" + this.GroupName + "\";" +
                "\"" + this.Birthday + "\";" +
                "\"" + this.Gender + "\";" +
                "\"" + this.Job + "\";" +
                "\"" + this.PersonalId1 + "\";" +
                "\"" + this.PersonalId2 + "\";" +
                "\"" + this.Phone1 + "\";" +
                "\"" + this.Phone2 + "\";" +
                "\"" + this.Phone3 + "\";" +
                "\"" + this.Address1 + "\";" +
                "\"" + this.Address2 + "\";" +
                "\"" + this.Company + "\";" +
                "\"" + this.GetEmailOptInText() + "\";" +
                "\"" + this.EmailOptOutDate + "\";" +
                "\"" + this.GetMailOptInText() + "\";" +
                "\"" + this.MailOptOutDate + "\";" +
                "\"" + this.MarketField + "\";" +
                "\"" + this.RegisterDate + "\";" +
                "\"" + this.RegisterSource + "\";" +
                "\"" + this.Role +"\";" +
                "\"" + this.GetSMSOptInText() + "\";" +
                "\"" + this.SMSOptOutDate + "\";" +
                "\"" + this.State + "\";" +
                "\"" + this.ZipCode + "\";" +
                "\"" + this.Country + "\";" +
                "\"" + this.City + "\";";
        }

        private string GetEmailOptInText() {
            return this.EmailOptIn ? "Sim" : "Não";
        }

        private string GetMailOptInText() {
            return this.MailOptIn ? "Sim" : "Não";
        }

        private string GetSMSOptInText() {
            return this.SMSOptIn ? "Sim" : "Não";
        }
    }
}
