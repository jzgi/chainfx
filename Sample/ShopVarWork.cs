using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class ShopVarWork : Work
    {
        protected ShopVarWork(WorkContext wc) : base(wc)
        {
        }

        public void icon(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(shopid)))
                {
                    var byteas = dc.GetByteAS();
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else
                    {
                        StaticContent cont = new StaticContent(byteas);
                        ac.Give(200, cont, pub: true, maxage: 60 * 5);
                    }
                }
                else ac.Give(404, pub: true, maxage: 60 * 5); // not found
            }
        }
    }

    public class PubShopVarWork : ShopVarWork
    {
        public PubShopVarWork(WorkContext wc) : base(wc)
        {
            CreateVar<ItemVarWork, string>(obj => ((Item) obj).name);
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                // query for the shop record
                const int proj = -1 ^ Shop.ICON;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToData<Shop>(proj);

                    // query for item records of the shop
                    const short proj2 = -1 ^ Item.ICON;
                    Item[] items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty, proj2)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToDatas<Item>(proj2);
                    }

                    ac.GivePage(200, m =>
                    {
                        m.Add("<div data-sticky-container>");
                        m.Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        m.Add("<div class=\"title-bar\">");
                        m.Add("<div class=\"title-bar-left\">");
                        m.Add("<span style=\"font-size: 1.25rem;\">");
                        m.Add(shop.name);
                        m.Add("</span>");
                        m.Add("</div>");
                        m.Add("<div class=\"title-bar-right\">");
                        m.Add("<a class=\"float-right\" href=\"/my//cart/\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-shopping-cart fa-stack-1x fa-inverse\"></i></span></a>");
                        m.Add("</div>");
                        m.Add("</div>");
                        m.Add("</div>");
                        m.Add("</div>");

                        m.Add("<div>");
                        m.Add("<p>");
                        m.Add(shop.city);
                        m.Add(shop.addr);
                        m.Add("</p>");
                        m.Add("<p>");
                        m.Add(shop.descr);
                        m.Add("</p>");
                        m.Add("</div>");

                        // display items

                        if (items == null)
                        {
                            m.Add("没有上架商品");
                            return;
                        }
                        for (int i = 0; i < items.Length; i++)
                        {
                            Item item = items[i];
                            m.Add("<form>");

                            var shopname = shop.name;

                            m.HIDDEN(nameof(shopid), shopid);
                            m.HIDDEN(nameof(shopname), shopname);
                            m.HIDDEN(nameof(item.name), item.name);
                            m.HIDDEN(nameof(item.unit), item.unit);
                            m.HIDDEN(nameof(item.price), item.price);

                            m.Add("<div class=\"row card align-middle\">");

                            m.Add("<div class=\"small-4 column\">");
                            m.Add("<img src=\"");
                            m.Add(item.name);
                            m.Add("/icon\" alt=\"\" class=\"thumbnail\">");
                            m.Add("</div>"); // column

                            m.Add("<div class=\"small-8 column\">");
                            m.Add("<h3>");
                            m.Add(item.name);
                            if (item.qty > 0)
                            {
                                m.Add("（");
                                m.Add(item.qty);
                                m.Add(item.unit);
                                m.Add("）");
                            }
                            m.Add("</h3>");
                            m.Add("<div>");
                            m.Add(item.descr);
                            m.Add("</div>");

                            m.Add("<p>");
                            m.Add("<strong class=\"money\">&yen;");
                            m.Add(item.price);
                            m.Add("</strong> 每");
                            m.Add(item.unit);
                            m.Add("</p>");

                            m.Add("<div class=\"row\">");

                            m.Add("<div class=\"small-7 columns\">");
                            m.NUMBER(nameof(item.qty), item.min, step: item.step);
                            m.Add("</div>");

                            m.Add("<div class=\"small-5 columns\">");
                            m.Add("<button type=\"button\" class=\"button success hollow\" onclick=\"var frm=this.form; $.post('/my//cart/add', $(frm).serialize(), function(data){alert('成功加入购物车'); frm.reset();});\">+ 购物车</button>");
                            m.Add("</div>");

                            m.Add("</div>"); // row

                            m.Add("</div>"); // column

                            m.Add("</div>"); // row card
                            m.Add("</form>");
                        }
                    }, @public: true, maxage: 60 * 5);
                }
                else
                {
                    ac.Give(404, pub: true, maxage: 60 * 5); // not found
                }
            }
        }
    }

    [Ui("设置")]
    [User(User.ASSISTANT)]
    public class OprShopVarWork : ShopVarWork
    {
        public OprShopVarWork(WorkContext wc) : base(wc)
        {
            Create<OprCartOrderWork>("cart");

            Create<OprActiveOrderWork>("paid");

            Create<OprPastOrderWork>("past");

            Create<OprAlienOrderWork>("alien");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200, @public: false, maxage: 60 * 5);
        }

        [Ui("基本信息", Mode = UiMode.AnchorDialog)]
        [User(User.ASSISTANT)]
        public async Task profile(ActionContext ac)
        {
            const short proj = -1 ^ Shop.ICON ^ Shop.ID ^ Shop.ADM;
            string id = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                    if (dc.Query1(p => p.Set(id)))
                    {
                        var o = dc.ToData<Shop>(proj);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.TEXT(nameof(o.name), o.name, label: "商家名称", max: 10, @readonly: true);
                            m.TEXT(nameof(o.descr), o.descr, label: "商家描述", max: 20, required: true);
                            m.TEXT(nameof(o.tel), o.tel, label: "电话", max: 11, min: 11, pattern: "[0-9]+", required: true);
                            m.TEXT(nameof(o.city), o.city, label: "城市", @readonly: true);
                            m.SELECT(nameof(o.distr), o.distr, ((ShopService) Service).GetDistrs(o.city), label: "区域");
                            m.TEXT(nameof(o.addr), o.addr, label: "地址");
                            m.SELECT(nameof(o.status), o.status, Shop.STATUS, label: "状态");
                            m._FORM();
                        });
                    }
                    else
                    {
                        ac.Give(404); // not found
                    }
                }
            }
            else // post
            {
                var o = await ac.ReadDataAsync<Shop>();
                o.id = ac[this];
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE shops")._SET_(Shop.Empty, proj)._("WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.WriteData(p, proj);
                        p.Set(id);
                    });
                }
                ac.GivePane(200);
            }
        }

        [Ui("场地照片", Mode = UiMode.AnchorCrop, Circle = true)]
        [User(User.ASSISTANT)]
        public new async Task icon(ActionContext ac)
        {
            string id = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(id)))
                    {
                        var byteas = dc.GetByteAS();
                        if (byteas.Count == 0) ac.Give(204); // no content
                        else
                        {
                            StaticContent cont = new StaticContent(byteas);
                            ac.Give(200, cont);
                        }
                    }
                    else ac.Give(404); // not found
                }
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                ArraySegment<byte> icon = f[nameof(icon)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET icon = @1 WHERE id = @2", p => p.Set(icon).Set(id));
                    ac.Give(200); // ok
                }
            }
        }

        [Ui("操作授权", Mode = UiMode.AnchorDialog)]
        [User(User.MANAGER)]
        public async Task crew(ActionContext ac, int subcmd)
        {
            string shopid = ac[this];

            // form submitted values
            string id;
            string oprid = null;
            short opr = 0;

            var f = await ac.ReadAsync<Form>();
            if (f != null)
            {
                id = f[nameof(id)];
                oprid = f[nameof(oprid)];
                opr = f[nameof(opr)];
                if (subcmd == 1) // remove
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = NULL, opr = 0 WHERE id = @1", p => p.Set(id));
                    }
                }
                else if (subcmd == 2) // add
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = @1, opr = @2 WHERE id = @3", p => p.Set(shopid).Set(opr).Set(oprid));
                    }
                }
            }

            ac.GivePane(200, m =>
            {
                m.FORM_();

                m.FIELDSET_("现有操作授权");
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT id, name, opr FROM users WHERE oprat = @1", p => p.Set(shopid)))
                    {
                        m.RADIOS(nameof(id), dc, (inp, h, prime) =>
                        {
                            if (prime)
                            {
                                h.Add(inp.GetString("id"));
                                h.Add(' ');
                                h.Add(inp.GetString("name"));
                                h.Add(' ');
                                h.Add(User.OPR[inp.GetShort("opr")]);
                            }
                            else
                            {
                                h.Add(inp.GetString("id"));
                            }
                        });
                        m.BUTTON(nameof(crew), 1, "删除");
                    }
                }
                m._FIELDSET();

                m.FIELDSET_("添加操作授权");
                m.TEXT(nameof(oprid), oprid, label: "个人手机号", max: 11, min: 11, pattern: "[0-9]+");
                m.SELECT(nameof(opr), opr, User.OPR, label: "操作权限");
                m.BUTTON(nameof(crew), 2, "添加");
                m._FIELDSET();
                m._FORM();
            });
        }
    }

    public class SprShopVarWork : ShopVarWork
    {
        public SprShopVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Mode = UiMode.AnchorDialog)]
        public async Task edit(ActionContext ac)
        {
            if (ac.GET)
            {
                string id = ac[this];
                string city = ac[typeof(CityVarWork)];
                using (var dc = ac.NewDbContext())
                {
                    const int proj = -1 ^ Shop.ICON;
                    dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1 AND city = @2");
                    if (dc.Query1(p => p.Set(id).Set(city)))
                    {
                        var o = dc.ToData<Shop>(proj);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.TEXT(nameof(o.name), o.name);
                            m.TEXT(nameof(o.city), o.city, label: "城市", @readonly: true);
                            m.SELECT(nameof(o.distr), o.distr, ((ShopService) Service).GetDistrs(o.city), label: "区域");
                            m.TEXT(nameof(o.lic), o.lic, label: "工商登记");
                            m.TEXT(nameof(o.mgrid), o.mgrid, label: "经理登录号");
                            m._FORM();
                        });
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
            else // post
            {
                var o = await ac.ReadDataAsync<Shop>();
                o.id = ac[this];
                using (var dc = ac.NewDbContext())
                {
                    const int proj = -1 ^ Shop.ICON;
                    dc.Sql("UPDATE shops")._SET_(Shop.Empty, proj)._("WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.WriteData(p, proj);
                        p.Set(o.id);
                    });
                }
                ac.GiveRedirect();
            }
        }

        [Ui("图片", Mode = UiMode.AnchorCrop, Circle = true)]
        public new async Task icon(ActionContext ac)
        {
            string id = ac[this];
            string city = ac[typeof(CityVarWork)];
            if (ac.GET)
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM shops WHERE id = @1 AND city = @2", p => p.Set(id).Set(city)))
                    {
                        var byteas = dc.GetByteAS();
                        if (byteas.Count == 0) ac.Give(204); // no content
                        else
                        {
                            StaticContent cont = new StaticContent(byteas);
                            ac.Give(200, cont);
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
                    if (dc.Execute("UPDATE shops SET icon = @1 WHERE id = @2 AND city = @3", p => p.Set(icon).Set(id).Set(city)) > 0)
                    {
                        ac.Give(200); // ok
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }

        [Ui("设置经理", Mode = UiMode.AnchorDialog)]
        public new async Task mgr(ActionContext ac)
        {
            string shopid = ac[this];
            string city = ac[typeof(CityVarWork)];
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        if (dc.Query("SELECT id, name, wx FROM users WHERE city = @1 AND NOT id IS NULL AND NOT name IS NULL", p => p.Set(city)))
                        {
                            m.RADIOS("id_wx", dc, (inp, h, prime) =>
                            {
                                if (prime)
                                {
                                    h.Add(inp.GetString("id"));
                                    h.Add(' ');
                                    h.Add(inp.GetString("name"));
                                }
                                else
                                {
                                    h.Add(inp.GetString("id"));
                                    h.Add('-');
                                    h.Add(inp.GetString("wx"));
                                }
                            });
                            m._FORM();
                        }
                    }
                });
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                string id_wx = f[nameof(id_wx)];
                Duo<string, string> duo = id_wx.ToStringString();
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(@"UPDATE shops SET mgrid = @1, mgrwx = @2 WHERE id = @3;
                        UPDATE users SET oprat = @3, opr = @4 WHERE wx = @2;", p => p.Set(duo.X).Set(duo.Y).Set(shopid).Set(User.MANAGER));
                }
                ac.GivePane(200);
            }
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}