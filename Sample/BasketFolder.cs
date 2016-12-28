using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /basket/
    ///
    public class BasketFolder : WebFolder
    {
        public BasketFolder(WebFolderContext dc) : base(dc)
        {
        }

        ///
        /// Get buyer's personal order list
        ///
        public void Add(WebActionContext ac)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void Remove(WebActionContext ac)
        {
            // string shopid = wc.Var(null);
        }

        ///
        /// find in shop's order list
        ///
        public void Checkout(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }
    }
}