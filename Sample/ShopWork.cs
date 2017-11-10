using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiStyle;

namespace Greatbone.Sample
{
    public abstract class ShopWork<V> : Work where V : ShopVarWork
    {
        protected ShopWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, string>(obj => ((Shop) obj).id);
        }
    }

    /// <summary>
    /// A before filter that ensures city & area are resolved and given in the URL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CityAreaAttribute : Attribute, IBefore
    {
        public bool Do(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            if (city == null) // absent
            {
                User prin = (User) ac.Principal;
                if (prin?.city != null) // use those in principal
                {
                    ac.AddParam(nameof(prin.city), prin.city);
                    ac.AddParam(nameof(prin.area), prin.area);
                    return true;
                }

                // give out a geolocation page
                //
                HtmlContent h = new HtmlContent(ac, true);
                // geolocator page
                h.T("<html><head><script>");
                h.T("var cities = ").JSON(City.All).T(";");
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
                ac.Give(200, h);
                return false;
            }
            return true;
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
        [CityArea]
        public void @default(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            string area = ac.Query[nameof(area)];
            if (city.Length == 0)
            {
                city = "南昌";
            }
            ac.GiveDoc(200, m =>
            {
                m.TOPBAR_("所在城市").SELECT(nameof(city), city, City.All, refresh: true).HIDDEN(nameof(area), area)._TOPBAR();

                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE city = @1 AND status > 0 ORDER BY id");
                    if (dc.Query(p => p.Set(city)))
                    {
                        var arr = dc.ToArray<Shop>();
                        m.GRIDVIEW(arr, (h, o) =>
                        {
                            h.CAPTION_().T(o.name)._CAPTION(Shop.Status[o.status], o.status == 2);
                            h.IMGM(o.id + "/icon", href: o.id + "/", box: 4);
                            h.BOX_(8).P(o.addr, "店址").P(o.schedule, "营业");
                            if (o.areas != null)
                            {
                                h.P(o.areas, "限送");
                            }
                            h._BOX();
                            h.BOX_().T("<p>特色：<a href=\"mark\">").T(o.marks).T("</a></p>")._BOX();
                            h.IMG(o.id + "/荞麦红枣粥/icon", box: 3).IMG(o.id + "/荞麦红枣粥/icon", box: 3).IMG(o.id + "/荞麦红枣粥/icon", box: 3).IMG(o.id + "/荞麦红枣粥/icon", box: 3);
                        });
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
                const short proj = Shop.ID | Shop.INITIAL | Shop.LATE;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj).T(" FROM shops ORDER BY id");
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

        [Ui("新建", Style = ButtonShow)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                var o = new Shop();
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, label: "名称", max: 10, required: true);
                    m.SELECT(nameof(o.city), o.city, City.All, "城市");
                    m.TEXT(nameof(o.addr), o.addr, label: "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度");
                    m.NUMBER(nameof(o.y), o.y, "纬度");
                    m._FORM();
                });
            }
            else // post
            {
                const short proj = Shop.INITIAL;
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