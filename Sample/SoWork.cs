using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.So;
using static Samp.User;
using static Greatbone.Style;

namespace Samp
{
    public abstract class SoWork<V> : Work where V : SoVarWork
    {
        protected SoWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, int>((obj) => ((So) obj).id);
        }

        // for customer side viewing
        protected void GiveBoardPage(WebContext wc, So[] arr, bool tooling = true)
        {
            wc.GivePage(200, h =>
            {
                if (tooling)
                {
                    h.TOOLBAR(title: "购物车");
                }
                h.BOARD(arr, o =>
                    {
                        h.T("<header class=\"uk-card-header uk-flex uk-flex-middle\">");
                        h.T("<h5>").T(o.tmid).T("</h5>").BADGE(Statuses[o.status], o.status == 0 ? Warning : o.status == 1 ? Success : None);
                        h.T("</header>");

                        h.UL_("uk-list uk-list-divider uk-card-body");

                        h.LI("收　货", o.uaddr, o.uname, o.utel);
                        h.LI_();
                        h.UL_("uk-list uk-list-divider uk-width-1-1");
                        h.LI_();
                        h.ICO_(w: 0x16).T('/').T(o.ctrid).T('/').T(o.item).T("/icon")._ICO();
                        if (o.status == CREATED)
                        {
                            h.P(o.item, w: 0x13).P_(w: 0x13).CUR(o.price).T("／").T(o.unit)._P();
                            h.T("<p class=\"uk-width-1-6 \">").TOOL(nameof(MySoVarWork.Upd), 0, o.qty.ToString())._P();
                        }
                        else
                        {
                            h.P(o.item, w: 0x12).P_(w: 0x16).CUR(o.price)._P().P_(w: 0x16).T(o.qty).T(o.unit)._P();
                        }
                        h._LI();
                        h._UL();
                        h._LI();
                        h.LI_("总　额").CUR(o.total)._LI();
                        h._UL(); // uk-card-body

                        if (tooling) h.VARTOOLS(css: "uk-card-footer");
                    }
                );
            }, false, 2);
        }

        // for org side viewing
        protected void GiveAccordionPage(WebContext wc, So[] arr, bool tools = true)
        {
            wc.GivePage(200, h =>
            {
                if (tools)
                {
                    h.TOOLBAR();
                }
                h.ACCORDION(arr,
                    o =>
                    {
                        h.T("<section class=\"uk-accordion-title\">");
                        h.T("<h4 class=\"uk-width-expand\">").T(o.uname).T("</h4>").BADGE(Statuses[o.status], o.status == 0 ? Warning : o.status == 1 ? Success : None);
                        h.T("</section>");

                        h.T("<section class=\"uk-accordion-content uk-grid\">");
                        h.P_("收　货").SP().T(o.uname).SP().T(o.uaddr).SP().T(o.utel)._P();
                        h.UL_("uk-grid");
                        h.LI_();
                        h.SPAN_(0x11).T(o.item)._SPAN().SPAN_(w: 0x23).T('￥').T(o.price).T("／").T(o.unit)._SPAN().SPAN(o.qty, w: 0x13);
                        h._LI();
                        h._UL();
                        h.P_("总　额", w: 0x12).CUR(o.total)._P();

                        h.VARTOOLS();

                        h.T("</section>");
                    }, null);
            }, false, 2);
        }
    }

    public class MySoWork : SoWork<MySoVarWork>
    {
        public MySoWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            int myid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<So>("SELECT * FROM sos WHERE status BETWEEN 0 AND 1 AND custid = @1 ORDER BY id DESC", p => p.Set(myid));
                GiveBoardPage(wc, arr);
            }
        }

        [Ui("历史订单"), Tool(AOpen, size: 2)]
        public void old(WebContext wc, int page)
        {
            int myid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<So>("SELECT * FROM sos WHERE status >= 2 AND custid = @1 ORDER BY id DESC", p => p.Set(myid));
                GiveBoardPage(wc, arr, false);
            }
        }
    }

    [Ui("订单")]
    public class TmSoWork : SoWork<TmSoVarWork>
    {
        public TmSoWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
        }
    }

    [Ui("新单")]
    public class CtrNewSoWork : SoWork<CtrNewSoVarWork>
    {
        public CtrNewSoWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status BETWEEN 0 AND 1 AND orgid = @1 ORDER BY id DESC", p => p.Set(orgid));
                GiveAccordionPage(wc, dc.ToArray<So>());
            }
        }

        static readonly Map<string, string> MSGS = new Map<string, string>
        {
            ["订单处理"] = "我们已经接到您的订单（金额{0}元）",
            ["派送通知"] = "销售人员正在派送您所购的商品",
            ["sdf"] = "",
        };

        [Ui("通知"), Tool(ButtonPickShow)]
        public void send(WebContext wc)
        {
            long[] key = wc.Query[nameof(key)];
            string msg = null;
            if (wc.GET)
            {
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.RADIOSET(nameof(msg), msg, MSGS, "消息通知买家", w: 0x4c);
                    m._FORM();
                });
            }
            else
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT wx FROM orders WHERE id")._IN_(key);
                    dc.Execute(prepare: false);
                }
                wc.GivePane(200);
            }
        }

        [Ui("清理", "清理三天以前未付款或者已撤销的订单"), Tool(ButtonConfirm)]
        public void clean(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Execute("DELETE FROM orders WHERE (status = 0 OR status = -1) AND orgid = @1 AND (created + interval '3 day' < localtimestamp)", p => p.Set(orgid));
            }
            wc.GiveRedirect();
        }
    }

    [Ui("旧单"), UserAccess(OPR)]
    public class CtrOldSoWork : SoWork<CtrOldSoVarWork>
    {
        public CtrOldSoWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<So>("SELECT * FROM orders WHERE status >= 2 AND orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
                GiveAccordionPage(wc, arr);
            }
        }

        [Ui("查询"), Tool(AShow)]
        public void send(WebContext wc)
        {
            long[] key = wc.Query[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                dc.Execute();
            }
            wc.GiveRedirect();
        }

        [Ui("回退", "【警告】把选中的订单回退成新单？"), Tool(ButtonPickConfirm)]
        public async Task back(WebContext wc)
        {
            string orgid = wc[-2];
            var f = await wc.ReadAsync<Form>();
            string[] key = f[nameof(key)];
            if (key != null)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = ").T(PAID).T(" WHERE status > ").T(PAID).T(" AND orgid = @1 AND id")._IN_(key);
                    dc.Execute(p => p.Set(orgid), prepare: false);
                }
            }
            wc.GiveRedirect();
        }
    }
}