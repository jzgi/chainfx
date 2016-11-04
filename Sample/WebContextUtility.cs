using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public static class WebContextUtility
    {

        static readonly Dictionary<string, string> Map = new Dictionary<string, string>
        {
            ["abc"] = "好的"
        };

        static void Layout()
        {

        }

        public static void SendMajorLayout(this WebContext wc, int status, string header, Action<HtContent> main, bool? pub = null, int maxage = 60000)
        {
            HtContent cont = new HtContent(8 * 1024)
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
            if (header != null)
            {
                cont.T("<header class=\"\">");
                cont.T("<span>").T(header).T("</span>");
                cont.T("</header>");
            }
            cont.T("<main class=\"pure-g\">");

            main(cont);

            cont.T("</main>");

            cont.T("</body>");
            cont.T("</html>");

            // cont.Render(main);
            wc.Send(status, cont, pub, maxage);
        }

        public static void SendMinorLayout(this WebContext wc, int status, string header, Action<HtContent> main, bool? pub = null, int maxage = 60000)
        {
        }

        public static void SendDialogLayout(this WebContext wc, int status, Action<HtContent> main, bool? pub = null, int maxage = 60000)
        {
            HtContent cont = new HtContent(8 * 1024)
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
            wc.Send(status, cont, pub, maxage);
        }

    }
    
}