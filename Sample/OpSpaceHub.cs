using Greatbone.Core;

namespace Greatbone.Sample
{
    public class OpSpaceHub : WebHub
    {
        public OpSpaceHub(WebHub parent) : base(parent)
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