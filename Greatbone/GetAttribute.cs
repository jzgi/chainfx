using System;

namespace Greatbone
{
    /// <summary>
    /// To document a GET request to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class GetAttribute : Attribute, IComment
    {
        readonly string query;

        readonly string headers;

        readonly string tip;

        public GetAttribute(string query = null, string headers = null, string tip = null)
        {
            this.query = query;
            this.headers = headers;
            this.tip = tip;
        }

        public void Print(HtmlContent h)
        {
            h.SECTION_().T("Query");
            h.T("<pre>").T(query).T("</pre>");
            h.T("<pre>").T(headers).T("</pre>");
            h._SECTION();
        }
    }
}