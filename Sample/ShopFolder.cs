using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    public class ShopFolder : Folder
    {
        public ShopFolder(FolderContext fc) : base(fc)
        {
            CreateVar<ShopVarFolder>((tok) => ((User)tok).shopid);
        }

        // [Shop]
        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query(p => p.Set(page)))
                {
                    ac.GiveFolderPage(Parent, 200, dc.ToList<Shop>(proj), proj);
                }
                else
                {
                    ac.GiveFolderPage(Parent, 200, (List<Shop>)null);
                }
            }
        }

        // whether an aligned floating point
        bool IsAligned(string v)
        {
            if (v == null) { return false; }

            int pt = v.IndexOf('.');
            return pt == v.Length - 2;
        }

        [User]
        public void lst(ActionContext ac)
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
                    h =>
                    {
                        h.Add("<div class=\"small-8 columns\"><span>附近的供应点</span></div>");
                        h.Add("<div class=\"small-4 columns text-right\"><a href=\"..//\">购物车<i class=\"fi-shopping-cart\"></i></a></div>");
                    },
                    m =>
                    {
                        var shops = dc.ToList<Shop>(-1 ^ Projection.BIN);
                        for (int i = 0; i < shops.Count; i++)
                        {
                            var shop = shops[i];

                            m.Add("<div class=\"row\">");
                            m.Add("<div class=\"small-2 columns\"><a href=\"#\"><span></span><img src=\""); m.Add(shop.id); m.Add("/_icon_\" alt=\"\" class=\" thumbnail\"></a></div>");
                            m.Add("</div>");
                        }

                    }, null);
                }
                else
                {
                    ac.GiveSnippet(200, h =>
                    {
                        h.CALLOUT("没有找到附近的供应点", true);
                    });
                }
            }
        }

        //
        // administrative actions
        //

        [Ui("申请", Dialog = 1)]
        public async Task apply(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GivePaneForm(200, Shop.Empty, -1 ^ Projection.TRANSF);
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
        [Ui("新建", Dialog = 1)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GivePaneForm(200, Shop.Empty, -1 ^ Projection.TRANSF);
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

        public async Task @null(ActionContext ac)
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
    }
}