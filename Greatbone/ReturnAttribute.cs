using System;

namespace Greatbone
{
    /// <summary>
    /// To document the response returned by the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ReturnAttribute : Attribute, IComment
    {
        readonly short status;

        readonly string headers;

        readonly string body;

        public ReturnAttribute(short status, string headers = null, string body = null)
        {
            this.status = status;
            this.headers = headers;
            this.body = body;
        }

        public void Print(HtmlContent h)
        {
            h.SECTION_().T("Query");
            h.T("<pre>").T(status).T("</pre>");
            h.T("<pre>").T(headers).T("</pre>");
            h.T("<pre>").T(body).T("</pre>");
            h._SECTION();
        }
    }
}