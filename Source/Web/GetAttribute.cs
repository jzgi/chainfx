using System;

namespace FabricQ.Web
{
    /// <summary>
    /// To document a GET request to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class GetAttribute : TagAttribute
    {
        readonly string tip;

        readonly string query;

        readonly string headers;

        public GetAttribute(string tip = null, string query = null, string headers = null)
        {
            this.tip = tip;
            this.query = Preprocess(query);
            this.headers = Preprocess(headers);
        }

        internal override void Describe(HtmlContent h)
        {
            h.P_();
            h.T("GET").SP().T(tip);
            if (query != null)
            {
                h.T("<pre>").TT(query).T("</pre>");
            }
            if (headers != null)
            {
                h.T("<pre>").TT(headers).T("</pre>");
            }
            h._P();
        }
    }
}