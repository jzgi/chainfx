using System;

namespace SkyCloud.Web
{
    /// <summary>
    /// To document the response returned by the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ReplyAttribute : TagAttribute
    {
        readonly short status;

        readonly string tip;

        readonly string headers;

        readonly string body;

        public ReplyAttribute(short status, string tip = null, string headers = null, string body = null)
        {
            this.status = status;
            this.tip = tip;
            this.headers = Preprocess(headers);
            this.body = Preprocess(body);
        }

        internal override void Describe(HtmlContent h)
        {
            h.P_();
            h.T(status).SP().T(tip);
            if (headers != null)
            {
                h.T("<pre>").TT(headers).T("</pre>");
            }
            if (body != null)
            {
                h.T("<pre>").TT(body).T("</pre>");
            }
            h._P();
        }
    }
}