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
                    ArraySegment<byte> byteas;
                    dc.Let(out byteas);
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
            CreateVar<ItemVarWork, string>(obj => ((Item)obj).name);
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                // query for the shop record
                const ushort proj = Shop.ID | Shop.BASIC;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToData<Shop>(proj);

                    // query for item records of the shop
                    Item[] items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty, Item.BASIC_SHOPID)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToDatas<Item>(Item.BASIC_SHOPID);
                    }

                    ac.GivePage(200, m =>
                    {
                        m.T("<div data-sticky-container>");
                        m.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        m.T("<div class=\"title-bar\">");
                        m.T("<div class=\"title-bar-left\">");
                        m.T("<span style=\"font-size: 1.25rem;\">").T(shop.name).T("</span>");
                        m.T("</div>");
                        m.T("<div class=\"title-bar-right\">");
                        m.T("<a class=\"float-right\" href=\"/my//cart/\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-shopping-cart fa-stack-1x fa-inverse\"></i></span></a>");
                        m.T("</div>");
                        m.T("</div>");
                        m.T("</div>");
                        m.T("</div>");

                        m.T("<div class=\"row\" style=\"background-color: white\">");
                        m.T("<div class=\"small-8 column\">");
                        m.T("<p>").T(shop.descr).T("</p>");
                        m.T("<p>").T(shop.city).T(shop.addr).T("</p>");
                        m.T("</div>");
                        m.T("<div class=\"small-4 column\">");
                        m.T("<p><i class=\"fa fa-phone\"></i>").T(shop.tel).T("</p>");
                        m.T("<a href=\"custsvc\" class=\"button hollow\" onclick=\"return dialog(this,4,2);\">在线客服</a>");
                        m.T("</div>");
                        m.T("</div>");

                        // display items

                        if (items == null)
                        {
                            m.T("没有上架商品");
                            return;
                        }
                        for (int i = 0; i < items.Length; i++)
                        {
                            Item item = items[i];
                            m.T("<form>");

                            var shopname = shop.name;

                            m.HIDDEN(nameof(shopid), shopid);
                            m.HIDDEN(nameof(shopname), shopname);
                            m.HIDDEN(nameof(item.name), item.name);
                            m.HIDDEN(nameof(item.unit), item.unit);
                            m.HIDDEN(nameof(item.price), item.price);

                            m.T("<div class=\"row card align-middle\">");

                            m.T("<div class=\"small-4 column\">");
                            m.T("<img src=\"").T(item.name).T("/icon\" alt=\"\" class=\"thumbnail circle\">");
                            m.T("</div>"); // column

                            m.T("<div class=\"small-8 column\">");
                            m.T("<h3>");
                            m.T(item.name);
                            if (item.qty > 0)
                            {
                                m.T("（").T(item.qty).T(item.unit).T("）");
                            }
                            m.T("</h3>");
                            m.T("<div>");
                            m.T(item.descr);
                            m.T("</div>");

                            m.T("<p>");
                            m.T("<strong class=\"money\">&yen;").T(item.price).T("</strong> ");
                            m.T(item.unit);
                            m.T("</p>");

                            m.T("<div class=\"row\">");

                            m.T("<div class=\"small-7 columns\">");
                            m.NUMBER(nameof(item.qty), item.min, min: item.min, step: item.step);
                            m.T("</div>");

                            m.T("<div class=\"small-5 columns\">");
                            m.T("<button type=\"button\" class=\"button success hollow\" onclick=\"var frm=this.form; $.post('/my//cart/add', $(frm).serialize(), function(data){alert('成功加入购物车'); frm.reset();});\">加入购物车</button>");
                            m.T("</div>");

                            m.T("</div>"); // row

                            m.T("</div>"); // column

                            m.T("</div>"); // row card
                            m.T("</form>");
                        }
                    }, @public: true, maxage: 60 * 5);
                }
                else
                {
                    ac.Give(404, pub: true, maxage: 60 * 5); // not found
                }
            }
        }

        public async Task custsvc(ActionContext ac, int subcmd)
        {
            string shopid = ac[this];

            User prin = (User)ac.Principal;

            string text = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    using (var dc = ac.NewDbContext())
                    {
                        if (dc.Query1("SELECT msgs FROM chats WHERE shopid = @1 AND wx = @2", p => p.Set(shopid).Set(prin.wx)))
                        {
                            ChatMsg[] msgs;
                            dc.Let(out msgs);
                            // m.CARD_();
                            for (int i = 0; i < msgs.Length; i++)
                            {
                                ChatMsg msg = msgs[i];
                                m.CARDITEM(msg.name, msg.text);
                            }
                            // m._CARD();
                        }
                    }
                    m.FORM_();
                    m.TEXTAREA(nameof(text), text, "发送信息", max: 30, required: true);
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                text = f[nameof(text)];
                ChatMsg[] msgs;
                string mgrwx = null;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT msgs FROM chats WHERE shopid = @1 AND wx = @2", p => p.Set(shopid).Set(prin.wx)))
                    {
                        dc.Let(out msgs);
                        msgs = msgs.AddOf(new ChatMsg() { name = prin.nickname, text = text });
                        dc.Execute("UPDATE chats SET msgs = @1, quested = localtimestamp WHERE shopid = @2 AND wx = @3", p => p.Set(msgs).Set(shopid).Set(mgrwx));
                    }
                    else
                    {
                        msgs = new[]
                        {
                            new ChatMsg() {name = prin.nickname, text = text}
                        };
                        dc.Execute("INSERT INTO chats (shopid, wx, nickname, msgs, quested) VALUES (@1, @2, @3, @4, localtimestamp)", p => p.Set(shopid).Set(mgrwx).Set(prin.nickname).Set(msgs));
                    }

                    mgrwx = (string)dc.Scalar("SELECT mgrwx FROM shops WHERE id = @1", p => p.Set(shopid));
                }
                await WeiXinUtility.PostSendAsync(mgrwx, "[" + prin.nickname + "]" + text);
                ac.GivePane(200);
            }
        }
    }

    [Ui("设置")]
    [User(User.AID)]
    public class OprShopVarWork : ShopVarWork
    {
        public OprShopVarWork(WorkContext wc) : base(wc)
        {
            Create<OprCartOrderWork>("cart");

            Create<OprActiveOrderWork>("active");

            Create<OprPastOrderWork>("past");

            Create<OprCoOrderWork>("coorder");

            Create<OprItemWork>("item");

            Create<OprChatWork>("chat");

            Create<OprRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200, @public: false, maxage: 60 * 5);
        }

        [Ui("基本信息", Mode = UiMode.AnchorShow)]
        [User(User.AID)]
        public async Task profile(ActionContext ac)
        {
            string id = ac[this];
            if (ac.GET)
            {
                const ushort proj = Shop.ID | Shop.BASIC;
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
                            m.SELECT(nameof(o.distr), o.distr, ((ShopService)Service).GetDistrs(o.city), label: "区域");
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
                const ushort proj = Shop.BASIC;
                var o = await ac.ReadDataAsync<Shop>(proj);
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
        [User(User.AID)]
        public new async Task icon(ActionContext ac)
        {
            string id = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(id)))
                    {
                        ArraySegment<byte> byteas;
                        dc.Let(out byteas);
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

        [Ui("操作授权", Mode = UiMode.AnchorOpen)]
        [User(User.MANAGER)]
        public async Task crew(ActionContext ac, int subcmd)
        {
            string shopid = ac[this];

            // form submitted values
            string id;
            string name;
            string oprid = null;
            short opr = 0;

            var f = await ac.ReadAsync<Form>();
            if (f != null)
            {
                f.Let(out id).Let(out oprid).Let(out opr);
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
                        while (dc.Next())
                        {
                            dc.Let(out id).Let(out name).Let(out opr);
                            m.RADIO(nameof(id), id, null, null, false, id, name, User.OPR[opr]);
                        }
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

        [Ui("修改", Mode = UiMode.AnchorShow)]
        public async Task edit(ActionContext ac)
        {
            string id = ac[this];
            string city = ac[typeof(CityVarWork)];
            string name;
            string distr;
            string lic;
            bool disabled = false;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT name, distr, lic FROM shops WHERE id = @1 AND city = @2", p => p.Set(id).Set(city)))
                    {
                        dc.Let(out name).Let(out distr).Let(out lic);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.TEXT(nameof(name), name, "商家名称");
                            m.SELECT(nameof(distr), distr, ((ShopService)Service).GetDistrs(city), "区域");
                            m.TEXT(nameof(lic), lic, "工商登记");
                            m.CHECKBOX(nameof(disabled), disabled, "禁止营业");
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
                var f = await ac.ReadAsync<Form>();
                f.Let(out name).Let(out distr).Let(out lic);
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE shops SET name = @1, distr = @2, lic = @3")._("WHERE id = @4");
                    dc.Execute(p => p.Set(name).Set(distr).Set(lic).Set(id));
                }
                ac.GivePane(200);
            }
        }

        [Ui("设置经理", Mode = UiMode.AnchorShow)]
        public async Task mgr(ActionContext ac)
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
                            while (dc.Next())
                            {
                                string id;
                                string name;
                                string wx;
                                dc.Let(out id).Let(out name).Let(out wx);
                                m.RADIO("id_wx_name", id, wx, name, false, id, name, null);
                            }
                            m._FORM();
                        }
                    }
                });
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                string id_wx_name = f[nameof(id_wx_name)];
                Triple tri = id_wx_name.ToTriple();
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(@"UPDATE shops SET mgrid = @1, mgrwx = @2, mgr = @3 WHERE id = @4;
                        UPDATE users SET oprat = @4, opr = @5 WHERE wx = @2;", p => p.Set(tri.X).Set(tri.Y).Set(tri.Z).Set(shopid).Set(User.MANAGER));
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