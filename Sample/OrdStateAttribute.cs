using Greatbone;

namespace Samp
{
    public class OrderStateAttribute : StateAttribute
    {
        readonly char state;

        public OrderStateAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext ac, object[] stack, int level)
        {
            if (stack[0] is Ord o)
            {
                if (state == 'P') // payable
                    return o.status < Ord.PAID;
                if (state == 'A')
                    return o.uaddr != null;
                if (state == 'E') // enable
                    return o.status >= Ord.PAID;
            }
            return false;
        }
    }
}