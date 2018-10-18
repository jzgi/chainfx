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
                var item = stack[0] as Item;
                return item?.status > 0;
            }
            return false;
        }
    }
}