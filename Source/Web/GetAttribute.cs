using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To document a GET request to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class GetAttribute : Attribute, IDocTag
    {
        readonly string tip;

        readonly string query;

        readonly string[] headers;

        public GetAttribute(string tip = null, string query = null, string[] headers = null)
        {
            this.tip = tip;
            this.query = query;
            this.headers = headers;
        }

        public void Render(HtmlBuilder h)
        {
            h.P_();
            h.T("GET").SP().T(tip);
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
            h._P();
        }
    }
}