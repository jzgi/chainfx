using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Greatbone.Core
{
    public static class WebUtility
    {
        public static string GetValue(this HttpHeaders headers, string name)
        {
            IEnumerable<string> values;
            if (headers.TryGetValues(name, out values))
            {
                string[] strs = values as string[];
                return strs?[0] ?? null;
            }
            return null;
        }

        ///
        /// Used in both client and server to parse received content into model.
        ///
        public static IModel ParseContent(string ctyp, byte[] buffer, int start, int count)
        {
            if (string.IsNullOrEmpty(ctyp)) return null;

            if ("application/x-www-form-urlencoded".Equals(ctyp))
            {
                return new FormParse(buffer, count).Parse();
            }
            else if (ctyp.StartsWith("multipart/form-data; boundary="))
            {
                string boundary = ctyp.Substring(30);
                return new FormMpParse(boundary, buffer, count).Parse();
            }
            else if (ctyp.StartsWith("application/json"))
            {
                return new JsonParse(buffer, count).Parse();
            }
            else if (ctyp.StartsWith("application/xml"))
            {
                return new XmlParse(buffer, 0, count).Parse();
            }
            else if (ctyp.StartsWith("text/plain"))
            {
                Text txt = new Text();
                for (int i = 0; i < count; i++)
                {
                    txt.Accept(buffer[i]);
                }
                return txt;
            }
            return null;
        }
    }
}