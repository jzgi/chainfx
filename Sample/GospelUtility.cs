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
            h.Add(title ?? ac.Work.Label);
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body class=\"doc\">");

            main(h);

            // zurb foundation
            h.Add("<script>");
            h.Add("$(document).foundation();");
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static void GiveFrame(this WebContext ac, int status, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%;\">");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? "粗粮达人");
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body style=\"height:100%; overflow-y: hidden\">");

            Work work = ac.Work;
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
                    if (!sub.DoAuthorize(ac)) continue;
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
                    if (!sub.DoAuthorize(ac)) continue;
                    h.Add("<li style=\"height: 100%\"><iframe src=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe></li>");
                }
            }
            h.Add(" </ul>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        /// <summary>
        /// Gives a frame page.
        /// </summary>
        public static void GivePage(this WebContext ac, int status, Action<HtmlContent> main, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(ac, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? ac.Work.Label);
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body class=\"page\">");

            main(h);

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
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
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body class=\"pane\">");

            main?.Invoke(h);

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static void GiveBoardDoc<D>(this WebContext ac, int status, D[] arr, Action<HtmlContent, D> cell, bool? @public = null, int maxage = 60, string title = null) where D : IData
        {
            ac.GiveDoc(status,
                main =>
                {
                    main.TOOLBAR();
                    main.BOARDVIEW(arr, cell);
                },
                @public, maxage, title
            );
        }

        public static void GiveBoardPage<D>(this WebContext ac, int status, D[] arr, Action<HtmlContent, D> cell, bool? @public = null, int maxage = 60, string title = null) where D : IData
        {
            ac.GivePage(status,
                main =>
                {
                    main.TOOLBAR();
                    main.BOARDVIEW(arr, cell);
                },
                @public, maxage, title
            );
        }

        public static void GiveSheetPage<D>(this WebContext ac, int status, D[] arr, Action<HtmlContent> head, Action<HtmlContent, D> row, bool? @public = null, int maxage = 60, string title = null) where D : IData
        {
            ac.GivePage(status,
                main =>
                {
                    main.TOOLBAR();
                    main.DATATABLE(arr, head, row);
                },
                @public, maxage, title
            );
        }

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
            h.T("<a class=\"button hollow round float-right\" href=\"/my//order/\">购物车</a>");
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