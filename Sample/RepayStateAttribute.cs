using Greatbone;

namespace Samp
{
    public class RepayStateAttribute : StateAttribute
    {
        readonly char state;

        public RepayStateAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext wc, object[] stack, int level)
        {
            if (state == 'A')
            {
                var o = stack[0] as Repay;
                return o?.status < 2;
            }
            return false;
        }
    }
}