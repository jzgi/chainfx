using System;
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

    public class ResolveCityAttribute : Attribute, IBefore
    {
        public bool Do(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            if (city == null)
            {
                city = ((User) ac.Principal).city;
                ac.AddParam(nameof(city), city);
            }
            return true;
        }
    }


    [User]
    public class PubShopWork : ShopWork<PubShopVarWork>
    {
        public PubShopWork(WorkContext wc) : base(wc)
        {
        }

        [ResolveCity]
        public void @default(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            using (var dc = ac.NewDbContext())
            {
                ac.GivePage(200, m =>
                {
                    m.T("<div data-sticky-container>");
                    m.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                    m.T("<div class=\"title-bar\">");
                    m.T("<div class=\"title-bar-title\">");
                    m.T("<select name=\"city\" style=\"margin: 0; border: 0; color: #ba55d3; font-size: 1.25rem;\" onchange=\"location = location.href.split('?')[0] + '?city=' + this.value;\">");
                    string[] vs = ((ShopService) Service).CityOpt;
                    for (int i = 0; i < vs.Length; i++)
                    {
                        string v = vs[i];
                        m.T("<option value=\"").T(v).T("\"");
                        if (v == city) m.T(" selected");
                        m.T(">");
                        m.T(v);
                        m.T("</option>");
                    }
                    m.T("</select>");
                    m.T("</div>");
                    m.T("<div class=\"title-bar-right\">");
                    m.T("<a class=\"float-right\" href=\"/my//cart/\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-shopping-cart fa-stack-1x fa-inverse\"></i></span></a>");
                    m.T("</div>");
                    m.T("</div>");
                    m.T("</div>");
                    m.T("</div>");

                    const int proj = Shop.ID | Shop.BASIC;
                    dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE city = @1 AND status > 0");
                    if (dc.Query(p => p.Set(city)))
                    {
                        var shops = dc.ToArray<Shop>(proj);
                        for (int i = 0; i < shops.Length; i++)
                        {
                            var shop = shops[i];

                            m.T("<div class=\"row card align-middle\">");
                            m.T("<div class=\"small-8 columns\">");
                            m.T("<h3><a href=\"").T(shop.id).T("/\">").T(shop.name).T("</a></h3>");
                            m.T("<p>").T(shop.city).T(shop.addr).T("</p>");
                            m.T("<p>").T(shop.descr).T("</p>");
                            m.T("</div>");
                            m.T("<div class=\"small-4 columns\"><a href=\"").T(shop.id).T("/\"><span></span><img src=\"").T(shop.id).T("/icon\" alt=\"\" class=\"thumbnail circle\"></a></div>");
                            m.T("</div>");
                        }
                    }
                    else
                    {
                        m.T("<div style=\"text-align: center\">");
                        m.T("<p>").T(city).T("目前没有商家</p>");
                        m.T("</div>");
                    }
                }, @public: true, maxage: 60 * 5);
            }
        }
    }


    public class OprShopWork : ShopWork<OprShopVarWork>
    {
        public OprShopWork(WorkContext wc) : base(wc)
        {
            CreateVar<OprShopVarWork, string>((prin) => ((User) prin).oprat);
        }

        public void @null(ActionContext ac)
        {
            ac.GivePage(200, m => { m.CALLOUT("您目前还没有访问权限", false); });
        }
    }

    [Ui("商家管理")]
    public class SprShopWork : ShopWork<SprShopVarWork>
    {
        public SprShopWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string city = ac[typeof(CityVarWork)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = Shop.ID | Shop.BASIC | Shop.SUPER;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE city = @1");
                if (dc.Query(p => p.Set(city)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Shop>(proj), proj, @public: false, maxage: 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Shop[]) null, @public: false, maxage: 3);
                }
            }
        }

        [Ui("新建", Mode = UiMode.AnchorShow)]
        public async Task @new(ActionContext ac)
        {
            string city = ac[typeof(CityVarWork)];
            if (ac.GET)
            {
                var o = new Shop {city = city};
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.id), o.id, "商家编号", max: 6, min: 6, required: true);
                    m.TEXT(nameof(o.name), o.name, "商家名称", max: 10, required: true);
                    m.TEXT(nameof(o.city), o.city, "所在城市", @readonly: true);
                    m.SELECT(nameof(o.distr), o.distr, ((ShopService) Service).GetDistrs(o.city), "区域");
                    m.TEXT(nameof(o.lic), o.lic, "工商登记", max: 20, min: 11, required: true);
                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Shop>();
                o.city = city;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Execute("INSERT INTO shops (id, name, city, distr, lic) VALUES (@1, @2, @3, @4, @5)", p => p.Set(o.id).Set(o.name).Set(city).Set(o.distr).Set(o.lic)) > 0)
                    {
                        ac.GivePane(200); // created
                    }
                    else
                    {
                        ac.GivePane(200); // internal server error
                    }
                }
            }
        }
    }
}