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
        /// Returns a home page pertaining to a related city
        /// </summary>
        public void @default(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            string area = ac.Query[nameof(area)];
            var cities = ((SampleService) Service).Cities;
            if (city == null)
            {
                HtmlContent h = new HtmlContent(ac, true);
                // geolocator page
                h.T("<html><head><script>");
                h.T("var cities = ").JSON(cities).T(";");
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
                return;
            }

            if (city.Length == 0)
            {
                city = "南昌";
            }

            ac.GivePage(200, m =>
            {
                m.T("<div data-sticky-container>");
                m.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                m.T("<div class=\"top-bar\">");
                m.T("<div class=\"top-bar-title\">所在城市</div>");
                m.T("<div class=\"top-bar-left\">");
                m.T("<form>");
                m.SELECT(nameof(city), city, cities, refresh: true);
                m.HIDDEN(nameof(area), area);
                m.T("</form>");
                m.T("</div>");
                m.T("<div class=\"top-bar-right\">");
                m.T("<a class=\"float-right\" href=\"/my//pre/\"><i class=\"typcn typcn-shopping-cart\" style=\"font-size: 1.5rem\"></i></a>");
                m.T("</div>");
                m.T("</div>");
                m.T("</div>");
                m.T("</div>");

                using (var dc = ac.NewDbContext())
                {
                    const int proj = Shop.ID | Shop.INITIAL;
                    dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE city = @1 AND status > 0");
                    if (dc.Query(p => p.Set(city)))
                    {
                        var shops = dc.ToArray<Shop>(proj);
                        for (int i = 0; i < shops.Length; i++)
                        {
                            var shop = shops[i];

                            m.T("<div class=\"grid-x\">");
                            m.T("<div class=\"small-8 cell\">");
                            m.T("<h3><a href=\"").T(shop.id).T("/?city=").T(city).T("\">").T(shop.name).T("</a></h3>");
                            m.T("<p>").T(shop.city).T(shop.addr).T("</p>");
                            var areas = shop.areas;
                            if (areas != null)
                                for (int k = 0; k < areas.Length; k++)
                                {
                                    m.T("<span>").T(areas[k]).T("</span>");
                                }
                            m.T("</div>");
                            m.T("<div class=\"small-4 cell\"><a href=\"").T(shop.id).T("/\"><img src=\"").T(shop.id).T("/icon\" class=\"thumbnail circle\"></a></div>");
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


    [Ui("作坊")]
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
                const int proj = Shop.ID | Shop.INITIAL | Shop.LATE;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops ORDER BY id");
                if (dc.Query())
                {
                    ac.GiveTablePage(200, dc.ToArray<Shop>(proj), h => h.TH("名称").TH("电话").TH("地址"), (h, o) => h.TD(o.name).TD(o.mgrtel).TD(o.addr), false, 3);
                }
                else
                {
                    ac.GiveTablePage(200, (Shop[]) null, null, null, false, 3);
                }
            }
        }

        [Ui("新建", Mode = UiMode.ButtonShow)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                var o = new Shop();
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.SELECT(nameof(o.city), o.city, ((SampleService) Service).Cities, "城市");
                    m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度");
                    m.NUMBER(nameof(o.y), o.y, "纬度");
                    m._FORM();
                });
            }
            else // post
            {
                const int proj = Shop.INITIAL;
                var o = await ac.ReadObjectAsync<Shop>();
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("INSERT INTO shops")._(Shop.Empty, proj)._VALUES_(Shop.Empty, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.GivePane(200); // created
            }
        }
    }
}