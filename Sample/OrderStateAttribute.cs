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
                    return o.status < OrdPaid;
                if (state == 'E') // 
                    return o.status >= OrdPaid;
            }
            return false;
        }
    }
}