using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/-id-/item/-id-/
    ///
    public class ItemVarFolder : WebFolder, IVar
    {
        public ItemVarFolder(WebFolderContext fc) : base(fc)
        {
        }

        #region /shop/-id-/item/-id-/

        public void my(WebActionContext ac)
        {

        }

        #endregion

        #region /shop/-id-/order/-id-/

        [Shop]
        public void @default(WebActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(WfOrder.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<WfOrder>();
                    ac.ReplyPage(200, main =>
                    {

                    });
                }
                else
                    ac.ReplyPage(200, main => { });
            }
        }

        public void pend(WebActionContext ac)
        {
        }

        public void cannel(WebActionContext ac)
        {
            string shopid = ac[0];
            int orderid = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(WfOrder.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(orderid).Set(shopid)))
                {
                    var order = dc.ToArray<WfOrder>();
                    ac.ReplyPage(200, main =>
                    {

                    });
                }
                else
                    ac.ReplyPage(200, main => { });
            }
        }

        public void close(WebActionContext ac)
        {
        }

        #endregion
    }
}