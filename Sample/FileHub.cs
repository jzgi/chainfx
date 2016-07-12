using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FileHub : WebHub
    {
        public FileHub(WebHub hub) : base(hub)
        {
        }

        protected override bool ResolveZone(WebContext wc, string zone)
        {
            throw new System.NotImplementedException();
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}