using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /-shopid-/myorder/
    ///
    public class MyOrderDirectory : WebDirectory
    {
        public MyOrderDirectory(WebMake mk) : base(mk)
        {
            MakeVariable<MyOrderVariableDirectory>();
        }

        ///
        /// Get buyer's personal order list
        ///
        public void all(WebExchange ex, string subscpt)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void list(WebExchange ex, string subscpt)
        {
            // string shopid = wc.Var(null);

        }

        ///
        /// find in shop's order list
        ///
        public void clear(WebExchange ex, string subscpt)
        {
            // string shopid = wc.Var(null);

        }
    }
}