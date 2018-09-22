using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class ItemVarWork : Work
    {
        const int PicAge = 60 * 60;

        protected ItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(WebContext wc)
        {
            string name = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE name = @1", p => p.Set(name)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PicAge);
                }
                else wc.Give(404, @public: true, maxage: PicAge); // not found
            }
        }

        public void img(WebContext wc, int ordinal)
        {
            string orgid = wc[-1];
            string name = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE orgid = @1 AND name = @2", p => p.Set(orgid).Set(name)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PicAge);
                }
                else wc.Give(404, @public: true, maxage: PicAge); // not found
            }
        }
    }

    public class RegItemVarWork : ItemVarWork
    {
        public RegItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [UserAuth(RegMgmt)]
        [Ui("资料", "填写货品资料"), Tool(ButtonShow, size: 2)]
        public async Task upd(WebContext wc)
        {
            string name = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Item>("SELECT * FROM items WHERE name = @1 ORDER BY status DESC, name", p => p.Set(name));
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDUL_(o.name);
                        h.LI_().TEXTAREA("简　介", nameof(o.descr), o.descr, max: 100, min: 20, required: true)._LI();
                        h.LI_().TEXTAREA("说　明", nameof(o.rich), o.descr, max: 500, min: 100, required: true)._LI();
                        h.LI_().URL("视　频", nameof(o.mp4), o.mp4)._LI();
                        h.LI_().TEXT("单　位", nameof(o.unit), o.unit, required: true)._LI();
                        h.LI_().NUMBER("单　价", nameof(o.price), o.price, required: true).LABEL("供应价").NUMBER(null, nameof(o.giverp), o.giverp, required: true)._LI();
                        h.LI_().NUMBER("派送费", nameof(o.dvrerp), o.dvrerp, required: true).LABEL("团组费").NUMBER(null, nameof(o.dvrerp), o.dvrerp, required: true)._LI();
                        h.LI_().NUMBER("起　订", nameof(o.min), o.min, min: (short) 1).LABEL("增　减").NUMBER(null, nameof(o.step), o.step, min: (short) 1)._LI();
                        h.LI_().LABEL("冷　藏").CHECKBOX(nameof(o.refrig), o.refrig)._LI();
                        h._FIELDUL();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                const byte proj = 0;
                var o = await wc.ReadObjectAsync<Item>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE name = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(name);
                    });
                }
                wc.GivePane(200); // close
            }
        }

        [UserAuth(RegMgmt)]
        [Ui("图片"), Tool(ButtonCrop, size: 1)]
        public new async Task icon(WebContext wc)
        {
            if (wc.GET)
            {
                base.icon(wc);
            }
            else // POST
            {
                string name = wc[this];
                var f = await wc.ReadAsync<Form>();
                ArraySegment<byte> img = f[nameof(img)];
                using (var dc = NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE name = @2", p => p.Set(img).Set(name)) > 0)
                    {
                        wc.Give(200); // ok
                    }
                    else wc.Give(500); // internal server error
                }
            }
        }

        [Ui("调度"), Tool(ButtonShow, size: 4)]
        public async Task plan(WebContext wc)
        {
            string name = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Item>("SELECT * FROM items WHERE name = @1", p => p.Set(name));
                    wc.GivePane(200, h =>
                    {
                        string tel = null;
                        h.FORM_();
                        h.FIELDUL_("填写供货信息");
                        h.TEL("手机", nameof(tel), tel);
                        h.TEXT("单位", nameof(o.unit), o.unit, required: true);
                        h.SELECT("状态", nameof(o.status), o.status, Item.Statuses);
                        h._FIELDUL();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                const byte proj = 0xff ^ Item.PK;
                var o = await wc.ReadObjectAsync<Item>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE orgid = @1 AND name = @2");
                    dc.Execute(p => { o.Write(p, proj); });
                }
                wc.GivePane(200); // close
            }
        }
    }
}