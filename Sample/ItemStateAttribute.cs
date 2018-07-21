using Greatbone;

namespace Samp
{
    public class ItemStateAttribute : StateAttribute
    {
        readonly char state;

        public ItemStateAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext ac, object[] stack, int level)
        {
            if (state == 'A')
            {
                var org = stack[0] as Org;
                var item = stack[1] as Item;
                return org?.status > 1 && item?.stock > 0;
            }
            return false;
        }
    }
}