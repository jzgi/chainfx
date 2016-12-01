using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /-shopid-/myorder/
    ///
    public class MyOrderDirectory : WebDirectory
    {
        public MyOrderDirectory(WebMakeContext mc) : base(mc)
        {
            MakeVariable<MyOrderVariableDirectory>();
        }

        ///
        /// Get buyer's personal order list
        ///
        public void all(WebActionContext ac)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void list(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        ///
        /// find in shop's order list
        ///
        public void clear(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }
    }
}