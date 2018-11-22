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

        public override string Type { get; set; } = "text/html; charset=utf-8";

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

        public void AddEsc(string v, int offset, int length)
        {
            if (v == null) return;
            int len = Math.Min(length, v.Length);
            for (int i = offset; i < len; i++)
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

        public HtmlContent TT(string v, bool cond = true)
        {
            if (cond)
            {
                AddEsc(v);
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

        public HtmlContent TT(string v, int offset, int len, bool cond = true)
        {
            if (cond)
            {
                AddEsc(v, offset, len);
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

        public HtmlContent TT(string[] v, bool cond = true)
        {
            if (cond && v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add("&nbsp;");
                    AddEsc(v[i]);
                }
            }
            return this;
        }


        public HtmlContent BR()
        {
            Add("<br>");
            return this;
        }

        public HtmlContent HR()
        {
            Add("<hr>");
            return this;
        }

        public HtmlContent SP()
        {
            Add("&nbsp;");
            return this;
        }

        public HtmlContent ROW_(string css = null)
        {
            Add("<div class=\"uk-row");
            if (css != null)
            {
                Add(' ');
                Add(css);
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

        public HtmlContent TD(short v, bool cond = true)
        {
            Add("<td style=\"text-align: right\">");
            if (cond)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD(int v, bool cond = true)
        {
            Add("<td style=\"text-align: right\">");
            if (cond)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD(long v, bool cond = true)
        {
            Add("<td style=\"text-align: right\">");
            if (cond)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD(decimal v, bool cond = true)
        {
            Add("<td style=\"text-align: right\">");
            if (cond)
            {
                Add(v);
            }
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
            LABEL(label);
            Add("<span class=\"uk-static\">");
            return this;
        }

        public HtmlContent _STATIC()
        {
            Add("</span>");
            return this;
        }

        public HtmlContent STATIC<V>(string label, V v)
        {
            STATIC_(label);
            AddPrimitive(v);
            _STATIC();
            return this;
        }

        public HtmlContent COL_(string css = null)
        {
            Add("<div class=\"uk-col");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\">");
            return this;
        }

        public HtmlContent _COL()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent SPAN_(string css = null)
        {
            Add("<span");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
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

        public HtmlContent SPAN<V>(V v, string css = null)
        {
            SPAN_(css);
            AddPrimitive(v);
            _SPAN();
            return this;
        }

        public HtmlContent H2_(string css = null)
        {
            Add("<h2");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _H2()
        {
            Add("</h2>");
            return this;
        }

        public HtmlContent H2<V>(V v, string css = null)
        {
            H2_(css);
            AddPrimitive(v);
            _H2();
            return this;
        }


        public HtmlContent H3_(string css = null)
        {
            Add("<h3");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _H3()
        {
            Add("</h3>");
            return this;
        }

        public HtmlContent H3<V>(V v, string css = null)
        {
            H3_(css);
            AddPrimitive(v);
            _H3();
            return this;
        }


        public HtmlContent H4_(string css = null)
        {
            Add("<h4");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _H4()
        {
            Add("</h4>");
            return this;
        }

        public HtmlContent H4<V>(V v, string css = null)
        {
            H4_(css);
            AddPrimitive(v);
            _H4();
            return this;
        }

        public HtmlContent H5_(string css = null)
        {
            Add("<h5");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _H5()
        {
            Add("</h5>");
            return this;
        }

        public HtmlContent H5<V>(V v, string css = null)
        {
            H5_(css);
            AddPrimitive(v);
            _H5();
            return this;
        }


        public HtmlContent P<V>(V v, string css = null)
        {
            Add("<p");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            AddPrimitive(v);
            Add("</p>");
            return this;
        }

        public HtmlContent P_(string css = null)
        {
            Add("<p");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _P()
        {
            Add("</p>");
            return this;
        }

        public HtmlContent DIV_(string css = null)
        {
            Add("<div");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _DIV()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent SECTION_(string css = null)
        {
            Add("<section");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _SECTION()
        {
            Add("</section>");
            return this;
        }

        public HtmlContent MAIN_(string css = null)
        {
            Add("<main");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _MAIN()
        {
            Add("</main>");
            return this;
        }

        public HtmlContent HEADER_(string css = null)
        {
            Add("<header");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _HEADER()
        {
            Add("</header>");
            return this;
        }

        public HtmlContent FOOTER_(string css = null)
        {
            Add("<footer");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _FOOTER()
        {
            Add("</footer>");
            return this;
        }

        public HtmlContent ARTICLE_(string css = null)
        {
            Add("<article");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _ARTICLE()
        {
            Add("</article>");
            return this;
        }

        public HtmlContent NAV_(string css = null)
        {
            Add("<nav");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _NAV()
        {
            Add("</nav>");
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


        public HtmlContent LI_(string css = null)
        {
            Add("<li");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _LI()
        {
            Add("</li>");
            return this;
        }

        public HtmlContent DL_(string css = null)
        {
            Add("<dl");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _DL()
        {
            Add("</dl>");
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

        public HtmlContent FI(string label, bool v)
        {
            LABEL(label);
            Add("<p>");
            if (v)
            {
                Add("&checkmark;");
            }
            Add("</p>");
            return this;
        }

        public HtmlContent FI<K, V>(string label, K[] keys, Map<K, V> map)
        {
            LABEL(label);
            Add("<p>");
            if (keys != null)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];
                    var val = map[key];
                    if (i > 0) Add("&nbsp;");
                    Add(val.ToString());
                }
            }
            Add("</p>");
            return this;
        }

        public HtmlContent CNY(decimal v, bool em = false)
        {
            Add("¥");
            if (em)
            {
                Add("<em>");
            }
            Add(v);
            if (em)
            {
                Add("</em>");
            }
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

        public HtmlContent PIC_(string css = null, bool circle = true)
        {
            Add("<div class=\"uk-margin-auto-vertical");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\"><img");
            if (circle)
            {
                Add(" class=\"uk-border-circle\"");
            }
            Add(" src=\"");
            return this;
        }

        public HtmlContent _PIC()
        {
            Add("\">");
            Add("</div>");
            return this;
        }

        public HtmlContent PIC(string src, string css = null, bool circle = true)
        {
            PIC_(css, circle);
            Add(src);
            _PIC();
            return this;
        }

        public HtmlContent PROGRESS(int v, int max, string css = null)
        {
            Add("<div class=\"uk-progress");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\"><span style=\"width: ");
            Add(v * 100 / max);
            Add("%\"></div>");
            return this;
        }

        public HtmlContent QRCODE(string v, string css = null)
        {
            Add("<div class=\"uk-qrcode");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\"><script type=\"text/javascript\">");
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

        public HtmlContent ALERT_(string css = null, bool close = false)
        {
            Add("<div class=\"");
            if (css != null)
            {
                Add(css);
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

        public HtmlContent ALERT(string p, string css = null, bool close = false)
        {
            ALERT_(css, close);
            Add("<p>");
            Add(p);
            Add("</p>");
            _ALERT();
            return this;
        }

        public HtmlContent FORM_(string action = null, bool post = true, bool mp = false, string oninput = null, string onsubmit = null, string css = null)
        {
            Add("<form class=\"");
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

        /// <summary>
        /// The combination of fieldset and ul elements.
        /// </summary>
        /// <param name="legend"></param>
        /// <param name="css"></param>
        /// <returns></returns>
        public HtmlContent FIELDUL_(string legend = null, string css = null)
        {
            Add("<fieldset class=\"uk-fieldset uk-width-1-1");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
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

        public HtmlContent _FIELDUL()
        {
            Add("</ul>");
            Add("</fieldset>");
            return this;
        }

        public HtmlContent BUTTON(string caption, string action = null, bool post = true, string css = "uk-button-default")
        {
            Add("<button class=\"uk-button ");
            Add(css);
            Add("\" formmethod=\"");
            Add(post ? "post" : "get");
            if (action != null)
            {
                Add("\" formaction=\"");
                Add(action);
            }
            Add("\">");
            AddEsc(caption);
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
                        Add(webCtx.QueryStr);
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
                    Add(webCtx.QueryStr);
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

        public HtmlContent TABLE_()
        {
            Add("<div class=\"uk-overflow-auto\">");
            Add("<table class=\"uk-table uk-table-divider uk-table-hover\">");
            return this;
        }

        public HtmlContent _TABLE()
        {
            Add("</table>");
            Add("</div>");
            return this;
        }

        public void TABLE<D>(D[] arr, Action head, Action<D> row, byte group = 0, int subscript = -1, bool pick = true)
        {
            Work w = webCtx.Work;
            Work vw = w.varwork;
            Add("<div class=\"uk-overflow-auto\">");
            Add("<table class=\"uk-table uk-table-divider uk-table-hover\">");
            Actioner[] acts = vw?.Tooled;
            if (head != null)
            {
                Add("<thead>");
                Add("<tr>");
                if (w.HasPick)
                {
                    Add("<th></th>"); // 
                }
                head();
                if (acts != null)
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
                    if (pick)
                    {
                        Add("<td style=\"width: 1%\">");
                        Add("<input form=\"tool-bar-form\" name=\"key\" type=\"checkbox\" class=\"uk-checkbox\" value=\"");
                        Work.PutVariableKey(obj, this);
                        Add("\" onchange=\"checkToggle(this);\">");
                        Add("</td>");
                    }
                    row(obj);
                    if (acts != null) // output trigger buttons
                    {
                        Add("<td style=\"text-align: right\">");
                        Add("<form class=\"uk-button-group\">");
                        for (int j = 0; j < acts.Length; j++)
                        {
                            var act = acts[j];
                            if (act.Group == 0 || group == act.Group)
                            {
                                var tool = act.Tool;
                                PutTool(act, tool, tool.IsAnchor ? -1 : subscript, css: "uk-button-secondary");
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

        public void BOARD<D>(D[] arr, Action<D> card, string css = "uk-card-default")
        {
            Add("<main class=\"uk-board\">");
            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;
                    Add("<form class=\"uk-card");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
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

        public void CARD_<D>(D obj, string css = "uk-card-default")
        {
            if (stack == null) stack = new object[4]; // init contexts
            level++; // enter a new level

            stack[level] = obj;
            Add("<form class=\"uk-card");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\">");
        }

        public void _CARD()
        {
            Add("</form>");
            stack[level] = null;

            level--; // exit the level
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

        public HtmlContent TOOLBAR(byte group = 0x0f, int subscript = -1, string title = null, bool refresh = true)
        {
            Add("<form id=\"tool-bar-form\" class=\"uk-top-bar\">");
            Add("<section class=\"uk-top-bar-left\">"); // ui tools
            int grp = -1;
            var acts = webCtx.Work.Tooled;
            if (acts != null)
            {
                for (int i = 0; i < acts.Length; i++)
                {
                    var act = acts[i];
                    int g = act.Group;
                    if (g == 0 || (g & group) == g)
                    {
                        if (g != grp)
                        {
                            if (grp != -1) Add("</div>");
                            Add("<div class=\"uk-button-group\">");
                        }
                        var tool = act.Tool;
                        // provide the state about current anchor as subscript 
                        PutTool(act, tool, tool.IsAnchor ? -1 : subscript);
                    }
                    grp = g;
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

        public HtmlContent TOOLS(byte group = 0, int subscript = -1, string css = null)
        {
            Add("<nav class=\"uk-flex");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\">");

            // output button group(s)
            int curg = -1;
            Work wrk = webCtx.Work;
            var acts = wrk.Tooled;
            if (acts != null)
            {
                for (int i = 0; i < acts.Length; i++)
                {
                    var act = acts[i];
                    int g = act.Group;
                    if (g == 0 || (g & group) == g)
                    {
                        if (g != curg)
                        {
                            if (curg != -1) Add("</div>");
                            Add("<div class=\"uk-button-group\">");
                        }
                        var tool = act.Tool;
                        PutTool(act, tool, tool.IsAnchor ? -1 : subscript, css: "uk-button-secondary");
                        curg = g;
                    }
                }
                Add("</div>");
            }
            Add("</nav>");
            return this;
        }


        public HtmlContent VARTOOLS(byte group = 0, int subscript = -1, bool pick = true, string css = null)
        {
            Add("<nav class=\"uk-flex");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\">");

            Work w = webCtx.Work;
            Work varw = w.varwork;

            // output a pick check
            if (varw != null && pick)
            {
                Add("<input form=\"tool-bar-form\" name=\"key\" type=\"checkbox\" class=\"uk-checkbox\" value=\"");
                object obj = stack[level];
                Work.PutVariableKey(obj, this);
                Add("\" onchange=\"checkToggle(this);\">");
            }

            // output button group(s)
            int curg = -1;
            var acts = varw?.Tooled;
            if (acts != null)
            {
                for (int i = 0; i < acts.Length; i++)
                {
                    var act = acts[i];
                    int g = act.Group;
                    if (g == 0 || (g & group) == g)
                    {
                        if (g != curg)
                        {
                            if (curg != -1) Add("</div>");
                            Add("<div class=\"uk-button-group\">");
                        }
                        var tool = act.Tool;
                        PutTool(act, tool, tool.IsAnchor ? -1 : subscript, css: "uk-button-secondary");
                        curg = g;
                    }
                }
                Add("</div>");
            }
            Add("</nav>");
            return this;
        }

        public HtmlContent TOOL(string action, int subscript = -1, string caption = null, string css = null)
        {
            // locate the proper work
            Work w = webCtx.Work;
            var act = w[action];
            var tool = act?.Tool;
            if (tool != null)
            {
                PutTool(act, tool, subscript, caption, css);
            }
            return this;
        }

        public HtmlContent VARTOOL(string action, int subscript = -1, string caption = null, string css = null)
        {
            // locate the proper work
            Work w = webCtx.Work.VarWork;
            if (w != null)
            {
                var act = w[action];
                var tool = act?.Tool;
                if (tool != null)
                {
                    PutTool(act, tool, subscript, caption, css);
                }
            }
            return this;
        }

        void PutTool(Actioner act, ToolAttribute tool, int subscript = -1, string caption = null, string css = null)
        {
            // check action's availability
            bool ok = !tool.Access || act.DoAuthorize(webCtx);
            if (ok && level >= 0)
            {
                ok = act.CheckState(webCtx, stack, level);
            }

            var anycss = tool.Css ?? css;
            if (tool.IsAnchorTag)
            {
                Add("<a class=\"uk-button ");
                Add(anycss ?? "uk-button-link");
                if (act == webCtx.Actioner) // if current action
                {
                    Add(" uk-active");
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
                        Work.PutVariableKey(stack[i], this);
                        Add('/');
                    }
                }
                Add(act.Relative);
                if (subscript >= 0 && act.HasSubscript)
                {
                    Add('-');
                    Add(subscript);
                }
                Add("\"");
            }
            else
            {
                Add("<button  class=\"uk-button ");
                Add(anycss ?? "uk-button-default");
                Add("\" name=\"");
                Add(act.Key);
                Add("\" formaction=\"");
                if (level >= 0)
                {
                    Work w = webCtx.Work;
                    for (int i = 0; i <= level; i++)
                    {
                        w = w.varwork;
                        Work.PutVariableKey(stack[i], this);
                        Add('/');
                    }
                }
                Add(act.Key);
                if (subscript >= 0 && act.HasSubscript)
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

            string cap = caption ?? act.Label;
            if (cap != null && cap.Length != 0)
            {
                Add(cap);
            }

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

        public HtmlContent TEXT(string label, string name, string v, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, bool list = false)
        {
            LABEL(label);
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
            return this;
        }

        public HtmlContent FILE(string label, string name, string v, string tip = null, bool @readonly = false, bool required = false, bool list = false)
        {
            LABEL(label);
            Add("<input class=\"uk-input\" type=\"file\" name=\"");
            Add(name);
            Add("\">");
            return this;
        }

        public HtmlContent TEL(string label, string name, string v, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            LABEL(label);
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
            return this;
        }

        public HtmlContent URL(string label, string name, string v, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            LABEL(label);
            Add("<input class=\"uk-input\" type=\"url\" name=\"");
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
            return this;
        }

        public HtmlContent SEARCH(string label, string name, string v, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool required = false)
        {
            LABEL(label);
            Add("<div class=\"uk-inline");
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
            return this;
        }

        public HtmlContent PASSWORD(string label, string name, string v, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            LABEL(label);
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
            return this;
        }

        public HtmlContent DATE(string label, string name, DateTime val, DateTime max = default, DateTime min = default, bool @readonly = false, bool required = false, int step = 0)
        {
            LABEL(label);
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

        public HtmlContent NUMBERPICK(string label, string name, short v, short max = default, short min = default, short step = 1, bool @readonly = false, bool required = false, string onchange = null, string css = null)
        {
            LABEL(label);
            Add("<div class=\"uk-inline");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }
            Add("\">");
            Add("<a class=\"uk-form-icon\" href=\"#\" uk-icon=\"icon: minus-circle; ratio: 1.5\" onclick=\"this.nextSibling.stepDown();this.nextSibling.onchange();\"></a>");
            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\" min=\"");
            Add(min);
            Add("\" max=\"");
            Add(max);
            Add("\" step=\"");
            Add(step);
            Add("\"");
            if (onchange != null)
            {
                Add(" onchange=\"");
                Add(onchange);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");
            Add("<a class=\"uk-form-icon uk-form-icon-flip\" href=\"#\" uk-icon=\"icon: plus-circle; ratio: 1.5\" onclick=\"this.previousSibling.stepUp();this.previousSibling.onchange();\"></a>");
            Add("</div>");
            return this;
        }

        public HtmlContent NUMBER(string label, string name, int v, string tip = null, int max = 0, int min = 0, int step = 0, bool @readonly = false, bool required = false)
        {
            LABEL(label);
            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            Add(" min=\"");
            Add(min);
            Add("\"");
            if (max > 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (step > 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");
            return this;
        }

        public HtmlContent NUMBER(string label, string name, decimal v, decimal max = decimal.MaxValue, decimal min = decimal.MinValue, decimal step = 0.00m, bool @readonly = false, bool required = false)
        {
            LABEL(label);
            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
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
            return this;
        }

        public HtmlContent NUMBER(string label, string name, double v, double max = double.MaxValue, double min = double.MinValue, double step = 0, bool @readonly = false, bool required = false)
        {
            LABEL(label);
            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (min > double.MinValue)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (max < double.MaxValue)
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

        public HtmlContent CHECKBOX(string label, string name, bool v, string tip = null, bool required = false)
        {
            LABEL(label);
            if (tip != null)
            {
                Add("<label>");
            }
            else
            {
                Add("<div class=\"uk-input uk-flex uk-flex-middle uk-margin-left-remove\">");
            }
            Add("<input type=\"checkbox\" class=\"uk-checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (v) Add(" checked");
            if (required) Add(" required");
            Add(">");
            if (tip != null)
            {
                Add(tip); // caption following the checkbox
                Add("</label>");
            }
            else
            {
                Add("</div>");
            }
            return this;
        }

        public HtmlContent CHECKBOX<V>(string label, string name, V v, string tip = null, bool required = false)
        {
            LABEL(label);
            if (tip != null)
            {
                Add("<label>");
            }
            else
            {
                Add("<div class=\"uk-input uk-flex uk-flex-middle uk-margin-left-remove\">");
            }
            Add("<input type=\"checkbox\" class=\"uk-checkbox\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
            Add("\"");
            if (required) Add("\" required");
            Add(">");
            if (tip != null)
            {
                Add(tip); // caption following the checkbox
                Add("</label>");
            }
            else
            {
                Add("</div>");
            }
            return this;
        }

        public HtmlContent CHECKBOXSET(string name, string[] v, string[] opt, string legend = null, string css = null)
        {
            FIELDUL_(legend, css);
            for (int i = 0; i < opt.Length; i++)
            {
                var e = opt[i];
                Add(" <label>");
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\"");
                if (v != null && v.Contains(e))
                {
                    Add(" checked");
                }
                Add(">");
                Add(e);
                Add(" </label>");
            }
            _FIELDUL();
            return this;
        }

        public HtmlContent RADIO<V>(string name, V v, string label = null, bool @checked = false, bool required = false, bool disabled = false)
        {
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
            if (disabled)
            {
                Add(" disabled");
            }
            Add(">");
            Add(label);
            Add("</label>");
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

        public HtmlContent RADIOSET<K, V>(string name, K v, Map<K, V> opt = null, string legend = null, string css = null, bool required = false, Predicate<V> filter = null)
        {
            FIELDUL_(legend, css);
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
            _FIELDUL();
            return this;
        }

        public HtmlContent RADIOSET2<K, V>(string name, K v, Map<K, V> opt = null, string legend = null, string css = null, bool required = false, Predicate<V> filter = null)
        {
            FIELDUL_(legend, css);
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
            _FIELDUL();
            return this;
        }

        public HtmlContent RADIOSET(string name, string v, string[] opt, string legend = null, string css = null, bool required = false)
        {
            FIELDUL_(legend, css);
            for (int i = 0; i < opt.Length; i++)
            {
                var o = opt[i];
                RADIO(name, o, o, o.Equals(v));
            }
            _FIELDUL();
            return this;
        }

        public HtmlContent TEXTAREA(string label, string name, string v, string tip = null, short max = 0, short min = 0, bool @readonly = false, bool required = false)
        {
            TEXTAREA_(label, name, tip, max, min, @readonly, required);
            Add(v);
            _TEXTAREA(label != null);
            return this;
        }

        public HtmlContent TEXTAREA_(string label, string name, string tip = null, short max = 0, short min = 0, bool @readonly = false, bool required = false)
        {
            LABEL(label);
            Add("<textarea class=\"uk-textarea");
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
            return this;
        }

        public HtmlContent _TEXTAREA(bool label = true)
        {
            Add("</textarea>");
            return this;
        }

        public HtmlContent SELECT_(string label, string name, bool multiple = false, bool required = false, int size = 0, bool refresh = false)
        {
            LABEL(label);
            Add("<select class=\"uk-select\" name=\"");
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
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + serialize(this.form);\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _SELECT()
        {
            Add("</select>");
            return this;
        }

        public HtmlContent OPTION<T>(T v, string caption, bool selected = false)
        {
            Add("<option value=\"");
            if (v is short shortv)
            {
                Add(shortv);
            }
            else if (v is int intv)
            {
                Add(intv);
            }
            else if (v is string strv)
            {
                Add(strv);
            }
            Add("\"");
            if (selected) Add(" selected");
            Add(">");
            Add(caption);
            Add("</option>");
            return this;
        }

        public HtmlContent SELECT<K, V>(string label, string name, K v, Map<K, V> opt, string tip = null, bool multiple = false, bool required = false, sbyte size = 0, bool refresh = false, Predicate<V> filter = null)
        {
            SELECT_(label, name, false, required, size, refresh);
            if (tip != null)
            {
                Add("<option value=\"\"");
                if (required)
                {
                    Add(" disabled");
                }
                Add(">");
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
            _SELECT();
            return this;
        }

        public HtmlContent SELECT<K, V>(string label, string name, K[] v, Map<K, V> opt, bool required = false, sbyte size = 0, bool refresh = false)
        {
            SELECT_(label, name, true, required, size, refresh);
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
            _SELECT();
            return this;
        }

        public HtmlContent SELECT(string label, string name, string v, string[] opt, bool required = false, sbyte size = 0, bool refresh = false)
        {
            SELECT_(label, name, false, required, size, refresh);
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
            _SELECT();
            return this;
        }

        public HtmlContent SELECT(string label, string name, string[] v, string[] opt, bool required = false, sbyte size = 0, bool refresh = false)
        {
            SELECT_(label, name, true, required, size, refresh);
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
            _SELECT();
            return this;
        }

        public HtmlContent SELECT<K, V>(string label, string name, K v, V[] opt, bool required = false, sbyte size = 0, bool refresh = false) where V : IKeyable<K>
        {
            SELECT_(label, name, false, required, size, refresh);
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
                    if (key.Equals(v)) Add(" selected");
                    Add(">");
                    Add(e.ToString());
                    Add("</option>");
                }
            }
            _SELECT();
            return this;
        }

        public HtmlContent SELECT<K, V>(string label, string name, K[] v, V[] opt, bool required = false, sbyte size = 0, bool refresh = false) where V : IKeyable<K>
        {
            SELECT_(label, name, true, required, size, refresh);
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
            _SELECT();
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

        public HtmlContent OUTPUT<V>(string label, string name, V v)
        {
            if (label != null)
            {
                Add("<label class=\"uk-label\">");
                Add(label);
                Add("</label>");
            }
            Add("<output class=\"uk-output\" name=\"");
            Add(name);
            Add("\">");
            AddPrimitive(v);
            Add("</output>");
            return this;
        }

        public HtmlContent OUTPUT(string name, decimal v, string label = null)
        {
            if (label != null)
            {
                Add("<label class=\"uk-label\">");
                Add(label);
                Add("</label>");
            }
            Add("<output class=\"uk-output\" name=\"");
            Add(name);
            Add("\">¥");
            Add(v);
            Add("</output>");
            return this;
        }

        public HtmlContent METER()
        {
            Add("<meter>");
            Add("</meter>");
            return this;
        }

        public HtmlContent CROP(string name, string caption, short width, short height)
        {
            Add("<a class=\"uk-button uk-button-default uk-margin-small-bottom\" onclick=\"document.getElementById(\'imginp\').click()\">");
            Add(caption);
            Add("</a>");
            Add("<div id=\"imgbnd\" style=\"height: ");
            Add(height + 16);
            Add("px\">");
            Add("<input type=\"file\" id=\"imginp\" style=\"display: none;\" name=\"");
            Add(name);
            Add("\" onchange=\"bind(this.parentNode, window.URL.createObjectURL(this.files[0]), 0, ");
            Add(width);
            Add(',');
            Add(height);
            Add(");\"></div>");
            return this;
        }
    }
}