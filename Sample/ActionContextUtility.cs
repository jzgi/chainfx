using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class ActionContextUtility
    {
        public static void ReplyRedirect(this ActionContext ac, string uri, bool? pub = null, int maxage = 60)
        {
            ac.SetHeader("Location", string.IsNullOrEmpty(uri) ? "/" : uri);
            ac.Reply(303);
        }

        public static void ReplyHtml(this ActionContext ac, int status, Action<HtmlContent> header, Action<HtmlContent> main, Action<HtmlContent> footer, bool? pub = null, int maxage = 60)
        {
            HtmlContent cont = new HtmlContent(true, true, 16 * 1024);

            cont.Add("<!DOCTYPE html>");
            cont.Add("<html>");

            cont.Add("<head>");
            cont.Add("<title>粗粮达人</title>");
            cont.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            cont.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.0/css/foundation.min.css\">");
            cont.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/font-awesome/4.7.0/css/font-awesome.min.css\">");
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
            cont.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.0/js/foundation.min.js\"></script>");
            cont.Add("<script src=\"/app.js\"></script>");
            cont.Add("<script>$(document).foundation();</script>");

            cont.Add("</body>");
            cont.Add("</html>");

            // cont.Render(main);
            ac.Reply(status, cont, pub, maxage);
        }

        public static void ReplySheet(this ActionContext ac, int status, Action<HtmlContent> inner, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status,
            null,
            m =>
            {
                m.Add("<form>");
                inner(m);
                m.Add("</form>");
            },
            null,
            pub, maxage);
        }

        public static void ReplyForm(this ActionContext ac, int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status,
            null,
            m =>
            {
                m.Add("<form>");
                m.ctx = HtmlContent.CTX_FORM;
                obj.WriteData(m, proj);
                m.BUTTON(ac.Doer);
                m.Add("</form>");
            },
            null,
            pub, maxage);
        }

        public static void ReplyForm(this ActionContext ac, int status, Action<HtmlContent> form, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status,
            null,
            m =>
            {
                m.Add("<form>");
                m.ctx = HtmlContent.CTX_FORM;
                form(m);
                m.BUTTON(ac.Doer);
                m.Add("</form>");
            },
            null,
            pub, maxage);
        }

        public static void ReplyFolderPage<D>(this ActionContext ac, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Folder folder = ac.Folder;

            ac.ReplyHtml(status,
            h =>
            {
                if (folder.Parent != null)
                {
                    h.Add("<a href=\"../\" class=\"button\">上层</a>");
                }
                Roll<Folder> subs = folder.subs;
                if (subs != null)
                {
                    h.Add(" <ul class=\"menu\">");
                    for (int i = 0; i < subs.Count; i++)
                    {
                        Folder sub = subs[i];
                        h.Add("<li class=\"\"><a href=\"");
                        h.Add(sub.Name);
                        h.Add("/\">");
                        h.Add(sub.Name);
                        h.Add("</a></li>");
                    }
                    h.Add(" </ul>");
                }
            },
            m =>
            {
                List<ActionInfo> actions = folder.GetUiActions(ac);
                m.GRID(actions, lst);
            },
            null,
            pub, maxage);
        }

        public static void ReplyPane(this ActionContext ac, int status, IDataInput input, Action<IDataInput, HtmlContent> valve, bool? pub = null, int maxage = 60)
        {
            List<ActionInfo> actions = ac.Folder.GetUiActions(ac);
            ac.ReplyHtml(status,
            null,
            m =>
            {
                m.GRID(actions, input, valve);
            },
            null,
            pub, maxage);
        }
    }
}