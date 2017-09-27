using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class ShopWork<V> : Work where V : ShopVarWork
    {
        protected ShopWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, short>(obj => ((Shop) obj).id);
        }
    }

    public class PubShopWork : ShopWork<PubShopVarWork>
    {
        public PubShopWork(WorkContext wc) : base(wc)
        {
        }

        /// <summary>
        /// Returns either the city home page, or the geolocator page.
        /// </summary>
        /// <param name="city">name of the located city, or empty</param>
        /// <param name="area">name of the located area, or empty</param>
        public void @default(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            string area = ac.Query[nameof(area)];
            if (city == null)
            {
                HtmlContent h = new HtmlContent(ac, true);
                h.T("<html><head><script>");

                h.T("var cities = [");
                bool bgn = false;
                foreach (var pair in ((CareService) Service).Cities)
                {
                    var c = pair.Value;
                    if (bgn)
                    {
                        h.T(",");
                    }
                    h.T("{");
                    h.T("name:\"").T(c.name).T("\",");
                    h.T("x1:").T(c.x1).T(",");
                    h.T("y1:").T(c.y1).T(",");
                    h.T("x2:").T(c.x2).T(",");
                    h.T("y2:").T(c.y2).T(",");
                    h.T("\"areas\":[");
                    if (c.areas != null)
                    {
                        for (int i = 0; i < c.areas.Length; i++)
                        {
                            if (i > 0)
                            {
                                h.T(",");
                            }
                            var a = c.areas[i];
                            h.T("{");
                            h.T("name:\"").T(a.name).T("\",");
                            h.T("x1:").T(a.x1).T(",");
                            h.T("y1:").T(a.y1).T(",");
                            h.T("x2:").T(a.x2).T(",");
                            h.T("y2:").T(a.y2);
                            h.T("}");
                        }
                    }
                    h.T("]}");
                    bgn = true;
                }
                h.T("];");

                h.T("navigator.geolocation.getCurrentPosition(function(p) {");
                h.T("var city=''; var area='';");
                h.T("var x=p.coords.longitude; var y=p.coords.latitude;");
                h.T("cities.forEach(function(c) {");
                h.T("if (c.x1 < x && x < c.x2 && c.y1 < y && y < c.y2) {");
                h.T("city=c.name;");
                h.T("c.areas.forEach(function(a) {");
                h.T("if (a.x1 < x && x < a.x2 && a.y1 < y && y < a.y2) {");
                h.T("area=a.name;");
                h.T("}});");
                h.T("}});");
                h.T("window.location.href = '/shop/?city=' + city + '&area=' + area;");
                h.T("},");
                h.T("function(e) {window.location.href = '/shop/?city=&area=';}, ");
                h.T("{enableHighAccuracy: true,timeout: 5000,maximumAge: 0}");
                h.T(")");
                h.T("</script></head></html>");
                ac.Give(200, h, true, 3600);
                return; // give the geolocating page
            }

            ac.GivePage(200, m =>
            {
                m.T("<div data-sticky-container>");
                m.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                m.T("<div class=\"top-bar\">");
                m.T("<div class=\"top-bar-left\">");
                m.T("<select name=\"city\" style=\"margin: 0; border: 0; color: #ba55d3; font-size: 1.25rem;\" onchange=\"location = location.href.split('?')[0] + '?city=' + this.value;\">");
                var vs = ((CareService) Service).Cities;
                foreach (var pair in vs)
                {
                    string v = pair.Value.ToString();
                    m.T("<option value=\"").T(v).T("\"");
                    if (pair.Key == city) m.T(" selected");
                    m.T(">");
                    m.T(v);
                    m.T("</option>");
                }
                m.T("</select>");
                m.T("</div>");
                m.T("<div class=\"top-bar-right\">");
                m.T("<a class=\"float-right\" href=\"/my//cart/\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-shopping-cart fa-stack-1x fa-inverse\"></i></span></a>");
                m.T("</div>");
                m.T("</div>");
                m.T("</div>");
                m.T("</div>");

                using (var dc = ac.NewDbContext())
                {
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
                            m.T("<h3><a href=\"").T(shop.id).T("/?city=").T(city).T("\">").T(shop.name).T("</a></h3>");
                            m.T("<p>").T(shop.city).T(shop.addr).T("</p>");
                            var areas = shop.areas;
                            if (areas != null)
                                for (int k = 0; k < areas.Length; k++)
                                {
                                    m.T("<span>").T(areas[k]).T("</span>");
                                }
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
                }
            }, true, 60 * 5);
        }
    }


    [Ui("商家管理")]
    public class AdmShopWork : ShopWork<AdmShopVarWork>
    {
        public AdmShopWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string city = ac[typeof(Work)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = Shop.ID | Shop.BASIC | Shop.ADMIN;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops ORDER BY id");
                if (dc.Query())
                {
                    ac.GiveSheetPage(200, dc.ToArray<Shop>(proj), proj, @public: false, maxage: 3);
                }
                else
                {
                    ac.GiveSheetPage(200, (Shop[]) null, @public: false, maxage: 3);
                }
            }
        }

        [Ui("新建", Mode = UiMode.AnchorShow)]
        public async Task @new(ActionContext ac)
        {
            string city = ac[typeof(Work)];
            if (ac.GET)
            {
                var o = new Shop {city = city};
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, "商家名称", max: 10, required: true);
                    m.TEXT(nameof(o.city), o.city, "所在城市", opt: ((CareService) Service).Cities, @readonly: true);
                    m.TEXT(nameof(o.addr), o.addr, label: "营业地址");
                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Shop>();
                o.city = city;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Execute("INSERT INTO shops (id, name, city, distr, lic) VALUES (@1, @2, @3, @4, @5)", p => p.Set(o.id).Set(o.name).Set(city).Set(o.addr)) > 0)
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