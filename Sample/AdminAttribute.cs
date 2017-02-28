using Greatbone.Core;

namespace Greatbone.Sample
{
    public class AdminAttribute : CheckAttribute
    {
        public override bool Check(ActionContext wc)
        {
            return ((Token)wc.Token).IsAdmin;
        }
    }
}