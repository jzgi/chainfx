using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public static class ActionContextUtility
    {
        ///
        public static void GiveRedirect(this ActionContext ac, string uri, bool? pub = null, int maxage = 60)
        {
            ac.SetHeader("Location", string.IsNullOrEmpty(uri) ? "/" : uri);
            ac.Give(303);
        }

        ///
        /// Gives a browser window page.
        ///
        public static void GivePage(this ActionContext ac, int status, Action<HtmlContent> main, bool? pub = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(true, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%\">");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.1/css/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            h.Add("</head>");

            h.Add("<body style=\"height:100%\">");

            main(h);

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.1.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            h.Add("<script src=\"/slim.jquery.min.js\"></script>");
            h.Add("<script src=\"/app.js\"></script>");
            h.Add("<script>$(document).foundation();</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, pub, maxage);
        }

        ///
        /// Gives an iframe pane.
        ///
        public static void GivePane(this ActionContext ac, int status, Action<HtmlContent> main, bool? pub = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(true, true, 16 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.1/css/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/slim.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            h.Add("</head>");

            h.Add("<body>");

            h.Add("<div class\"row\">");
            h.Add("<div class=\"small-centered small-10 medium-8 large-6 columns\">");
            main(h);
            h.Add("</div>");
            h.Add("</div>");

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.1.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            h.Add("<script src=\"/slim.jquery.min.js\"></script>");
            h.Add("<script src=\"/app.js\"></script>");
            h.Add("<script>");
            h.Add("$(document).foundation();");
            h.Add("$('body').slim('parse');");
            // enabling ok button
            h.Add("$(document).ready(function(){");
            h.Add("$('#dynadlg', window.parent.document).find('button').prop('disabled', false);");
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
                m.FORM_();
                m.FIELDSET_();
                form(m);
                m._FIELDSET();
                m._FORM();
            },
            pub, maxage);
        }

        public static void GiveFormPane(this ActionContext ac, int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.GivePane(status, m =>
            {
                m.FILLFORM(ac.Doer, obj, proj);
            },
            pub, maxage);
        }

        public static void GiveFormPane(this ActionContext ac, int status, IDataInput input, Action<IDataInput, HtmlContent> valve, bool? pub = null, int maxage = 60)
        {
            ac.GivePane(status, m =>
            {
                m.FILLFORM(ac.Doer, input, valve);
            },
            pub, maxage);
        }

        public static void GiveFrame(this ActionContext ac, int status, bool? pub = null, int maxage = 60)
        {
            HtmlContent h = new HtmlContent(true, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%\">");

            h.Add("<head>");
            h.Add("<title>粗粮达人</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.1/css/foundation.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            h.Add("</head>");

            h.Add("<body style=\"height:100%\">");

            Work work = ac.Work;
            Roll<Work> subs = work.Subworks;
            h.Add("<ul class=\"tabs\" data-tabs id=\"example-tabs\">");

            h.Add("<li class=\"tabs-title is-active\"><a style=\"padding:0.25rem 0.5rem;\" href=\"#paneltop\">"); h.Add(work.Label); h.Add("</a></li>");

            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    h.Add("<li class=\"tabs-title primary\"><a style=\"padding:0.25rem 0.5rem;\" href=\"#panel"); h.Add(i); h.Add("\">"); h.Add(sub.Label); h.Add("</a></li>");
                }
            }
            h.Add(" </ul>");

            h.Add("<div class=\"tabs-content\" style=\"height: calc(100% - 2em)\" data-tabs-content=\"example-tabs\">");

            h.Add("<div class=\"tabs-panel is-active\" id=\"paneltop\">");
            h.Add(" </div>");

            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    h.Add("<div class=\"tabs-panel\" style=\"height: 100%\" id=\"panel"); h.Add(i); h.Add("\">");
                    h.Add("<iframe id=\""); h.Add(sub.Name); h.Add("/\" style=\"width:100%; height:100%;\"></iframe>");
                    h.Add(" </div>");
                }
            }
            h.Add(" </div>");

            h.Add(" </div>");

            // zurb foundation
            h.Add("<script src=\"//cdn.bootcss.com/jquery/3.1.1/jquery.min.js\"></script>");
            h.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            h.Add("<script src=\"/slim.jquery.min.js\"></script>");
            h.Add("<script src=\"/app.js\"></script>");
            h.Add("<script>");
            h.Add("$(document).foundation();");
            h.Add("$('#example-tabs').on('change.zf.tabs', function(e){var ifr = $('.tabs-panel.is-active').find('iframe'); if (ifr && !ifr[0].src) ifr[0].src = ifr[0].id;});");
            h.Add("</script>");
            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, pub, maxage);
        }

        public static void GiveGridFormPage<D>(this ActionContext ac, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
            ac.GivePage(status, main =>
            {
                main.GRIDFORM(ac, lst, proj);
            },
            pub, maxage);
        }

        public static void GiveGridFormPage<D>(this ActionContext ac, int status, List<D> lst, Action<HtmlContent, D> putobj, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
        }

        public static void GiveTableFormPage<D>(this ActionContext ac, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;

        }

        public static void GiveTableFormPage<D>(this ActionContext ac, int status, List<D> lst, Action<HtmlContent, D> putobj, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;
        }

    }
}