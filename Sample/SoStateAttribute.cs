using Greatbone;

namespace Samp
{
    public class SoStateAttribute : StateAttribute
    {
        readonly char state;

        public SoStateAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext ac, object[] stack, int level)
        {
            if (stack[0] is So o)
            {
                if (state == 'P') // payable
                    return o.status < So.PAID;
                if (state == 'A')
                    return o.custaddr != null;
                if (state == 'E') // enable
                    return o.status >= So.PAID;
            }
            return false;
        }
    }
}