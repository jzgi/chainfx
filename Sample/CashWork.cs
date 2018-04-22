using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class CashWork : Work
    {
        protected CashWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [Ui("财务"), User(OPRMGR)]
    public class OprCashWork : CashWork
    {
        public OprCashWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Cash>("SELECT * FROM cashes WHERE orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLEVIEW(arr,
                        () => h.TH("日期").TH("项目").TH("收入").TH("支出").TH("记账"),
                        o => h.TD(o.date).TD(Cash.Codes[o.code]).TD(o.receive).TD(o.pay).TD(o.creator));
                }, false, 2);
            }
        }

        [Ui("记账"), Tool(ButtonShow)]
        public async Task entry(WebContext wc)
        {
            string orgid = wc[-1];
            Cash o = null;
            if (wc.GET)
            {
                o = new Cash() { };
                o.Read(wc.Query);
                wc.GivePane(200, h =>
                {
                    h.FORM_();

                    h.FIELDSET_("填写交易信息");
                    h.SELECT(nameof(o.code), o.code, Cash.Codes, label: "类型");
                    h.TEXT(nameof(o.descr), o.descr, "简述", max: 20);
                    h.LI_().LABEL("收入").NUMBER(nameof(o.receive), o.receive).LABEL("支出").NUMBER(nameof(o.pay), o.pay)._LI();
                    h._FIELDSET();

                    h._FORM();
                });
                return;
            }
            o = await wc.ReadObjectAsync(obj: new Cash
            {
                orgid = orgid,
                date = DateTime.Now,
                creator = ((User) wc.Principal).name
            });
            using (var dc = NewDbContext())
            {
                const byte proj = 0xff ^ Cash.ID;
                dc.Sql("INSERT INTO cashes")._(Cash.Empty, proj)._VALUES_(Cash.Empty, proj);
                dc.Execute(p => o.Write(p, proj));
            }
            wc.GivePane(200);
        }

        [Ui("月报"), Tool(ButtonOpen)]
        public void monthly(WebContext wc)
        {
            string orgid = wc[-1];
            wc.GivePane(200, m =>
            {
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT to_char(date, 'YYYY-MM') as yrmon, code, SUM(receive), SUM(pay) FROM cashes WHERE orgid = @1 GROUP BY yrmon, txn ORDER BY yrmon DESC", p => p.Set(orgid));
                    while (dc.Next())
                    {
                        dc.Let(out string yrmon).Let(out short txn).Let(out decimal recieved).Let(out decimal paid);
                        m.FIELDSET_(yrmon);

                        m._FIELDSET();
                    }
                }
            }, false, 3);
        }
    }
}