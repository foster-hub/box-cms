using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;
using Box.Composition;
using System.Threading;

namespace Box.ContactForms.Services {

    public partial class EmailForm  {


        public EmailForm(string targetEmail, string templatePath = null, string templatePathAutoReply = null,  string replyTo = null) {

            SendError = null;
            ShouldShowForm = true;
            AutoReply = true;
            SaveEmail = true;
            Fields = new Dictionary<string,string>();
            Files = new Dictionary<string, System.Web.HttpPostedFileBase>();

            ReplyTo = replyTo;

            if (System.Web.HttpContext.Current == null) {
                SendError = "NO HTTP CONTEXT";
                ShouldShowForm = true;
                return;
            }

            TargetEmail = targetEmail;

            ReplyTo = replyTo;

            if (templatePath == null)
                TemplatePath = System.Web.HttpContext.Current.Request.MapPath("~/box_contactForms_templates/contact_email.cshtml");
            else
                TemplatePath = templatePath;

            if (templatePathAutoReply == null)
                TemplatePathAutoReply = System.Web.HttpContext.Current.Request.MapPath("~/box_contactForms_templates/contact_email_autoreply.cshtml");
            else
                TemplatePathAutoReply = templatePathAutoReply;

        }

        public bool AutoReply { get; set; }

        public bool SaveEmail { get; set; }

        public ContactForm Form { get; private set; }

        public Dictionary<string, string> Fields { get; private set; }

        public Dictionary<string, System.Web.HttpPostedFileBase> Files { get; private set; }

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

        private string ContactSenderEmailAccount {
            get {
                string email = System.Configuration.ConfigurationManager.AppSettings["CONTACT_SENDER_EMAIL"] as String;
                if (email == null)
                    email = "noreply@yourcompany.com";
                return email;
            }
        }

        private string ContactAutoReplySubject {
            get {
                string subject = System.Configuration.ConfigurationManager.AppSettings["CONTACT_AUTOREPLY_SUBJECT"] as String;
                if (subject == null)
                    subject = "Thahk you for your feedback.";
                return subject;
            }
        }

        public void AddField(string key) {
            Fields.Add(key, System.Web.HttpContext.Current.Request.Form[key]);
        }

        public void AddFile(System.Web.HttpPostedFileBase file, string name = null) {
            if (name == null) {                
                name = "file" + (Files.Count+1) + System.IO.Path.GetExtension(file.FileName);
            }
            Files.Add(name, file);
        }

        public string SendError { get; private set; }

        public bool ShouldShowForm { get; private set; }

        public string TargetEmail { get; private set; }
        
        public string TemplatePath { get; private set; }
        public string TemplatePathAutoReply { get; private set; }


        public string ReplyTo { get; private set; }

        public string Root {
            get {
                if (System.Web.HttpContext.Current == null)
                    return null;
                return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
            }
        }

        private void CreateForm() {

            var webForm = System.Web.HttpContext.Current.Request.Form;

            Form = new ContactForm();
            Form.ContactFormUId = Guid.NewGuid().ToString();
            Form.CreateDate = DateTime.Now.ToUniversalTime();
            Form.TargetEmail = TargetEmail;

            Form.Subject = webForm["subject"];
            Form.ContactEmail = webForm["contactEmail"];
            Form.ContactName = webForm["contactName"];
            Form.ContactPersonalId = webForm["contactPersonalId"];
            Form.ContactPersonalId2 = webForm["contactPersonalId2"];
            Form.ContactCountry = webForm["contactCountry"];
            Form.ContactState = webForm["contactState"];
            Form.ContactCity = webForm["contactCity"];            

            Form.Message = webForm["message"];

            if (webForm["message"] != null && webForm["message"].Length > 150)
                Form.ShortMessage = webForm["message"].Substring(0, 150);
            else
                Form.ShortMessage = webForm["message"];
        }

        public void Send(bool isPost = true) {

            if (!isPost)
                return;

            CreateForm();

            string mailMessage = null;
    
            try {
                mailMessage = SendEmail();
                if(AutoReply)
                    SendAutoReplyEmail();
            } catch (Exception ex) {
                SendError = ex.Message;
                ShouldShowForm = true;
                return;
            }

            if (SaveEmail)
                SaveMessage(mailMessage);

            SendError = null;
            ShouldShowForm = false;
        }

        private string RenderTemplate(string templatePath) {

            string template = System.IO.File.ReadAllText(templatePath, Encoding.Default);

            if(IsRazorEngineOn)
                return RazorEngine.Razor.Parse(template, this);

            string body = Box.Composition.EmailParser.FakeRazorParser(template, "@Model.", this);
            body = Box.Composition.EmailParser.FakeRazorParser(body, "@Model.Form.", this.Form);
            body = Box.Composition.EmailParser.FakeRazorParser(body, "@Model.Fields", this.Fields);

            return body;
        }

        private string SendEmail() {

            string body = RenderTemplate(TemplatePath);
            
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(ContactSenderEmailAccount, TargetEmail);
            msg.Subject = Form.Subject;
            if (ReplyTo != null)
                msg.ReplyToList.Add(ReplyTo);
            else
                msg.ReplyToList.Add(Form.ContactEmail);
            
            msg.IsBodyHtml = true;
            msg.Body = body;

            // Attach files
            foreach (var filename in Files.Keys) {
                msg.Attachments.Add(new System.Net.Mail.Attachment(Files[filename].InputStream, filename));
            }

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            ThreadPool.QueueUserWorkItem(t => {
                smtp.Send(msg);
            });

            return body;
        }

        private void SendAutoReplyEmail() {

            string body = RenderTemplate(TemplatePathAutoReply);
            
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(ContactSenderEmailAccount, Form.ContactEmail);
            msg.Subject = ContactAutoReplySubject;
            
            msg.IsBodyHtml = true;

            msg.Body = body;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            ThreadPool.QueueUserWorkItem(t => {
                smtp.Send(msg);
            });
        }

        private void SaveMessage(string body) {
            Services.ContentFormsService service = new ContentFormsService();
            Form.Data = new ContactFormData() { ContactFormId = Form.ContactFormUId, Data = body };
            service.SaveContactForm(Form);
        }


        public System.Web.IHtmlString MapTo(string fieldName) {
            var webForm = System.Web.HttpContext.Current.Request.Form;
            return new System.Web.HtmlString(" id=\"" + fieldName + "\" name=\"" + fieldName + "\" value=\"" + webForm[fieldName] + "\" ");
        }

        public System.Web.IHtmlString MapToSubject { get { return MapTo("Subject"); } }
        public System.Web.IHtmlString MapToContactEmail { get { return MapTo("ContactEmail"); } }
        public System.Web.IHtmlString MapToContactName { get { return MapTo("ContactName"); } }
        public System.Web.IHtmlString MapToContactPersonalId { get { return MapTo("ContactPersonalId"); } }
        public System.Web.IHtmlString MapToContactPersonalId2 { get { return MapTo("ContactPersonalId2"); } }
        public System.Web.IHtmlString MapToContactCountry { get { return MapTo("ContactCountry"); } }
        public System.Web.IHtmlString MapToContactState { get { return MapTo("ContactState"); } }
        public System.Web.IHtmlString MapToContactCity { get { return MapTo("ContactCity"); } }
        public System.Web.IHtmlString MapToMessage { get { return MapTo("Message"); } }
        
        //WE <3 GIT and GITHUB

    }
   
}
