using System;

namespace Greatbone
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

        internal override void Print(HtmlContent h)
        {
            h.P_();
            h.T("POST").SP().T(tip);
            if (query != null)
            {
                h.T("<pre>").T(query).T("</pre>");
            }
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