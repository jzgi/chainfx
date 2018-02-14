using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A before filter that ensures city is resolved and given in the URL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CityAttribute : Attribute, IBefore
    {
        public bool Do(WebContext ac)
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
}