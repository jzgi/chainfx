using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/-id-/item/
    ///
    public class ItemFolder : WebFolder
    {
        public ItemFolder(WebFolderContext fc) : base(fc)
        {
            CreateVar<ItemVarFolder>();
        }

        #region /user/-userwx-/order/

        public void my(WebActionContext ac)
        {

        }

        #endregion

        #region /shop/-shopid-/order/

        [ToShop]
        [Ui]
        public void @default(WebActionContext ac)
        {

        }

        [ToShop]
        [Ui]
        public void all(WebActionContext ac)
        {

        }

        [ToShop]
        [Ui]
        public void list(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        [ToShop]
        [Ui]
        public void clear(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        #endregion

        #region /order/

        [ToAdmin]
        [Ui]
        public void exam(WebActionContext ac)
        {

        }

        #endregion
    }
}