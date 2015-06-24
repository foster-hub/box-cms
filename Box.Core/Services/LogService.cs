using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using System.Collections.Specialized;

namespace Box.Core.Services {
    
    [Export]
    public class LogService {

        [Import]
        private SecurityService security { get; set; }

        public void Log(string actionDescription, string errorDescription = null, bool saveParameters = true) {

            string url = "unknow";
            string login = "unknow";
            string ip = "unknow";
            DateTime when = DateTime.Now.ToUniversalTime();
            Dictionary<string, object> dicParameters = null;

            var httpContext = System.Web.HttpContext.Current;
            if (httpContext != null) {
                url = httpContext.Request.Url.OriginalString;
                ip = httpContext.Request.UserHostAddress;

                // if was not via IMPORT
                if (security == null)
                    security = new SecurityService();

                User user = security.GetSignedUser();
                if (user != null)
                    login = user.Email;
                if (user.LoginNT != null)
                    login = login + " (" + user.LoginNT + ")";

                if (saveParameters) {

                    dicParameters = new Dictionary<string, object>();

                    //Salvar dados que são enviados no corpo da requisição
                    if (httpContext.Request.InputStream.Length > 0) {
                        httpContext.Request.InputStream.Position = 0;
                        using (StreamReader reader = new StreamReader(httpContext.Request.InputStream)) {
                            if (httpContext.Request.ContentType.ToLower().Contains("application/json;"))
                                dicParameters["body"] = Newtonsoft.Json.JsonConvert.DeserializeObject(reader.ReadToEnd());
                            else
                                dicParameters["body"] = reader.ReadToEnd();
                        }
                    }
                    //Salvar dados que são enviados via GET
                    if (httpContext.Request.QueryString.HasKeys()) {
                        Dictionary<string, string> dicGet = new Dictionary<string, string>();
                        dicParameters["get"] = NameValueCollectionToDictionary(httpContext.Request.QueryString);
                    }
                    //Salvar dados que são enviados via POST
                    if (httpContext.Request.Form.HasKeys()) {
                        dicParameters["post"] = NameValueCollectionToDictionary(httpContext.Request.Form);
                    }
                }
            }

            string parameters = null;

            if (dicParameters != null && dicParameters.Count > 0)
                parameters = Newtonsoft.Json.JsonConvert.SerializeObject(dicParameters);

            short logType = 0;
            if (!string.IsNullOrEmpty(errorDescription)) {
                logType = (short)LogTypes.ERROR;
            }

            Log log = new Log() { LogUId = Guid.NewGuid().ToString(), ActionDescription = actionDescription, ErrorDescription = errorDescription, LogType = logType, SignedUser = login, Url = url, UserIp = ip, When = when, Parameters = parameters };

            try {
                using (var context = new Data.CoreContext()) {
                    context.Logs.Add(log);
                    context.SaveChanges();

                    // delete old records
                    DateTime yearAgo = when.AddYears(-1);
                    context.Database.ExecuteSqlCommand("DELETE Logs WHERE GETDATE() < '" + yearAgo.Year + "-" + yearAgo.Month + "-" + yearAgo.Day + "'");
                }
            }
            catch (Exception) { }

        }

        public Log GetLog(string id) {

            if (string.IsNullOrEmpty(id))
                return null;

            using (var context = new Data.CoreContext())
                return context.Logs.Where(l => l.LogUId == id).SingleOrDefault();
        }

        public IEnumerable<Log> GetLogs(string filter = null, int skip = 0, int top = 0, DateTime? dataDe = null, DateTime? dataAte = null) {

            using (var context = new Data.CoreContext()) {
                IQueryable<Log> logs = context.Logs;

                if (!String.IsNullOrEmpty(filter)) {

                    string[] tags = filter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    logs = logs.Where(l => tags.All(t =>
                        l.SignedUser.ToLower() == t ||
                        l.Url.ToLower().Contains(t) ||
                        l.ActionDescription.ToLower().Contains(t) ||
                        l.UserIp.ToLower() == t));
                }

                if (dataDe != null) {
                    dataDe = dataDe.Value.Date;
                    logs = logs.Where(l => l.When >= dataDe.Value);
                }

                if (dataAte != null) {
                    DateTime dataAteDiaSeguinte = dataAte.Value.AddDays(1).Date;
                    logs = logs.Where(l => l.When < dataAteDiaSeguinte);
                }

                logs = logs.OrderByDescending(l => l.When);

                if (skip != 0)
                    logs = logs.Skip(skip);

                if (top != 0)
                    logs = logs.Take(top);

                return logs.ToList().Select(l => new Log {
                    ActionDescription = l.ActionDescription,
                    ErrorDescription = l.ErrorDescription,
                    LogType = l.LogType,
                    LogUId = l.LogUId,
                    SignedUser = l.SignedUser,
                    Url = l.Url,
                    UserIp = l.UserIp,
                    When = l.When
                }).ToArray();


            }
        }

        protected Dictionary<string, object> NameValueCollectionToDictionary(NameValueCollection nvc) {
            var result = new Dictionary<string, object>();

            foreach (string key in nvc.Keys) {

                string[] values = nvc.GetValues(key);
                if (values.Length == 1) 
                    result.Add(key, values[0]);
                else 
                    result.Add(key, values);
            }

            return result;
        }
    }
}
