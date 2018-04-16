using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A before filter that ensures city is resolved and given in the URL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CityIdAttribute : Attribute, IBefore
    {
        public bool Do(WebContext wc)
        {
            string cityid = wc.Query[nameof(cityid)];
            if (cityid == null) // no cityid given then return a geolocator page 
            {
                string json = DataUtility.ToString(City.All);
                // give out a geolocation page
                //
                HtmlContent h = new HtmlContent(wc, true);
                h.T("<html><head><script>");
                h.T("var cities = ").T(json).T(";");
                h.T("var cityid = cities[0].id;");
                h.T("navigator.geolocation.getCurrentPosition(function(p) {");
                h.T("var x=p.coords.longitude; var y=p.coords.latitude;");
                h.T("for (var i = 0; i < cities.length; i++) {");
                h.T("var c = cities[i];");
                h.T("if (c.x1 < x && x < c.x2 && c.y1 < y && y < c.y2) {");
                h.T("cityid=c.id;");
                h.T("break;");
                h.T("}");
                h.T("}");
                h.T("window.location.href = '/list?cityid=' + cityid;");
                h.T("},");
                h.T("function(e) {");
                h.T("window.location.href = '/list?cityid=' + cityid;");
                h.T("}, {enableHighAccuracy: true,timeout: 5000,maximumAge: 3600000}");
                h.T(")");
                h.T("</script></head></html>");
                wc.Give(200, h);
                return false;
            }
            return true;
        }
    }
}