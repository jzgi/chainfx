using Greatbone.Core;

namespace Greatbone.Sample
{
    public class AdmWork : Work
    {
        public AdmWork(WorkContext wc) : base(wc)
        {
            Create<AdmUserWork>("user"); // users management

            Create<AdmShopWork>("shop"); // shops management

            Create<AdmRepayWork>("repay"); // repays management
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }
}