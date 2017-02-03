using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/-id-/order/
    /// or
    /// /user/-id-/order/
    ///
    public class OrderFolder : WebFolder
    {
        public OrderFolder(WebFolderContext fc) : base(fc)
        {
            CreateVar<OrderVarFolder>();
        }

        #region /user/-userwx-/order/

        public void my(WebActionContext ac)
        {

        }

        #endregion

        #region /shop/-shopid-/order/

        [CheckShop]
        [Ui]
        public void @default(WebActionContext ac)
        {

        }

        [CheckShop]
        [Ui]
        public void all(WebActionContext ac)
        {

        }

        [CheckShop]
        [Ui]
        public void list(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        [CheckShop]
        [Ui]
        public void clear(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        #endregion
    }
}