using System;
using System.Threading.Tasks;
using Greatbone.Core;
using Microsoft.VisualBasic.CompilerServices;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    public abstract class CashWork<V> : Work where V : CashVarWork
    {
        protected CashWork(WorkContext wc) : base(wc)
        {
        }
    }

    [Ui(" 钱账"), Allow(OPRMGR)]
    public class OprCashWork : CashWork<OprCashVarWork>
    {
        public OprCashWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM cashes WHERE shopid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20));
                ac.GiveSheetPage(200, dc.ToArray<Cash>(),
                    h => h.TH("日期").TH("项目").TH("收入").TH("支出").TD("记账"),
                    (h, o) => h.TD(o.date).TD(Cash.Codes[o.code]).TD(o.received).TD(o.paid).TD(o.recorder),
                    false, 3);
            }
        }

        [Ui("登记"), Trigger(ButtonShow)]
        public async Task @new(ActionContext ac)
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
                    m.SELECT(nameof(o.code), o.code, Cash.Codes, label: "类型");
                    m.TEXT(nameof(o.descr), o.descr, "简述", max: 20);
                    m.NUMBER(nameof(o.received), o.received, "收入", box: 6).NUMBER(nameof(o.paid), o.paid, "支出", box: 6);
                    m._FORM();
                });
                return;
            }
            o = await ac.ReadObjectAsync<Cash>();
            o.shopid = shopid;
            o.date = DateTime.Now;
            o.recorder = ((User) ac.Principal).name;
            using (var dc = ac.NewDbContext())
            {
                const short proj = -1 ^ Cash.ID;
                dc.Sql("INSERT INTO cashes")._(Cash.Empty, proj)._VALUES_(Cash.Empty, proj);
                dc.Execute(p => o.Write(p, proj));
            }
            ac.GivePane(200);
        }

        [Ui("月报"), Trigger(ButtonOpen)]
        public void monthly(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM cashes WHERE shopid = @1 ORDER BY id DESC", p => p.Set(shopid));
                var items = dc.ToArray<Cash>();
                ac.GivePane(200, m =>
                    {
                        m.SHEETVIEW(items,
                            h => h.TH("日期").TH("项目").TH("收入").TH("支出").TD("记账"),
                            (h, o) => h.TD(o.date).TD(Cash.Codes[o.code]).TD(o.received).TD(o.paid).TD(o.recorder));
                    },
                    false, 3);
            }
        }
    }
}