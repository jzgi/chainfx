using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To document a POST request to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PostAttribute : Attribute, IDocTag
    {
        readonly string query;

        readonly string[] headers;

        readonly string body;

        readonly string tip;

        public PostAttribute(string tip = null, string query = null, string[] headers = null, string body = null)
        {
            this.query = query;
            this.headers = headers;
            this.body = body;
            this.tip = tip;
        }

        public void Render(HtmlBuilder h)
        {
            h.P_();

            h.T("POST").SP().T(tip);
            if (query != null)
            {
                h.T("<pre>").TT(query).T("</pre>");
            }

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