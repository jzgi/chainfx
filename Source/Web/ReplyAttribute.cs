using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To document the response returned by the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ReplyAttribute : Attribute, IDocTag
    {
        readonly short status;

        readonly string tip;

        readonly string[] headers;

        readonly string body;

        public ReplyAttribute(short status, string tip = null, string[] headers = null, string body = null)
        {
            this.status = status;
            this.tip = tip;
            this.headers = headers;
            this.body = body;
        }

        public void Render(HtmlBuilder h)
        {
            h.P_();

            h.T(status).SP().T(tip);

            if (headers != null)
            {
                h.T("<pre>");
                foreach (var v in headers)
                {
                    h.TT(v);
                }
                h.T("</pre>");
            }

            if (body != null)
            {
                h.T("<pre>").TT(body).T("</pre>");
            }

            h._P();
        }
    }
}