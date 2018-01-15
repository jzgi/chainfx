using System;
using Greatbone.Core;

namespace Greatbone.Samp
{
    public static class SampUtility
    {
        public static void GiveRedirect(this ActionContext ac, string uri = null, bool? @public = null, int maxage = 60)
        {
            ac.SetHeader("Location", uri ?? "./");
            ac.Give(303);
        }

        /// <summary>
        /// Gives a frame page.
        /// </summary>
        public static void GiveDoc(this ActionContext ac, int status, Action<HtmlContent> main, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(ac, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? ac.Work.Label);
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
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

        public static void GiveFrame(this ActionContext ac, int status, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%;\">");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? "粗粮达人");
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body style=\"height:100%; overflow-y: hidden\">");

            Work work = ac.Work;
            Map<string, Work> subs = work.Works;
            // tabs
            h.Add("<ul class=\"tabs\" data-tabs id=\"frametabs\">");
            h.Add("<li class=\"tabs-title is-active\">");
            h.Add("<a href=\"#tabs_\">");
            h.Add(work.Label);
            h.Add("</a>");
            h.Add("</li>");
            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.DoAuthorize(ac)) continue;
                    h.Add("<li class=\"tabs-title\"><a href=\"#tabs_");
                    h.Add(sub.Key);
                    h.Add("\">");
                    h.Add(sub.Label);
                    h.Add("</a></li>");
                }
            }
            h.Add("</ul>");
            // tabs content
            h.Add("<div class=\"tabs-content\" data-tabs-content=\"frametabs\">");
            // the first panel
            h.Add("<div class=\"tabs-panel is-active\" style=\"height: 100%\" id=\"tabs_\">");
            h.Add("<iframe src=\"?inner=true\" frameborder=\"0\" style=\"width: 100%; height: 100%;\"></iframe>");
            h.Add("</div>");
            if (subs != null)
            {
                // the sub-level panels
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.DoAuthorize(ac)) continue;
                    h.Add("<div class=\"tabs-panel\" style=\"height: 100%\" id=\"tabs_");
                    h.Add(sub.Key);
                    h.Add("\">");
                    h.Add("<iframe id=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe>");
                    h.Add("</div>");
                }
            }
            h.Add(" </div>");

            // zurb foundation
            h.Add("<script>");
            h.Add("$(document).foundation();\n");
            h.Add("$('#frametabs').on('change.zf.tabs', function(e){\nvar ifr = $('.tabs-panel.is-active').find('iframe'); \nif (ifr && !ifr[0].src) ifr[0].src = ifr[0].id;});");
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        /// <summary>
        /// Gives a frame page.
        /// </summary>
        public static void GivePage(this ActionContext ac, int status, Action<HtmlContent> main, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(ac, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? ac.Work.Label);
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body class=\"page\">");

            main(h);

            // zurb foundation
            h.Add("<script>");
            h.Add("$(document).foundation();");
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        /// <summary>
        /// Gives out adialog pane
        /// </summary>
        public static void GivePane(this ActionContext ac, int status, Action<HtmlContent> main = null, bool? @public = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body class=\"pane\">");

            main?.Invoke(h);

            // zurb foundation
            h.Add("<script>");

            h.Add("$(document).ready(function(){");
            h.Add("$(document).foundation();");
            if (main != null) // enable the ok button
            {
                h.Add("$('#dyndlg', window.parent.document).find('button').prop('disabled', document.forms.length == 0);");
            }
            else // trigger click on the close-button
            {
                h.Add("close(true);");
            }
            h.Add("});");
            h.Add("</script>");
            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static void GiveBoardDoc<D>(this ActionContext ac, int status, D[] arr, Action<HtmlContent, D> cell, bool? @public = null, int maxage = 60, string title = null) where D : IData
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

        public static void GiveBoardPage<D>(this ActionContext ac, int status, D[] arr, Action<HtmlContent, D> cell, bool? @public = null, int maxage = 60, string title = null) where D : IData
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

        public static void GiveSheetPage<D>(this ActionContext ac, int status, D[] arr, Action<HtmlContent> head, Action<HtmlContent, D> row, bool? @public = null, int maxage = 60, string title = null) where D : IData
        {
            ac.GivePage(status,
                main =>
                {
                    main.TOOLBAR();
                    main.SHEETVIEW(arr, head, row);
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