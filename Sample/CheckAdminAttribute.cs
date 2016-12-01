using Greatbone.Core;

namespace Greatbone.Sample
{
    public class CheckAdminAttribute : CheckAttribute
    {
        public CheckAdminAttribute() : base(false) { }

        public override bool Test(WebActionContext wc)
        {
            return wc.Principal is ShopToken;
        }
    }

}