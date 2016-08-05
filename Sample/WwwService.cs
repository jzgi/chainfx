using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service controller.
    ///
    public class WwwService : WebService
    {
        public WwwService(WebServiceContext wsc) : base(wsc)
        {
            AddSub<WwwMySub>("my", null);
        }

        public void Show(WebContext wc)
        {
            Fame v = new Fame();
            v.To(wc.Response, 0);
        }

        public void Contact(WebContext wc)
        {
        }
    }
}