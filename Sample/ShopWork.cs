using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Proj;

namespace Greatbone.Sample
{
    public class ShopWork : Work
    {
        public ShopWork(WorkContext wc) : base(wc)
        {
            CreateVar<ShopVarWork>((prin) => ((User)prin).shopid);
        }

        [User]
        public void @default(ActionContext ac)
        {
            bool dlg = ac.Query[nameof(dlg)];
            if (dlg)
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT DISTINCT city FROM shops"))
                    {
                        ac.GivePaneForm(200, f =>
                        {
                            int i = 0;
                            while (dc.Next())
                            {
                                i++;
                                string city = dc.GetString();
                                f.Add("<input type=\"radio\" name=\"city\" id=\"city"); f.Add(i);
                                f.Add("\" value=\""); f.Add(city); f.Add("\">");
                                f.Add("<label for=\"city"); f.Add(i); f.Add("\">"); f.Add(city); f.Add("</label>");
                            }
                        });
                    }
                    else { ac.Give(204); }
                }
            }
            else
            {
                string city = ac.Query[nameof(city)];
                if (city == null)
                {
                    city = ((User)ac.Principal).city;
                }
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM shops WHERE ((scope = 0 AND city = @1) OR scope = 1) AND enabled", p => p.Set(city)))
                    {
                        ac.GivePage(200,
                        null,
                        m =>
                        {
                            m.Add("<div class=\"row\">");
                            m.Add("<div class=\"small-8 columns\"><h1><a href=\"\" onclick=\"dialog(this, 2);return false;\">"); m.Add(city); m.Add("（切换城市）</a></h1></div>");
                            m.Add("<div class=\"small-4 columns text-right\"><a href=\"/user//cart/\">购物车<i class=\"fi-shopping-cart warning\"></i>付款</a></div>");
                            m.Add("</div>");

                            var shops = dc.ToList<Shop>(-1 ^ Proj.BIN);
                            for (int i = 0; i < shops.Count; i++)
                            {
                                var shop = shops[i];

                                m.Add("<div class=\"row\">");
                                m.Add("<div class=\"small-3 columns\"><a href=\"#\"><span></span><img src=\""); m.Add(shop.id); m.Add("/_icon_\" alt=\"\" class=\" thumbnail\"></a></div>");
                                m.Add("<div class=\"small-9 columns\">");
                                m.Add("<h3><a href=\""); m.Add(shop.id); m.Add("/\">"); m.Add(shop.name); m.Add("</a></h3>");
                                m.Add("<p>"); m.Add(shop.city); m.Add(shop.addr); m.Add("</p>");
                                m.Add("<p>"); m.Add(shop.descr); m.Add("</p>");
                                m.Add("</div>");
                                m.Add("</div>");
                            }

                        }, null);
                    }
                    else
                    {
                        // ac.GivePage(200, h =>
                        // {
                        //     h.CALLOUT("没有找到附近的供应点", true);
                        // });
                    }
                }
            }
        }

        [User]
        public void _(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query(p => p.Set(page)))
                {
                    ac.GiveWorkPage(Parent, 200, dc.ToList<Shop>(proj), proj);
                }
                else
                {
                    ac.GiveWorkPage(Parent, 200, (List<Shop>)null);
                }
            }
        }

        public async Task @goto(ActionContext ac)
        {
            if (ac.GET)
            {
                // return a form
                ac.GivePaneForm(200, (x) =>
                {
                    x.TEXT("shopid", "");
                    x.PASSWORD("password", "");
                });

            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                string shopid = f[nameof(shopid)];
                string password = f[nameof(password)];
                string orig = f[nameof(orig)];

                // data op
                User prin = (User)ac.Principal;
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE users SET shopid = @1 WHERE id = @2");
                }

                // return back
                ac.GiveRedirect(orig);
            }
        }


        //
        // administrative actions
        //

        [Ui("申请", Mode = UiMode.AnchorDialog)]
        public async Task apply(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GivePaneForm(200, Shop.Empty, -1 ^ Proj.TRANSF);
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>();

                // validate

                using (var dc = Service.NewDbContext())
                {
                    shop.credential = TextUtility.MD5(shop.id + ':' + shop.credential);
                    dc.Sql("INSERT INTO shops")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(p => p.Set(shop)) > 0)
                    {
                        ac.Give(201); // created
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }

        [User()]
        [Ui("新建", Mode = UiMode.AnchorDialog)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GivePaneForm(200, Shop.Empty, -1 ^ Proj.TRANSF);
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>();

                // validate

                using (var dc = Service.NewDbContext())
                {
                    shop.credential = TextUtility.MD5(shop.id + ':' + shop.credential);
                    dc.Sql("INSERT INTO shops")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(p => p.Set(shop)) > 0)
                    {
                        ac.Give(201); // created
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }

        [Ui("停业/启用")]
        public void toggle(ActionContext ac)
        {

        }

        [Ui("分布报告")]
        public void rpt(ActionContext ac)
        {

        }
    }
}