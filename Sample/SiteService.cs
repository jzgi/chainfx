using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The main website service (WWW).
    ///
    public class SiteService : WebService
    {
        public SiteService(WebCreationContext wcc) : base(wcc)
        {
            SetMux<SiteSpaceMux, Space>(null);

            AddSub<SiteCartSub>("cart", null);


            //
            // sub services, placed here to achieve single-machine-deployment

            AddSub<DirectoryService>("dir", null);

            AddSub<BusinessService>("biz", null);

            AddSub<OrderService>("ord", null);

            AddSub<AccountingService>("acctg", null);
        }

        public void Show(WebContext wc)
        {
        }

        public void Contact(WebContext wc)
        {
        }
    }
}