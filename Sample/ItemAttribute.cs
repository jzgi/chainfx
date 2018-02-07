using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ItemAttribute : StateAttribute
    {
        readonly char state;

        public ItemAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(ActionContext ac, object obj)
        {
            var o = obj as Item;
            if (state == 'A')
            {
                var on = ac.Obtain<Shop>()?.status > 1;
                return on && o != null && o.stock > 0;
            }
            return false;
        }
    }
}