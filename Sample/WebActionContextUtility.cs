using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public static class WebActionContextUtility
    {

        static readonly Dictionary<string, string> Map = new Dictionary<string, string>
        {
            ["abc"] = "好的"
        };

        static void Layout()
        {

        }

        public static void ReplyPage(this WebActionContext ac, int status, string title, Action<HtmlContent> main, bool? pub = null, int maxage = 60)
        {
            HtmlContent cont = new HtmlContent(true, true, 8 * 1024)
            {
                Map = Map
            };

            cont.T("<!doctype html>");
            cont.T("<html>");

            cont.T("<head>");
            cont.T("<title>").T(title).T("</title>");
            cont.T("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            cont.T("<link href=\"//cdn.bootcss.com/weui/1.1.0/style/weui.min.css\" rel=\"stylesheet\">");
            cont.T("<link href=\"//cdn.bootcss.com/font-awesome/4.7.0/css/font-awesome.min.css\" rel=\"stylesheet\">");
            cont.T("<link href=\"/app.css\" rel=\"stylesheet\">");
            cont.T("<script src=\"/app.js\"></script>");
            cont.T("</head>");

            cont.T("<body>");
            cont.T("<main class=\"pure-g\">");

            main(cont);

            cont.T("</main>");

            cont.T("</body>");
            cont.T("</html>");

            // cont.Render(main);
            ac.Reply(status, cont, pub, maxage);
        }

        public static void ReplyPane(this WebActionContext wc, int status, string header, Action<HtmlContent> main, bool? pub = null, int maxage = 60000)
        {
        }

        public static void ReplyDialog(this WebActionContext wc, int status, Action<HtmlContent> main, bool? pub = null, int maxage = 60000)
        {
            HtmlContent cont = new HtmlContent(true, true, 8 * 1024)
            {
                Map = Map
            };

            cont.T("<!doctype html>");
            cont.T("<html>");

            cont.T("<head>");
            cont.T("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            // cont.T("<link href=\"//cdn.bootcss.com/normalize/5.0.0/normalize.min.css\" rel=\"stylesheet\">");
            // cont.T("<link href=\"//cdn.bootcss.com/pure/0.6.0/pure-min.css\" rel=\"stylesheet\">");
            cont.T("<link href=\"//cdn.bootcss.com/font-awesome/4.7.0/css/font-awesome.min.css\" rel=\"stylesheet\">");
            cont.T("<link href=\"/app.css\" rel=\"stylesheet\">");
            cont.T("<script src=\"/app.js\"></script>");
            cont.T("</head>");

            cont.T("<body>");
            cont.T("<main class=\"pure-g\">");

            main(cont);

            cont.T("</main>");

            cont.T("</body>");
            cont.T("</html>");

            // cont.Render(main);
            wc.Reply(status, cont, pub, maxage);
        }

    }

}