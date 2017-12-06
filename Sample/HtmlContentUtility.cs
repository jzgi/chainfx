using Greatbone.Core;

namespace Greatbone.Samp
{
    public static class HtmlContentUtility
    {
        public static HtmlContent TOPBAR_(this HtmlContent h, string title = null)
        {
            h.T("<header data-sticky-container>");
            h.T("<form class=\"sticky top-bar\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
            if (title != null)
            {
                h.T("<div class=\"top-bar-title\">").T(title).T("</div>");
            }
            h.T("<div class=\"top-bar-left\">");
            return h;
        }

        public static HtmlContent _TOPBAR(this HtmlContent h)
        {
            h.T("</div>"); // top-bar-left
            h.T("<div class=\"top-bar-right\">");
            h.T("<a class=\"float-right\" href=\"/my//cart/\"><div style=\"padding: 2px; border: 2px solid darkblue; border-radius: 50%\"><img src=\"/cart.svg\" style=\"width: 1.5rem\"></div></a>");
            h.T("</div>");
            h.T("</form>");
            h.T("</header>");
            return h;
        }

        public static HtmlContent A_POI(this HtmlContent h, double x, double y, string title, string addr, string tel = null)
        {
            h.T("<a href=\"http://apis.map.qq.com/uri/v1/marker?marker=coord:").T(y).T(',').T(x).T(";title:").T(title).T(";addr:").T(addr);
            if (tel != null)
            {
                h.T(";tel:").T(tel);
            }
            h.T("&referer=粗狼达人\">地图</a>");
            return h;
        }
    }
}