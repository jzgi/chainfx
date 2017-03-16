using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.Order;

namespace Greatbone.Sample
{
    public class OrderFolder : Folder
    {
        public OrderFolder(FolderContext fc) : base(fc)
        {
            CreateVar<OrderVarFolder>();
        }

        // [Shop]
        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[typeof(ShopVarFolder)];
            short status = Minor;
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = @2 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(status).Set(page * 20)))
                {
                    ac.GiveFolderPage(Parent, 200, dc.ToList<Order>());
                }
                else
                {
                    ac.GiveFolderPage(Parent, 200, (List<Order>)null);
                }
            }
        }

        [Ui("核对付款")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public async Task check(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        [Ui("锁定备货")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public async Task fix(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        [Ui("标注完成")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public void close(ActionContext ac)
        {
        }

        [Ui("取消")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public async Task cancel(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        var order = dc.ToArray<Order>();
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id IN () AND shopid = @1 AND ").statecond();
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        ac.Give(303); // see other
                    }
                    else
                    {
                        ac.Give(303); // see other
                    }
                }
            }
        }

        [User]
        [Ui]
        public void clear(ActionContext ac)
        {
            // string shopid = wc.Var(null);

        }






        [User]
        [Ui]
        public void exam(ActionContext ac)
        {

        }

    }
}