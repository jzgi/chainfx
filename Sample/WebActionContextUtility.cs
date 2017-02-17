using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class WebActionContextUtility
    {
        public static void ReplyRedirect(this WebActionContext ac, string uri, bool? pub = null, int maxage = 60)
        {
            ac.SetHeader("Location", string.IsNullOrEmpty(uri) ? "/" : uri);
            ac.Reply(303);
        }

        public static void ReplyHtml(this WebActionContext ac, int status, Action<HtmlContent> header, Action<HtmlContent> main, Action<HtmlContent> footer, bool? pub = null, int maxage = 60)
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

        public static void ReplySheet(this WebActionContext ac, int status, Action<HtmlContent> inner, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status,
            null,
            main =>
            {
                main.Add("<form>");
                inner(main);
                main.Add("</form>");
            },
            null,
            pub, maxage);
        }

        public static void ReplyForm(this WebActionContext ac, int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status,
            null,
            main =>
            {
                main.Add("<form>");
                main.ctx = HtmlContent.CTX_FORM;
                obj.WriteData(main, proj);
                main.BUTTON(ac.Handle);
                main.Add("</form>");
            },
            null,
            pub, maxage);
        }

        public static void ReplyForm(this WebActionContext ac, int status, Action<HtmlContent> form, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status,
            null,
            main =>
            {
                main.Add("<form>");
                main.ctx = HtmlContent.CTX_FORM;
                form(main);
                main.BUTTON(ac.Handle);
                main.Add("</form>");
            },
            null,
            pub, maxage);
        }

        public static void ReplyFolderPage<D>(this WebActionContext ac, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            WebFolder folder = ac.Folder;

            ac.ReplyHtml(status,
            header =>
            {
                if (folder.Parent != null)
                {
                    header.Add("<a href=\"../\" class=\"button\">上层</a>");
                }
                Roll<WebFolder> subs = folder.subs;
                if (subs != null)
                {
                    header.Add(" <ul class=\"menu\">");
                    for (int i = 0; i < subs.Count; i++)
                    {
                        WebFolder sub = subs[i];
                        header.Add("<li class=\"\"><a href=\"");
                        header.Add(sub.Name);
                        header.Add("/\">");
                        header.Add(sub.Name);
                        header.Add("</a></li>");
                    }
                    header.Add(" </ul>");
                }
            },
            main =>
            {
                List<WebAction> actions = folder.GetUiActions(ac);
                main.GRID(actions, lst);
            },
            null,
            pub, maxage);
        }

        public static void ReplyPane(this WebActionContext ac, int status, IDataInput input, Action<IDataInput, HtmlContent> valve, bool? pub = null, int maxage = 60)
        {
            List<WebAction> actions = ac.Folder.GetUiActions(ac);
            ac.ReplyHtml(status,
            null,
            main =>
            {
                main.GRID(actions, input, valve);
            },
            null,
            pub, maxage);
        }
    }
}