using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// WWW
    ///
    public class SiteService : WebService
    {
        public SiteService(WebCreationContext wcc) : base(wcc)
        {
            SetMux<SiteSpaceMux, Space>(null);

            AddSub<DirService>("dir", null);

            AddSub<SiteCartSub>("cart", null);
        }

        public void Show(WebContext wc)
        {
        }

        public void Contact(WebContext wc)
        {
        }
    }
}