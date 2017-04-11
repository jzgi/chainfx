using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class AdmWork : Work
    {
        public AdmWork(WorkContext ctx) : base(ctx)
        {
            Create<AdmUserWork>("user");

            Create<AdmShopWork>("shop");

            Create<AdmRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }
}