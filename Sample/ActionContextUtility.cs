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
            HtmlContent h = new HtmlContent(true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%\">");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/croppie/2.4.1/croppie.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            h.Add("</head>");

            h.Add("<body style=\"height:100%\">");

            Work work = ac.Work;
            Roll<Work> subs = work.Subworks;
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
            h.Add("<div class=\"title-bar\">");
            h.Add("<div class=\"title-bar-left\">");
            h.TRIGGERS(work.UiActions, ac);
            h.Add("</div>");
            h.Add("<div class=\"title-bar-title\">");
            h.Add("<span class=\"button primary hollow\">");
            string title = ac[work];
            if (title.Length > 20) title = ((User) ac.Principal).nickname;
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
                    h.Add(sub.Name);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe>");
                    h.Add(" </div>");
                }
            }
            h.Add(" </div>");

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/croppie/2.4.1/croppie.min.js\"></script>");
            h.Add("<script src=\"/app.js\"></script>");
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
            HtmlContent h = new HtmlContent(true, 16 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/croppie/2.4.1/croppie.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/font-awesome/4.7.0/css/font-awesome.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            h.Add("</head>");

            h.Add("<body>");

            main(h);

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/croppie/2.4.1/croppie.min.js\"></script>");
            h.Add("<script src=\"/app.js\"></script>");
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
            HtmlContent h = new HtmlContent(true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/font-awesome/4.7.0/css/font-awesome.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            h.Add("</head>");

            h.Add("<body>");

            main?.Invoke(h);

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.js\"></script>");
            h.Add("<script>");

            h.Add("$(document).ready(function(){");
            h.Add("$(document).foundation();");
            if (main != null) // enable the ok button
            {
                h.Add("$('#dyndlg', window.parent.document).find('button').prop('disabled', false);");
            }
            else // trigger click on the close-button
            {
                h.Add("var par = window.parent;");
                h.Add("$('#dyndlg', par.document).find('.close-button').trigger('click');");
                h.Add("par.location.reload(false);");
            }
            h.Add("});");
            h.Add("</script>");
            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static void GiveGridPage<D>(this ActionContext ac, int status, D[] objs, ushort proj = 0x00ff, bool? @public = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
            ac.GivePage(status, main => { main.GRID(ac, work.varwork, objs, proj); }, @public, maxage);
        }

        public static void GiveGridPage<D>(this ActionContext ac, int status, D[] objs, Action<HtmlContent, D> putobj, bool? @public = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
        }

        public static void GiveTablePage<D>(this ActionContext ac, int status, D[] objs, ushort proj = 0x00ff, bool? @public = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
            ac.GivePage(status, main => { main.TABLE(ac, work.varwork, objs, proj); }, @public, maxage);
        }
    }
}