using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Web;

namespace Box.Core.Oauth {


    
    public abstract class Generic {

        protected abstract string ID { get; }

        protected string CLIENT_ID {
            get {
                return System.Configuration.ConfigurationManager.AppSettings[ID + "_CLIENT_ID"] as String;
            }
        }
        protected string SECRET {
            get {
                return System.Configuration.ConfigurationManager.AppSettings[ID + "_SECRET"] as String;
            }
        }

        protected string GET_TOKEN_URL {
            get {
                return System.Configuration.ConfigurationManager.AppSettings[ID + "_GET_TOKEN_URL"] as String;
            }
        }
        protected string GET_USER_URL {
            get {
                return System.Configuration.ConfigurationManager.AppSettings[ID + "_GET_USER_URL"] as String;
            }
        }

        protected string HOST_NAME {
            get {
                if (System.Web.HttpContext.Current == null)
                    throw new Exception("No web context");

                string scheme = System.Web.HttpContext.Current.Request.Url.Scheme;
                string host = System.Web.HttpContext.Current.Request.Url.Host;
                int port = System.Web.HttpContext.Current.Request.Url.Port;
                
                string defHost = System.Configuration.ConfigurationManager.AppSettings["BOX_DEFAULT_HOST_NAME"] as String;
                var appPath = System.Web.HttpContext.Current.Request.ApplicationPath;
                if (appPath == "/")
                    appPath = "";

                return scheme + "://" + (String.IsNullOrEmpty(defHost) ? host : defHost) + (port == 80 ? "" : ":" + port) + appPath;


            }
        }

        public string LOGIN_URL {
            get {
                string url = System.Configuration.ConfigurationManager.AppSettings[ID + "_LOGIN_URL"] as String;
                if (url == null)
                    return null;
                return String.Format(url, CLIENT_ID, CALLBACK_URL);                
            }
        }

        protected virtual string CALLBACK_URL {
            get {
                string returnUrl = "";
                if (System.Web.HttpContext.Current != null)
                    returnUrl = System.Web.HttpContext.Current.Request["ReturnUrl"];
                return HOST_NAME + "/core_signin/" +  ID + "callback?ReturnUrl=" + returnUrl;
            }
        }

        public string GetUserEmail(string authCode) {
            string token = GetAccessToken(authCode);
            if (token == null)
                return null;
            
            dynamic user = GetUserData(token);
            if (user == null)
                return null;

            return _GetUserEmail(user);
        }

        protected abstract string _GetUserEmail(dynamic user);

        protected virtual string GetAccessToken(string authCode) {

            System.Net.Http.HttpClient wc = new System.Net.Http.HttpClient();

            string postData = "code={0}&client_id={1}&redirect_uri={2}&grant_type={3}&client_secret={4}";
            postData = string.Format(postData,
                HttpUtility.UrlEncode(authCode),
                CLIENT_ID,
                HttpUtility.UrlEncode(CALLBACK_URL),
                "authorization_code",
                SECRET);

            System.Net.Http.HttpResponseMessage msg = wc.PostAsync(GET_TOKEN_URL, new System.Net.Http.StringContent(postData, Encoding.Default, "application/x-www-form-urlencoded")).Result;

            if (msg.StatusCode == System.Net.HttpStatusCode.OK) {
                string json = msg.Content.ReadAsStringAsync().Result;
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                return data["access_token"];
            }
            return null;
        }

        protected dynamic GetUserData(string token) {
            System.Net.Http.HttpClient wc = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage msg = wc.GetAsync(String.Format(GET_USER_URL, token)).Result;

            dynamic data = null;

            if (msg.StatusCode == System.Net.HttpStatusCode.OK) {
                string json = msg.Content.ReadAsStringAsync().Result;
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            }

            return data;
        }

    }
}
