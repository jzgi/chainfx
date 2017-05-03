using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public static class ActionContextUtility
    {
        ///
        public static void GiveRedirect(this ActionContext ac, string uri = null, bool? pub = null, int maxage = 60)
        {
            ac.SetHeader("Location", uri ?? "./");
            ac.Give(303);
        }

        public static void GiveFrame(this ActionContext ac, int status, bool? pub = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(true, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%\">");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            h.Add("</head>");

            h.Add("<body style=\"height:100%\">");

            Work work = ac.Work;
            Roll<Work> subs = work.Subworks;
            h.Add("<ul class=\"tabs\" data-tabs id=\"example-tabs\">");

            h.Add("<li class=\"tabs-title is-active\"><a href=\"#paneltop\">");
            h.Add(work.Label);
            h.Add("</a></li>");

            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    h.Add("<li class=\"tabs-title\"><a href=\"#panel");
                    h.Add(i);
                    h.Add("\">");
                    h.Add(sub.Label);
                    h.Add("</a></li>");
                }
            }
            h.Add("</ul>");

            h.Add("<div class=\"tabs-content\" data-tabs-content=\"example-tabs\">");

            h.Add("<div class=\"tabs-panel is-active\" id=\"paneltop\">");
            h.Add(" </div>");

            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
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

            h.Add(" </div>");

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.2.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            h.Add("<script src=\"/app.js\"></script>");
            h.Add("<script>");
            h.Add("$(document).foundation();");
            h.Add("$('#example-tabs').on('change.zf.tabs', function(e){var ifr = $('.tabs-panel.is-active').find('iframe'); if (ifr && !ifr[0].src) ifr[0].src = ifr[0].id;});");
            h.Add("</script>");
            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, pub, maxage);
        }

        ///
        /// Gives a browser window page.
        ///
        public static void GivePage(this ActionContext ac, int status, Action<HtmlContent> main, bool? pub = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(true, true, 32 * 1024);

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

            ac.Give(status, h, pub, maxage);
        }

        ///
        /// Gives an iframe pane.
        ///
        public static void GivePane(this ActionContext ac, int status, Action<HtmlContent> main, bool? pub = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(true, true, 8 * 1024);

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
            h.Add("$(document).foundation();");
            // enabling ok button
            h.Add("$(document).ready(function(){");
            h.Add("$('#dyndlg', window.parent.document).find('button').prop('disabled', false);");
            if (main == null)
            {
                h.Add("$('#dyndlg').find('.close-button').trigger('click')");
            }
            h.Add("});");
            h.Add("</script>");
            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, pub, maxage);
        }

        public static void GiveFormPage(this ActionContext ac, int status, string action, string legend, Action<HtmlContent> form, bool? pub = null, int maxage = 60)
        {
            ac.GivePage(status, m =>
            {
                m.Add("<div class\"row\">");
                m.Add("<div class=\"small-centered small-10 medium-8 large-6 columns\">");
                m.FORM_(action);
                m.FIELDSET_(legend);
                form(m);
                m.BUTTON("确定");
                m._FIELDSET();
                m._FORM();
                m.Add("</div>");
                m.Add("</div>");
            }, pub, maxage);
        }

        public static void GiveFormPane(this ActionContext ac, int status, Action<HtmlContent> form, bool? pub = null, int maxage = 60)
        {
            ac.GivePane(status, m =>
                {
                    m.FORM_(mp: true);
                    m.FIELDSET_();
                    form(m);
                    m._FIELDSET();
                    m._FORM();
                },
                pub, maxage);
        }

        public static void GiveFormPane(this ActionContext ac, int status, IData obj, short proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.GivePane(status,
                m => { m.FILLER(ac.Doer, obj, proj); },
                pub, maxage
            );
        }

        public static void GiveFormPane(this ActionContext ac, int status, IDataInput input, Action<IDataInput, HtmlContent> valve, bool? pub = null, int maxage = 60)
        {
            ac.GivePane(status, m => { m.FILLER(ac.Doer, input, valve); },
                pub, maxage);
        }

        public static void GiveGridFormPage<D>(this ActionContext ac, int status, D[] lst, short proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
            ac.GivePage(status, main => { main.GRID(ac, ac.Work, 2, lst, proj); }, pub, maxage);
        }

        public static void GiveGridFormPage<D>(this ActionContext ac, int status, D[] lst, Action<HtmlContent, D> putobj, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
        }

        public static void GiveTableFormPage<D>(this ActionContext ac, int status, D[] lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
        }

        public static void GiveTableFormPage<D>(this ActionContext ac, int status, D[] lst, Action<HtmlContent, D> putobj, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
        }
    }
}