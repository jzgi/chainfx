using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class HtmlUtility
    {
        public static void ReplyHtml(this WebActionContext ac, int status, Action<HtmlContent> inner, bool? pub = null, int maxage = 60)
        {
            HtmlContent cont = new HtmlContent(true, true, 16 * 1024);

            cont.T("<!DOCTYPE html>");
            cont.T("<html>");

            cont.T("<head>");
            cont.T("<title>粗粮达人</title>");
            cont.T("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            cont.T("<link rel=\"stylesheet\" href=\"//cdn.bootcss.com/foundation/6.3.0/css/foundation.min.css\">");
            cont.T("</head>");

            cont.T("<body>");

            inner(cont);

            // zurb foundation
            cont.T("<script src=\"//cdn.bootcss.com/jquery/3.1.1/jquery.min.js\"></script>");
            cont.T("<script src=\"//cdn.bootcss.com/foundation/6.3.0/js/foundation.min.js\"></script>");
            cont.T("<script>$(document).foundation();</script>");

            cont.T("</body>");
            cont.T("</html>");

            // cont.Render(main);
            ac.Reply(status, cont, pub, maxage);
        }

        public static void ReplySheet(this WebActionContext ac, int status, Action<HtmlContent> inner, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status, cont =>
            {
                cont.T("<form>");
                inner(cont);
                cont.T("</form>");
            },
            pub, maxage);
        }

        public static void ReplyForm(this WebActionContext ac, int status, IData obj, int proj = 0, bool? pub = null, int maxage = 60)
        {
            ac.ReplyHtml(status, cont =>
            {
                cont.T("<form>");
                cont.ctx = HtmlContent.CTX_FORM;
                obj.WriteData(cont, proj);
                cont.T("</form>");
            },
            pub, maxage);
        }

        public static void ReplyGrid<D>(this WebActionContext ac, int status, List<D> lst, int proj = 0, bool? pub = null, int maxage = 60) where D : IData
        {
            List<WebAction> actions = ac.Folder.GetUiActions(ac);
            ac.ReplyHtml(status, cont =>
            {
                cont.GRID(actions, lst);
            },
            pub, maxage);
        }

        public static void ReplyPane(this WebActionContext ac, int status, IDataInput input, Action<IDataInput, HtmlContent> valve, bool? pub = null, int maxage = 60)
        {
            List<WebAction> actions = ac.Folder.GetUiActions(ac);
            ac.ReplyHtml(status, cont =>
            {
                cont.GRID(actions, input, valve);
            },
            pub, maxage);
        }
    }
}