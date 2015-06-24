using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Box.Composition {

    public class EmailParser {


        public static string FakeRazorParser(string template, string pattern, object model) {

            Dictionary<string, string> dic = model as Dictionary<string, string>;

            if (dic != null) {
                foreach (string key in dic.Keys) {
                    template = template.Replace(pattern + "[\"" + key + "\"]", dic[key]);
                }
                return template;
            }

            Type type = model.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties());

            foreach (PropertyInfo prop in props) {
                object propValue = prop.GetValue(model, null);
                string propValueStr = propValue == null ? "" : propValue.ToString();
                template = template.Replace(pattern + prop.Name + " ", propValueStr);
                template = template.Replace(pattern + prop.Name + "\n", propValueStr + "\n");
                template = template.Replace(pattern + prop.Name + "\r", propValueStr + "\r");
                template = template.Replace(pattern + prop.Name + ")", propValueStr + ")");
                template = template.Replace(pattern + prop.Name + "<", propValueStr + "<");
            }

            return template;
        }

    }
}
