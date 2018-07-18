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
            CreateVar<V, short>(obj => ((Tut) obj).id);
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
                        h.PIC_(w: 0x13).T(o.id).T("/icon")._PIC();
                        h.COL_(0x23, css: "uk-padding-small");
                        h.T("<h3>").T(o.subject).T("</h3>");
                        h.P(o.text);
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
                    h.FIELDSET_();
                    h.TEXT(nameof(o.subject), o.subject, "标题", min: 4, max: 20, required: true);
                    h.TEXTAREA(nameof(o.text), o.text, "内容", max: 200, required: true);
                    h._FIELDSET();
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