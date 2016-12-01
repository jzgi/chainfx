using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /-shopid-/mycart/
    ///
    public class MyCartDirectory : WebDirectory
    {
        public MyCartDirectory(WebMake mk) : base(mk)
        {
        }

        ///
        /// Get buyer's personal order list
        ///
        public void Add(WebExchange wc, string subscpt)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void Remove(WebExchange wc, string subscpt)
        {
            // string shopid = wc.Var(null);
        }

        ///
        /// find in shop's order list
        ///
        public void Checkout(WebExchange wc, string subscpt)
        {
            // string shopid = wc.Var(null);

        }
    }
}