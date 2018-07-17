using System.Threading.Tasks;
using Greatbone;
using static System.Data.IsolationLevel;
using static Greatbone.Modal;
using static Samp.So;
using static Samp.SampUtility;

namespace Samp
{
    public abstract class SoVarWork : Work
    {
        protected SoVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class MySoVarWork : SoVarWork
    {
        public MySoVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("付款"), Tool(ButtonScript), SoState('P')]
        public async Task prepay(WebContext wc)
        {
            var prin = (User) wc.Principal;
            int orderid = wc[this];
            So o;
            using (var dc = NewDbContext())
            {
                const byte proj = 0xff ^ DETAIL;
                dc.Sql("SELECT ").collst(Empty, proj).T(" FROM orders WHERE id = @1 AND custid = @2");
                o = dc.Query1<So>(p => p.Set(orderid).Set(prin.id), proj);
            }
            var (prepay_id, _) = await WeiXinUtility.PostUnifiedOrderAsync(
                orderid + "-" + o.rev,
                o.cash,
                prin.wx,
                wc.RemoteAddr.ToString(),
                NETADDR + "/" + nameof(SampService.onpay),
                "粗粮达人-健康产品"
            );
            if (prepay_id != null)
            {
                wc.Give(200, WeiXinUtility.BuildPrepayContent(prepay_id));
            }
            else
            {
                wc.Give(500);
            }
        }

        [Ui("修改", grou: 0x10), Tool(AShow, Style.Icon, size: 2)]
        public async Task Upd(WebContext wc, int idx)
        {
            int orderid = wc[this];
            int myid = wc[-2];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<So>("SELECT * FROM orders WHERE id = @1 AND custid = @2", p => p.Set(orderid).Set(myid));
                    var oi = o.items[idx];
                    var item = Obtain<Map<(string, string), Item>>()[(o.orgid, oi.name)];
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("购买数量");
                        h.LI_("货品").PIC("/" + o.orgid + "/" + oi.name + "/icon", w: 0x16).SP().T(oi.name)._LI();
                        h.NUMBER(nameof(oi.qty), oi.qty, "购量", max: item.max, min: (short) 0, step: item.step);
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
                    var o = dc.Query1<So>("SELECT * FROM orders WHERE id = @1 AND custid = @2", p => p.Set(orderid).Set(myid));
                    o.UpdItem(idx, qty);
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2, net = @3 WHERE id = @4", p => p.Set(o.items).Set(o.total).Set(o.points).Set(o.id));
                }
                wc.GivePane(200);
            }
        }
    }

    public class OprNewoVarWork : SoVarWork
    {
        public OprNewoVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("撤消", "确认要撤销此单吗？实收款项将退回给买家"), Tool(ButtonConfirm)]
        public async Task abort(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            short rev = 0;
            decimal cash = 0;
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT rev, cash FROM orders WHERE id = @1 AND status = 1", p => p.Set(orderid)))
                {
                    dc.Let(out rev).Let(out cash);
                }
            }
            if (cash > 0)
            {
                string err = await WeiXinUtility.PostRefundAsync(orderid + "-" + rev, cash, cash);
                if (err == null) // success
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE orders SET status = -1, aborted = localtimestamp WHERE id = @1 AND orgid = @2", p => p.Set(orderid).Set(orgid));
                    }
                }
            }
            wc.GiveRedirect("../");
        }

        [Ui("完成", "确定结束该订单？"), Tool(ButtonConfirm), SoState('E')]
        public void end(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            using (var dc = NewDbContext(ReadCommitted))
            {
                dc.Execute("UPDATE orders SET status = 2, ended = localtimestamp WHERE id = @1 AND orgid = @2 AND status = 1", p => p.Set(orderid).Set(orgid));
            }
            wc.GiveRedirect("../");
        }
    }

    public class OprOldoVarWork : SoVarWork
    {
        public OprOldoVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class AdmKickVarWork : SoVarWork
    {
        public AdmKickVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}