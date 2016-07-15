using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// WWW
    ///
    public class SiteHub : WebHub
    {
        public SiteHub() : base(null)
        {
            SetMux<SiteSpaceMux, Space>(null);

            AddSub<DirectoryHub>("dir", null);
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