using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
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

        public void @default(WebContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM cashes WHERE shopid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20));
                ac.GiveSheetPage(
                    200,
                    dc.ToArray<Cash>(),
                    h => h.TH("日期").TH("项目").TH("收入").TH("支出").TD("记账"),
                    (h, o) => h.TD(o.date).TD(Cash.Codes[o.txn]).TD(o.received).TD(o.paid).TD(o.keeper),
                    false, 3
                );
            }
        }

        [Ui("记账"), Tool(ButtonShow)]
        public async Task entry(WebContext ac)
        {
            string shopid = ac[-1];
            Cash o = null;
            if (ac.GET)
            {
                o = new Cash() { };
                o.Read(ac.Query);
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.SELECT(nameof(o.txn), o.txn, Cash.Codes, label: "类型");
                    m.TEXT(nameof(o.descr), o.descr, "简述", max: 20);
                    m.NUMBER(nameof(o.received), o.received, "收入", box: 6).NUMBER(nameof(o.paid), o.paid, "支出", box: 6);
                    m._FORM();
                });
                return;
            }
            o = await ac.ReadObjectAsync(obj: new Cash
            {
                shopid = shopid,
                date = DateTime.Now,
                keeper = ((User)ac.Principal).name
            });
            using (var dc = ac.NewDbContext())
            {
                const byte proj = 0xff ^ Cash.ID;
                dc.Execute(dc.Sql("INSERT INTO cashes")._(Cash.Empty, proj)._VALUES_(Cash.Empty, proj), p => o.Write(p, proj));
            }
            ac.GivePane(200);
        }

        [Ui("月报"), Tool(ButtonOpen, 2)]
        public void monthly(WebContext ac)
        {
            string shopid = ac[-1];
            ac.GivePane(200, m =>
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query("SELECT to_char(date, 'YYYY-MM') as yrmon, txn, SUM(received), SUM(paid) FROM cashes WHERE shopid = @1 GROUP BY yrmon, txn ORDER BY yrmon DESC", p => p.Set(shopid));
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