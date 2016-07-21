using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// WWW
    ///
    public class SiteService : WebService
    {
        protected internal override void Init()
        {
            SetMux<SiteSpaceMux, Space>(null);

            AddSub<DirectoryService>("dir", null);

            AddSub<SiteCartSub>("cart", null);
        }

        public override void Default(WebContext wc)
        {
            Console.WriteLine("start Action: ");

            string id = "";
        }

        public void Show(WebContext wc)
        {
        }
    }
}