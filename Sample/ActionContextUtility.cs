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
        public static void GiveHtml(this ActionContext ac, int status, Action<HtmlContent> header, Action<HtmlContent> main, Action<HtmlContent> footer, bool? pub = null, int maxage = 60)
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
            cont.Add("<script src=\"/app.js\"></script>");
            cont.Add("<script>$(document).foundation();$('body').slim('parse');</script>");

            cont.Add("</body>");
            cont.Add("</html>");

            // cont.Render(main);
            ac.Give(status, cont, pub, maxage);
        }

        public static void GiveModal(this ActionContext ac, int status, Action<HtmlContent> header, Action<HtmlContent> main, Action<HtmlContent> footer, bool? pub = null, int maxage = 60)
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
            cont.Add("<script src=\"/app.js\"></script>");
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

        public static void GiveModalForm(this ActionContext ac, int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.GiveModal(status,
            null,
            m =>
            {
                m.FORM_INP(ac.Doer, obj, proj);
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
                // m.ctx = HtmlContent.CTX_INPUT;
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

            ac.GiveHtml(status, (Action<HtmlContent>)(h =>
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
                        string key = fdr.GetVarKey(ac.Principal);
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

        public static void GiveFolderPage<D>(this ActionContext ac, Folder @base, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            Folder folder = ac.Folder;
            bool top = folder == @base;

            ac.GiveHtml(status,
            h =>
            {

                Roll<Folder> subs = @base.subfolders;
                if (subs != null)
                {
                    h.Add("<ul class=\"menu\">");

                    h.Add("<li><a href=\"\">");
                    h.Add("<span class=\"fi-folder\" style=\"font-size: 70px\">");
                    h.Add(@base.Label);
                    h.Add("</span></a></li>");
                    for (int i = 0; i < subs.Count; i++)
                    {
                        Folder sub = subs[i];
                        h.Add("<li");
                        if (sub == folder) h.Add(" class=\"active primary\"");
                        h.Add("><a href=\"");
                        if (!top) h.Add("../");
                        h.Add(sub.Name);
                        h.Add("/\">");
                        h.Add(sub.Label);
                        h.Add("</a></li>");
                    }
                    h.Add(" </ul>");
                }
            },
            m =>
            {
                List<ActionInfo> actions = folder.GetUiActions(ac);
                m.FORM_GRID(ac, actions, lst);
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
                m.FORM_GRID(actions, input, valve);
            },
            null,
            pub, maxage);
        }
    }
}