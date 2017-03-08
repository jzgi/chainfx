using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class ActionContextUtility
    {
        public static void GiveRedirect(this ActionContext ac, string uri, bool? pub = null, int maxage = 60)
        {
            ac.SetHeader("Location", string.IsNullOrEmpty(uri) ? "/" : uri);
            ac.Give(303);
        }

        public static void GiveHtml(this ActionContext ac, int status, Action<HtmlContent> header, Action<HtmlContent> main, Action<HtmlContent> footer, bool? pub = null, int maxage = 60)
        {
            HtmlContent cont = new HtmlContent(true, true, 16 * 1024);

            cont.Add("<!DOCTYPE html>");
            cont.Add("<html>");

            cont.Add("<head>");
            cont.Add("<title>粗粮达人</title>");
            cont.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            cont.Add("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.1/css/foundation.min.css\">");
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
            cont.Add("<script src=\"//cdn.bootcss.com/foundation/6.3.1/js/foundation.min.js\"></script>");
            cont.Add("<script src=\"/app.js\"></script>");
            cont.Add("<script>$(document).foundation();</script>");

            cont.Add("</body>");
            cont.Add("</html>");

            // cont.Render(main);
            ac.Give(status, cont, pub, maxage);
        }

        public static void GiveSheet(this ActionContext ac, int status, Action<HtmlContent> inner, bool? pub = null, int maxage = 60)
        {
            ac.GiveHtml(status,
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

        public static void GiveForm(this ActionContext ac, int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.GiveHtml(status,
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

        public static void GiveForm(this ActionContext ac, int status, Action<HtmlContent> form, bool? pub = null, int maxage = 60)
        {
            ac.GiveHtml(status,
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

        public static void GiveStartPage(this ActionContext ac, int status, bool? pub = null, int maxage = 60)
        {
            Folder folder = ac.Folder;

            ac.GiveHtml(status,
(Action<HtmlContent>)(            h =>
            {
                Roll<Folder> subs = folder.subfolders;
                if (subs != null)
                {
                    h.Add(" <ul class=\"menu\">");
                    for (int i = 0; i < subs.Count; i++)
                    {
                        Folder fdr = subs[i];
                        if (!fdr.HasUi) continue;

                        AuthorizeAttribute auth = fdr.Authorize;
                        if (auth != null && auth.Check(ac))
                        {
                            h.Add("<li class=\"\"><a href=\"");
                            h.Add(fdr.Name);
                            h.Add("/\">");
                            h.Add(fdr.Label);
                            h.Add("</a></li>");
                        }
                        string key = fdr.GetVarKey(ac.Token);
                        if (key != null)
                        {
                            h.Add("<li class=\"\"><a href=\"");
                            h.Add(fdr.Name);
                            h.Add('/');
                            h.Add(key);
                            h.Add("/\">");
                            h.Add(fdr.VarFolder.Label);
                            h.Add("</a></li>");
                        }
                    }
                    h.Add(" </ul>");
                }
            }),
            m =>
            {

            },
            null,
            pub, maxage);
        }

        public static void GiveFolderPage<D>(this ActionContext ac, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Folder folder = ac.Folder;

            ac.GiveHtml(status,
            h =>
            {
                if (folder.Parent != null)
                {
                    h.Add("<a href=\"../\" class=\"button\">上层</a>");
                }
                Roll<Folder> subs = folder.subfolders;
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

        public static void GivePane(this ActionContext ac, int status, IDataInput input, Action<IDataInput, HtmlContent> valve, bool? pub = null, int maxage = 60)
        {
            List<ActionInfo> actions = ac.Folder.GetUiActions(ac);
            ac.GiveHtml(status,
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