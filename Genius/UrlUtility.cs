using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Genius
{
    public class UrlUtility
    {
        public static string ConstructQueryString(NameValueCollection Params)
        {
            return string.Join("&", (from string name in Params
                select String.Concat(name, "=", HttpUtility.UrlEncode(Params[name])))
                .ToArray());
        }

        public static string ObjectToParams(object parameters)
        {
            Type t = parameters.GetType();
            var nvc = new NameValueCollection();
            foreach (var p in t.GetProperties())
            {
                var name = p.Name;
                var value = p.GetValue(parameters, null).ToString();
                nvc.Add(name, value);
            }

            var result = ConstructQueryString(nvc);
            return result;
        }
    }
}