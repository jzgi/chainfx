using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class GospelUtility
    {
        public const string NETADDR = "http://144000.tv";

        // an invisible/unprintable char
        public const char SEPCHAR = '\u200f';

        public static void GiveRedirect(this WebContext ac, string uri = null, bool? @public = null, int maxage = 60)
        {
            ac.SetHeader("Location", uri ?? "./");
            ac.Give(303);
        }

        /// <summary>
        /// Gives a frame page.
        /// </summary>
        public static void GiveDoc(this WebContext ac, int status, Action<HtmlContent> main, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(ac, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? "全粮派");
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body>");

            main(h);

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static void GiveFrame(this WebContext wc, int status, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(wc, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%;\">");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? "全粮派");
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body style=\"height:100%; overflow-y: hidden\">");

            Work work = wc.Work;
            Map<string, Work> subs = work.Works;
            // tabs
            h.Add("<ul class=\"uk-tab uk-margin-remove\" uk-tab>");
            h.Add("<li class=\"uk-active\"><a href=\"#\">");
            h.Add(work.Label);
            h.Add("</a></li>");
            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.DoAuthorize(wc, true)) continue;
                    h.Add("<li><a href=\"#\">");
                    h.Add(sub.Label);
                    h.Add("</a></li>");
                }
            }
            h.Add("</ul>");
            // tabs content
            h.Add("<ul class=\"uk-switcher\" style=\"height: calc(100% - 2.5rem); height: -webkit-calc(100% - 2.5rem);\">");
            // the first panel
            h.Add("<li class=\"uk-active\" style=\"height: 100%\">");
            h.Add("<iframe src=\"?inner=true\" frameborder=\"0\" style=\"width: 100%; height: 100%;\"></iframe>");
            h.Add("</li>");
            if (subs != null)
            {
                // the sub-level panels
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.DoAuthorize(wc, true)) continue;
                    h.Add("<li style=\"height: 100%\"><iframe src=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe></li>");
                }
            }
            h.Add(" </ul>");

            h.Add("</body>");
            h.Add("</html>");

            wc.Give(status, h, @public, maxage);
        }

        public static void GiveOffCanvas(this WebContext wc, int status, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(wc, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%;\">");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? "全粮派");
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body style=\"height:100%; overflow-y: hidden\">");

            h.Add("<div class=\"uk-offcanvas-content uk-height-1-1\">");
            h.Add("<a class=\"uk-icon-link\" uk-icon=\"icon: menu; ratio: 1.5\" uk-toggle=\"target: #offcanvas-push\" style=\"position: absolute; top: 0.5rem\"></a>");
            h.Add("<div id=\"offcanvas-push\" uk-offcanvas=\"mode: push; overlay: true\">");
            h.Add("<div class=\"uk-offcanvas-bar\">");
            h.Add("<button class=\"uk-offcanvas-close\" type=\"button\" uk-close></button>");

            Work work = wc.Work;
            Map<string, Work> subs = work.Works;

            // tabs
            h.Add("<ul class=\"uk-tab uk-tab-right\" uk-tab=\"connect: #iswitcher; media: 270\">");
            h.Add("<li class=\"uk-active\"><a href=\"#\">");
            h.Add(work.Label);
            h.Add("</a></li>");
            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.DoAuthorize(wc, true)) continue;
                    h.Add("<li><a href=\"#\">");
                    h.Add(sub.Label);
                    h.Add("</a></li>");
                }
            }
            h.Add("</ul>");

            h.Add("</div>");
            h.Add("</div>");

            // switcher
            h.Add("<ul id=\"iswitcher\" class=\"uk-switcher uk-height-1-1\">");
            // the first panel
            h.Add("<li class=\"uk-active\" style=\"height: 100%\">");
            h.Add("<iframe src=\"?inner=true\" frameborder=\"0\" style=\"width: 100%; height: 100%;\"></iframe>");
            h.Add("</li>");
            if (subs != null)
            {
                // the sub-level panels
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.DoAuthorize(wc, true)) continue;
                    h.Add("<li style=\"height: 100%\"><iframe src=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe></li>");
                }
            }
            h.Add(" </ul>");

            h.Add("</div>");

            h.Add("</body>");
            h.Add("</html>");

            wc.Give(status, h, @public, maxage);
        }

        /// <summary>
        /// Gives a frame page.
        /// </summary>
        public static void GivePage(this WebContext wc, int status, Action<HtmlContent> main, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(wc, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? "全粮派");
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body>");

            main(h);

            h.Add("</body>");
            h.Add("</html>");

            wc.Give(status, h, @public, maxage);
        }

        /// <summary>
        /// Gives out adialog pane
        /// </summary>
        public static void GivePane(this WebContext ac, int status, Action<HtmlContent> main = null, bool? @public = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>全粮派</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body class=\"pane\">");

            main?.Invoke(h);

            h.Add("<script>");
            if (main != null) // enable the ok button
            {
                h.Add("window.parent.document.getElementById('okbtn').disabled = (document.forms.length == 0);");
            }
            else // trigger click on the close-button
            {
                h.Add("closeUp(true);");
            }
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static HtmlContent TOPBAR_(this HtmlContent h, string title = null)
        {
            h.T("<form class=\"top-bar uk-flex-between\">");
            if (title != null)
            {
                h.T("<div>").T(title).T("</div>");
            }
            return h;
        }

        public static HtmlContent _TOPBAR(this HtmlContent h)
        {
            h.T("<a class=\"uk-button uk-button-default uk-border-rounded\" href=\"/my//order/\">我的订单</a>");
            h.T("</form>");
            h.T("<div class=\"top-bar-placeholder\"></div>");
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