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
        public static void GivePage(this ActionContext ac, int status, Action<HtmlContent> header, Action<HtmlContent> main, Action<HtmlContent> footer, bool? pub = null, int maxage = 60)
        {
            HtmlContent cont = new HtmlContent(true, true, 16 * 1024);

            cont.Add("<!DOCTYPE html>");
            cont.Add("<html>");

            cont.Add("<head>");
            cont.Add("<title>粗粮达人</title>");
            cont.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            cont.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.1/css/foundation.min.css\">");
            cont.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            cont.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            cont.Add("</head>");

            cont.Add("<body>");

            if (header != null)
            {
                cont.Add("<div class\"row\">");
                header(cont);
                cont.Add("</div>");
            }

            cont.Add("<div class\"row\">");
            main(cont);
            cont.Add("</div>");

            if (footer != null)
            {
                cont.Add("<div class\"row\">");
                footer(cont);
                cont.Add("</div>");
            }

            // zurb foundation
            cont.Add("<script src=\"//cdn.bootcss.com/jquery/3.1.1/jquery.min.js\"></script>");
            cont.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            cont.Add("<script src=\"/slim.jquery.min.js\"></script>");
            cont.Add("<script src=\"/app01.js\"></script>");
            cont.Add("<script>$(document).foundation();</script>");

            cont.Add("</body>");
            cont.Add("</html>");

            // cont.Render(main);
            ac.Give(status, cont, pub, maxage);
        }

        public static void GivePane(this ActionContext ac, int status, Action<HtmlContent> header, Action<HtmlContent> main, Action<HtmlContent> footer, bool? pub = null, int maxage = 60)
        {
            HtmlContent cont = new HtmlContent(true, true, 16 * 1024);

            cont.Add("<!DOCTYPE html>");
            cont.Add("<html>");

            cont.Add("<head>");
            cont.Add("<title>粗粮达人</title>");
            cont.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            cont.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.1/css/foundation.min.css\">");
            cont.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundicons/3.0.0/foundation-icons.min.css\">");
            cont.Add("<link rel=\"stylesheet\" href=\"/slim.min.css\">");
            cont.Add("<link rel=\"stylesheet\" href=\"/app.css\">");
            cont.Add("</head>");

            cont.Add("<body>");

            if (header != null)
            {
                cont.Add("<div class\"row\">");
                cont.Add("<div class\"small-8 text-center column\">");
                header(cont);
                cont.Add("</div>");
                cont.Add("</div>");
            }

            cont.Add("<div class\"row\">");
            main(cont);
            cont.Add("</div>");

            if (footer != null)
            {
                cont.Add("<div class\"row\">");
                footer(cont);
                cont.Add("</div>");
            }

            // zurb foundation
            cont.Add("<script src=\"//cdn.bootcss.com/jquery/3.1.1/jquery.min.js\"></script>");
            cont.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            cont.Add("<script src=\"/slim.jquery.min.js\"></script>");
            cont.Add("<script src=\"/app01.js\"></script>");
            cont.Add("<script>");
            cont.Add("$(document).foundation();");
            cont.Add("$('body').slim('parse');");
            // enabling ok button
            cont.Add("$(document).ready(function(){");
            cont.Add("$('#dynadlg', window.parent.document).find('button').prop('disabled', false);");
            cont.Add("});");
            cont.Add("</script>");
            cont.Add("</body>");
            cont.Add("</html>");

            // cont.Render(main);
            ac.Give(status, cont, pub, maxage);
        }

        public static void GivePaneForm(this ActionContext ac, int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.GivePane(status,
            null,
            m =>
            {
                m.FILLFORM(ac.Doer, obj, proj);
            },
            null,
            pub, maxage);
        }

        public static void GivePaneForm(this ActionContext ac, int status, Action<HtmlContent> form, bool? pub = null, int maxage = 60)
        {
            ac.GivePane(status,
            null,
            m =>
            {
                m.FORM_();
                form(m);
                m._FORM();
            },
            null,
            pub, maxage);
        }

        public static void GivePaneForm(this ActionContext ac, int status, IDataInput input, Action<IDataInput, HtmlContent> valve, bool? pub = null, int maxage = 60)
        {
            ac.GivePage(status,
            null,
            m =>
            {
                m.FILLFORM(ac.Doer, input, valve);
            },
            null,
            pub, maxage);
        }

        public static void GiveWorkPage<D>(this ActionContext ac, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            GiveWorkPage(ac, null, status, lst, proj, pub, maxage);
        }

        public static void GiveWorkPage<D>(this ActionContext ac, Work @base, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Work work = ac.Work;

            Action<HtmlContent> header = @base == null ? (Action<HtmlContent>)null : (h) =>
            {
                bool top = work == @base;
                Roll<Work> subs = @base.children;
                if (subs != null)
                {
                    h.Add("<ul class=\"menu\">");

                    h.Add("<li><a href=\"\">");
                    h.Add("<span class=\"fi-folder\" style=\"font-size: 70px\">");
                    h.Add(@base.Label);
                    h.Add("</span></a></li>");
                    for (int i = 0; i < subs.Count; i++)
                    {
                        Work sub = subs[i];
                        h.Add("<li");
                        if (sub == work) h.Add(" class=\"active primary\"");
                        h.Add("><a href=\"");
                        if (!top) h.Add("../");
                        h.Add(sub.Name);
                        h.Add("/_\">");
                        h.Add(sub.Label);
                        h.Add("</a></li>");
                    }
                    h.Add(" </ul>");
                }
            };


            ac.GivePage(status, header, main =>
            {
                main.GRIDFORM(ac, lst, proj);
            },
            null,
            pub, maxage);
        }
    }
}