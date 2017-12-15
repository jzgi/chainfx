using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;

namespace Greatbone.Samp
{
    public abstract class ShopWork<V> : Work where V : ShopVarWork
    {
        protected ShopWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Shop) obj).id);
        }
    }

    /// <summary>
    /// A before filter that ensures city is resolved and given in the URL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CityAttribute : Attribute, IBefore
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
                    return true;
                }

                // give out a geolocation page
                //
                HtmlContent h = new HtmlContent(ac, true);
                // geolocator page
                h.T("<html><head><script>");
                //                h.T("var cities = ").JSON(City.All).T(";");
                h.T("var city = cities[0].name;");
                h.T("navigator.geolocation.getCurrentPosition(function(p) {");
                h.T("var x=p.coords.longitude; var y=p.coords.latitude;");
                h.T("for (var i = 0; i < city.length; i++) {");
                h.T("var c = cities[i];");
                h.T("if (c.x1 < x && x < c.x2 && c.y1 < y && y < c.y2) {");
                h.T("city=c.name;");
                h.T("break;");
                h.T("}");
                h.T("}");
                h.T("window.location.href = '/shop/?city=' + city;");
                h.T("},");
                h.T("function(e) {");
                h.T("window.location.href = '/shop/?city=' + city;");
                h.T("}, {enableHighAccuracy: true,timeout: 1000,maximumAge: 0}");
                h.T(")");
                h.T("</script></head></html>");
                ac.Give(200, h);
                return false;
            }
            return true;
        }
    }

    [User] // we are forced to put check here because  weixin auth does't work in iframe
    public class PubShopWork : ShopWork<PubShopVarWork>
    {
        public PubShopWork(WorkConfig cfg) : base(cfg)
        {
        }

        /// <summary>
        /// Returns a home page pertaining to a related city
        /// </summary>
//        [City]
        public void @default(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            if (string.IsNullOrEmpty(city))
            {
                city = "南昌";
            }
            ac.GiveDoc(200, m =>
            {
                m.TOPBAR_().SELECT(nameof(city), city, City.All, refresh: true, box: 0)._TOPBAR();

                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE city = @1 AND status > 0 ORDER BY id");
                    dc.Query(p => p.Set(city));
                    m.BOARDVIEW(dc.ToArray<Shop>(), (h, o) =>
                    {
                        h.CAPTION_().T(o.name)._CAPTION(Shop.Statuses[o.status], o.status == 2);
                        h.ICON(o.id + "/icon", href: o.id + "/", box: 0x14);
                        h.BOX_(0x48);
                        h.P_("地址").T(o.addr).T(" ").A_POI(o.x, o.y, o.name, o.addr)._P();
                        h.P_("派送").T(o.delivery);
                        if (o.areas != null) h.SEP().T("限送").T(o.areas);
                        h._P();
                        h.P(o.schedule, "营业");
                        if (o.off > 0) h.P_("促销").T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元")._P();
                        h._BOX();
                        h.THUMBNAIL(o.id + "/img-1", box: 3).THUMBNAIL(o.id + "/img-2", box: 3).THUMBNAIL(o.id + "/img-3", box: 3).THUMBNAIL(o.id + "/img-4", box: 3);
                        h.TAIL();
                    });
                }
            }, true, 60 * 5);
        }
    }


    [Ui("网点")]
    public class AdmShopWork : ShopWork<AdmShopVarWork>
    {
        public AdmShopWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops ORDER BY id");
                dc.Query();
                ac.GiveBoardPage(200, dc.ToArray<Shop>(), (h, o) =>
                {
                    h.CAPTION_().T(o.name).T(" / ").T(o.id)._CAPTION();
                    h.FIELD_("地址").T(o.city)._T(o.addr)._FIELD();
                    h.FIELD_("坐标").T(o.x)._T(o.y)._FIELD();
                    h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                    h.TAIL();
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(ActionContext ac)
        {
            const short proj = Shop.ADM;
            if (ac.GET)
            {
                var o = new Shop() {city = City.All[0].name};
                o.Read(ac.Query, proj);
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.id), o.id, "编号", max: 4, min: 4, required: true);
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.SELECT(nameof(o.city), o.city, City.All, "城市", refresh: true);
                    m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度", max: 20, box: 6).NUMBER(nameof(o.x), o.x, "纬度", max: 20, box: 6);
                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Shop>(proj);
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