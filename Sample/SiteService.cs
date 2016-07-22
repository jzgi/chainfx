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

            AddSub<DirectoryService>("dir", null);

            AddSub<SiteCartSub>("cart", null);
        }

        public void Show(WebContext wc)
        {
        }
    }
}