using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class ItemVarWork : Work
    {
        protected ItemVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class PubItemVarWork : ItemVarWork
    {
        public PubItemVarWork(WorkContext wc) : base(wc)
        {
        }

        public void icon(ActionContext ac)
        {
            short shopid = ac[-1];
            string name = ac[this];
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                {
                    ArraySegment<byte> byteas;
                    dc.Let(out byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else
                    {
                        ac.Give(200, new StaticContent(byteas), true, 60 * 5);
                    }
                }
                else ac.Give(404, pub: true, maxage: 60 * 5); // not found
            }
        }
    }

    public class OprItemVarWork : ItemVarWork
    {
        public OprItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Mode = UiMode.ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            short shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        var o = dc.ToObject<Item>();
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.CELL(o.name, "名称");
                            m.TEXT(nameof(o.descr), o.descr, "简述", max: 30, required: true);
                            m.TEXT(nameof(o.unit), o.unit, "单位", required: true);
                            m.NUMBER(nameof(o.price), o.price, "单价", required: true);
                            m.NUMBER(nameof(o.min), o.min, "起订数量", min: (short) 1);
                            m.NUMBER(nameof(o.step), o.step, "增减间隔", min: (short) 1);
                            m.NUMBER(nameof(o.max), o.max, "剩余供给");
                            m.SELECT(nameof(o.status), o.status, Item.STATUS, "状态");
                            m._FORM();
                        });
                    }
                    else ac.Give(500); // internal server error
                }
            }
            else // post
            {
                const short proj = -1 ^ Item.UNMOD;
                var o = await ac.ReadObjectAsync<Item>(proj);
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj)._("WHERE shopid = @1 AND name = @2");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(shopid).Set(name);
                    });
                }
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("图片", Mode = UiMode.ACrop, Circle = true)]
        public new async Task icon(ActionContext ac)
        {
            short shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        ArraySegment<byte> byteas;
                        dc.Let(out byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else
                        {
                            ac.Give(200, new StaticContent(byteas));
                        }
                    }
                    else ac.Give(404); // not found           
                }
            }
            else // post
            {
                var frm = await ac.ReadAsync<Form>();
                ArraySegment<byte> icon = frm[nameof(icon)];
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE shopid = @2 AND name = @3", p => p.Set(icon).Set(shopid).Set(name)) > 0)
                    {
                        ac.Give(200); // ok
                    }
                    else ac.Give(500); // internal server error
                }
            }
        }
    }
}