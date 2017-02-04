using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ToUserAttribute : ToAttribute
    {
        public override bool Check(WebActionContext wc)
        {
            return wc.Token is Token;
        }
    }
}