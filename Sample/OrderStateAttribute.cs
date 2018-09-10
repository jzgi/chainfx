using Greatbone;
using static Samp.Order;

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
            if (stack[0] is Order o)
            {
                if (state == 'P') // payable
                    return o.status < ORD_PAID;
                if (state == 'C') // 
                    return o.status < ORD_ASSIGNED;
                if (state == 'E') // 
                    return o.status >= ORD_PAID;
            }
            return false;
        }
    }
}