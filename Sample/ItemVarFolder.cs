using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/-id-/item/-id-/
    ///
    public class ItemVarFolder : Folder, IVar
    {
        public ItemVarFolder(FolderContext fc) : base(fc)
        {
        }

        #region /shop/-id-/item/-id-/

        public void my(ActionContext ac)
        {

        }

        #endregion

        #region /shop/-id-/order/-id-/

        [User]
        public void @default(ActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {

                }
            }
        }

        public void pend(ActionContext ac)
        {
        }

        public void cannel(ActionContext ac)
        {
            string shopid = ac[0];
            int orderid = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(orderid).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {

                }
            }
        }

        #endregion
    }
}