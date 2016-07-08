using Greatbone.Core;

namespace Greatbone.Sample
{
    public class OpUserMgtSub : WebSub
    {
        public OpUserMgtSub(WebHub hub) : base(hub)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }

        public void Get(WebContext wc)
        {
        }

        public void Search(WebContext wc)
        {
        }

        public void Disable(WebContext wc)
        {
        }
    }
}