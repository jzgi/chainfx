using Greatbone.Core;

namespace Greatbone.Samp
{
    public class ItemAttribute : StateAttribute
    {
        readonly char state;

        public ItemAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(ActionContext ac, object model)
        {
            var o = model as Item;
            if (state == 'A')
            {
                var on = ac.Obtain<Shop>()?.status > 1;
                return on && o != null && o.stock > 0;
            }
            return false;
        }
    }
}