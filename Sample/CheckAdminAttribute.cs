using Greatbone.Core;

namespace Greatbone.Sample
{
    public class CheckAdminAttribute : CheckAttribute
    {
        public CheckAdminAttribute() : base(false) { }

        public override bool Check(WebActionContext wc)
        {
            return wc.Token is Token;
        }
    }

}