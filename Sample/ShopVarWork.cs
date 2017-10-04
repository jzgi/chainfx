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
        }

        public void @default(ActionContext ac)
        {
            short shopid = ac[this];
            using (var dc = ac.NewDbContext())
            {
                const int proj = Shop.ID | Shop.BASIC;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToObject<Shop>(proj);

                    // items of the shop
                    Item[] items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty, Item.UNMOD)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToArray<Item>(Item.UNMOD);
                    }

                    ac.GivePage(200, m =>
                    {
                        m.T("<div data-sticky-container>");
                        m.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        m.T("<div class=\"top-bar\">");
                        m.T("<div class=\"top-bar-left\">");
                        m.T("<a href=\"https://mp.weixin.qq.com/mp/profile_ext?action=home&__biz=MzU4NDAxMTAwOQ==&scene=124#wechat_redirect\">关注公众号</a>");
                        m.T("<a href=\"../\"><i class=\"fa fa-arrow-left\"></i></a>");
                        m.T("<span style=\"font-size: 1.25rem;\">").T(shop.name).T("</span>");
                        m.T("</div>");
                        m.T("<div class=\"top-bar-right\">");
                        m.T("<a class=\"float-right\" href=\"/my//cart/\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-shopping-cart fa-stack-1x fa-inverse\"></i></span></a>");
                        m.T("</div>");
                        m.T("</div>");
                        m.T("</div>");
                        m.T("</div>");

                        m.T("<div class=\"row\" style=\"background-color: white\">");
                        m.T("<div class=\"small-8 column\">");
                        m.T("<p>").T(shop.city).T(shop.addr).T("</p>");
                        m.T("</div>");
                        m.T("<div class=\"small-4 column\">");
                        m.T("<p><i class=\"fa fa-phone\"></i>").T(shop.oprtel).T("</p>");
                        m.T("<a href=\"custsvc\" class=\"button hollow\" onclick=\"return dialog(this,4,2);\">在线客服</a>");
                        m.T("</div>");
                        m.T("</div>");

                        // display items

                        if (items == null)
                        {
                            m.T("没有上架商品");
                            return;
                        }

                        m.T("<div class=\"grid-x grid-padding-x small-up-1 medium-up-2\">");
                        for (int i = 0; i < items.Length; i++)
                        {
                            m.T("<div =\"cell\">");
                            var item = items[i];
                            m.T("<form>");

                            var shopname = shop.name;

                            m.HIDDEN(nameof(shopid), shopid);
                            m.HIDDEN(nameof(shopname), shopname);
                            m.HIDDEN(nameof(item.name), item.name);
                            m.HIDDEN(nameof(item.unit), item.unit);
                            m.HIDDEN(nameof(item.price), item.price);

                            m.T("<div class=\"card\">");

                            m.T("<div class=\"small-4 column\">");
                            m.T("<img src=\"").T(item.name).T("/icon\" alt=\"\" class=\"thumbnail circle\">");
                            m.T("</div>"); // column

                            m.T("<div class=\"small-8 column\">");
                            m.T("<h3>");
                            m.T(item.name);
                            if (item.max > 0)
                            {
                                m.T("（").T(item.max).T(item.unit).T("）");
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
                            m.NUMBER(nameof(item.max), item.min, min: item.min, step: item.step);
                            m.T("</div>");

                            m.T("<div class=\"small-5 columns\">");
                            m.T("<button type=\"button\" class=\"button primary\" onclick=\"var frm=this.form; $.post('/my//pre/add', $(frm).serialize(), function(data){alert('成功加入购物车'); frm.reset();});\">加入购物车</button>");
                            m.T("</div>");

                            m.T("</div>"); // row

                            m.T("</div>"); // column

                            m.T("</div>"); // row card
                            m.T("</form>");
                            m.T("</div>");
                        }
                        m.T("</div>");
                    }, true, 60 * 5);
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

            User prin = (User) ac.Principal;

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
                            m.CARD_();
                            for (int i = 0; i < msgs.Length; i++)
                            {
                                ChatMsg msg = msgs[i];
                                m.CARDITEM(msg.name, msg.text);
                            }
                            m._CARD();
                        }
                    }
                    m.FORM_();
                    m.TEXT(nameof(text), text, "发送信息", pattern: "[\\S]*", max: 30, required: true);
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
                        msgs = msgs.AddOf(new ChatMsg() {name = prin.name, text = text});
                        dc.Execute("UPDATE chats SET msgs = @1, quested = localtimestamp WHERE shopid = @2 AND wx = @3", p => p.Set(msgs).Set(shopid).Set(prin.wx));
                    }
                    else
                    {
                        msgs = new[]
                        {
                            new ChatMsg() {name = prin.name, text = text}
                        };
                        dc.Execute("INSERT INTO chats (shopid, wx, name, msgs, quested) VALUES (@1, @2, @3, @4, localtimestamp)", p => p.Set(shopid).Set(prin.wx).Set(prin.name).Set(msgs));
                    }

                    mgrwx = (string) dc.Scalar("SELECT mgrwx FROM shops WHERE id = @1", p => p.Set(shopid));
                }
                await WeiXinUtility.PostSendAsync(mgrwx, "【买家消息】" + prin.name + "：" + text);
                ac.GivePane(200);
            }
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Mode = UiMode.ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string id = ac[this];
            string city = ac[typeof(Work)];
            string name;
            string distr;
            string lic;
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
                            m.SELECT(nameof(distr), distr, ((SampleService) Service).Cities, "区域");
                            m.TEXT(nameof(lic), lic, "工商登记");
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

        [Ui("设置经理", Mode = UiMode.ButtonShow)]
        public async Task setmgr(ActionContext ac)
        {
            string shopid = ac[this];
            if (ac.GET)
            {
                string forid = ac.Query[nameof(forid)];
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.SEARCH(nameof(forid), forid, label: "查询后台帐号（手机号）", min: 11, max: 11, pattern: "[0-9]+");
                    m.BUTTON("查询", post: false);
                    if (forid != null)
                    {
                        using (var dc = ac.NewDbContext())
                        {
                            if (dc.Query1("SELECT id, name, wx FROM users WHERE id = @1", p => p.Set(forid)))
                            {
                                string id;
                                string name;
                                string wx;
                                dc.Let(out id).Let(out name).Let(out wx);
                                m.FIELDSET_("确认并选择以下用户");
                                m.RADIO("id_wx_name", id, wx, name, false, id, name, null);
                                m._FIELDSET();
                            }
                        }
                    }
                    m._FORM();
                });
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                string id_wx_name = f[nameof(id_wx_name)];
                var tri = id_wx_name.ToTriple<string, string, string>();
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(@"UPDATE shops SET mgrid = @1, mgrwx = @2, mgr = @3 WHERE id = @4;
                        UPDATE users SET oprat = @4, opr = @5 WHERE wx = @2;", p => p.Set(tri.Item1).Set(tri.Item2).Set(tri.Item3).Set(shopid).Set(User.OPRMGR));
                }
                ac.GivePane(200);
            }
        }
    }
}