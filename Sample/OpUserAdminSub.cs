using Greatbone.Core;

namespace Greatbone.Sample
{
    public class OpUserAdminSub : WebSub
    {
        public OpUserAdminSub(WebHub hub) : base(hub)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}