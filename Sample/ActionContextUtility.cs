using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public static class ActionContextUtility
    {
        ///
        public static void GiveRedirect(this ActionContext ac, string uri = null, bool? @public = null, int maxage = 60)
        {
            ac.SetHeader("Location", uri ?? "./");
            ac.Give(303);
        }

        public static void GiveFrame(this ActionContext ac, int status, bool? @public = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%\">");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/typicons/2.0.9/typicons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("</head>");

            h.Add("<body style=\"height:100%\">");

            Work work = ac.Work;
            Roll<Work> subs = work.Works;
            h.Add("<ul class=\"tabs\" data-tabs id=\"frame-tabs\">");

            h.Add("<li class=\"tabs-title is-active\">");
            h.Add("<a href=\"#paneltop\">");
            h.Add(work.Label);
            h.Add("</a>");
            h.Add("</li>");

            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];

                    if (!sub.DoAuthorize(ac)) continue;

                    h.Add("<li class=\"tabs-title\"><a href=\"#panel");
                    h.Add(i);
                    h.Add("\">");
                    h.Add(sub.Label);
                    h.Add("</a></li>");
                }
            }
            h.Add("</ul>");

            h.Add("<div class=\"tabs-content\" data-tabs-content=\"frame-tabs\">");

            h.Add("<div class=\"tabs-panel is-active\" id=\"paneltop\">");
            h.Add("<div class=\"top-bar\">");
            h.Add("<div class=\"top-bar-left\">");
            h.TRIGGERS(work.UiActions);
            h.Add("</div>");
            h.Add("<div class=\"top-bar-right\">");
            h.Add("<span class=\"button primary hollow\">");
            string title = ac[work];
            if (title.Length > 20) title = ((User) ac.Principal).name;
            h.Add(title);
            h.Add("</span>");
            h.Add("</div>");
            h.Add("</div>");
            h.Add("</div>");

            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];

                    if (!sub.DoAuthorize(ac)) continue;

                    h.Add("<div class=\"tabs-panel\" style=\"height: 100%\" id=\"panel");
                    h.Add(i);
                    h.Add("\">");
                    h.Add("<iframe id=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe>");
                    h.Add(" </div>");
                }
            }
            h.Add(" </div>");

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("<script>");
            h.Add("$(document).foundation();");
            h.Add("$('#frame-tabs').on('change.zf.tabs', function(e){var ifr = $('.tabs-panel.is-active').find('iframe'); if (ifr && !ifr[0].src) ifr[0].src = ifr[0].id;});");
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        ///
        /// Gives a browser window page.
        ///
        public static void GivePage(this ActionContext ac, int status, Action<HtmlContent> main, bool? @public = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(ac, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/typicons/2.0.9/typicons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("</head>");

            h.Add("<body>");

            main(h);

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/zepto/1.2.0/zepto.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("<script>");
            h.Add("$(document).foundation();");
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        ///
        /// Gives an iframe pane.
        ///
        public static void GivePane(this ActionContext ac, int status, Action<HtmlContent> main = null, bool? @public = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/typicons/2.0.9/typicons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("</head>");

            h.Add("<body>");

            main?.Invoke(h);

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//foundation.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("<script>");

            h.Add("$(document).ready(function(){");
            h.Add("$(document).foundation();");
            if (main != null) // enable the ok button
            {
                h.Add("$('#dyndlg', window.parent.document).find('button').prop('disabled', false);");
            }
            else // trigger click on the close-button
            {
                h.Add("var win = window.parent;");
                h.Add("var dlg = $('#dyndlg', win.document);");
                h.Add("dlg.find('.close-button').trigger('click');");
                h.Add("if (dlg.hasClass('button-trig')) win.location.reload(false);");
            }
            h.Add("});");
            h.Add("</script>");
            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static void GiveGridPage<D>(this ActionContext ac, int status, D[] arr, Action<HtmlContent, D> cell, bool? @public = null, int maxage = 60) where D : IData
        {
            ac.GivePage(status, main => { main.GRID(arr, cell); }, @public, maxage);
        }

        public static void GiveTablePage<D>(this ActionContext ac, int status, D[] arr, Action<HtmlContent> hd, Action<HtmlContent, D> row, bool? @public = null, int maxage = 60) where D : IData
        {
            ac.GivePage(status, main => { main.TABLE(arr, hd, row); }, @public, maxage);
        }
    }
}