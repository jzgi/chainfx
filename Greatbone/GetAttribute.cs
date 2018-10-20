using System;

namespace Greatbone
{
    /// <summary>
    /// To document a GET request to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class GetAttribute : TagAttribute
    {
        readonly string query;

        readonly string headers;

        readonly string tip;

        public GetAttribute(string tip = null, string query = null, string headers = null)
        {
            this.query = query;
            this.headers = headers;
            this.tip = tip;
        }

        internal override void Print(HtmlContent h)
        {
            h.SECTION_();
            h.T("GET").SP().T(tip);
            if (query != null)
            {
                h.T("<pre>").T(query).T("</pre>");
                h.BR();
            }
            if (headers != null)
            {
                h.T("<pre>").T(headers).T("</pre>");
                h.BR();
            }
            h._SECTION();
        }
    }
}