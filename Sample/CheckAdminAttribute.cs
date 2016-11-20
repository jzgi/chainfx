using Greatbone.Core;

namespace Greatbone.Sample
{

    public class CheckAdminAttribute : CheckAttribute
    {
        public CheckAdminAttribute() : base(false) { }

        public override bool Test(WebContext wc)
        {
            return wc.Principal is Login;
        }
    }

}