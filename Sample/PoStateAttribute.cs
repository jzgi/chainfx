using Greatbone;

namespace Samp
{
    public class PoStateAttribute : StateAttribute
    {
        readonly char state;

        public PoStateAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext ac, object[] stack, int level)
        {
            if (state == 'A')
            {
                var org = stack[0] as Org;
                var item = stack[1] as Item;
                return org?.status > 1 && item?.max > 0;
            }
            return false;
        }
    }
}