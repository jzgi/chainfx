using Greatbone.Core;

namespace Ministry.Dietary
{
    ///
    /// /-shopid-/order/
    ///
    public class OrderDir : WebDir
    {
        public OrderDir(WebDirContext ctx) : base(ctx)
        {
            SetVariable<OrderVariableDir>();
        }

        public void lst(WebContext wc, string subscpt)
        {
        }
    }
}