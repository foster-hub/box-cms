using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.Helpers;

namespace Box.CMS.Services {

    [Export]
    public class CaptchaService {

        private string inputFieldName = null;

        

        public CaptchaService() {
            
        }

        public CaptchaService(string inputFieldName, int? width = null, int? height = null, int? fontSize = null, string backgroundColorName = null, string textColorName = null) {
            this.inputFieldName = inputFieldName;
            this.BackgroundColorName = backgroundColorName;
            this.TextColorName = textColorName;
            this.ImageWidth = width;
            this.ImageHeight = height;
            this.FontSize = fontSize;
        }

        internal string CurrentText {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session["_CAPTCHA_TEXT"]==null)
                    return null;
                return HttpContext.Current.Session["_CAPTCHA_TEXT"].ToString();
            }
            set {
                if (HttpContext.Current == null)
                    return;
                HttpContext.Current.Session["_CAPTCHA_TEXT"] = value;
            }
        }

        public string BackgroundColorName {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session["_CAPTCHA_BG"] == null)
                    return null;
                return HttpContext.Current.Session["_CAPTCHA_BG"].ToString();
            }
            private set {
                if (HttpContext.Current == null)
                    return;
                HttpContext.Current.Session["_CAPTCHA_BG"] = value;
            }
        }

        public string TextColorName {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session["_CAPTCHA_FG"] == null)
                    return null;
                return HttpContext.Current.Session["_CAPTCHA_FG"].ToString();
            }
            private set {
                if (HttpContext.Current == null)
                    return;
                HttpContext.Current.Session["_CAPTCHA_FG"] = value;
            }
        }

        public int? ImageWidth {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session["_CAPTCHA_WIDTH"] == null)
                    return null;
                return int.Parse(HttpContext.Current.Session["_CAPTCHA_WIDTH"].ToString());
            }
            private set {
                if (HttpContext.Current == null)
                    return;
                HttpContext.Current.Session["_CAPTCHA_WIDTH"] = value;
            }
        }

        public int? ImageHeight {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session["_CAPTCHA_HEIGHT"] == null)
                    return null;
                return int.Parse(HttpContext.Current.Session["_CAPTCHA_HEIGHT"].ToString());
            }
            private set {
                if (HttpContext.Current == null)
                    return;
                HttpContext.Current.Session["_CAPTCHA_HEIGHT"] = value;
            }
        }

        public int? FontSize {
            get {
                if (HttpContext.Current == null || HttpContext.Current.Session["_CAPTCHA_SIZE"] == null)
                    return null;
                return int.Parse(HttpContext.Current.Session["_CAPTCHA_SIZE"].ToString());
            }
            private set {
                if (HttpContext.Current == null)
                    return;
                HttpContext.Current.Session["_CAPTCHA_SIZE"] = value;
            }
        }

        

        internal void GenerateNewText(int length) {
            int intZero = '1';
            int intNine = '9';
            int intA = 'A';
            int intZ = 'Z';
            int intCount = 0;
            int intRandomNumber = 0;
            string strCaptchaString = "";

            Random random = new Random(DateTime.Now.Millisecond);

            while (intCount < length) {
                intRandomNumber = random.Next(intZero, intZ);
                if (((intRandomNumber >= intZero) && (intRandomNumber <= intNine) || (intRandomNumber >= intA) && (intRandomNumber <= intZ))) {
                    strCaptchaString = strCaptchaString + (char)intRandomNumber;
                    intCount = intCount + 1;
                }
            }
            CurrentText = strCaptchaString;
        }

        public bool IsValidCaptcha() {
            
            if (String.IsNullOrEmpty(inputFieldName))
                return false;

            if (HttpContext.Current == null)
                return false;

            return IsValidCaptcha(HttpContext.Current.Request.Form[inputFieldName]);            

        }

        public bool IsValidCaptcha(string text) {
            if (text == null || CurrentText == null)
                return false;
            return text.ToLower().Equals(CurrentText.ToLower());
        }

        public IHtmlString Image() {
            string url = System.Web.HttpContext.Current.Request.ApplicationPath + "captcha";
            return new HtmlString("<img id=\"__captchaIMG\" src=\"" + url + "\"/>");
        }

        public string RefreshCaptchaJS {
            get {             
                string url = System.Web.HttpContext.Current.Request.ApplicationPath + "captcha?r=";
                return "document.getElementById('__captchaIMG').src='" + url + "' + Math.random();return false;";
            }
        }

        public void ValidateAntiForgery() {
            string cookieToken = "";
            string formToken = "";

            if (HttpContext.Current == null)
                return;

            if (HttpContext.Current.Request["__RequestVerificationToken"] == null)
                return;

            string[] tokens = HttpContext.Current.Request["__RequestVerificationToken"].Split(Convert.ToChar(":"));

            if (tokens.Length == 2) {
                cookieToken = tokens[0].Trim();
                formToken = tokens[1].Trim();
            }
            
            System.Web.Helpers.AntiForgery.Validate(cookieToken, formToken);
        }

        public IHtmlString AntiForgeryField() {
            var key = AntiForgeryToken();
            return new HtmlString("<input value=\"" + key + "\" type=\"hidden\" name=\"__RequestVerificationToken\" />");
        }

        public string AntiForgeryToken() {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return cookieToken + ":" + formToken;
            
        }
    }
}
