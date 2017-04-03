using Greatbone.Core;

namespace Greatbone.Sample
{
    [Ui("用户管理")]
    public class AdmWork : Work
    {
        public AdmWork(WorkContext wc) : base(wc)
        {
            Create<AdmUserWork>("user");

            Create<AdmShopWork>("shop");

            Create<AdmRepayWork>("repay");
        }
    }
}