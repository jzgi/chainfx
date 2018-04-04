using Greatbone;

namespace Core
{
    public class OrderAttribute : StateAttribute
    {
        readonly char state;

        public OrderAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext ac, object[] stack, int level)
        {
            if (stack[0] is Order o)
            {
                if (state == 'P')
                    return o.status < Order.PAID;
                if (state == 'A')
                    return o.custaddr != null;
            }
            return false;
        }
    }
}