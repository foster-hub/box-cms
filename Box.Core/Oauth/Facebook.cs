using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Web;

namespace Box.Core.Oauth
{


    [Export]
    public class Facebook : Generic
    {

        protected override string ID
        {
            get
            {
                return "FB";
            }
        }

        protected override string _GetUserEmail(dynamic user)
        {
            return user["email"];
        }

        protected override string CALLBACK_URL
        {
            get
            {
                return HOST_NAME + "/core_signin/" + ID + "callback";
            }
        }

        protected override string GetAccessToken(string authCode)
        {
            System.Net.Http.HttpClient wc = new System.Net.Http.HttpClient();

            string postData = "code={0}&client_id={1}&redirect_uri={2}&grant_type={3}&client_secret={4}";
            postData = string.Format(postData,
                HttpUtility.UrlEncode(authCode),
                CLIENT_ID,
                HttpUtility.UrlEncode(CALLBACK_URL),
                "authorization_code",
                SECRET);

            System.Net.Http.HttpResponseMessage msg = wc.PostAsync(GET_TOKEN_URL, new System.Net.Http.StringContent(postData, Encoding.Default, "application/x-www-form-urlencoded")).Result;

            if (msg.StatusCode == System.Net.HttpStatusCode.OK)
                return msg.Content.ReadAsStringAsync().Result.Replace("access_token=", "");

            return null;
        }
    }
}
