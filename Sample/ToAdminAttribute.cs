using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ToAdminAttribute : ToAttribute
    {
        public override bool Test(WebContext wc)
        {
            return wc.Principal is Login;
        }
    }
}