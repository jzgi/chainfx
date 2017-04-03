using Greatbone.Core;

namespace Greatbone.Sample
{
    public class PubWork : Work
    {
        public PubWork(WorkContext wc) : base(wc)
        {
            Create<PubShopWork>("shop");
        }
    }
}