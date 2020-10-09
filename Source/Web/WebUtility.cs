using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Skyiah.Web
{
    /// <summary>
    /// A global byte buffer pool.
    /// </summary>
    public static class WebUtility
    {
        public static void GiveRedirect(this WebContext wc, string uri = null, bool? @public = null, int maxage = 60)
        {
            string a;
            if (uri != null)
            {
                a = Uri.EscapeUriString(uri);
            }
            else
            {
                a = "./";
            }

            wc.SetHeader("Location", a);
            wc.Give(303);
        }

        public static void GiveFrame(this WebContext wc, int status, bool? shared = null, short maxage = 60, string title = null, byte group = 0)
        {
            var h = new HtmlContent(8 * 1024)
            {
                Web = wc
            };

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

            var wrk = wc.Work;
            var subwrks = wrk.Works;
            // render tabs
            h.Add("<ul class=\"uk-tab uk-margin-remove\" uk-tab>");
            h.Add("<li class=\"uk-active\"><a href=\"#\">");
            h.Add(wrk.Label);
            h.Add("</a></li>");
            for (int i = 0; i < subwrks?.Count; i++)
            {
                var sub = subwrks.ValueAt(i);
                if (sub.Group != 0 && (sub.Group & group) != sub.Group)
                {
                    continue;
                }

                if (sub.Ui == null || !sub.DoAuthorize(wc))
                {
                    continue;
                }

                h.Add("<li><a href=\"#\">");
                h.Add(sub.Label);
                h.Add("</a></li>");
            }

            h.Add("</ul>");
            // tabs content
            h.Add("<ul class=\"uk-switcher\" style=\"height: calc(100% - 2.25rem); height: -webkit-calc(100% - 2.25rem);\">");
            // the first panel
            h.Add("<li class=\"uk-active\" style=\"height: 100%\">");
            h.Add("<iframe src=\"?inner=true\" frameborder=\"0\" style=\"width: 100%; height: 100%;\"></iframe>");
            h.Add("</li>");
            // the sub-level panels
            for (int i = 0; i < subwrks?.Count; i++)
            {
                var sub = subwrks.ValueAt(i);
                if (sub.Group != 0 && (sub.Group & group) != sub.Group)
                {
                    continue;
                }

                if (sub.Ui == null || !sub.DoAuthorize(wc))
                {
                    continue;
                }

                h.Add("<li style=\"height: 100%\"><iframe id=\"");
                h.Add(sub.Key);
                h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe></li>");
            }

            h.Add(" </ul>");

            // lazy set src for iframes
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

            wc.Give(status, h, shared, maxage);
        }

        public static void GiveOffCanvas(this WebContext wc, short status, bool? shared = null, short maxage = 60, string title = null)
        {
            var h = new HtmlContent(8 * 1024)
            {
                Web = wc
            };

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
            h.Add(
                "<a class=\"uk-icon-link uk-offcanvas-toggle\" uk-icon=\"icon: chevron-right; ratio: 1.5\" uk-toggle=\"target: #offcanvas-push\"></a>");
            h.Add("<div id=\"offcanvas-push\" uk-offcanvas=\"mode: push; overlay: true\">");
            h.Add("<div class=\"uk-offcanvas-bar\">");
            h.Add("<button class=\"uk-offcanvas-close\" type=\"button\" uk-close></button>");

            var wrk = wc.Work;
            var subs = wrk.Works;

            // tabs
            h.Add("<ul class=\"uk-tab uk-tab-right\" uk-tab=\"connect: #iswitcher; media: 270\">");
            h.Add("<li class=\"uk-active\"><a href=\"#\">");
            h.Add(wrk.Label);
            h.Add("</a></li>");
            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    var sub = subs.ValueAt(i);
                    if (!sub.DoAuthorize(wc)) continue;
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
                    var sub = subs.ValueAt(i);
                    if (!sub.DoAuthorize(wc)) continue;
                    h.Add("<li style=\"height: 100%\"><iframe src=\"");
                    h.Add(sub.Key);
                    h.Add("/\" frameborder=\"0\" style=\"width:100%; height:100%;\"></iframe></li>");
                }
            }

            h.Add(" </ul>");

            h.Add("</div>");

            h.Add("</body>");
            h.Add("</html>");

            wc.Give(status, h, shared, maxage);
        }


        /// <summary>
        /// Gives a frame page.
        /// </summary>
        public static void GivePage(this WebContext wc, short status, Action<HtmlContent> main, bool? shared = null, short maxage = 12, string title = null, short refresh = 0)
        {
            var h = new HtmlContent(32 * 1024)
            {
                Web = wc
            };

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

            wc.Give(status, h, shared, maxage);
        }

        /// <summary>
        /// Gives out adialog pane
        /// </summary>
        public static void GivePane(this WebContext wc, short status, Action<HtmlContent> main = null, bool? shared = null, short maxage = 12)
        {
            var h = new HtmlContent(8 * 1024)
            {
                Web = wc
            };

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

            wc.Give(status, h, shared, maxage);
        }


        public static string GetValue(this HttpHeaders headers, string name)
        {
            if (headers.TryGetValues(name, out var values))
            {
                string[] strs = values as string[];
                return strs?[0];
            }

            return null;
        }


        public static async void CallAll(Task<HttpResponseMessage>[] requests, Action<HttpResponseMessage> a)
        {
            HttpResponseMessage[] results = await Task.WhenAll(requests);
            for (int i = 0; i < results.Length; i++)
            {
                a(results[i]);
            }
        }
    }
}