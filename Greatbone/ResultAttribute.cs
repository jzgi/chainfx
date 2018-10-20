using System;

namespace Greatbone
{
    /// <summary>
    /// To document the response returned by the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ResultAttribute : TagAttribute
    {
        readonly short status;

        readonly string tip;

        readonly string headers;

        readonly string body;

        public ResultAttribute(short status, string tip = null, string headers = null, string body = null)
        {
            this.status = status;
            this.tip = tip;
            this.headers = headers;
            this.body = body;
        }

        internal override void Print(HtmlContent h)
        {
            h.P_();
            h.T("RESULT:").SP().T(status).SP().T(tip);
            if (headers != null)
            {
                h.T("<pre>").T(headers).T("</pre>");
            }
            if (body != null)
            {
                h.T("<pre style=\"overflow-x: scroll;\">").T(body).T("</pre>");
            }
            h._P();
        }
    }
}