using System;
using Greatbone;

namespace Samp
{
    public static class SampUtility
    {
        public const string NetAddr = "http://144000.tv";

        public static void GiveRedirect(this WebContext wc, string uri = null, bool? @public = null, int maxage = 60)
        {
            string a;
            if (uri != null)
            {
                FormContent c = new FormContent(false, 1024);
                c.AddNonAscii(uri);
                a = c.ToString();
            }
            else
            {
                a = "./";
            }
            wc.SetHeader("Location", a);
            wc.Give(303);
        }

        public static void GiveFrame(this WebContext wc, int status, bool? @public = null, short maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(wc, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%;\">");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title);
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body style=\"height:100%; overflow-y: hidden\">");

            Work work = wc.Work;
            Map<string, Work> subs = work.Works;
            // tabs
            h.Add("<ul class=\"uk-tab uk-margin-remove\" uk-tab>");
            h.Add("<li class=\"uk-active\"><a href=\"#\">");
            h.Add(work.Label);
            h.Add("</a></li>");
            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.Authorize(wc)) continue;
                    h.Add("<li><a href=\"#\">");
                    h.Add(sub.Label);
                    h.Add("</a></li>");
                }
            }
            h.Add("</ul>");
            // tabs content
            h.Add("<ul class=\"uk-switcher\" style=\"height: calc(100% - 2.5rem); height: -webkit-calc(100% - 2.5rem);\">");
            // the first panel
            h.Add("<li class=\"uk-active\" style=\"height: 100%\">");
            h.Add("<iframe src=\"?inner=true\" frameborder=\"0\" style=\"width: 100%; height: 100%;\"></iframe>");
            h.Add("</li>");
            if (subs != null)
            {
                // the sub-level panels
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.Authorize(wc)) continue;
                    h.Add("<li style=\"height: 100%\"><iframe id=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe></li>");
                }
            }
            h.Add(" </ul>");

            // lazy init src of iframes
            h.Add("<script>");
            h.Add("var lis = document.querySelector('.uk-switcher').children;");
            h.Add("for (var i = 0; i < lis.length; i++) {");
            h.Add("lis[i].addEventListener('show', function(e) {");
            h.Add("if (!this.firstChild.src) this.firstChild.src = this.firstChild.id;");
            h.Add("});");
            h.Add("}");
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            wc.Give(status, h, @public, maxage);
        }

        public static void GiveOffCanvas(this WebContext wc, int status, bool? @public = null, short maxage = 60, string title = null)
        {
            HtmlContent h = new HtmlContent(wc, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html style=\"height:100%;\">");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? wc.Work.Label);
            h.Add("</title>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body style=\"height:100%; overflow-y: hidden\">");

            h.Add("<div class=\"uk-offcanvas-content uk-height-1-1\">");
            h.Add("<a class=\"uk-icon-link uk-offcanvas-toggle\" uk-icon=\"icon: chevron-right; ratio: 1.5\" uk-toggle=\"target: #offcanvas-push\"></a>");
            h.Add("<div id=\"offcanvas-push\" uk-offcanvas=\"mode: push; overlay: true\">");
            h.Add("<div class=\"uk-offcanvas-bar\">");
            h.Add("<button class=\"uk-offcanvas-close\" type=\"button\" uk-close></button>");

            Work work = wc.Work;
            Map<string, Work> subs = work.Works;

            // tabs
            h.Add("<ul class=\"uk-tab uk-tab-right\" uk-tab=\"connect: #iswitcher; media: 270\">");
            h.Add("<li class=\"uk-active\"><a href=\"#\">");
            h.Add(work.Label);
            h.Add("</a></li>");
            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.Authorize(wc)) continue;
                    h.Add("<li><a href=\"#\">");
                    h.Add(sub.Label);
                    h.Add("</a></li>");
                }
            }
            h.Add("</ul>");

            h.Add("</div>");
            h.Add("</div>");

            // switcher
            h.Add("<ul id=\"iswitcher\" class=\"uk-switcher uk-height-1-1\">");
            // the first panel
            h.Add("<li class=\"uk-active\" style=\"height: 100%\">");
            h.Add("<iframe src=\"?inner=true\" frameborder=\"0\" style=\"width: 100%; height: 100%;\"></iframe>");
            h.Add("</li>");
            if (subs != null)
            {
                // the sub-level panels
                for (int i = 0; i < subs.Count; i++)
                {
                    Work sub = subs[i];
                    if (!sub.Authorize(wc)) continue;
                    h.Add("<li style=\"height: 100%\"><iframe src=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe></li>");
                }
            }
            h.Add(" </ul>");

            h.Add("</div>");

            h.Add("</body>");
            h.Add("</html>");

            wc.Give(status, h, @public, maxage);
        }


        /// <summary>
        /// Gives a frame page.
        /// </summary>
        public static void GivePage(this WebContext wc, int status, Action<HtmlContent> main, bool? @public = null, short maxage = 12, string title = null, int refresh = 0)
        {
            HtmlContent h = new HtmlContent(wc, true, 32 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<title>");
            h.Add(title ?? wc.Work.Label);
            h.Add("</title>");
            if (refresh > 0) // auto refresh of the page
            {
                h.Add("<meta http-equiv=\"refresh\" content=\"");
                h.Add(refresh);
                h.Add("\">");
            }
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body>");

            main(h);

            h.Add("</body>");
            h.Add("</html>");

            wc.Give(status, h, @public, maxage);
        }

        /// <summary>
        /// Gives out adialog pane
        /// </summary>
        public static void GivePane(this WebContext ac, int status, Action<HtmlContent> main = null, bool? @public = null, short maxage = 12)
        {
            HtmlContent h = new HtmlContent(ac, true, 8 * 1024);

            h.Add("<!DOCTYPE html>");
            h.Add("<html>");

            h.Add("<head>");
            h.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            h.Add("<link rel=\"stylesheet\" href=\"/uikit.min.css\">");
            h.Add("<link rel=\"stylesheet\" href=\"/app.min.css\">");
            h.Add("<script src=\"/uikit.min.js\"></script>");
            h.Add("<script src=\"/uikit-icons.min.js\"></script>");
            h.Add("<script src=\"/app.min.js\"></script>");
            h.Add("</head>");

            h.Add("<body class=\"uk-pane\">");

            main?.Invoke(h);

            h.Add("<script>");
            if (main != null) // enable the ok button
            {
                h.Add("var btn = window.parent.document.getElementById('okbtn');");
                h.Add("if (btn) btn.disabled = (document.forms.length == 0);");
            }
            else // trigger click on the close-button
            {
                h.Add("closeUp(true);");
            }
            h.Add("</script>");

            h.Add("</body>");
            h.Add("</html>");

            ac.Give(status, h, @public, maxage);
        }

        public static HtmlContent TOPBAR(this HtmlContent h, bool def)
        {
            h.T("<nav class=\"uk-top-bar\">");
            h.T("<ul class=\"uk-subnav\">");
            if (def)
            {
                h.T("<li class=\"uk-active \"><a href=\"#\">下单</a></li>");
                h.T("<li><a href=\"chat/\">社区交流</a></li>");
            }
            else
            {
                h.T("<li><a href=\"../\">下单</a></li>");
                h.T("<li class=\"uk-active \"><a href=\"#\">社区交流</a></li>");
            }
            h.T("</ul>");
            string hubid = h.WebCtx[0];
            h.T("<a class=\"uk-button uk-button-link\" href=\"/").T(hubid).T("/my//order/\">我的订单</a>");
            h.T("</nav>");
            h.T("<div class=\"uk-top-placeholder\"></div>");
            return h;
        }

        public static HtmlContent POI(this HtmlContent h, double x, double y, string title, string addr, string tel = null)
        {
            h.T("<a class=\"\" href=\"http://apis.map.qq.com/uri/v1/marker?marker=coord:").T(y).T(',').T(x).T(";title:").T(title).T(";addr:").T(addr);
            if (tel != null)
            {
                h.T(";tel:").T(tel);
            }
            h.T("&referer=\">").T(addr).T("</a>");
            return h;
        }
    }
}