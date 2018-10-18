using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class TutWork<V> : Work where V : TutVarWork
    {
        protected TutWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<V>();
        }
    }

    public class PubTutWork : TutWork<PubTutVarWork>
    {
        public PubTutWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT  ").collst(Tut.Empty).T(" FROM infs ORDER BY id LIMIT 20 OFFSET @1");
                var arr = dc.Query<Tut>(p => p.Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.LIST(arr, o =>
                    {
                        h.T("<a class=\"uk-link-reset uk-grid uk-width-1-1\" href=\"").T(o.id).T("/\">");
                        h.PIC_("uk-width-1-3").T(o.id).T("/icon")._PIC();
                        h.COL_(css: "uk-padding-small");
                        h.T("<h3>").T(o.subject).T("</h3>");
                        h.FI(null, o.text);
                        h._COL();
                        h.T("</a>");
                    }, li: "uk-card uk-card-primary");
                }, title: "家庭健康如此行");
            }
        }
    }

    [Ui("资讯")]
    public class PlatTutWork : TutWork<PlatTutVarWork>
    {
        public PlatTutWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT  ").collst(Tut.Empty).T(" FROM infs ORDER BY id LIMIT 20 OFFSET @1");
                var arr = dc.Query<Tut>(p => p.Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr,
                        () => h.TH("日期").TH("标题").TH("内容").TH("状态"),
                        o => h.TD(o.created).TD(o.subject).TD(o.text).TD(Tut.Statuses[o.status]));
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.GET)
            {
                var o = new Tut();
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDUL_();
                    h.TEXT("标题", nameof(o.subject), o.subject, max: 20, min: 4, required: true);
                    h.TEXTAREA("内容", nameof(o.text), o.text, max: 200, required: true);
                    h._FIELDUL();
                    h._FORM();
                });
            }
            else // POST
            {
                var o = await wc.ReadObjectAsync(obj: new Tut
                {
                    created = DateTime.Today
                });
                using (var dc = NewDbContext())
                {
                    const byte proj = 0xff ^ Tut.ID;
                    dc.Sql("INSERT INTO infs ")._(Tut.Empty, proj)._VALUES_(Tut.Empty, proj);
                    dc.Execute(p => o.Write(p));
                    wc.GivePane(200);
                }
            }
        }
    }
}