using Greatbone.Core;

namespace Greatbone.Sample
{
    public class OpUserHub : WebHub
    {
        public OpUserHub(WebHub parent) : base(parent)
        {
        }

        public override void Default(WebContext wc)
        {
        }

        public void Add(WebContext wc)
        {
        }

        protected  override bool ResolveZone(string zoneId, WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}