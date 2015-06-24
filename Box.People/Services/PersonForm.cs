using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.People;
using Box.Composition;
using System.Threading;

namespace Box.People.Services {


    public class PersonForm {

        Services.PeopleService pService = new Services.PeopleService();

        public Person person { get; private set; }

        public bool RegistrationComplete { get; private set; }

        public string SendError { get; private set; }

        private string ContactSenderEmailAccount {
            get {
                string email = System.Configuration.ConfigurationManager.AppSettings["CONTACT_SENDER_EMAIL"] as String;
                if (email == null)
                    email = "noreply@yourcompany.com";
                return email;
            }
        }

        private string RegistrationSubject {
            get {
                string subject = System.Configuration.ConfigurationManager.AppSettings["REGISTRATION_SUBJECT"] as String;
                if (subject == null)
                    subject = "Registration Sucefull";
                return subject;
            }
        }

        private bool IsRazorEngineOn {
            get {
                object isOn = System.Configuration.ConfigurationManager.AppSettings["RAZOR_ENGINE_ON"];
                if (isOn == null)
                    return true;

                bool isOnBool = true;
                Boolean.TryParse(isOn.ToString(), out isOnBool);

                return isOnBool;
            }
        }

        public void AddPerson(string registerSource, string groupName, string autoReplyEmailTemplate = "~/box_register_templates/Register_Email.cshtml") {
            
            try {
                CreatePerson(registerSource, groupName);
                SendEmail(autoReplyEmailTemplate);
                SavePerson();
                RegistrationComplete = true;
                SendError = null;
            }
            catch (Exception ex) {
                SendError = ex.Message;
                RegistrationComplete = false;
                return;
            }
        }

        public void CreatePerson(string registerSource, string groupName) {

            var webForm = System.Web.HttpContext.Current.Request.Form;
            person = new Person();
            person.PersonUId = Guid.NewGuid().ToString();
            person.Name = webForm["Name"];
            person.Surename = webForm["Surename"];
            person.Email = webForm["Email"];
            person.Address1 = webForm["Address1"];
            person.Address2 = webForm["Address2"];
            person.Gender = webForm["Gender"];
            person.ZipCode = webForm["ZipCode"];
            person.Phone1 = webForm["DDD1"] + webForm["Phone1"];
            person.Phone2 = webForm["DDD2"] + webForm["Phone2"];
            person.Phone3 = webForm["Phone3"];
            person.Job = webForm["Job"];
            person.Role = webForm["Role"];
            person.MarketField = webForm["MarketField"];
            person.State = webForm["State"];
            person.PersonalId1 = webForm["PersonalId1"];
            person.PersonalId2 = webForm["PersonalId2"];
            person.Country = webForm["Country"];
            person.State = webForm["State"];
            person.City = webForm["City"];
            person.Title = webForm["Title"];
            person.Company = webForm["Company"];
            person.GroupName = groupName;

            string personEmail = webForm["Email"];
            if (String.IsNullOrEmpty(personEmail))
                throw new System.ArgumentException(SharedStrings.EmailIsRequired);

            if (pService.IsEmailAlreadyRegistred(personEmail))
                throw new System.ArgumentException(SharedStrings.EmailsIsInvalid);

            person.Email = personEmail;
            
            DateTime? birthDay = null;
            string sBirthDay = webForm["Birthday"]; 
            if(!String.IsNullOrEmpty(sBirthDay))
                birthDay = Convert.ToDateTime(sBirthDay);

            person.Birthday = birthDay;
            person.RegisterDate = DateTime.Now.ToUniversalTime();
            person.RegisterSource = registerSource;

            bool emailOptin = false;
            if (webForm["newsletterEmail"] != null)
                emailOptin = true;

            person.EmailOptIn = emailOptin;

            bool smsOptin = false;
            if (webForm["newsletterSMS"] != null)
                smsOptin = true;

            person.SMSOptIn = smsOptin;

            bool mailOptin = false;
            if (webForm["newsletterMail"] != null)
                mailOptin = true;

            person.MailOptIn = mailOptin;
        }

        public void SavePerson() {
            pService.SavePerson(person);
        }

        public Person PutOptinChoicesInPerson(Person person) {

            var request = System.Web.HttpContext.Current.Request;
            person.EmailOptIn = request.Form["emailOptin"] != null && request.Form["emailOptin"] == "on";
            person.SMSOptIn = request.Form["smsOptin"] != null && request.Form["smsOptin"] == "on";
            person.MailOptIn = request.Form["mailOptin"] != null && request.Form["mailOptin"] == "on";

            DateTime? emailOptOutDate = null;
            DateTime? smsOptOutDate = null;
            DateTime? mailOptOutDate = null;

            if (!person.EmailOptIn)
                emailOptOutDate = DateTime.Now.ToUniversalTime();

            if (!person.SMSOptIn)
                smsOptOutDate = DateTime.Now.ToUniversalTime();

            if (!person.MailOptIn)
                mailOptOutDate = DateTime.Now.ToUniversalTime();

            person.EmailOptOutDate = emailOptOutDate;
            person.SMSOptOutDate = smsOptOutDate;
            person.MailOptOutDate = mailOptOutDate;

            return person;
        }

        private void SendEmail(string autoReplyEmailTemplate) {

            string template = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath(autoReplyEmailTemplate), Encoding.Default);
            string body = "";

            if (IsRazorEngineOn)
                body = RazorEngine.Razor.Parse(template, person);
            else {
                body = Box.Composition.EmailParser.FakeRazorParser(template, "@Model.", person);                
            }


            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(ContactSenderEmailAccount, person.Email);
            msg.Subject = RegistrationSubject; 

            msg.IsBodyHtml = true;

            msg.Body = body;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            
            ThreadPool.QueueUserWorkItem(t => {
                smtp.Send(msg);
            });

        }
    }
}
