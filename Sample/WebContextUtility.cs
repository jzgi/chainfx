using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class WebContextUtility
    {

        public static void SendMajorLayout(this WebContext wc, int status, string header, Action<HtContent> main, bool? pub = null, int maxage = 60000)
        {
            HtContent cont = new HtContent(8 * 1024);

            cont.T("<!doctype html>");
            cont.T("<html>");

            cont.T("<head>");
            cont.T("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            cont.T("<script src=\"//cdn.bootcss.com/material-design-lite/1.2.1/material.min.js\"></script>");
            cont.T("<link href=\"//cdn.bootcss.com/material-design-lite/1.2.1/material.min.css\" rel=\"stylesheet\">");
            cont.T("<link href=\"//cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.min.css\" rel=\"stylesheet\">");
            cont.T("<script src=\"/app.js\"></script>");
            cont.T("<link href=\"/app.css\" rel=\"stylesheet\">");
            cont.T("</head>");

            cont.T("<body>");
            cont.T("<div class=\"mdl-layout mdl-js-layout\">");

            if (header != null)
            {
                cont.T("<header class=\"mdl-layout__header\">");
                cont.T("<div class=\"mdl-layout-icon\"></div>");
                cont.T("<div class=\"mdl-layout__header-row\">");
                cont.T("<span class=\"mdl-layout__title\">").T(header).T("</span>");
                cont.T("<div class=\"mdl-layout-spacer\"></div>");
                cont.T("</div>");
                cont.T("</header>");
            }
            cont.T("<main class=\"mdl-layout__content\">");
            cont.T("<div>");

            main(cont);

            cont.T("</div>");
            cont.T("</main>");
            cont.T("</div>");


            cont.T("</body>");
            cont.T("</html>");

            // cont.Render(main);
            wc.Send(status, cont, pub, maxage);
        }

        public static void SendMinorLayout(this WebContext wc, int status, string header, Action<HtContent> main, bool? pub = null, int maxage = 60000)
        {
        }

        public static void SendDialogLayout(this WebContext wc, int status, string header, Action<HtContent> main, bool? pub = null, int maxage = 60000)
        {
        }

    }
}