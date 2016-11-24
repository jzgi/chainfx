using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /-shopid-/myorder/
    ///
    public class MyOrderDirectory : WebDirectory
    {
        public MyOrderDirectory(WebDirectoryContext ctx) : base(ctx)
        {
            SetVariable<MyOrderVariableDirectory>();
        }

        ///
        /// Get buyer's personal order list
        ///
        public void all(WebContext wc, string subscpt)
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
        public void clear(WebContext wc, string subscpt)
        {
            // string shopid = wc.Var(null);

        }
    }
}