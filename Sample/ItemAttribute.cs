using Greatbone;

namespace Core
{
    public class ItemAttribute : StateAttribute
    {
        readonly char state;

        public ItemAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext ac, object obj)
        {
            var o = obj as Item;
            if (state == 'A')
            {
                var on = ac.Obtain<Org>()?.status > 1;
                return on && o != null && o.stock > 0;
            }
            return false;
        }
    }
}