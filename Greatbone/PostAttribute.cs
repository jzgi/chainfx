using System;

namespace Greatbone
{
    /// <summary>
    /// To document a POST request to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PostAttribute : Attribute, IComment
    {
        readonly string query;

        readonly string headers;

        readonly string body;

        readonly string tip;

        public PostAttribute(string query = null, string headers = null, string body = null, string tip = null)
        {
            this.query = query;
            this.headers = headers;
            this.body = body;
            this.tip = tip;
        }

        public void Print(HtmlContent h)
        {
            h.SECTION_().T("Query");
            h.T("<pre>").T(query).T("</pre>");
            h.T("<pre>").T(headers).T("</pre>");
            h.T("<pre>").T(body).T("</pre>");
            h._SECTION();
        }
    }
}