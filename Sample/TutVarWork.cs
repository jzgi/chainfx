using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class TutVarWork : Work
    {
        protected TutVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(WebContext wc)
        {
            short infid = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM infs WHERE id = @1", p => p.Set(infid)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes));
                }
                else wc.Give(404); // not found           
            }
        }
    }

    public class PubTutVarWork : TutVarWork
    {
        public PubTutVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac)
        {
        }
    }

    public class PlatTutVarWork : TutVarWork
    {
        public PlatTutVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(WebContext ac)
        {
            short infid = ac[this];
            if (ac.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Tut.Empty).T(" FROM infos WHERE id = @1");
                    var o = dc.Query1<Tut>(p => p.Set(infid));
                    ac.GivePane(200, h =>
                    {
                        h.FORM_().FIELDUL_();
                        h.DATE("日期", nameof(o.created), o.created);
                        h.TEXT("标题", nameof(o.subject), o.subject);
                        h.TEXTAREA("正文", nameof(o.text), o.text);
                        h._FIELDUL()._FORM();
                    });
                }
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Tut>();
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE ")._SET_(o)._VALUES_(o).T(" WHERE id = @1");
                    dc.Execute(p => p.Set(infid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("插图"), Tool(ButtonCrop, size: 2)]
        public new async Task icon(WebContext wc)
        {
            short infid = wc[this];
            if (wc.IsGet)
            {
                base.icon(wc);
            }
            else // POST
            {
                ArraySegment<byte> img = (await wc.ReadAsync<Form>())[nameof(img)];
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE infs SET icon = @1 WHERE id = @2", p => p.Set(img).Set(infid));
                }
                wc.Give(200);
            }
        }
    }
}