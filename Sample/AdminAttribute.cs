using Greatbone.Core;

namespace Greatbone.Sample
{
    public class AdminAttribute : AccessAttribute
    {
        public override bool Check(ActionContext wc)
        {
            return ((Token)wc.Token).IsAdmin;
        }
    }
}