using System;

namespace SkyChain.Web
{
    /// <summary>
    /// For generating dynamic HTML5 content tooled with UiKit.
    /// </summary>
    public class HtmlContent : FormContent
    {
        public WebContext Web { get; set; }


        public HtmlContent(bool octet, int capacity = 32 * 1024) : base(octet, capacity)
        {
        }

        public override string Type { get; set; } = "text/html; charset=utf-8";

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

        public HtmlContent T(DateTime v, byte date = 3, byte time = 3)
        {
            Add(v, date, time);
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

        public HtmlContent S2<V>(V v, bool cond = true)
        {
            if (cond)
            {
                Add("<s>");
            }

            AddPrimitive(v);
            if (cond)
            {
                Add("</s>");
            }

            return this;
        }

        public HtmlContent S2<V, X>(V v, X x, bool cond = true)
        {
            if (cond)
            {
                Add("<s>");
            }

            AddPrimitive(v);
            Add("&nbsp;");
            AddPrimitive(x);
            if (cond)
            {
                Add("</s>");
            }

            return this;
        }


        public HtmlContent B_()
        {
            Add("<b>");
            return this;
        }

        public HtmlContent _B()
        {
            Add("</b>");
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

        public HtmlContent MARK<V>(V v)
        {
            Add("<mark>");
            AddPrimitive(v);
            Add("</mark>");
            return this;
        }

        public HtmlContent MARK_(string css = null)
        {
            Add("<mark");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }

            Add(">");
            return this;
        }

        public HtmlContent _MARK()
        {
            Add("</mark>");
            return this;
        }

        public HtmlContent ABBR<V>(string title, V v, string color = null)
        {
            Add("<abbr title=\"");
            Add(title);
            if (color != null)
            {
                Add("\" style=\"background-color: ");
                Add(color);
            }

            Add("\">");
            AddPrimitive(v);
            Add("</abbr>");
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


        public HtmlContent TH(string caption = null)
        {
            Add("<th>");
            if (caption != null)
            {
                Add(caption);
            }

            Add("</th>");
            return this;
        }

        public HtmlContent TH_(string css = null)
        {
            Add("<th");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }

            Add(">");
            return this;
        }

        public HtmlContent _TH()
        {
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

        public HtmlContent TD(short v, bool cond = true, bool right = true)
        {
            Add("<td style=\"text-align: ");
            Add(right ? "right" : "center");
            Add("\">");

            if (cond)
            {
                Add(v);
            }

            Add("</td>");
            return this;
        }

        public HtmlContent TD(int v, bool cond = true, bool right = true)
        {
            Add("<td style=\"text-align: ");
            Add(right ? "right" : "center");
            Add("\">");

            if (cond)
            {
                Add(v);
            }

            Add("</td>");
            return this;
        }

        public HtmlContent TD(long v, bool cond = true, bool right = true)
        {
            Add("<td style=\"text-align: ");
            Add(right ? "right" : "center");
            Add("\">");

            if (cond)
            {
                Add(v);
            }

            Add("</td>");
            return this;
        }

        public HtmlContent TD(decimal v, bool currency = false, bool cond = true)
        {
            Add("<td style=\"text-align: right\">");
            if (cond)
            {
                if (currency)
                {
                    Add('¥');
                }

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

        public HtmlContent TD(string v, bool cond = true)
        {
            Add("<td>");
            if (cond)
            {
                AddEsc(v);
            }

            Add("</td>");
            return this;
        }

        public HtmlContent TD2<V, X>(V v, X x, string css = null)
        {
            Add("<td");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }

            Add('>');
            AddPrimitive(v);
            Add("&nbsp;");
            AddPrimitive(x);
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

        public HtmlContent TDFORM(Action rowform)
        {
            if (rowform != null)
            {
                Add("<td style=\"text-align: right\">");
                Add("<form class=\"uk-button-group\">");
                rowform();
                Add("</form>");
                Add("</td>");
            }

            return this;
        }


        public HtmlContent LABEL(string caption, string css = null)
        {
            if (caption != null)
            {
                Add("<label class=\"uk-label");
                if (css != null)
                {
                    Add(' ');
                    Add(css);
                }

                Add("\">");
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

        public HtmlContent SPAN2<A, B>(A a, B b, string css = null)
        {
            SPAN_(css);
            AddPrimitive(a);
            Add("&nbsp;");
            AddPrimitive(b);
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

        public HtmlContent P2<A, B>(A a, B b, string css = null)
        {
            Add("<p");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }

            Add(">");
            AddPrimitive(a);
            Add("&nbsp;");
            AddPrimitive(b);
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

        public HtmlContent MAIN_(string css = null, bool grid = false)
        {
            Add("<main");
            if (grid)
            {
                Add(" uk-grid class=\"uk-child-width-1-1 uk-child-width-1-2@s uk-child-width-1-3@m uk-child-width-1-4@l uk-child-width-1-5@xl");
                if (css != null)
                {
                    Add(' ');
                    Add(css);
                }
                Add("\"");
            }
            else if (css != null)
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

        public HtmlContent OL_(string css = null)
        {
            Add("<ol");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }

            Add(">");
            return this;
        }

        public HtmlContent _OL()
        {
            Add("</ol>");
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

        public HtmlContent DT(string v)
        {
            Add("<dt>");
            AddEsc(v);
            Add("</dt>");
            return this;
        }


        public HtmlContent DD(string v)
        {
            Add("<dd>");
            AddEsc(v);
            Add("</dd>");
            return this;
        }

        public HtmlContent FIELD<V>(string label, V p)
        {
            LABEL(label);
            Add("<p>");
            AddPrimitive(p);
            Add("</p>");
            return this;
        }

        public HtmlContent FIELD2<V, X>(string label, V p, X x)
        {
            LABEL(label);
            Add("<p>");
            AddPrimitive(p);
            Add("&nbsp;");
            AddPrimitive(x);
            Add("</p>");
            return this;
        }

        public HtmlContent FIELD(string label, decimal p, bool currency = false)
        {
            LABEL(label);
            Add("<p>");
            if (currency)
            {
                Add('¥');
            }

            Add(p);
            Add("</p>");
            return this;
        }

        public HtmlContent FIELD(string label, bool v)
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

        public HtmlContent FIELD<K, V>(string label, K[] keys, Map<K, V> map)
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

        public HtmlContent CNY(decimal v, bool em = false, bool s = false)
        {
            Add('¥');
            if (em)
            {
                Add("<em>");
            }

            if (s)
            {
                Add("<s>");
            }

            Add(v);
            if (s)
            {
                Add("</s>");
            }

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

        public HtmlContent A_HREF_(string a, string css = null, bool end = false)
        {
            Add("<a");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(" href=\"");
            AddEsc(a);
            Add("\"");
            if (end)
            {
                Add(">");
            }
            return this;
        }

        public HtmlContent A_HREF_<B>(string a, B b, string css = null, bool end = false)
        {
            Add("<a");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(" href=\"");
            AddEsc(a);
            AddPrimitive(b);
            Add("\"");
            if (end)
            {
                Add(">");
            }
            return this;
        }

        public HtmlContent A_HREF_<B, C>(string a, B b, C c, string css = null, bool end = false)
        {
            Add("<a");
            if (css != null)
            {
                Add(" class=\"");
                Add(css);
                Add("\"");
            }
            Add(" href=\"");
            AddEsc(a);
            AddPrimitive(b);
            AddPrimitive(c);
            Add("\"");
            if (end)
            {
                Add(">");
            }
            return this;
        }

        public HtmlContent _ONCLICK_(string a)
        {
            Add(" onclick=\"");
            AddEsc(a);
            Add("\">");
            return this;
        }

        public HtmlContent _ONCLICK_<B>(string a, B b)
        {
            Add(" onclick=\"");
            AddEsc(a);
            AddPrimitive(b);
            Add("\">");
            return this;
        }

        public HtmlContent _ONCLICK_<B, C>(string a, B b, C c)
        {
            Add(" onclick=\"");
            AddEsc(a);
            AddPrimitive(b);
            AddPrimitive(c);
            Add("\">");
            return this;
        }


        public HtmlContent _A()
        {
            Add("</a>");
            return this;
        }

        public HtmlContent PIC_(string css = null, bool circle = false)
        {
            Add("<div class=\"uk-margin-auto-vertical");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }

            Add("\"><img style=\"width: 100%\"");
            if (circle)
            {
                Add(" class=\"uk-border-circle\"");
            }

            Add(" src=\"");
            return this;
        }

        public HtmlContent _PIC()
        {
            Add("\"></div>");
            return this;
        }

        public HtmlContent PIC(string a, string css = null, bool circle = false)
        {
            PIC_(css, circle);
            AddEsc(a);
            _PIC();
            return this;
        }

        public HtmlContent PIC<B>(string a, B b, string css = null, bool circle = false)
        {
            PIC_(css, circle);
            AddEsc(a);
            AddPrimitive(b);
            _PIC();
            return this;
        }

        public HtmlContent PIC<B, C>(string a, B b, C c, string css = null, bool circle = false)
        {
            PIC_(css, circle);
            AddEsc(a);
            AddPrimitive(b);
            AddPrimitive(c);
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
            Add("<div class=\"uk-qrcode uk-flex-center");
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

        public HtmlContent ALERT(string p, string head = null, string css = null, bool close = false)
        {
            ALERT_(css, close);
            if (head != null)
            {
                Add("<h4>");
                Add(head);
                Add("</h4>");
            }

            Add("<p>");
            Add(p);
            Add("</p>");
            _ALERT();
            return this;
        }

        public HtmlContent SUBNAV(string[] items, string uri, int subscript)
        {
            Add("<ul class=\"uk-subnav uk-subnav-pill\" uk-sticky>");
            for (int i = 0; i < items.Length; i++)
            {
                Add("<li");
                if (i == subscript)
                {
                    Add(" class=\"uk-active\"");
                }

                Add("><a href=\"");
                Add(uri);
                Add('-');
                Add(i);
                Add("\">");
                Add(items[i]);
                Add("</a></li>");
            }

            Add("</ul>");
            return this;
        }

        public HtmlContent FORM_(string css = null, string action = null, bool post = true, bool mp = false, string oninput = null, string onsubmit = null)
        {
            Add("<form class=\"");
            if (css != null)
            {
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
        public HtmlContent FIELDSUL_(string legend = null, string css = null)
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

        public HtmlContent _FIELDSUL()
        {
            Add("</ul>");
            Add("</fieldset>");
            return this;
        }

        public HtmlContent BUTTON_(string action, string css = null)
        {
            Add("<button type=\"submit\" class=\"");
            Add(css);
            Add(" formaction=\"");
            Add(action);
            Add("\">");
            return this;
        }

        public HtmlContent _BUTTON()
        {
            Add("</button>");
            return this;
        }

        public HtmlContent BUTTON(string caption, string action = null, int subscript = -1, bool post = true, string css = "uk-button-default")
        {
            Add("<button type=\"submit\" class=\"uk-button ");
            Add(css);
            Add("\" formmethod=\"");
            Add(post ? "post" : "get");
            if (action != null)
            {
                Add("\" formaction=\"");
                Add(action);
            }

            if (subscript > -1)
            {
                Add('-');
                Add(subscript);
            }

            Add("\" onclick=\"btnSubmit(this);\">");
            AddEsc(caption);
            Add("</button>");
            return this;
        }

        public void PAGENATION(bool more, int begin = 0, int step = 1)
        {
            var act = Web.Action;
            if (act.Subscript != null)
            {
                Add("<ul class=\"uk-pagination uk-flex-center\">");

                int page = Web.Subscript;
                if (page > begin)
                {
                    Add("<li class=\"uk-active\">");
                    Add("<a href=\"");
                    Add(act.Key);
                    Add('-');
                    Add(page - step);
                    Add(Web.QueryStr);
                    Add("\">≪</a>");
                    Add("</li>");
                }
                else
                {
                    Add("<li class=\"uk-disabled\">≪</li>");
                }

                if (more)
                {
                    Add("<li class=\"uk-active\">");
                    Add("<a href=\"");
                    Add(act.Key);
                    Add('-');
                    Add(page + step);
                    Add(Web.QueryStr);
                    Add("\">≫</a>");
                    Add("</li>");
                }
                else
                {
                    Add("<li class=\"uk-disabled\">≫</li>");
                }

                Add("</ul>");
            }
        }

        public HtmlContent LIST<M>(M[] arr, Action<M> item, string ul = null, string li = null)
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
                for (int i = 0; i < arr.Length; i++)
                {
                    M obj = arr[i];
                    Add("<li class=\"uk-flex");
                    if (li != null)
                    {
                        Add(' ');
                        Add(li);
                    }

                    Add("\">");
                    item(obj);
                    Add("</li>");
                }
            }

            Add("</ul>");
            return this;
        }

        public void LIST<M, K>(Map<K, M> map, Action<Map<K, M>.Entry> item, string ul = null, string li = null)
        {
            Add("<ul class=\"uk-list");
            if (ul != null)
            {
                Add(' ');
                Add(ul);
            }

            Add("\">");

            if (map != null)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    var ety = map.EntryAt(i);
                    Add("<li class=\"");
                    if (li != null)
                    {
                        Add(' ');
                        Add(li);
                    }

                    Add("\">");
                    item(ety);
                    Add("</li>");
                }
            }

            Add("</ul>");
        }

        public HtmlContent LIST<S>(S src, Action<S> item, string ul = null, string li = null) where S : ISource
        {
            Add("<ul class=\"uk-list uk-list-divider");
            if (ul != null)
            {
                Add(' ');
                Add(ul);
            }

            Add("\">");

            if (src != null && src.IsDataSet)
            {
                while (src.Next())
                {
                    Add("<li class=\"uk-flex");
                    if (li != null)
                    {
                        Add(' ');
                        Add(li);
                    }

                    Add("\">");
                    item(src);
                    Add("</li>");
                }
            }

            Add("</ul>");
            return this;
        }

        public HtmlContent ACCORDION<M>(M[] arr, Action<M> item, string ul = null, string li = "uk-card-default")
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
                for (int i = 0; i < arr.Length; i++)
                {
                    M obj = arr[i];
                    Add("<li class=\"uk-flex uk-card");
                    if (li != null)
                    {
                        Add(' ');
                        Add(li);
                    }

                    Add("\">");
                    item(obj);
                    Add("</li>");
                }
            }

            // pagination if any
            Add("</ul>");
            return this;
        }

        public HtmlContent ACCORDION<K, M>(Map<K, M> map, Action<Map<K, M>.Entry> card, string ul = null, string li = "uk-card-default")
        {
            Add("<ul uk-accordion=\"multiple: true\" class=\"");
            if (ul != null)
            {
                Add(' ');
                Add(ul);
            }

            Add("\">");

            if (map != null)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    var ety = map.EntryAt(i);
                    Add("<li class=\"uk-card");
                    if (li != null)
                    {
                        Add(' ');
                        Add(li);
                    }

                    Add("\">");
                    card(ety);
                    Add("</li>");
                }
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

        public HtmlContent TDCHECK<K>(K key, bool toolbar = true)
        {
            Add("<td style=\"width: 1%\"><input");
            if (toolbar)
            {
                Add(" form=\"tool-bar-form\"");
            }

            Add(" name=\"key\" type=\"checkbox\" class=\"uk-checkbox\" value=\"");
            PutKey(key);
            Add("\" onchange=\"checkToggle(this);\">");
            Add("</td>");
            return this;
        }

        public HtmlContent TDRADIO<K>(K key, bool toolbar = true)
        {
            Add("<td style=\"width: 1%\"><input");
            if (toolbar)
            {
                Add(" form=\"tool-bar-form\"");
            }

            Add(" name=\"key\" type=\"radio\" class=\"uk-radio\" value=\"");
            PutKey(key);
            Add("\"></td>");
            return this;
        }


        public void TABLE<M>(M[] arr, Action<M> tr, Action thead = null, short height = 0)
        {
            Add("<div ");
            if (height > 0)
            {
                Add("style=\"overflow-y: scroll; height: ");
                Add(height);
                Add("px;\">");
            }
            else
            {
                Add("class=\"uk-overflow-auto\">");
            }

            Add("<table class=\"uk-table uk-table-hover uk-table-divider\">");

            if (arr != null && tr != null) // tbody if having data objects
            {
                Add("<tbody>");
                thead?.Invoke();

                for (int i = 0; i < arr.Length; i++)
                {
                    var obj = arr[i];
                    Add("<tr>");
                    tr(obj);
                    Add("</tr>");
                }

                Add("</tbody>");
            }

            Add("</table>");
            Add("</div>");
        }

        public void TABLE<K, M>(Map<K, M> arr, Action<Map<K, M>.Entry> tr, Action thead = null, short height = 0)
        {
            Add("<div ");
            if (height > 0)
            {
                Add("style=\"overflow-y: scroll; height: ");
                Add(height);
                Add("px;\">");
            }
            else
            {
                Add("class=\"uk-overflow-auto\">");
            }

            Add("<table class=\"uk-table uk-table-hover uk-table-divider\">");

            if (arr != null && tr != null) // tbody if having data objects
            {
                Add("<tbody>");
                thead?.Invoke();

                for (int i = 0; i < arr.Count; i++)
                {
                    var obj = arr.EntryAt(i);
                    Add("<tr>");
                    tr(obj);
                    Add("</tr>");
                }
                Add("</tbody>");
            }
            Add("</table>");
            Add("</div>");
        }

        public void BOARD<M>(M[] arr, Action<M> card, string css = "uk-card-default")
        {
            Add("<main class=\"uk-board\">");
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    M obj = arr[i];
                    Add("<form class=\"uk-card");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
                    }

                    Add("\">");
                    card(obj);
                    Add("</form>");
                }
            }

            Add("</main>");
        }

        public void BOARD<M, K>(Map<K, M> map, Action<Map<K, M>.Entry> card, string css = "uk-card-default")
        {
            Add("<main class=\"uk-board\">");
            if (map != null)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    var ety = map.EntryAt(i);
                    Add("<form class=\"uk-card");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
                    }

                    Add("\">");
                    card(ety);
                    Add("</form>");
                }
            }

            Add("</main>");
        }

        public void BOARD<S>(S src, Action<S> card, string css = "uk-card-default") where S : ISource
        {
            Add("<main class=\"uk-board\">");
            if (src != null && src.IsDataSet)
            {
                while (src.Next())
                {
                    Add("<form class=\"uk-card");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
                    }

                    Add("\">");
                    card(src);
                    Add("</form>");
                }
            }

            Add("</main>");
        }

        public void GRID<M>(M[] arr, Action<M> card, string css = "uk-card-default")
        {
            Add("<main uk-grid class=\"uk-child-width-1-1 uk-child-width-1-2@m uk-child-width-1-3@l uk-child-width-1-4@xl uk-child-width-1-5@xxl\">");
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    M obj = arr[i];
                    Add("<div>");
                    Add("<form class=\"uk-card");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
                    }

                    Add("\">");
                    card(obj);
                    Add("</form>");
                    Add("</div>");
                }
            }

            Add("</main>");
        }

        public void GRID<K, M>(Map<K, M> map, Action<Map<K, M>.Entry> card, string css = "uk-card-default")
        {
            Add("<main uk-grid class=\"uk-child-width-1-1 uk-child-width-1-2@s uk-child-width-1-3@m uk-child-width-1-4@l uk-child-width-1-5@xl\">");
            if (map != null)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    var ety = map.EntryAt(i);
                    Add("<div>");
                    Add("<form class=\"uk-card");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
                    }

                    Add("\">");
                    card(ety);
                    Add("</form>");
                    Add("</div>");
                }
            }

            Add("</main>");
        }

        public void GRID<S>(S src, Action<S> card, string css = null) where S : ISource
        {
            Add("<main uk-grid class=\"uk-child-width-1-1 uk-child-width-1-2@s uk-child-width-1-3@m uk-child-width-1-4@l uk-child-width-1-5@xl\">");
            if (src != null && src.IsDataSet)
            {
                while (src.Next())
                {
                    Add("<div>");
                    Add("<article class=\"uk-card uk-card-default");
                    if (css != null)
                    {
                        Add(' ');
                        Add(css);
                    }

                    Add("\">");
                    card(src);
                    Add("</article>");
                    Add("</div>");
                }
            }

            Add("</main>");
        }

        void OnClickDialog(byte mode, bool pick, Appear size, string tip)
        {
            Add(" onclick=\"return dialog(this,");
            Add(mode);
            Add(",");
            Add(pick);
            Add(",");
            Add((byte) size);
            Add(",'");
            Add(tip);
            Add("');\"");
        }

        public HtmlContent TABBEDBAR(bool group = true)
        {
            var wrk = Web.Work;
            var wrks = wrk.Parent.Works;

            Add("<form class=\"uk-top-bar uk-flex-center\">");
            Add("<ul class=\"uk-subnav\">");

            for (int i = 0; i < wrks.Count; i++)
            {
                var w = wrks.ValueAt(i);
                if (group && w.Group != wrk.Group) continue;

                // render tabs
                Add("<li");
                if (w == wrk)
                {
                    Add(" class=\"uk-active\"");
                }
                Add("><a href=\"../");
                Add(w.Key);
                Add("/\">");
                Add(w.Label);
                Add("</a></li>");
            }

            Add("</ul>");

            Add("</form>");
            Add("<div class=\"uk-top-placeholder\"></div>");
            return this;
        }


        public HtmlContent TOOLBAR(byte group = 0, int subscript = -1, bool toggle = false, string caption = null, string rangekey = null, bool refresh = true, bool top = true)
        {
            byte ctxgrp = group > 0 ? group : Web.Action.Group; // the contextual group

            Add("<form id=\"tool-bar-form\" class=\"");
            Add(top ? "uk-top-bar" : "uk-bottom-bar");
            Add("\">");
            Add("<div class=\"uk-button-group\">");
            if (toggle)
            {
                Add("<input type=\"checkbox\" class=\"uk-checkbox\" onchange=\"return toggleAll(this);\">&nbsp;");
            }

            var acts = Web.Work.Tooled;
            if (acts != null)
            {
                for (int i = 0; i < acts.Length; i++)
                {
                    var act = acts[i];
                    int g = act.Group;
                    var tool = act.Tool;
                    if (tool.IsAnchorTag || ctxgrp == g || (g & ctxgrp) > 0)
                    {
                        // provide the state about current anchor as subscript 
                        PutTool(act, tool, tool.IsAnchorTag ? -1 : subscript, css: "uk-button-primary");
                    }
                }
            }

            Add("</div>");

            Add("<section class=\"uk-flex uk-flex-middle\">");
            if (caption != null)
            {
                Add(caption);
            }

            if (rangekey != null)
            {
                Add("<input name=\"rangekey\" type=\"hidden\" value=\"");
                Add(rangekey);
                Add("\">");
            }

            if (refresh)
            {
                Add("<a class=\"uk-icon-button\" href=\"javascript: location.reload(false);\" uk-icon=\"refresh\"></a>");
            }

            Add("</section>");

            Add("</form>");
            Add("<div class=\"");
            Add(top ? "uk-top-placeholder" : "uk-bottom-placeholder");
            Add("\"></div>");
            return this;
        }

        public HtmlContent BOTTOMBAR_(string css = null)
        {
            Add("<div class=\"uk-bottom-placeholder\"></div>");
            Add("<footer class=\"uk-bottom-bar");
            if (css != null)
            {
                Add(' ');
                Add(css);
            }

            Add("\">");
            return this;
        }

        public HtmlContent _BOTTOMBAR()
        {
            Add("</footer>");
            return this;
        }


        public HtmlContent PICK<K>(K varkey, string label = null)
        {
            Add("<label>");
            Add("<input form=\"tool-bar-form\" name=\"key\" type=\"checkbox\" class=\"uk-checkbox\" value=\"");
            PutKey(varkey);
            Add("\" onchange=\"checkToggle(this);\">");
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent VARTOOLS<K>(K varkey, int subscript = -1, string pick = null, string css = null, bool nav = true)
        {
            if (nav)
            {
                Add("<nav class=\"uk-flex uk-button-group");
                if (css != null)
                {
                    Add(' ');
                    Add(css);
                }

                Add("\">");
            }

            var w = Web.Work;
            var vw = w.VarWork;

            // output a pick checkbox
            if (vw != null && pick != null)
            {
                PICK(varkey, pick);
            }

            // output button group
            byte actgrp = Web.Action.Group;
            var acts = vw?.Tooled;
            if (acts != null)
            {
                for (int i = 0; i < acts.Length; i++)
                {
                    var act = acts[i];
                    int g = act.Group;
                    if (g == actgrp || (g & actgrp) > 0)
                    {
                        var tool = act.Tool;
                        PutVarTool(act, tool, varkey, tool.IsAnchorTag ? -1 : subscript, null, null, true, "uk-button-secondary");
                    }
                }
            }

            if (nav)
            {
                Add("</nav>");
            }

            return this;
        }

        public HtmlContent TOOL(string action, int subscript = -1, string caption = null, string tip = null, ToolAttribute tool = null, bool enabled = true, string css = "uk-button-primary")
        {
            // locate the proper work
            var w = Web.Work;
            var act = w[action];
            var toola = tool ?? act?.Tool;
            if (toola != null)
            {
                PutTool(act, toola, subscript, caption, tip, enabled, css);
            }

            return this;
        }

        public HtmlContent VARTOOL<K>(K varkey, string action, int subscript = -1, string caption = null, string tip = null, ToolAttribute tool = null, bool enabled = true, string css = "uk-button-secondary")
        {
            // get the var work
            var vw = Web.Work.VarWork;
            if (vw != null)
            {
                var act = vw[action];
                var toola = tool ?? act?.Tool;
                if (toola != null)
                {
                    PutVarTool(act, toola, varkey, subscript, caption, tip, enabled, css);
                }
            }

            return this;
        }

        public HtmlContent CROPPIE(short wid, short hei, string caption, bool large = false)
        {
            Add("<main id=\"imgbnd\" class=\"");
            Add(large ? "uk-height-large" : "uk-height-medium");
            Add("\"><input class=\"uk-button uk-button-secondary\" type=\"file\" id=\"imginp\" style=\"display: none;\" required onchange=\"bind(this.parentNode, window.URL.createObjectURL(this.files[0]),");
            Add(wid);
            Add(", ");
            Add(hei);
            Add(");\">");
            Add("<input type=\"hidden\" id=\"img\" name=\"img\">");
            Add("</main>");
            Add("<hr>");
            Add("<div class=\"uk-card-footer uk-flex-center\">");
            Add("<button type=\"button\" class=\"uk-button uk-button-secondary\" onclick=\"$('#imginp').click(); return;\">");
            Add(caption);
            Add("</button>");
            Add("</div>");

            return this;
        }

        void PutKey<K>(K k)
        {
            if (k is short shortv) Add(shortv);
            else if (k is int intv) Add(intv);
            else if (k is long longv) Add(longv);
            else if (k is string strv) Add(strv);
            else if (k is DateTime dtv) Add(dtv);
            else if (k is ValueTuple<short, short> k11)
            {
                Add(k11.Item1);
                Add('-');
                Add(k11.Item2);
            }
            else if (k is ValueTuple<int, int> k22)
            {
                Add(k22.Item1);
                Add('-');
                Add(k22.Item2);
            }
            else if (k is ValueTuple<string, string> k33)
            {
                Add(k33.Item1);
                Add('-');
                Add(k33.Item2);
            }
            else if (k is ValueTuple<string, int> k32)
            {
                Add(k32.Item1);
                Add('-');
                Add(k32.Item2);
            }
            else if (k is ValueTuple<int, string> k23)
            {
                Add(k23.Item1);
                Add('-');
                Add(k23.Item2);
            }
        }

        void PutTool(WebAction act, ToolAttribute tool, int subscript = -1, string caption = null, string tip = null, bool enabled = true, string css = null)
        {
            // check action's availability
            //
            bool ok = enabled && (Web.Principal == null || act.DoAuthorize(Web));
            tip ??= act.Tip;

            if (tool.IsAnchorTag)
            {
                Add("<a class=\"uk-button ");
                Add(css ?? "uk-button-link");
                if (act == Web.Action) // if current action
                {
                    Add(" uk-active");
                }

                if (!ok)
                {
                    Add(" disabled");
                }

                Add("\" href=\"");
                Add(act == Web.Action ? act.Key : act.Relative);
                if (subscript != -1 && act.Subscript != null)
                {
                    Add('-');
                    Add(subscript);
                }

                Add("\"");
            }
            else
            {
                Add("<button class=\"uk-button ");
                Add(css ?? "uk-button-default");
                Add("\" name=\"");
                Add(act.Key);
                Add("\" formaction=\"");
                Add(act.Key);
                if (subscript != -1 && act.Subscript != null)
                {
                    Add('-');
                    Add(subscript);
                }

                Add("\"");
                if (tool.IsPost)
                {
                    Add(" formmethod=\"post\"");
                }
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
                Add(tip ?? act.Label);
                Add("');\"");
            }
            else if (tool.HasPrompt)
            {
                OnClickDialog(2, tool.MustPick, tool.Size, tip);
            }
            else if (tool.HasShow)
            {
                OnClickDialog(4, tool.MustPick, tool.Size, tip);
            }
            else if (tool.HasOpen)
            {
                OnClickDialog(8, tool.MustPick, tool.Size, tip);
            }
            else if (tool.HasScript)
            {
                Add(" onclick=\"return by"); // prefix to avoid js naming conflict
                Add(act.Name);
                Add("(this);\"");
            }
            else if (tool.HasCrop)
            {
                Add(" onclick=\"return crop(this,");
                Add((byte) tool.Size);
                Add(",'");
                Add(tip);
                Add("');\"");
            }

            Add(">");

            string cap = caption ?? act.Label;
            if (!string.IsNullOrEmpty(cap))
            {
                Add(cap);
            }

            // put the closing tag
            Add(tool.IsAnchorTag ? "</a>" : "</button>");
        }

        void PutVarTool<K>(WebAction act, ToolAttribute tool, K varkey, int subscript = -1, string caption = null, string tip = null, bool enabled = true, string css = null)
        {
            // check action's availability
            //
            bool ok = enabled && (Web.Principal == null || act.DoAuthorize(Web));
            tip ??= act.Tip;

            if (tool.IsAnchorTag)
            {
                Add("<a class=\"uk-button ");
                Add(css ?? "uk-button-link");
                if (act == Web.Action) // if current action
                {
                    Add(" uk-active");
                }

                if (!ok)
                {
                    Add(" disabled");
                }

                Add("\" href=\"");
                PutKey(varkey);
                Add('/');
                Add(act == Web.Action ? act.Key : act.Relative);
                if (subscript != -1 && act.Subscript != null)
                {
                    Add('-');
                    Add(subscript);
                }

                Add("\"");
            }
            else
            {
                Add("<button  class=\"uk-button ");
                Add(css ?? "uk-button-default");
                Add("\" name=\"");
                Add(act.Key);
                Add("\" formaction=\"");
                PutKey(varkey);
                Add('/');
                Add(act.Key);
                if (subscript != -1 && act.Subscript != null)
                {
                    Add('-');
                    Add(subscript);
                }

                Add("\"");
                if (tool.IsPost)
                {
                    Add(" formmethod=\"post\"");
                }
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
                Add(tip ?? act.Label);
                Add("');\"");
            }
            else if (tool.HasPrompt)
            {
                OnClickDialog(2, tool.MustPick, tool.Size, tip);
            }
            else if (tool.HasShow)
            {
                OnClickDialog(4, tool.MustPick, tool.Size, tip);
            }
            else if (tool.HasOpen)
            {
                OnClickDialog(8, tool.MustPick, tool.Size, tip);
            }
            else if (tool.HasScript)
            {
                Add(" onclick=\"return by"); // prefix to avoid js naming conflict
                Add(act.Name);
                Add("(this);\"");
            }
            else if (tool.HasCrop)
            {
                Add(" onclick=\"return crop(this,");
                Add((byte) tool.Size);
                Add(",'");
                Add(tip);
                Add("');\"");
            }

            Add(">");

            string cap = caption ?? act.Label;
            if (!string.IsNullOrEmpty(cap))
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

        public HtmlContent HIDDEN(string name, string val)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(val);
            Add("\">");
            return this;
        }

        public HtmlContent HIDDENS<V>(string name, V[] vals)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                Add("<input type=\"hidden\" name=\"");
                Add(name);
                Add("\" value=\"");
                AddPrimitive(vals[i]);
                Add("\">");
            }
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
            Add("<input type=\"date\" class=\"uk-width-1-1\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(val, 3, 0);
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

        public HtmlContent DATETIME(string label, string name, DateTime val, DateTime max = default, DateTime min = default, bool @readonly = false, bool required = false, int step = 0)
        {
            LABEL(label);
            Add("<input type=\"datetime-local\" class=\"uk-width-1-1\" name=\"");
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

        public HtmlContent RANGE<V>(string label, string name, V v, V max, V min, V step, bool @readonly = false)
        {
            LABEL(label);
            Add("<input type=\"range\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
            Add("\" max=\"");
            AddPrimitive(max);
            Add("\" min=\"");
            AddPrimitive(min);
            Add("\" step=\"");
            AddPrimitive(step);
            Add("\"");
            if (@readonly) Add(" readonly");
            Add(">");
            return this;
        }

        public HtmlContent COLOR(string label, string name, string v, bool @readonly = false, bool required = false)
        {
            LABEL(label);
            Add("<input type=\"color\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");
            return this;
        }

        public HtmlContent CHECKBOX(string label, string name, bool check, string tip = null, bool required = false)
        {
            LABEL(label);
            if (tip != null)
            {
                Add("<label class=\"uk-flex uk-input uk-flex-middle\">");
            }

            Add("<input type=\"checkbox\" class=\"uk-checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (check) Add(" checked");
            if (required) Add(" required");
            Add(">&nbsp;");
            if (tip != null)
            {
                Add(tip); // caption following the checkbox
                Add("</label>");
            }

            return this;
        }

        public HtmlContent CHECKBOX<V>(string label, string name, V v, bool check, string tip = null, bool required = false, bool disabled = false)
        {
            LABEL(label);
            if (tip != null)
            {
                Add("<label class=\"uk-flex uk-input uk-flex-middle\">");
            }

            Add("<input type=\"checkbox\" class=\"uk-checkbox\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
            Add("\"");
            if (check) Add(" checked=\"checked\"");
            if (required) Add("\" required=\"required\"");
            if (disabled) Add("\" disabled=\"disabled\"");
            Add(">&nbsp;");
            if (tip != null)
            {
                Add(tip); // caption following the checkbox
                Add("</label>");
            }

            return this;
        }


        public HtmlContent CHECKBOXSET(string name, string[] v, string[] opt, string legend = null, string css = null)
        {
            FIELDSUL_(legend, css);
            for (int i = 0; i < opt.Length; i++)
            {
                var e = opt[i];
                Add(" <label>");
                Add("<p class=\"uk-flex uk-input\"><input type=\"checkbox\" name=\"");
                Add(name);
                Add("\"");
                if (v != null && v.Contains(e))
                {
                    Add(" checked");
                }

                Add("></p>");
                Add(e);
                Add(" </label>");
            }

            _FIELDSUL();
            return this;
        }

        public HtmlContent RADIO<V>(string name, V v, string label = null, bool @checked = false, bool required = false, bool disabled = false, string tip = null)
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
            if (tip != null)
            {
                Add("&nbsp;&nbsp;");
                Add("<span class=\"uk-text-small\">");
                Add(tip);
                Add("</span>");
            }
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
            FIELDSUL_(legend, css);
            if (opt != null)
            {
                lock (opt)
                {
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.EntryAt(i);
                        if (filter != null && !filter(e.Value)) continue;
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

            _FIELDSUL();
            return this;
        }

        public HtmlContent RADIOSET2<K, V>(string name, K v, Map<K, V> opt = null, string legend = null, string css = null, bool required = false, Func<K, V, bool> filter = null)
        {
            FIELDSUL_(legend, css);
            if (opt != null)
            {
                lock (opt)
                {
                    bool odd = true;
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.EntryAt(i);
                        if (filter != null && !filter(e.key, e.Value)) continue;
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

            _FIELDSUL();
            return this;
        }

        public HtmlContent RADIOSET(string name, string v, string[] opt, string legend = null, string css = null, bool required = false)
        {
            if (legend != null)
            {
                FIELDSUL_(legend, css);
            }

            for (int i = 0; i < opt.Length; i++)
            {
                var o = opt[i];
                if (i > 0) Add("&nbsp;&nbsp;");
                RADIO(name, o, o, o.Equals(v));
            }

            if (legend != null)
            {
                _FIELDSUL();
            }

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
                Add(max <= 30 ? 2 :
                    max <= 60 ? 3 :
                    max <= 100 ? 4 :
                    max <= 150 ? 5 :
                    max <= 200 ? 6 :
                    max <= 250 ? 7 :
                    max <= 300 ? 8 :
                    max <= 400 ? 10 :
                    12
                );
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

        public HtmlContent SELECT_<V>(string label, V name, bool multiple = false, bool required = true, int size = 0, bool rtl = false, bool refresh = false, string css = null)
        {
            LABEL(label);
            Add("<select class=\"uk-select");
            if (rtl)
            {
                Add(" uk-select-right");
            }

            if (css != null)
            {
                Add(' ');
                Add(css);
            }

            Add("\" name=\"");
            AddPrimitive(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required)
            {
                Add(" value=\"\" required");
            }
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
            if (!required)
            {
                Add("<option style=\"display:none\" selected></option>");
            }
            return this;
        }

        public HtmlContent _SELECT()
        {
            Add("</select>");
            return this;
        }

        public HtmlContent OPTION<T>(T v, string caption = null, bool selected = false, bool enabled = true)
        {
            Add("<option value=\"");
            AddPrimitive(v);

            Add("\"");
            if (selected) Add(" selected");
            if (!enabled) Add(" disabled");
            Add(">");
            if (caption != null)
            {
                Add(caption);
            }
            else
            {
                AddPrimitive(v);
            }

            Add("</option>");
            return this;
        }

        public HtmlContent OPTION_<T>(T v, bool selected = false, bool enabled = true)
        {
            Add("<option value=\"");
            AddPrimitive(v);
            Add("\"");
            if (selected) Add(" selected");
            if (!enabled) Add(" disabled");
            Add(">");
            return this;
        }

        public HtmlContent _OPTION()
        {
            Add("</option>");
            return this;
        }

        public HtmlContent OPTGROUP_<T>(T v)
        {
            Add("<optgroup label=\"");
            AddPrimitive(v);
            Add("\">");
            return this;
        }

        public HtmlContent _OPTGROUP()
        {
            Add("</optgroup>");
            return this;
        }

        public HtmlContent SELECT<K, V>(string label, string name, K v, Map<K, V> opt, string tip = null, bool multiple = false, bool required = true, sbyte size = 0, bool rtl = false, bool refresh = false, Func<K, V, bool> filter = null)
        {
            SELECT_(label, name, false, required, size, rtl, refresh);
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
                bool grpopen = false;
                for (int i = 0; i < opt.Count; i++)
                {
                    var e = opt.EntryAt(i);
                    if (filter != null && !filter(e.key, e.Value)) continue;
                    if (e.IsHead)
                    {
                        if (grpopen)
                        {
                            Add("</optgroup>");
                            grpopen = false;
                        }

                        Add("<optgroup label=\"");
                        AddPrimitive(e.Value);
                        Add("\">");
                        grpopen = true;
                    }
                    else
                    {
                        var key = e.Key;
                        Add("<option value=\"");
                        AddPrimitive(key);
                        Add("\"");
                        if (key.Equals(v)) Add(" selected");
                        Add(">");
                        AddPrimitive(e.Value);
                        Add("</option>");
                    }
                }

                if (grpopen)
                {
                    Add("</optgroup>");
                    grpopen = false;
                }
            }
            _SELECT();
            return this;
        }


        public HtmlContent SELECT<K, V>(string label, string name, K[] v, Map<K, V> opt, bool required = true, sbyte size = 0, bool refresh = false)
        {
            SELECT_(label, name, true, required, size, refresh);
            if (opt != null)
            {
                lock (opt)
                {
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.EntryAt(i);
                        var key = e.Key;
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
                        AddPrimitive(e.Value);
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

        public HtmlContent DATALIST(string id, string[] opt)
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
            Add("\" onchange=\"bind(this.parentNode, window.URL.createObjectURL(this.files[0]),");
            Add(width);
            Add(',');
            Add(height);
            Add(");\"></div>");
            return this;
        }
    }
}