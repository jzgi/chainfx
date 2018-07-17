using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class PoWork<V> : Work where V : PoVarWork
    {
        protected PoWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, short>(obj => ((Tut) obj).id);
        }
    }

    [Ui("资讯")]
    public class CtrlyPoWork : PoWork<CtrlyPoVarWork>
    {
        public CtrlyPoWork(WorkConfig cfg) : base(cfg)
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