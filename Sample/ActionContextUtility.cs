using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class ActionContextUtility
    {
        public static void GiveRedirect(this ActionContext ac, string uri = null, bool? @public = null, int maxage = 60)
        {
            ac.SetHeader("Location", uri ?? "./");
            ac.Give(303);
        }

        public static void GiveFrame(this ActionContext ac, int status, bool? @public = null, int maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%;\">");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title);
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("</head>");

            h.Add("<body style=\"height:100%; overflow-y: hidden\">");

            Work work = ac.Work;
            Roll<Work> subs = work.Works;
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
                    h.Add(sub.Name);
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
                    h.Add(sub.Name);
                    h.Add("\">");
                    h.Add("<iframe id=\"");
                    h.Add(sub.Name);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe>");
                    h.Add("</div>");
                }
            }
            h.Add(" </div>");

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("<script>");
            h.Add("$(document).foundation();\n");
            h.Add("$('#frametabs').on('change.zf.tabs', function(e){\nvar ifr = $('.tabs-panel.is-active').find('iframe'); \nif (ifr && !ifr[0].src) ifr[0].src = ifr[0].id;});");
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        ///
        /// Gives a frame page.
        ///
        public static void GivePage(this ActionContext ac, int status, Action<HtmlContent> main, bool? @public = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(ac, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("</head>");

            h.Add("<body class=\"frame-page\">");

            main(h);

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
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
        /// dialog pane
        ///
        public static void GivePane(this ActionContext ac, int status, Action<HtmlContent> main = null, bool? @public = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("</head>");

            h.Add("<body style=\"padding: 0.5rem; background-color: gainsboro\">");

            main?.Invoke(h);

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"/foundation.min.js\"></script>");
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
            ac.GivePage(status, main => { main.GridView(arr, cell); }, @public, maxage);
        }

        public static void GiveTablePage<D>(this ActionContext ac, int status, D[] arr, Action<HtmlContent> hd, Action<HtmlContent, D> row, bool? @public = null, int maxage = 60) where D : IData
        {
            ac.GivePage(status, main => { main.TableView(arr, hd, row); }, @public, maxage);
        }
    }
}