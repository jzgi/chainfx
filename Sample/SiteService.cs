using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// WWW
    ///
    public class SiteService : WebService
    {
        public SiteService() : base(null)
        {
            SetMux<SiteSpaceMux, Space>(null);

            AddSub<DirectoryService>("dir", null);
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