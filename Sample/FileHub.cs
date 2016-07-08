using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FileHub : WebHub
    {
        public FileHub(WebHub parent) : base(parent)
        {
        }

        protected override bool ResolveZone(string zoneId, WebContext wc)
        {
            throw new System.NotImplementedException();
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}