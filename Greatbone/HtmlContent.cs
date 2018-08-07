using System;

namespace Greatbone
{
    /// <summary>
    /// For generating dynamic HTML5 content tooled with UiKit.
    /// </summary>
    public class HtmlContent : DynamicContent
    {
        readonly WebContext webCtx;

        // data output context in levels, if any
        object[] stack;
        int level = -1;

        public HtmlContent(WebContext webCtx, bool bin, int capacity = 32 * 1024) : base(bin, capacity)
        {
            this.webCtx = webCtx;
        }

        public override string Type => "text/html; charset=utf-8";

        public WebContext WebCtx => webCtx;

        public void AddEsc(string v)
        {
            if (v == null) return;

            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c == '<')
                {
                    Add("&lt;");
                }
                else if (c == '>')
                {
                    Add("&gt;");
                }
                else if (c == '&')
                {
                    Add("&amp;");
                }
                else if (c == '"')
                {
                    Add("&quot;");
                }
                else
                {
                    Add(c);
                }
            }
        }

        void Width(byte w)
        {
            if (w > 0)
            {
                Add(" uk-width-");
                if (w == 0x0f)
                {
                    Add("expand");
                }
                else
                {
                    int hi = w >> 4;
                    int lo = w & 0x0f;
                    if (hi > 0) // if a percentage value
                    {
                        Add(hi);
                        Add('-');
                    }
                    Add(lo);
                }
            }
        }

        public HtmlContent T(char v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(bool v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(short v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(int v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(long v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(DateTime v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(decimal v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(double v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(string v, bool cond = true)
        {
            if (cond)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(string v, int offset, int len, bool cond = true)
        {
            if (cond)
            {
                Add(v, offset, len);
            }
            return this;
        }

        public HtmlContent T(string[] v, bool cond = true)
        {
            if (cond && v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add("&nbsp;");
                    Add(v[i]);
                }
            }
            return this;
        }


        public HtmlContent BR()
        {
            Add("<br>");
            return this;
        }

        public HtmlContent SP()
        {
            Add("&nbsp;");
            return this;
        }

        public HtmlContent ROW_(byte w = 0x11)
        {
            Add("<div class=\"uk-row");
            if (w > 0)
            {
                int lo = w & 0x0f;
                int hi = w >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            Add("\">");
            return this;
        }

        public HtmlContent _ROW()
        {
            Add("</div>");
            return this;
        }


        public HtmlContent TH(string caption)
        {
            Add("<th>");
            Add(caption);
            Add("</th>");
            return this;
        }

        public HtmlContent TD(bool v)
        {
            Add("<td style=\"text-align: center\">");
            if (v)
            {
                Add("&radic;");
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD(short v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(int v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(long v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(decimal v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(DateTime v)
        {
            Add("<td style=\"text-align: center\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(string v)
        {
            Add("<td>");
            AddEsc(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(string v, string v2)
        {
            Add("<td>");
            AddEsc(v);
            Add("&nbsp;");
            AddEsc(v2);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(string[] v)
        {
            Add("<td>");
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(' ');
                    Add(v[i]);
                }
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TDIF(short v)
        {
            Add("<td style=\"text-align: right\">");
            if (v != 0)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TDIF(int v)
        {
            Add("<td style=\"text-align: right\">");
            if (v != 0)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TDIF(long v)
        {
            Add("<td style=\"text-align: right\">");
            if (v != 0)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TDIF(decimal v)
        {
            Add("<td style=\"text-align: right\">");
            if (v != 0)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TDIF(DateTime v)
        {
            Add("<td style=\"text-align: center\">");
            if (v != default)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD_(string css = null)
        {
            Add("<td");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _TD()
        {
            Add("</td>");
            return this;
        }

        public HtmlContent UL_(string css = null)
        {
            Add("<ul");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _UL()
        {
            Add("</ul>");
            return this;
        }


        public HtmlContent LI_(string label = null)
        {
            Add("<li>");
            LABEL(label);
            return this;
        }

        public HtmlContent _LI()
        {
            Add("</li>");
            return this;
        }

        public HtmlContent LI<V>(string label, V p)
        {
            Add("<li>");
            Add("<label class=\"uk-label\">");
            Add(label);
            Add("</label>");
            Add("<p>");
            AddPrimitive(p);
            Add("</p>");
            Add("</li>");
            return this;
        }

        public HtmlContent LI(string label, decimal p)
        {
            Add("<li>");
            LABEL(label);
            Add("<p>");
            Add('¥');
            Add(p);
            Add("</p>");
            Add("</li>");
            return this;
        }

        public HtmlContent LI(string label, params string[] p)
        {
            Add("<li>");
            Add("<label class=\"uk-label\">");
            Add(label);
            Add("</label>");
            Add("<p>");
            for (int i = 0; i < p.Length; i++)
            {
                if (i > 0) Add("&nbsp;");
                Add(p[i]);
            }
            Add("</p>");
            Add("</li>");
            return this;
        }

        public HtmlContent LABEL(string caption)
        {
            if (caption != null)
            {
                Add("<label class=\"uk-label\">");
                Add(caption);
                Add("</label>");
            }
            return this;
        }

        public HtmlContent STATIC_(string label)
        {
            LI_(label);
            Add("<span class=\"uk-static\">");
            return this;
        }

        public HtmlContent _STATIC()
        {
            Add("</span>");
            _LI();
            return this;
        }

        public HtmlContent STATIC<V>(V v, string label)
        {
            STATIC_(label);
            AddPrimitive(v);
            _STATIC();
            return this;
        }

        public HtmlContent COL_(byte w = 0x11, string css = null)
        {
            Add("<div class=\"uk-col");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            if (w > 0)
            {
                int lo = w & 0x0f;
                int hi = w >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            Add("\">");
            return this;
        }

        public HtmlContent _COL()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent P_(string label = null, byte w = 0x11)
        {
            Add("<p");
            if (w > 0)
            {
                Add(" class=\"");
                Width(w);
                Add("\"");
            }
            Add(">");
            if (label != null)
            {
                Add("<label class=\"uk-label\">");
                Add(label);
                Add("</label>");
            }
            return this;
        }

        public HtmlContent _P()
        {
            Add("</p>");
            return this;
        }

        /// <summary>
        /// A field item
        /// </summary>
        /// <param name="label"></param>
        /// <param name="p"></param>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public HtmlContent FI<V>(string label, V p)
        {
            LABEL(label);
            Add("<p>");
            AddPrimitive(p);
            Add("</p>");
            return this;
        }

        public HtmlContent SPAN(short v, byte w = 0x11)
        {
            Add("<span class=\"uk-text-right");
            Width(w);
            Add("\">");
            Add(v);
            Add("</span>");
            return this;
        }

        public HtmlContent SPAN(int v, byte w = 0x11)
        {
            Add("<span class=\"uk-text-right");
            Width(w);
            Add("\">");
            Add(v);
            Add("</span>");
            return this;
        }

        public HtmlContent SPAN(decimal v, byte w = 0x11)
        {
            Add("<span class=\"uk-text-right");
            Width(w);
            Add("\">");
            Add(v);
            Add("</span>");
            return this;
        }

        public HtmlContent SPAN(DateTime v, byte w = 0x11)
        {
            Add("<span class=\"uk-text-center");
            Width(w);
            Add("\">");
            Add(v);
            Add("</span>");
            return this;
        }

        public HtmlContent SPAN(string v, byte w = 0x11)
        {
            Add("<span class=\"");
            Width(w);
            Add("\">");
            Add(v);
            Add("</span>");
            return this;
        }

        public HtmlContent NUMIF(int v, byte w = 0x11)
        {
            Add("<span class=\"uk-text-right");
            if (w > 0)
            {
                int lo = w & 0x0f;
                int hi = w >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            Add("\">");
            if (v != 0)
            {
                Add(v);
            }
            Add("</span>");
            return this;
        }

        public HtmlContent NUMIF(decimal v, byte w = 0x11)
        {
            Add("<span class=\"uk-text-right");
            if (w > 0)
            {
                int lo = w & 0x0f;
                int hi = w >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            Add("\">");
            if (v != 0)
            {
                Add(v);
            }
            Add("</span>");
            return this;
        }

        public HtmlContent CUR(decimal v, byte w = 0)
        {
            Add("<span class=\"uk-text-right");
            Width(w);
            Add("\">¥");
            Add(v);
            Add("</span>");
            return this;
        }

        public HtmlContent CELL<V>(V v, byte w = 0x11)
        {
            Add("<span");
            Width(w);
            Add(">");
            if (v is short shortv)
            {
                Add(" uk-text-right");
                Add(shortv);
            }
            else if (v is int intv)
            {
                Add(" uk-text-right");
                Add(intv);
            }
            else if (v is long longv)
            {
                Add(" uk-text-right");
                Add(longv);
            }
            else if (v is string strv)
            {
                Add(strv);
            }
            else if (v is decimal decv)
            {
                Add(" uk-text-right");
                Add(decv);
            }
            else if (v is double doublev)
            {
                Add(doublev);
            }
            else if (v is DateTime dtv)
            {
                Add(" uk-text-center");
                Add(dtv);
            }
            Add("</span>");
            return this;
        }

        public HtmlContent SPAN_(byte w = 0x11)
        {
            Add("<span");
            if (w > 0)
            {
                int lo = w & 0x0f;
                int hi = w >> 4;
                Add(" class=\"uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _SPAN()
        {
            Add("</span>");
            return this;
        }

        public HtmlContent A_(string css = null, string href = null, string onclick = null)
        {
            Add("<a class=\"");
            Add(css);
            if (href != null)
            {
                Add("\" href=\"");
                Add(href);
            }
            if (onclick != null)
            {
                Add("\" onclick=\"");
                Add(onclick);
            }
            Add("\">");
            return this;
        }

        public HtmlContent A_HREF_(string @class = null, string onclick = null)
        {
            Add("<a class=\"");
            Add(@class);
            if (onclick != null)
            {
                Add(" onclick=\"");
                Add(onclick);
            }
            Add("\" href=\"");
            return this;
        }

        public HtmlContent _HREF_()
        {
            Add("\">");
            return this;
        }

        public HtmlContent _A(string label = null)
        {
            if (label != null)
            {
                Add(label);
            }
            Add("</a>");
            return this;
        }

        public HtmlContent A_CLOSEUP(string caption)
        {
            Add("<a href=\"#\" onclick=\"closeUp(false); return false;\"");
            Add(" class=\"uk-button uk-button-default uk-border-rounded\"");
            Add(">");
            Add(caption);
            Add("</a>");
            return this;
        }

        public HtmlContent A_GOTO(string caption, string icon = null, string href = null)
        {
            Add("<a target=\"_parent\" href=\"");
            Add(href);
            Add("\" class=\"uk-button uk-button-default\"");
            Add(">");
            Add(caption);
            if (icon != null)
            {
                Add("&nbsp;");
                Add("<span uk-icon=\"");
                Add(icon);
                Add("\"></span>");
            }
            Add("</a>");
            return this;
        }

        public HtmlContent _BUTTON()
        {
            Add("</button>");
            return this;
        }

        public HtmlContent IMG_(string @class = null, string alt = null)
        {
            Add("<img class=\"");
            Add(@class);
            if (alt != null)
            {
                Add(" alt=\"");
                Add(alt);
            }
            Add("\" src=\"");
            return this;
        }

        public HtmlContent _IMG()
        {
            Add("\">");
            return this;
        }

        public HtmlContent PIC_(byte w = 0, string css = null)
        {
            Add("<div class=\"uk-margin-auto-vertical uk-text-center");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Width(w);
            Add("\"><img alt=\"\" src=\"");
            return this;
        }

        public HtmlContent _PIC()
        {
            Add("\">");
            Add("</div>");
            return this;
        }

        public HtmlContent ICO_(string css = null, string alt = null)
        {
            Add("<div class=\"uk-margin-auto-vertical");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\"><img class=\"uk-border-circle\" alt=\"");
            if (alt != null)
            {
                Add(alt);
            }
            Add("\" src=\"");
            return this;
        }

        public HtmlContent _ICO()
        {
            Add("\">");
            Add("</div>");
            return this;
        }

        public HtmlContent ICO(string src, string css = null, string alt = null)
        {
            ICO_(css, alt);
            Add(src);
            _ICO();
            return this;
        }

        public HtmlContent IMG(string src, string href = null, byte w = 0x0c)
        {
            if (href != null)
            {
                Add("<a href=\"");
                Add(href);
                Add("\">");
            }
            Add("<img class=\"uk-img");
            Width(w);
            Add("\" src=\"");
            Add(src);
            Add("\">");
            if (href != null)
            {
                Add("</a>");
            }
            return this;
        }

        public HtmlContent PIC(string src, byte w = 0x0c)
        {
            Add("<div class=\"uk-margin-vertical-auto");
            Width(w);
            Add("\"><img class=\"uk-img");
            Add("\" src=\"");
            Add(src);
            Add("\"></div>");
            return this;
        }

        public HtmlContent QRCODE(string v)
        {
            Add("<div class=\"uk-qrcode\">");
            Add("<script type=\"text/javascript\">");
            Add("var scripte = document.scripts[document.scripts.length - 1];");
            Add("new QRCode(scripte.parentNode, \"");
            Add(v);
            Add("\");");
            Add("</script>");
            Add("</div>");
            return this;
        }

        //
        // UIKIT COMPONENTS
        //

        public HtmlContent MSG_(bool yes, string title, string msg)
        {
            Add("<div class=\"uk-msg\">");

            // add an icon
            Add("<header class=\"");
            Add(yes ? "uk-msg-yes" : "uk-msg-no");
            Add("\">");

            Add("<span class=\"uk-width-auto\" uk-icon=\"icon: ");
            Add(yes ? "check" : "close");
            Add("; ratio: 2\"></span>");

            Add("<h3 class=\"uk-width-expand\">");
            Add(title);
            Add("</h3>");

            Add("</header>");

            Add("<div>");
            Add(msg);
            Add("</div>");

            Add("</div>");
            return this;
        }

        public HtmlContent ALERT_(Style style = 0, bool close = false)
        {
            Add("<div class=\"");
            if (style > 0)
            {
                switch (style)
                {
                    case Style.Primary:
                        Add("uk-alert-primary");
                        break;
                    case Style.Success:
                        Add("uk-alert-success");
                        break;
                    case Style.Warning:
                        Add("uk-alert-warning");
                        break;
                    case Style.Danger:
                        Add("uk-alert-danger");
                        break;
                }
            }
            Add("\" uk-alert>");
            if (close)
            {
                Add("<a class=\"uk-alert-close\" uk-close></a>");
            }
            return this;
        }

        public HtmlContent _ALERT()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent ALERT(string p, Style style = 0, bool close = false)
        {
            ALERT_(style, close);
            Add("<p>");
            Add(p);
            Add("</p>");
            _ALERT();
            return this;
        }

        public HtmlContent BADGE_(Style style = 0)
        {
            Add("<span class=\"uk-badge");
            if (style > 0)
            {
                switch (style)
                {
                    case Style.Primary:
                        Add(" uk-badge-primary");
                        break;
                    case Style.Success:
                        Add(" uk-badge-success");
                        break;
                    case Style.Warning:
                        Add(" uk-badge-warning");
                        break;
                    case Style.Danger:
                        Add(" uk-badge-danger");
                        break;
                }
            }
            Add("\">");
            return this;
        }

        public HtmlContent _BADGE()
        {
            Add("</span>");
            return this;
        }

        public HtmlContent BADGE(string caption, Style style = 0)
        {
            BADGE_(style);
            Add(caption);
            _BADGE();
            return this;
        }


        public HtmlContent FORM_(string action = null, bool post = true, bool mp = false, string oninput = null, string onsubmit = null, string css = null)
        {
            Add("<form class=\"uk-grid uk-flex-center");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add('"');
            if (action != null)
            {
                Add(" action=\"");
                Add(action);
                Add('"');
            }
            if (post)
            {
                Add(" method=\"post\"");
            }
            if (mp)
            {
                Add(" enctype=\"multipart/form-data\"");
            }
            if (oninput != null)
            {
                Add(" oninput=\"");
                Add(oninput);
                Add('"');
            }
            if (onsubmit != null)
            {
                Add(" onsubmit=\"");
                Add(onsubmit);
                Add('"');
            }
            Add(">");
            return this;
        }

        public HtmlContent _FORM()
        {
            Add("</form>");
            return this;
        }

        public HtmlContent FIELDSET_(string legend = null, byte w = 0x11)
        {
            Add("<fieldset class=\"uk-fieldset");
            Width(w);
            Add("\">");
            if (legend != null)
            {
                Add("<legend>");
                AddEsc(legend);
                Add("</legend>");
            }
            Add("<ul>");
            return this;
        }

        public HtmlContent _FIELDSET()
        {
            Add("</ul>");
            Add("</fieldset>");
            return this;
        }

        public HtmlContent BUTTON(string v, bool post = true, bool top = false)
        {
            Add("<button class=\"uk-button uk-button-default\" formmethod=\"");
            Add(post ? "post" : "get");
            if (top)
            {
                Add("\" formtarget=\"_top");
            }
            Add("\">");
            AddEsc(v);
            Add("</button>");
            return this;
        }

        public HtmlContent BUTTON(string name, int subcmd, string v, bool post = true, Style style = Style.Default)
        {
            Add("<button class=\"uk-button");
            if (style == Style.Default) Add(" uk-button-default");
            if (style == Style.Primary) Add(" uk-button-primary");
            else if (style == Style.Secondary) Add(" uk-button-secondary");
            else if (style == Style.Danger) Add(" uk-button-danger");
            else if (style == Style.Text) Add(" uk-button-text");
            else if (style == Style.Link) Add(" uk-button-link");
            else if (style == Style.Icon) Add(" uk-icon-button");
            Add("\" formmethod=\"");
            Add(post ? "post" : "get");
            Add("\" formaction=\"");
            Add(name);
            Add('-');
            Add(subcmd);
            Add("\">");
            AddEsc(v);
            Add("</button>");
            return this;
        }

        public void PAGENATION(int count, int limit = 20)
        {
            // pagination
            var actr = webCtx.Actioner;
            if (actr.HasSubscript)
            {
                Add("<ul class=\"uk-pagination uk-flex-center\">");
                int subscpt = webCtx.Subscript;
                for (int i = 0; i <= subscpt; i++)
                {
                    if (subscpt == i)
                    {
                        Add("<li class=\"uk-active\">");
                        Add(i + 1);
                        Add("</li>");
                    }
                    else
                    {
                        Add("<li><a href=\"");
                        Add(actr.Key);
                        Add('-');
                        Add(i);
                        Add(webCtx.QueryString);
                        Add("\">");
                        Add(i + 1);
                        Add("</a></li>");
                    }
                }
                if (count == limit)
                {
                    Add("<li class=\"pagination-next\"><a href=\"");
                    Add(actr.Key);
                    Add('-');
                    Add(subscpt + 1);
                    Add(webCtx.QueryString);
                    Add("\">");
                    Add(subscpt + 2);
                    Add("</a></li>");
                }
                Add("</ul>");
            }
        }

        public HtmlContent LIST<D>(D[] arr, Action<D> item, string ul = null, string li = null)
        {
            Add("<ul class=\"uk-list uk-list-divider");
            if (ul != null)
            {
                Add(' ');
                Add(ul);
            }
            Add("\">");

            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;

                    Add("<li class=\"uk-flex");
                    if (li != null)
                    {
                        Add(' ');
                        Add(li);
                    }
                    Add("\">");
                    item(obj);
                    Add("</li>");

                    stack[level] = null;
                }

                level--; // exit the level
            }

            Add("</ul>");
            return this;
        }

        public HtmlContent ACCORDION<D>(D[] arr, Action<D> item, string ul = null, string li = null)
        {
            Add("<ul uk-accordion=\"multiple: true\" class=\"");
            if (ul != null)
            {
                Add(' ');
                Add(ul);
            }
            Add("\">");

            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;

                    Add("<li class=\"uk-flex uk-card uk-card-default");
                    if (li != null)
                    {
                        Add(' ');
                        Add(li);
                    }
                    Add("\">");
                    item(obj);
                    Add("</li>");

                    stack[level] = null;
                }

                level--; // exit the level
            }
            // pagination if any
            Add("</ul>");
            return this;
        }

        public void TABLE<D>(D[] arr, Action head, Action<D> row, byte sort = 0)
        {
            Work w = webCtx.Work;
            Work vw = w.varwork;
            Add("<div class=\"uk-overflow-auto\">");
            Add("<table class=\"uk-table uk-table-divider uk-table-hover\">");
            Actioner[] actrs = vw?.Tooled;
            if (head != null)
            {
                Add("<thead>");
                Add("<tr>");
                if (w.HasPick)
                {
                    Add("<th></th>"); // 
                }
                head();
                if (actrs != null)
                {
                    Add("<th></th>"); // for triggers
                }
                Add("</tr>");
                Add("</thead>");
            }

            if (arr != null && row != null) // tbody if having data objects
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                Add("<tbody>");
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;

                    Add("<tr>");
                    if (vw != null && w.HasPick)
                    {
                        Add("<td style=\"width: 1%\">");
                        Add("<input form=\"tool-bar-form\" name=\"key\" type=\"checkbox\" class=\"uk-checkbox\" value=\"");
                        vw.PutVariableKey(obj, this);
                        Add("\" onchange=\"checkit(this);\">");
                        Add("</td>");
                    }
                    row(obj);
                    if (actrs != null) // triggers
                    {
                        Add("<td>");
                        Add("<form class=\"uk-button-group\">");
                        for (int j = 0; j < actrs.Length; j++)
                        {
                            var actr = actrs[j];
                            if (!actr.IsCapital && (actr.Sort == 0 || sort == actr.Sort))
                            {
                                PutTool(actr);
                            }
                        }
                        Add("</form>");
                        Add("</td>");
                    }
                    Add("</tr>");

                    stack[level] = null;
                }
                Add("</tbody>");

                level--; // exit the level
            }
            Add("</table>");
            Add("</div>");
        }

        public void BOARD<D>(D[] arr, Action<D> card, string main = null, string article = "uk-card-default")
        {
            Add("<main class=\"uk-board");
            if (main != null)
            {
                Add(' ');
                Add(main);
            }
            Add("\">");
            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;
                    Add("<form class=\"uk-card");
                    if (article != null)
                    {
                        Add(' ');
                        Add(article);
                    }
                    Add("\">");
                    card(obj);
                    Add("</form>");
                    stack[level] = null;
                }
                level--; // exit the level
            }
            Add("</main>");
        }

        public void GRID<D>(D[] arr, Action<D> card, string css = null)
        {
            Add("<main uk-grid class=\"uk-child-width-1-1 uk-child-width-1-2@s uk-child-width-1-3@m uk-child-width-1-4@l uk-child-width-1-5@xl\">");
            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;
                    Add("<article class=\"uk-card uk-card-default uk-margin-remove uk-padding-small");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
                    }
                    Add("\">");
                    card(obj);
                    Add("</article>");
                    stack[level] = null;
                }
                level--; // exit the level
            }
            Add("</main>");
        }

        void OnClickDialog(byte mode, bool pick, byte size, string tip)
        {
            Add(" onclick=\"return dialog(this,");
            Add(mode);
            Add(",");
            Add(pick);
            Add(",");
            Add(size);
            Add(",'");
            Add(tip);
            Add("');\"");
        }

        public HtmlContent TOOLBAR(byte group = 0x0f, string title = null, bool refresh = true)
        {
            Add("<form id=\"tool-bar-form\" class=\"uk-top-bar\">");
            Add("<section class=\"uk-top-bar-left\">");
            int gogrp = -1;
            var actrs = webCtx.Work.Tooled;
            if (actrs != null)
            {
                for (int i = 0; i < actrs.Length; i++)
                {
                    var actr = actrs[i];
                    int g = actr.Sort;
                    if (g == 0 || (g & group) > 0)
                    {
                        if (g != gogrp)
                        {
                            if (gogrp != -1) Add("</div>");
                            Add("<div class=\"uk-button-group\">");
                        }
                        PutTool(actr);
                    }
                    gogrp = g;
                }
                Add("</div>");
            }
            Add("</section>");

            Add("<section class=\"uk-flex uk-flex-middle\">");
            if (title != null)
            {
                Add(title);
            }
            if (refresh)
            {
                Add("<a class=\"uk-icon-button uk-light\" href=\"javascript: location.reload(false);\" uk-icon=\"refresh\"></a>");
            }
            Add("</section>");

            Add("</form>");
            Add("<div class=\"uk-top-placeholder\"></div>");
            return this;
        }

        public HtmlContent BOTTOMBAR_()
        {
            Add("<div class=\"uk-bottom-placeholder\"></div>");
            Add("<footer class=\"uk-bottom-bar\">");
            return this;
        }

        public HtmlContent _BOTTOMBAR()
        {
            Add("</footer>");
            return this;
        }

        public HtmlContent TOOLS(byte group = 0, string css = null)
        {
            Add("<nav class=\"uk-flex uk-flex-center");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\">");

            int curg = -1;
            Work wrk = webCtx.Work;
            var acts = wrk.Tooled;
            if (acts != null)
            {
                for (int i = 0; i < acts.Length; i++)
                {
                    var act = acts[i];
                    int g = act.Sort;
                    if (g == 0 || (g & group) > 0)
                    {
                        if (g != curg)
                        {
                            if (curg != -1) Add("</div>");
                            Add("<div class=\"uk-button-group\">");
                        }
                        PutTool(act);
                        curg = g;
                    }
                }
                Add("</div>");
            }
            Add("</nav>");
            return this;
        }


        public HtmlContent VARTOOLS(byte group = 0, string css = null)
        {
            Add("<nav class=\"uk-flex uk-flex-center uk-width-1-1");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\">");

            int curg = -1;
            Work wrk = webCtx.Work.VarWork;
            var acts = wrk?.Tooled;
            if (acts != null)
            {
                for (int i = 0; i < acts.Length; i++)
                {
                    var actr = acts[i];
                    int g = actr.Sort;
                    if (g == 0 || (g & group) > 0)
                    {
                        if (g != curg)
                        {
                            if (curg != -1) Add("</div>");
                            Add("<div class=\"uk-button-group\">");
                        }
                        PutTool(actr);
                        curg = g;
                    }
                }
                Add("</div>");
            }
            Add("</nav>");
            return this;
        }

        public HtmlContent TOOL(string action, int subscript = -1, string caption = null)
        {
            // locate the proper work
            Work w = webCtx.Work;
            for (int i = -1; i < level; i++)
            {
                w = w.varwork;
            }
            var actr = w[action];
            if (actr != null)
            {
                PutTool(actr, subscript, caption);
            }
            return this;
        }

        void PutTool(Actioner act, int subscript = -1, string caption = null)
        {
            var tool = act.Tool;

            // check action's availability
            bool ok = !tool.Auth || act.CheckAccess(webCtx, out _);
            if (ok && level >= 0)
            {
                ok = act.CheckState(webCtx, stack, level);
            }

            if (tool.IsAnchorTag)
            {
                Add("<a class=\"");
                var style = tool.Style;
                if (style > Style.None)
                {
                    Add("uk-button");
                    if (style == Style.Default) Add(" uk-button-default");
                    if (style == Style.Primary) Add(" uk-button-primary");
                    else if (style == Style.Secondary) Add(" uk-button-secondary");
                    else if (style == Style.Danger) Add(" uk-button-danger");
                    else if (style == Style.Text) Add(" uk-button-text");
                    else if (style == Style.Link) Add(" uk-button-link");
                    else if (style == Style.Icon) Add(" uk-icon-button");
                    if (act == webCtx.Actioner) // if current action
                    {
                        Add(" uk-active");
                    }
                }
                if (!ok)
                {
                    Add(" disabled");
                }
                Add("\" href=\"");
                if (level >= 0)
                {
                    Work w = webCtx.Work;
                    for (int i = 0; i <= level; i++)
                    {
                        w = w.varwork;
                        w.PutVariableKey(stack[i], this);
                        Add('/');
                    }
                }
                Add(act.RealPath);
                if (subscript >= 0)
                {
                    Add('-');
                    Add(subscript);
                }
                Add("\"");
            }
            else
            {
                Add("<button  class=\"uk-button");
                var style = tool.Style;
                if (style == Style.Default) Add(" uk-button-default");
                else if (style == Style.Primary) Add(" uk-button-primary");
                else if (style == Style.Secondary) Add(" uk-button-secondary");
                else if (style == Style.Danger) Add(" uk-button-danger");
                else if (style == Style.Text) Add(" uk-button-text");
                else if (style == Style.Link) Add(" uk-button-link");
                else if (style == Style.Icon) Add(" uk-icon-button");

                Add("\" name=\"");
                Add(act.Key);
                Add("\" formaction=\"");
                if (level >= 0)
                {
                    Work w = webCtx.Work;
                    for (int i = 0; i <= level; i++)
                    {
                        w = w.varwork;
                        w.PutVariableKey(stack[i], this);
                        Add('/');
                    }
                }
                Add(act.Key);
                if (subscript >= 0)
                {
                    Add('-');
                    Add(subscript);
                }
                Add("\" formmethod=\"post\"");
            }

            if (!ok)
            {
                Add(" disabled=\"disabled\" onclick=\"return false;\"");
            }
            else if (tool.HasConfirm)
            {
                Add(" onclick=\"return ");
                if (tool.MustPick)
                {
                    Add("!serialize(this.form) ? false : ");
                }
                Add("confirm('");
                Add(act.Tip ?? act.Label);
                Add("');\"");
            }
            else if (tool.HasPrompt)
            {
                OnClickDialog(2, tool.MustPick, tool.Size, act.Tip);
            }
            else if (tool.HasShow)
            {
                OnClickDialog(4, tool.MustPick, tool.Size, act.Tip);
            }
            else if (tool.HasOpen)
            {
                OnClickDialog(8, tool.MustPick, tool.Size, act.Tip);
            }
            else if (tool.HasScript)
            {
                Add(" onclick=\"return by"); // prefix to avoid js naming conflict
                Add(act.Lower);
                Add("(this);\"");
            }
            else if (tool.HasCrop)
            {
                Add(" onclick=\"return crop(this,");
                Add(tool.Ordinals);
                Add(',');
                Add(tool.Size);
                Add(",'");
                Add(act.Tip);
                Add("');\"");
            }
            Add(">");

            Add(caption ?? act.Label);

            // put the closing tag
            Add(tool.IsAnchorTag ? "</a>" : "</button>");
        }

        //
        // CONTROLS
        //

        public HtmlContent HIDDEN<V>(string name, V val)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(val);
            Add("\">");
            return this;
        }

        public HtmlContent TEXT(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, bool list = false)
        {
            if (label != null) LI_(label);

            Add("<input type=\"text\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");
            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (list)
            {
                Add(" list=\"");
                Add(name);
                Add("-list\"");
            }
            Add(">");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent FILE(string name, string v, string label = null, string tip = null, bool @readonly = false, bool required = false, bool list = false)
        {
            if (label != null) LI_(label);

            Add("<input class=\"uk-input\" type=\"file\" name=\"");
            Add(name);
            Add("\">");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent TEL(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            if (label != null) LI_(label);

            Add("<input class=\"uk-input\" type=\"tel\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");
            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent SEARCH(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool required = false, byte w = 0)
        {
            if (label != null) LI_(label);

            Add("<div class=\"uk-inline");
            Width(w);
            Add("\"><input type=\"search\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
                Add(" size=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            Add(">");

            Add("<a class=\"uk-form-icon uk-form-icon-flip\" href=\"#\" onclick=\"this.previousSibling.form.method = 'get'; this.previousSibling.form.submit();\" uk-icon=\"search\"></a>");
            Add("</div>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent PASSWORD(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            if (label != null) LI_(label);

            Add("<input type=\"password\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (tip != null)
            {
                Add(" Help=\"");
                Add(tip);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
                Add(" size=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent DATE(string name, DateTime val, string label = null, DateTime max = default, DateTime min = default, bool @readonly = false, bool required = false, int step = 0)
        {
            if (label != null) LI_(label);

            Add("<input type=\"date\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(val);
            Add("\"");

            if (max != default)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != default)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            Add(">");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent TIME()
        {
            T("</tbody>");
            return this;
        }

        void AddPrimitive<V>(V v)
        {
            if (v is short shortv) Add(shortv);
            else if (v is int intv) Add(intv);
            else if (v is long longv) Add(longv);
            else if (v is string strv) Add(strv);
            else if (v is decimal decv) Add(decv);
            else if (v is double doublev) Add(doublev);
            else if (v is DateTime dtv) Add(dtv);
        }

        public HtmlContent NUMBER<V>(string name, V v, string label = null, string tip = null, V max = default, V min = default, V step = default, bool @readonly = false, bool required = false)
        {
            if (label != null) LI_(label);

            bool grp = !step.Equals(default(V)); // input group with up and down
            if (grp)
            {
                Add("<div class=\"uk-inline uk-width-1-2\">");
                Add("<a class=\"uk-form-icon\" href=\"#\" uk-icon=\"icon: minus-circle; ratio: 1.5\" onclick=\"this.nextSibling.stepDown();this.nextSibling.form.oninput();\"></a>");
            }
            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            if (v is short shortv && shortv != 0) Add(shortv);
            else if (v is int intv && intv != 0) Add(intv);
            else if (v is long longv && longv != 0) Add(longv);
            else if (v is decimal decv) Add(decv);
            else if (v is double doublev) Add(doublev);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            Add(" min=\"");
            AddPrimitive(min);
            Add("\"");
            if (!max.Equals(default(V)))
            {
                Add(" max=\"");
                AddPrimitive(max);
                Add("\"");
            }
            if (!step.Equals(default(V)))
            {
                Add(" step=\"");
                AddPrimitive(step);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");

            if (grp)
            {
                Add("<a class=\"uk-form-icon uk-form-icon-flip\" href=\"#\" uk-icon=\"icon: plus-circle; ratio: 1.5\" onclick=\"this.previousSibling.stepUp();this.previousSibling.form.oninput();\"></a>");
                Add("</div>");
            }

            if (label != null) _LI();
            return this;
        }

        public HtmlContent NUMBER(string name, decimal v, string label = null, string tip = null, decimal max = decimal.MaxValue, decimal min = decimal.MinValue, decimal step = 0.00m, bool @readonly = false, bool required = false)
        {
            if (label != null) LI_(label);

            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            if (v != 0 || tip == null)
            {
                Add(v);
            }
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (min != decimal.MinValue)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (max != decimal.MaxValue)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            Add(" step=\"");
            if (step > 0)
            {
                Add(step);
            }
            else
            {
                Add("any");
            }
            Add("\"");
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent RANGE()
        {
            return this;
        }

        public HtmlContent COLOR()
        {
            return this;
        }

        public HtmlContent CHECKBOX(string name, bool val, string label = null, bool required = false)
        {
            Add("<li>");
            if (label != null)
            {
                Add("<label>");
            }
            Add("<input type=\"checkbox\" class=\"uk-checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (val) Add(" checked");
            if (required) Add(" required");
            Add(">");
            if (label != null)
            {
                Add(label);
                Add("</label>");
            }
            Add("</li>");
            return this;
        }

        public HtmlContent CHECKBOXSET(string name, string[] val, string[] opt, string legend = null, byte w = 0x0c)
        {
            FIELDSET_(legend, w);
            for (int i = 0; i < opt.Length; i++)
            {
                var e = opt[i];
                Add(" <label>");
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\"");
                if (val != null && val.Contains(e))
                {
                    Add(" checked");
                }
                Add(">");
                Add(e);
                Add(" </label>");
            }
            _FIELDSET();
            return this;
        }

        public HtmlContent RADIO<V>(string name, V v, string label = null, bool @checked = false, bool required = false)
        {
            Add("<li>");
            Add("<label>");
            Add("<input type=\"radio\" class=\"uk-radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
            Add("\"");
            if (required)
            {
                Add(" required");
            }
            if (@checked)
            {
                Add(" checked");
            }
            Add(">");
            Add(label);
            Add("</label>");
            Add("</li>");
            return this;
        }

        public HtmlContent RADIO2<V>(string name, V v, string label = null, bool @checked = false, bool required = false, int idx = 0, int last = 0)
        {
            if (idx % 2 == 0)
            {
                Add("<li>");
            }
            Add("<label class=\"uk-width-1-2\">");
            Add("<input type=\"radio\" class=\"uk-radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
            Add("\"");
            if (required)
            {
                Add(" required");
            }
            if (@checked)
            {
                Add(" checked");
            }
            Add(">");
            Add(label);
            Add("</label>");
            if (idx == last || idx % 2 == 1)
            {
                Add("</li>");
            }
            return this;
        }

        public HtmlContent RADIOSET<K, V>(string name, K v, Map<K, V> opt = null, string legend = null, bool required = false, byte w = 0x11, Predicate<V> filter = null)
        {
            FIELDSET_(legend, w);
            if (opt != null)
            {
                lock (opt)
                {
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.At(i);
                        if (filter != null && !filter(e.value)) continue;
                        if (e.IsHead)
                        {
                            STATIC_(null);
                            Add(e.Value.ToString());
                            _STATIC();
                        }
                        else
                        {
                            Add("<li>");
                            Add("<label>");
                            Add("<input type=\"radio\" class=\"uk-radio\" name=\"");
                            Add(name);
                            Add("\" id=\"");
                            Add(name);
                            AddPrimitive(e.Key);
                            Add("\"");
                            Add("\" value=\"");
                            AddPrimitive(e.Key);
                            Add("\"");
                            if (e.Key.Equals(v)) Add(" checked");
                            if (required) Add(" required");
                            Add(">");
                            Add(e.Value.ToString());
                            Add("</label>");
                            Add("</li>");
                        }
                    }
                }
            }
            _FIELDSET();
            return this;
        }

        public HtmlContent RADIOSET2<K, V>(string name, K v, Map<K, V> opt = null, string legend = null, bool required = false, byte w = 0x11, Predicate<V> filter = null)
        {
            FIELDSET_(legend, w);
            if (opt != null)
            {
                lock (opt)
                {
                    bool odd = true;
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.At(i);
                        if (filter != null && !filter(e.value)) continue;
                        if (e.IsHead)
                        {
                            STATIC_(null);
                            Add(e.Value.ToString());
                            _STATIC();
                            odd = true;
                        }
                        else
                        {
                            if (odd)
                            {
                                Add("<li>");
                            }
                            Add("<label class=\"uk-width-expand\">");
                            Add("<input type=\"radio\" class=\"uk-radio\" name=\"");
                            Add(name);
                            Add("\" id=\"");
                            Add(name);
                            AddPrimitive(e.Key);
                            Add("\"");
                            Add("\" value=\"");
                            AddPrimitive(e.Key);
                            Add("\"");
                            if (e.Key.Equals(v)) Add(" checked");
                            if (required) Add(" required");
                            Add(">");
                            Add(e.Value.ToString());
                            Add("</label>");
                            if (!odd)
                            {
                                Add("</li>");
                            }
                            odd = !odd;
                        }
                    }
                }
            }
            _FIELDSET();
            return this;
        }

        public HtmlContent RADIOSET(string name, string v, string[] opt, string legend = null, bool required = false, byte w = 0x11)
        {
            FIELDSET_(legend, w);
            for (int i = 0; i < opt.Length; i++)
            {
                var o = opt[i];
                RADIO(name, o, o, o.Equals(v));
            }
            _FIELDSET();
            return this;
        }

        public HtmlContent TEXTAREA(string name, string v, string label = null, string tip = null, short max = 0, short min = 0, bool @readonly = false, bool required = false, byte w = 0)
        {
            if (label != null) LI_(label);

            Add("<textarea class=\"uk-textarea");
            Width(w);
            Add("\" name=\"");
            Add(name);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");

                Add(" rows=\"");
                Add(max < 200 ? 3 :
                    max < 400 ? 4 : 5);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            AddEsc(v);
            Add("</textarea>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent TEXTAREA(string name, string[] val, string label = null, string tip = null, short max = 0, short min = 0, bool @readonly = false, bool required = false)
        {
            if (label != null) LI_(label);

            Add("<textarea class=\"uk-textarea\" name=\"");
            Add(name);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");

                Add(" rows=\"");
                Add(max < 200 ? 3 :
                    max < 400 ? 4 : 5);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            if (val != null)
            {
                for (int i = 0; i < val.Length; i++)
                {
                    if (i > 0)
                    {
                        Add('\n');
                    }
                    AddEsc(val[i]);
                }
            }
            Add("</textarea>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent SELECT_(string name, string label = null, bool multiple = false, bool required = false, int size = 0)
        {
            LI_(label);

            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _SELECT()
        {
            Add("</select>");
            _LI();
            return this;
        }

        public HtmlContent OPTION<T>(T val, string label, bool selected = false)
        {
            Add("<option value=\"");
            if (val is short shortv)
            {
                Add(shortv);
            }
            else if (val is int intv)
            {
                Add(intv);
            }
            else if (val is string strv)
            {
                Add(strv);
            }
            Add("\"");
            if (selected) Add(" selected");
            Add(">");
            Add(label);
            Add("</option>");
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K v, Map<K, V> opt, string label = null, string tip = null, bool required = false, sbyte size = 0, bool refresh = false, Predicate<V> filter = null)
        {
            if (label != null) LI_(label);

            Add("<select class=\"uk-select\" name=\"");
            Add(name);
            Add("\"");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + serialize(this.form);\"");
            }
            Add(">");
            if (tip != null)
            {
                Add("<option disabled selected>");
                Add(tip);
                Add("</option>");
            }
            if (opt != null)
            {
                lock (opt)
                {
                    bool grpopen = false;
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.At(i);
                        if (filter != null && !filter(e.value)) continue;
                        if (e.IsHead)
                        {
                            if (grpopen)
                            {
                                Add("</optgroup>");
                                grpopen = false;
                            }
                            Add("<optgroup label=\"");
                            Add(e.value?.ToString());
                            Add("\">");
                            grpopen = true;
                        }
                        else
                        {
                            var key = e.key;
                            Add("<option value=\"");
                            if (key is short shortv)
                            {
                                Add(shortv);
                            }
                            else if (key is int intv)
                            {
                                Add(intv);
                            }
                            else if (key is string strv)
                            {
                                Add(strv);
                            }
                            Add("\"");
                            if (key.Equals(v)) Add(" selected");
                            Add(">");
                            Add(e.value?.ToString());
                            Add("</option>");
                        }
                    }
                    if (grpopen)
                    {
                        Add("</optgroup>");
                        grpopen = false;
                    }
                }
            }
            Add("</select>");
            if (label != null) _LI();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K[] v, Map<K, V> opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false)
        {
            if (label != null) LI_(label);

            Add("<select name=\"");
            Add(name);
            Add("\" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + serialize(this.form);\"");
            }
            Add(">");

            if (opt != null)
            {
                lock (opt)
                {
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.At(i);
                        var key = e.key;
                        Add("<option value=\"");
                        if (key is short shortv)
                        {
                            Add(shortv);
                        }
                        else if (key is int intv)
                        {
                            Add(intv);
                        }
                        else if (key is string strv)
                        {
                            Add(strv);
                        }
                        Add("\"");
                        if (v.Contains(key)) Add(" selected");
                        Add(">");
                        Add(e.value.ToString());
                        Add("</option>");
                    }
                }
            }
            Add("</select>");
            if (label != null) _LI();
            return this;
        }

        public HtmlContent SELECT(string name, string v, string[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false)
        {
            if (label != null) LI_(label);

            Add("<select class=\"uk-select\" name=\"");
            Add(name);
            Add("\"");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + serialize(this.form);\"");
            }
            Add(">");
            if (opt != null)
            {
                lock (opt)
                {
                    for (int i = 0; i < opt.Length; i++)
                    {
                        var e = opt[i];
                        Add("<option value=\"");
                        Add(e);
                        Add("\"");
                        if (e.Equals(v)) Add(" selected");
                        Add(">");
                        Add(e);
                        Add("</option>");
                    }
                }
            }
            Add("</select>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent SELECT(string name, string[] v, string[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false)
        {
            if (label != null) LI_(label);

            Add("<select name=\"");
            Add(name);
            Add("\" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + serialize(this.form);\"");
            }
            Add(">");

            if (opt != null)
            {
                for (int i = 0; i < opt.Length; i++)
                {
                    var e = opt[i];
                    Add("<option value=\"");
                    Add(e);
                    Add("\"");
                    if (v.Contains(e)) Add(" selected");
                    Add(">");
                    Add(e);
                    Add("</option>");
                }
            }
            Add("</select>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K val, V[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false) where V : IKeyable<K>
        {
            if (label != null) LI_(label);

            Add("<select class=\"uk-select\" name=\"");
            Add(name);
            Add("\"");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + serialize(this.form);\"");
            }
            Add(">");

            if (opt != null)
            {
                for (int i = 0; i < opt.Length; i++)
                {
                    var e = opt[i];
                    var key = e.Key;
                    Add("<option value=\"");
                    if (key is short shortv) Add(shortv);
                    else if (key is int intv) Add(intv);
                    else if (key is string strv) Add(strv);
                    Add("\"");
                    if (key.Equals(val)) Add(" selected");
                    Add(">");
                    Add(e.ToString());
                    Add("</option>");
                }
            }
            Add("</select>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K[] v, V[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false) where V : IKeyable<K>
        {
            if (label != null) LI_(label);

            Add("<select name=\"");
            Add(name);
            Add("\" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + $(this.form).serialize();\"");
            }
            Add(">");

            if (opt != null)
            {
                for (int i = 0; i < opt.Length; i++)
                {
                    var e = opt[i];
                    var key = e.Key;
                    Add("<option value=\"");
                    if (key is short shortv) Add(shortv);
                    else if (key is int intv) Add(intv);
                    else if (key is string strv) Add(strv);
                    Add("\"");
                    if (v != null && v.Contains(key)) Add(" selected");
                    Add(">");
                    if (key is short shortc) Add(shortc);
                    else if (key is int intc) Add(intc);
                    else if (key is string strc) Add(strc);
                    Add("</option>");
                }
            }
            Add("</select>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent DATALIST(short id, string[] opt)
        {
            Add("<datalist");
            Add(" id=\"datalist-");
            Add(id);
            Add("\">");
            for (int i = 0; i < opt?.Length; i++)
            {
                string v = opt[i];
                Add("<option value=\"");
                Add(v);
                Add("\">");
                Add(v);
                Add("</option>");
            }
            Add("</datalist>");
            return this;
        }

        public HtmlContent PROGRES<V>(V max, V val, bool percent = false)
        {
            Add("<progress max=\"");
            AddPrimitive(max);
            Add("\" value=\"");
            AddPrimitive(val);
            Add("\">");
            if (percent)
            {
                AddPrimitive(val);
                Add('%');
            }
            Add("</progress>");
            return this;
        }

        public HtmlContent OUTPUT<V>(string name, V v, string label = null)
        {
            if (label != null) LI_(label);

            Add("<output class=\"uk-output\" name=\"");
            Add(name);
            Add("\">");
            AddPrimitive(v);
            Add("</output>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent OUTPUT(string name, decimal v, string label = null)
        {
            if (label != null) LI_(label);

            Add("<output class=\"uk-output\" name=\"");
            Add(name);
            Add("\">¥");
            Add(v);
            Add("</output>");

            if (label != null) _LI();
            return this;
        }

        public HtmlContent METER()
        {
            Add("<meter>");
            Add("</meter>");
            return this;
        }
    }
}