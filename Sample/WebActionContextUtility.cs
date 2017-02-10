using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public static class WebActionContextUtility
    {

        public static void ReplyPage(this WebActionContext ac, int status, Action<HtmlContent> inner, bool? pub = null, int maxage = 60)
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

            cont.T("<script src=\"//cdn.bootcss.com/jquery/3.1.1/jquery.min.js\"></script>");
            cont.T("<script src=\"//cdn.bootcss.com/foundation/6.3.0/js/foundation.min.js\"></script>");
            cont.T("</body>");
            cont.T("</html>");

            // cont.Render(main);
            ac.Reply(status, cont, pub, maxage);
        }

        public static void ReplyPane(this WebActionContext ac, int status, string header, Action<HtmlContent> main, bool? pub = null, int maxage = 60000)
        {
        }

        public static void ReplyDlg(this WebActionContext ac, int status, Action<HtmlContent> inner, bool? pub = null, int maxage = 60)
        {
            ac.ReplyPage(status, cont =>
            {
                cont.T("<form>");
                inner(cont);
                cont.T("</form>");
            },
            pub, maxage);
        }

    }

}