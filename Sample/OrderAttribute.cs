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
            for (int i = 0; i < level; i++)
            {
                if (stack[i] is Order o)
                {
                    if (state == 'A')
                        return o.custaddr != null;
                }
            }
            return false;
        }
    }
}