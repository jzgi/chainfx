using Greatbone.Core;

namespace Ministry.Dietary
{
    ///
    /// The order multiplex directory under shop.
    ///
    public class OrderVariableDir : WebDir, IVariable
    {
        public OrderVariableDir(WebDirContext ctx) : base(ctx)
        {
        }

        ///
        /// Get order's detail.
        ///
        public void @default(WebContext wc)
        {
        }

        public void cannel(WebContext wc)
        {
        }

        public void pend(WebContext wc)
        {
        }

        public void close(WebContext wc)
        {
        }
    }
}