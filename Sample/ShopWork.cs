using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class ShopWork<V> : Work where V : ShopVarWork
    {
        protected ShopWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, string>(obj => ((Shop) obj).id);
        }
    }

    [User]
    public class PubShopWork : ShopWork<PubShopVarWork>
    {
        public PubShopWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            bool dlg = ac.Query[nameof(dlg)];
            if (dlg)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT DISTINCT city FROM shops"))
                    {
                        ac.GivePane(200, f => f.RADIOS("city", dc, () => f.Add(dc.GetString())));
                    }
                    else
                    {
                        ac.Give(204);
                    }
                }
            }
            else
            {
                string city = ac.Query[nameof(city)];
                if (city == null)
                {
                    city = ((User) ac.Principal).city;
                }
                using (var dc = ac.NewDbContext())
                {
                    ac.GivePage(200, m =>
                    {
                        m.Add("<div data-sticky-container>");
                        m.Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        m.Add("<div class=\"title-bar\">");
                        m.Add("<div class=\"title-bar-title\">");
                        m.Add("<select name=\"city\" style=\"margin: 0; border: 0; color: #ba55d3; font-size: 1.25rem;\" onchange=\"location = location.href.split('?')[0] + '?city=' + this.value;\">");
                        string[] vs = ((ShopService) Service).CityOpt;
                        for (int i = 0; i < vs.Length; i++)
                        {
                            string v = vs[i];
                            m.Add("<option value=\"");
                            m.Add(v);
                            m.Add("\"");
                            if (v == city) m.Add(" selected");
                            m.Add(">");
                            m.Add(v);
                            m.Add("</option>");
                        }
                        m.Add("</select>");
                        m.Add("</div>");
                        m.Add("<div class=\"title-bar-right\">");
                        m.Add("<a class=\"float-right\" href=\"/my//cart/\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-shopping-cart fa-stack-1x fa-inverse\"></i></span></a>");
                        m.Add("</div>");
                        m.Add("</div>");
                        m.Add("</div>");
                        m.Add("</div>");

                        if (dc.Query("SELECT * FROM shops WHERE city = @1 AND status > 0", p => p.Set(city)))
                        {
                            var shops = dc.ToArray<Shop>(-1 ^ Shop.ICON);
                            for (int i = 0; i < shops.Length; i++)
                            {
                                var shop = shops[i];

                                m.Add("<div class=\"row card\">");
                                m.Add("<div class=\"small-4 columns\"><a href=\"#\"><span></span><img src=\"");
                                m.Add(shop.id);
                                m.Add("/icon\" alt=\"\" class=\" thumbnail\"></a></div>");
                                m.Add("<div class=\"small-8 columns\">");
                                m.Add("<h3><a href=\"");
                                m.Add(shop.id);
                                m.Add("/\">");
                                m.Add(shop.name);
                                m.Add("</a></h3>");
                                m.Add("<p>");
                                m.Add(shop.city);
                                m.Add(shop.addr);
                                m.Add("</p>");
                                m.Add("<p>");
                                m.Add(shop.descr);
                                m.Add("</p>");
                                m.Add("</div>");
                                m.Add("</div>");
                            }
                        }
                        else
                        {
                            m.Add("<div style=\"text-align: center\">");
                            m.Add("<p>");
                            m.Add(city);
                            m.Add("目前没有商家</p>");
                            m.Add("</div>");
                        }
                    });
                }
            }
        }
    }


    public class OprShopWork : ShopWork<OprShopVarWork>
    {
        public OprShopWork(WorkContext wc) : base(wc)
        {
            CreateVar<OprShopVarWork, string>((prin) => ((User) prin).oprat);
        }

        public async Task @goto(ActionContext ac)
        {
            string shopid = null;
            string password = null;
            string orig = ac.Query[nameof(orig)];
            if (ac.GET)
            {
                ac.GiveFormPage(200, nameof(@goto), "请绑定商家", (x) =>
                {
                    x.TEXT(nameof(shopid), shopid, label: "商家编号", required: true);
                    x.PASSWORD(nameof(password), password, label: "密码");
                    x.HIDDEN(nameof(orig), orig);
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                shopid = f[nameof(shopid)];
                password = f[nameof(password)];
                orig = f[nameof(orig)];

                // data op
                User prin = (User) ac.Principal;
                using (var dc = ac.NewDbContext())
                {
                    var credential = (string) dc.Scalar("SELECT credential FROM shops WHERE id = @1", p => p.Set(shopid));
                    if (credential.EqualsCredential(shopid, password))
                    {
                        dc.Execute("UPDATE users SET oprat = @1 WHERE wx = @2", p => p.Set(shopid).Set(prin.wx));
                        prin.oprat = shopid;
                        ac.SetTokenCookie(prin, -1);
                    }
                }
                ac.GiveRedirect(orig);
            }
        }
    }

    public sealed class DvrShopWork : ShopWork<DvrShopVarWork>
    {
        public DvrShopWork(WorkContext wc) : base(wc)
        {
            CreateVar<DvrShopVarWork, string>((prin) => ((User) prin).dvrat);
        }
    }

    [Ui("商家管理")]
    public class MgrShopWork : ShopWork<MgrShopVarWork>
    {
        public MgrShopWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string city = ac[typeof(CityVarWork)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ Shop.ICON;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE city = @1");
                if (dc.Query(p => p.Set(city)))
                {
                    ac.GiveGridFormPage(200, dc.ToArray<Shop>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (Shop[]) null);
                }
            }
        }

        [Ui("新建", UiMode.AnchorDialog)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GivePane(200, m => { });
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>();

                // validate

                using (var dc = Service.NewDbContext())
                {
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
    }

    [Ui("商家管理")]
    public class AdmShopWork : ShopWork<AdmShopVarWork>
    {
        public AdmShopWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ Shop.ICON;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query(p => p.Set(page)))
                {
                    ac.GiveGridFormPage(200, dc.ToArray<Shop>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (Shop[]) null);
                }
            }
        }
    }
}