using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Web;
using Box.Core.Services;

namespace Box.Core.Oauth {


    [Export]
    public class Azure : Generic {

        [Import]
        protected override LogService log { get; set; }

        protected override string ID {
            get {
                return "AZURE";
            }            
        }

        protected override string _GetUserEmail(dynamic user) {
            
            return user["userPrincipalName"];                
        }

        protected override string CALLBACK_URL {
            get {
                string returnUrl = "";
                if (System.Web.HttpContext.Current != null)
                    returnUrl = System.Web.HttpContext.Current.Request["ReturnUrl"];         
                return HOST_NAME + "/core_signin/" + ID + "callback";
            }
        }

        protected override dynamic GetUserData(string token)
        {
            System.Net.Http.HttpClient wc = new System.Net.Http.HttpClient();
            wc.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            System.Net.Http.HttpResponseMessage msg = wc.GetAsync(GET_USER_URL).Result;
            
            dynamic data = null;

            if (msg.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = msg.Content.ReadAsStringAsync().Result;
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                log.Log("Get external user data:" + json);
            }
            else
            {
                var error = msg.Content.ReadAsStringAsync().Result;
                log.Log("Error reading external user", error);
            }

            
            return data;
        }


    }
}
