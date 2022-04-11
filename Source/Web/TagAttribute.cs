using System;
using System.Text;

namespace Chainly.Web
{
    /// <summary>
    /// Tag for API documentation comments. 
    /// </summary>
    public abstract class TagAttribute : Attribute
    {
        private const string CRLF = "\r\n";

        internal abstract void Describe(HtmlContent h);

        internal static string Preprocess(string v)
        {
            if (v == null) return null;

            string[] arr = v.Split(CRLF);
            int count = arr.Length;

            StringBuilder sb = new StringBuilder(v.Length);
            if (count > 0)
            {
                // add first line
                sb.Append(arr[0].TrimStart());
            }

            // calculate spaces to be removed for each next line
            int spaces = 0;
            if (count > 2)
            {
                string sec = arr[1];
                int seclen = sec.Length;
                int p = 0;
                while (p < seclen && char.IsWhiteSpace(sec[p])) p++;
                spaces = p - 4;
            }

            // add rest of the lines
            for (int i = 1; i < count; i++)
            {
                var e = arr[i];
                int elen = e.Length;
                sb.Append(CRLF);
                if (spaces > 0 && spaces < elen)
                {
                    sb.Append(e, spaces, e.Length - spaces);
                }
                else
                {
                    sb.Append(e);
                }
            }
            return sb.ToString();
        }
    }
}