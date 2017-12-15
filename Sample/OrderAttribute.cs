using Greatbone.Core;

namespace Greatbone.Samp
{
    public class OrderAttribute : StateAttribute
    {
        readonly char state;

        public OrderAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(ActionContext ac, object model)
        {
            var o = model as Order;
            if (state == 'A')
                return o.addr != null;
            return false;
        }
    }
}