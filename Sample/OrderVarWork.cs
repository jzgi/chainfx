using System.Threading.Tasks;
using Greatbone;
using static System.Data.IsolationLevel;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class OrderVarWork : Work
    {
        protected OrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class MyOrderVarWork : OrderVarWork
    {
        public MyOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("撤单"), Tool(AShow, size: 2)]
        public async Task cancel(WebContext wc, int idx)
        {
            int orderid = wc[this];
            int myid = wc[-2];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND custid = @2", p => p.Set(orderid).Set(myid));
                    var item = Obtain<Map<string, Item>>()[o.item];
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("购买数量");
                        h.LI_("货品").PIC("/" + o.item + "/icon", w: 0x16).SP().T(o.item)._LI();
//                        h.NUMBER(nameof(oi.qty), oi.qty, "购量", max: item.max, min: (short) 0, step: item.step);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                short qty = f[nameof(qty)];
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND custid = @2", p => p.Set(orderid).Set(myid));
//                    o.UpdItem(idx, qty);
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2, net = @3 WHERE id = @4", p => p.Set(o.item).Set(o.total).Set(o.score).Set(o.id));
                }
                wc.GivePane(200);
            }
        }
    }

    public class GrpOrderVarWork : OrderVarWork
    {
        public GrpOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class CtrOrderVarWork : OrderVarWork
    {
        public CtrOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class SprOrderVarWork : OrderVarWork
    {
        public SprOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class DvrOrderVarWork : OrderVarWork
    {
        public DvrOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}