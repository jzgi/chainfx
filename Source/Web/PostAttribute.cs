using System;

namespace Chainly.Web
{
    /// <summary>
    /// To document a POST request to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PostAttribute : TagAttribute
    {
        readonly string query;

        readonly string headers;

        readonly string body;

        readonly string tip;

        public PostAttribute(string tip = null, string query = null, string headers = null, string body = null)
        {
            this.query = Preprocess(query);
            this.headers = Preprocess(headers);
            this.body = Preprocess(body);
            this.tip = tip;
        }

        internal override void Describe(HtmlContent h)
        {
            h.P_();
            h.T("POST").SP().T(tip);
            if (query != null)
            {
                h.T("<pre>").TT(query).T("</pre>");
            }
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