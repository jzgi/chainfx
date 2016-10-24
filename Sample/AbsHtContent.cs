using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public class AbsHtContent : MdlHtmlContent
    {

        string header;

        bool footer;

        public AbsHtContent(string header, bool footer) : base(8 * 1024)
        {
            this.header = header;
            this.footer = footer;
        }

        public void Render(Action<AbsHtContent> content)
        {
            T("<!doctype html>");
            T("<html>");
            T("<head>");

            T("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");

            T("<script src=\"//cdn.bootcss.com/material-design-lite/1.2.1/material.min.js\"></script>");
            T("<link href=\"//cdn.bootcss.com/material-design-lite/1.2.1/material.min.css\" rel=\"stylesheet\">");
            T("<link href=\"//cdn.bootcss.com/material-design-icons/3.0.1/iconfont/material-icons.min.css\" rel=\"stylesheet\">");
            T("<script src=\"/app.js\"></script>");
            T("<link href=\"/app.css\" rel=\"stylesheet\">");

            T("</head>");

            T("<body>");


            T("<div class=\"mdl-layout mdl-js-layout\">");

            if (header != null)
            {
                T("<header class=\"mdl-layout__header\">");
                T("<div class=\"mdl-layout-icon\"></div>");
                T("<div class=\"mdl-layout__header-row\">");
                T("<span class=\"mdl-layout__title\">").T(header).T("</span>");
                T("<div class=\"mdl-layout-spacer\"></div>");
                T("</div>");
                T("</header>");
            }
            T("<main class=\"mdl-layout__content\">");
            T("<div>");

            content(this);

            T("</div>");
            T("</main>");
            T("</div>");

            if (footer)
            {
                Footer();
            }

            T("</body>");
            T("</html>");
        }

        public void Header(Action<AbsHtContent> a)
        {

        }

        public void Footer()
        {

        }

    }
}