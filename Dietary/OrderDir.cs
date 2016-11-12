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
            SetMux<OrderMuxDir>();
        }

        ///
        /// Get buyer's personal order list
        ///
        public void my(WebContext wc, string subscpt)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void list(WebContext wc, string subscpt)
        {
            // string shopid = wc.Var(null);

        }

        ///
        /// find in shop's order list
        ///
        public void find(WebContext wc, string subscpt)
        {
            // string shopid = wc.Var(null);

        }
    }
}