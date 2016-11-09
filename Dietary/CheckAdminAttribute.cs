using Greatbone.Core;

namespace Ministry.Dietary
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