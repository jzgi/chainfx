using Greatbone.Core;

namespace Greatbone.Sample
{
    [User(adm: true)]
    public class AdmWork : Work
    {
        public AdmWork(WorkContext wc) : base(wc)
        {
            Create<AdmUserWork>("user"); // users management

            Create<AdmShopWork>("shop"); // shops management

            Create<AdmRepayWork>("repay"); // repays present

            Create<AdmPastRepayWork>("pastrepay"); // repays past

            Create<AdmFeekbackWork>("feedback");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }
}