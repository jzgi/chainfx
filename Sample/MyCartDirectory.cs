using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /-shopid-/mycart/
    ///
    public class MyCartDirectory : WebDirectory
    {
        public MyCartDirectory(WebDirectoryContext ctx) : base(ctx)
        {
        }

        ///
        /// Get buyer's personal order list
        ///
        public void Add(WebContext wc, string subscpt)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void Remove(WebContext wc, string subscpt)
        {
            // string shopid = wc.Var(null);
        }

        ///
        /// find in shop's order list
        ///
        public void Checkout(WebContext wc, string subscpt)
        {
            // string shopid = wc.Var(null);

        }
    }
}