using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class HtmlContentUtility
    {
        public static HtmlContent TOPBAR_(this HtmlContent h, string title)
        {
            h.T("<header data-sticky-container>");
            h.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
            h.T("<form>");
            h.T("<div class=\"top-bar\">");
            if (title != null)
            {
                h.T("<div class=\"top-bar-title\">").T(title).T("</div>");
            }
            h.T("<div class=\"top-bar-left\">");
            return h;
        }

        public static HtmlContent _TOPBAR(this HtmlContent h)
        {
            h.T("</div>"); // closing of top-bar-left

            h.T("<div class=\"top-bar-right\">");
            h.T("<a class=\"float-right\" href=\"/my//cart/\"><div style=\"padding: 2px; border: 2px solid darkblue; border-radius: 50%\"><img src=\"/cart.svg\" style=\"width: 1.5rem\"></div></a>");
            h.T("</div>");

            h.T("</div>");
            h.T("</form>");
            h.T("</div>");
            h.T("</header>");
            return h;
        }
    }
}