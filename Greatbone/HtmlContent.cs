using System;

namespace Greatbone
{
    /// <summary>
    /// For dynamic HTML5 content tooled with Zurb Foundation
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


        public HtmlContent T(char v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(short v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(int v, sbyte fix = 0)
        {
            if (fix > 0)
            {
            }
            else
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent T(long v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(DateTime v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(decimal v)
        {
            Add(v);
            return this;
        }

        public HtmlContent RMB(decimal v)
        {
            Add('¥');
            Add(v);
            return this;
        }

        public HtmlContent T(double v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(string v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(string[] v)
        {
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(", ");
                    Add(v[i]);
                }
            }
            return this;
        }

        public HtmlContent _T(short v)
        {
            Add(' ');
            Add(v);
            return this;
        }

        public HtmlContent _T(int v)
        {
            Add(' ');
            Add(v);
            return this;
        }

        public HtmlContent _T(long v)
        {
            Add(' ');
            Add(v);
            return this;
        }

        public HtmlContent _T(double v)
        {
            Add(' ');
            Add(v);
            return this;
        }

        public HtmlContent _T(DateTime v)
        {
            Add(' ');
            Add(v);
            return this;
        }

        public HtmlContent IF_<V>(V v) where V : IEquatable<V>
        {
            if (v.Equals(default))
            {
                AddPrimitive(v);
                Add(' ');
            }
            return this;
        }

        public HtmlContent IF<V>(V v) where V : IEquatable<V>
        {
            if (v.Equals(default))
            {
                Add(' ');
                AddPrimitive(v);
            }
            return this;
        }

        public HtmlContent _IF<V>(V v) where V : IEquatable<V>
        {
            if (v.Equals(default))
            {
                Add(' ');
                AddPrimitive(v);
            }
            return this;
        }

        public HtmlContent _T(decimal v)
        {
            Add(' ');
            Add(v);
            return this;
        }

        public HtmlContent _T(string str)
        {
            if (str != null)
            {
                Add("&nbsp;");
                Add(str);
            }
            return this;
        }

        public HtmlContent _T(string[] v)
        {
            Add(' ');
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add("、");
                    Add(v[i]);
                }
            }
            return this;
        }

        public HtmlContent T_(short v)
        {
            Add(v);
            Add(' ');
            return this;
        }

        public HtmlContent T_(int v)
        {
            Add(v);
            Add(' ');
            return this;
        }

        public HtmlContent T_(long v)
        {
            Add(v);
            Add(' ');
            return this;
        }

        public HtmlContent T_(double v)
        {
            Add(v);
            Add(' ');
            return this;
        }

        public HtmlContent T_(DateTime v)
        {
            Add(v);
            Add(' ');
            return this;
        }

        public HtmlContent T_(string v)
        {
            Add(v);
            Add(' ');
            return this;
        }

        public HtmlContent BR()
        {
            Add("<br>");
            return this;
        }

        public HtmlContent ABBR_()
        {
            Add("<abbr>");
            return this;
        }

        public HtmlContent _ABBR()
        {
            Add("</abbr>");
            return this;
        }

        public HtmlContent ABBR<V>(V v)
        {
            ABBR_();
            AddPrimitive(v);
            _ABBR();
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

        public HtmlContent B<V>(V v)
        {
            B_();
            AddPrimitive(v);
            _B();
            return this;
        }

        public HtmlContent CITE_()
        {
            Add("<cite>");
            return this;
        }

        public HtmlContent _CITE()
        {
            Add("</cite>");
            return this;
        }

        public HtmlContent CITE<V>(V v)
        {
            CITE_();
            AddPrimitive(v);
            _CITE();
            return this;
        }

        public HtmlContent CODE_()
        {
            Add("<code>");
            return this;
        }

        public HtmlContent _CODE()
        {
            Add("</code>");
            return this;
        }

        public HtmlContent CODE<V>(V v)
        {
            CODE_();
            AddPrimitive(v);
            _CODE();
            return this;
        }

        public HtmlContent DEL_()
        {
            Add("<del>");
            return this;
        }

        public HtmlContent _DEL()
        {
            Add("</del>");
            return this;
        }

        public HtmlContent DEL<V>(V v)
        {
            DEL_();
            AddPrimitive(v);
            _DEL();
            return this;
        }

        public HtmlContent DFN_()
        {
            Add("<dfn>");
            return this;
        }

        public HtmlContent _DFN()
        {
            Add("</dfn>");
            return this;
        }

        public HtmlContent DFN<V>(V v)
        {
            DFN_();
            AddPrimitive(v);
            _DFN();
            return this;
        }

        public HtmlContent HR(bool icon = false)
        {
            Add("<hr");
            if (icon)
            {
                Add(" class=\"uk-divider-icon\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent EM_()
        {
            Add("<em>");
            return this;
        }

        public HtmlContent _EM()
        {
            Add("</em>");
            return this;
        }

        public HtmlContent EM<V>(V v)
        {
            EM_();
            AddPrimitive(v);
            _EM();
            return this;
        }

        public HtmlContent SEP()
        {
            Add("&nbsp;/&nbsp;");
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

        public HtmlContent ARTICLE_()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent BADGE_(Style style = 0)
        {
            Add("<span class=\"uk-badge");
            if (style > 0)
            {
                if (style == Style.Primary) Add("-primary");
                else if (style == Style.Success) Add("-success");
                else if (style == Style.Warning) Add("-warning");
                else if (style == Style.Danger) Add("-danger");
            }
            Add("\">");
            return this;
        }

        public HtmlContent _BADGE()
        {
            Add("</span>");
            return this;
        }

        public HtmlContent BADGE<V>(V v, Style style = 0)
        {
            BADGE_(style);
            AddPrimitive(v);
            _BADGE();
            return this;
        }

        public HtmlContent A(string v, string href, Style style = Style.Link, string targ = null)
        {
            Add("<a class=\"uk-button");

            if (style == Style.Default) Add(" uk-button-default uk-border-rounded");
            else if (style == Style.Primary) Add(" uk-button-primary");
            else if (style == Style.Secondary) Add(" uk-button-secondary");
            else if (style == Style.Danger) Add(" uk-button-danger");
            else if (style == Style.Text) Add(" uk-button-text");
            else if (style == Style.Link) Add(" uk-button-link");

            Add("\" href=\"");
            Add(href);
            if (targ != null)
            {
                Add("\" target=\"");
                Add(targ);
                Add("\"");
            }
            Add("\">");
            Add(v);
            Add("</a>");
            return this;
        }

        public HtmlContent A_CLOSE(string v, bool button = false)
        {
            Add("<a href=\"#\" onclick=\"closeUp(false); return false;\"");
            if (button)
            {
                Add(" class=\"uk-button uk-button-default uk-border-rounded\"");
            }
            else
            {
                Add(" class=\"uk-button uk-button-link\"");
            }
            Add(">");
            Add(v);
            Add("</a>");
            return this;
        }

        public HtmlContent A_(string href, Style style = 0, bool? size = null, byte width = 0, char target = (char) 0)
        {
            Add("<a href=\"");
            Add(href);
            Add("\" class=\"uk-button");
            if (style == 0) Add(" uk-button-default");
            else if (style == Style.Primary) Add(" uk-button-primary");
            else if (style == Style.Secondary) Add(" uk-button-secondary");
            else if (style == Style.Danger) Add(" uk-button-danger");
            else if (style == Style.Text) Add(" uk-button-text");
            else if (style == Style.Link) Add(" uk-button-link");
            if (size != null)
            {
                Add(size.Value ? " uk-button-small" : " uk-button-large");
            }
            Add("\""); // end of class
            if (target == 'p')
            {
                Add(" target=\"_parent\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _A()
        {
            Add("</a>");
            return this;
        }

        public HtmlContent A_HREF_(Style style = 0, bool? size = null, byte width = 0, char target = (char) 0)
        {
            Add("<a class=\"uk-button");
            if (style == 0) Add(" uk-button-default");
            else if (style == Style.Primary) Add(" uk-button-primary");
            else if (style == Style.Secondary) Add(" uk-button-secondary");
            else if (style == Style.Danger) Add(" uk-button-danger");
            else if (style == Style.Text) Add(" uk-button-text");
            else if (style == Style.Link) Add(" uk-button-link");
            if (size != null)
            {
                Add(size.Value ? " uk-button-small" : " uk-button-large");
            }
            Add("\""); // end of class
            if (target == 'p')
            {
                Add(" target=\"_parent\"");
            }
            Add(" href=\"");
            return this;
        }

        public HtmlContent _A_HREF(string v)
        {
            Add("\">");
            Add(v);
            Add("</a>");
            return this;
        }

        public HtmlContent BADGE_LINK(string href, string icon, string label = null)
        {
            Add("<a class=\"uk-button-link uk-card-badge-primary uk-icon-link\" href=\"");
            Add(href);
            Add("\" uk-icon=\"");
            Add(icon);
            Add("\">");
            if (label != null)
            {
                Add(label);
            }
            Add("</a>");
            return this;
        }

        public HtmlContent STATUS(string label, Style style = Style.None)
        {
            Add("<span class=\"uk-card-badge");
            if (style > 0)
            {
                if (style == Style.Primary) Add("-primary");
                else if (style == Style.Success) Add("-success");
                else if (style == Style.Warning) Add("-warning");
                else if (style == Style.Danger) Add("-danger");
            }
            Add("\">");
            Add(label);
            Add("</span>");
            return this;
        }

        public HtmlContent H2(string v, char line = (char) 0)
        {
            Add("<h2 class=\"uk-h3");
            if (line == 'd')
            {
                Add(" uk-heading-divider");
            }
            else if (line == 'c')
            {
                Add(" uk-heading-line uk-text-center");
            }
            else if (line == 'r')
            {
                Add(" uk-heading-line uk-text-right");
            }
            Add("\">");
            if (line == 'c' || line == 'r')
            {
                Add("<span>");
                Add(v);
                Add("</span>");
            }
            else
            {
                Add(v);
            }
            Add("</h2>");
            return this;
        }

        public HtmlContent H3(string v, char line = (char) 0)
        {
            Add("<h3 class=\"uk-h3");
            if (line == 'd')
            {
                Add(" uk-heading-divider");
            }
            else if (line == 'c')
            {
                Add(" uk-heading-line uk-text-center");
            }
            else if (line == 'r')
            {
                Add(" uk-heading-line uk-text-right");
            }
            Add("\">");
            if (line == 'c' || line == 'r')
            {
                Add("<span>");
                Add(v);
                Add("</span>");
            }
            else
            {
                Add(v);
            }
            Add("</h3>");
            return this;
        }

        public HtmlContent H4(string v, char line = (char) 0)
        {
            Add("<h4 class=\"uk-h4");
            if (line == 'b')
            {
                Add(" uk-heading-divider");
            }
            else if (line == 'c')
            {
                Add(" uk-heading-line uk-text-center");
            }
            else if (line == 'r')
            {
                Add(" uk-heading-line uk-text-right");
            }
            Add("\">");
            if (line == 'c' || line == 'r')
            {
                Add("<span>");
                Add(v);
                Add("</span>");
            }
            else
            {
                Add(v);
            }
            Add("</h4>");
            return this;
        }

        public HtmlContent H5(string v, char line = (char) 0)
        {
            Add("<h5 class=\"uk-h5");
            if (line == 'b')
            {
                Add(" uk-heading-divider");
            }
            else if (line == 'c')
            {
                Add(" uk-heading-line uk-text-center");
            }
            else if (line == 'r')
            {
                Add(" uk-heading-line uk-text-right");
            }
            Add("\">");
            if (line == 'c' || line == 'r')
            {
                Add("<span>");
                Add(v);
                Add("</span>");
            }
            else
            {
                Add(v);
            }
            Add("</h5>");
            return this;
        }

        public HtmlContent SP()
        {
            Add("&nbsp;");
            return this;
        }

        public HtmlContent ROW_(byte width = 0x11)
        {
            Add("<div class=\"uk-row");
            if (width > 0)
            {
                int lo = width & 0x0f;
                int hi = width >> 4;
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


        public HtmlContent TH(string label)
        {
            Add("<th>");
            Add(label);
            Add("</th>");
            return this;
        }

        public HtmlContent TH_()
        {
            Add("<th>");
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

        public HtmlContent TD(short v, bool zero = false)
        {
            Add("<td style=\"text-align: right\">");
            if (v != 0 || zero)
            {
                Add(v);
            }
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

        public HtmlContent TD(decimal v, bool zero = false)
        {
            Add("<td style=\"text-align: right\">");
            if (v != 0 || zero)
            {
                Add(v);
            }
            Add("</td>");
            return this;
        }

        public HtmlContent TD(DateTime v)
        {
            Add("<td>");
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
            Add("<span>");
            AddEsc(v);
            Add("</span><span>");
            AddEsc(v2);
            Add("</span>");
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

        public HtmlContent TD_(short v)
        {
            Add("<td>");
            Add(v);
            return this;
        }

        public HtmlContent TD_(int v)
        {
            Add("<td>");
            Add(v);
            return this;
        }

        public HtmlContent TD_(string v = null)
        {
            Add("<td>");
            if (v != null)
            {
                AddEsc(v);
            }
            return this;
        }

        public HtmlContent TD_(double v)
        {
            Add("<td>");
            Add(v);
            return this;
        }

        public HtmlContent _TD()
        {
            Add("</td>");
            return this;
        }


        public HtmlContent FIELD_(string label = null)
        {
            Add("<li>");
            LABEL(label);
            return this;
        }

        public HtmlContent _FIELD()
        {
            Add("</li>");
            return this;
        }

        public HtmlContent LABEL(string label)
        {
            if (label != null)
            {
                Add("<label class=\"uk-label\">");
                Add(label);
                Add("</label>");
            }
            return this;
        }


        public HtmlContent STATIC_(string label)
        {
            FIELD_(label);
            Add("<span class=\"uk-static\">");
            return this;
        }

        public HtmlContent _STATIC()
        {
            Add("</span>");
            _FIELD();
            return this;
        }

        public HtmlContent STATIC<V>(V v, string label)
        {
            STATIC_(label);
            AddPrimitive(v);
            _STATIC();
            return this;
        }

        public HtmlContent COL_(byte wid = 0x11)
        {
            Add("<div class=\"uk-col");
            if (wid > 0)
            {
                int lo = wid & 0x0f;
                int hi = wid >> 4;
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

        public HtmlContent P_(string label = null, byte wid = 0x11)
        {
            Add("<p");
            if (wid > 0)
            {
                int lo = wid & 0x0f;
                int hi = wid >> 4;
                Add(" class=\"uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
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

        public HtmlContent P<V>(V v, string label = null, byte wid = 0x11)
        {
            P_(label, wid);
            AddPrimitive(v);
            _P();
            return this;
        }

        public HtmlContent SPAN<V>(V v, byte wid = 0x11)
        {
            Add("<span class=\"");
            if (wid > 0)
            {
                int lo = wid & 0x0f;
                int hi = wid >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            if (v is short shortv)
            {
                Add(" uk-align-right");
                Add("\">");
                Add(shortv);
            }
            else if (v is int intv)
            {
                Add(" uk-align-right");
                Add("\">");
                Add(intv);
            }
            else if (v is long longv)
            {
                Add(" uk-align-right");
                Add("\">");
                Add(longv);
            }
            else if (v is string strv)
            {
                Add("\">");
                Add(strv);
            }
            else if (v is decimal decv)
            {
                Add(" uk-align-right");
                Add("\">");
                Add(decv);
            }
            else if (v is double doublev)
            {
                Add("\">");
                Add(doublev);
            }
            else if (v is DateTime dtv)
            {
                Add("\">");
                Add(dtv);
            }
            Add("</span>");
            return this;
        }

        public HtmlContent IMG(string src, string href = null, byte box = 0x0c)
        {
            FIELD_(null);
            if (href != null)
            {
                Add("<a href=\"");
                Add(href);
                Add("\">");
            }
            Add("<img class=\"img\" src=\"");
            Add(src);
            Add("\">");
            if (href != null)
            {
                Add("</a>");
            }
            _FIELD();
            return this;
        }

        public HtmlContent THUMBNAIL(string src, string href = null, byte wid = 0x0c)
        {
            FIELD_(null);
            if (href != null)
            {
                Add("<a href=\"");
                Add(href);
                Add("\">");
            }
            Add("<img class=\"thumbnail\" src=\"");
            Add(src);
            Add("\">");
            if (href != null)
            {
                Add("</a>");
            }
            _FIELD();
            return this;
        }

        public HtmlContent ICON(string src, string alt = null, string href = null, byte wid = 0)
        {
            Add("<div class=\"uk-margin-auto-vertical");
            if (wid > 0)
            {
                int lo = wid & 0x0f;
                int hi = wid >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            Add("\">");
            if (href != null)
            {
                Add("<a href=\"");
                Add(href);
                Add("\">");
            }
            Add("<img class=\"uk-border-circle\" src=\"");
            Add(src);
            if (alt != null)
            {
                Add("\" alt=\"");
                Add(alt);
            }
            Add("\">");
            if (href != null)
            {
                Add("</a>");
            }
            Add("</div>");
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

        public HtmlContent A_DROPDOWN_(string label, sbyte size = 0)
        {
            Add("<a href=\"#orginfo\" class=\"uk-button uk-button-link\" uk-toggle>");
            Add(label);
            Add("</a>");
            Add("<div id=\"orginfo\" class=\"uk-modal\" uk-modal>");
            Add("<div class=\"uk-modal-dialog uk-modal-body\">");
            return this;
        }

        public HtmlContent _A_DROPDOWN()
        {
            Add("</div>");
            Add("</div>");
            return this;
        }

        public HtmlContent FORM_(string action = null, bool post = true, bool mp = false, string oninput = null, string onsubmit = null)
        {
            Add("<form class=\"uk-grid uk-flex-center\"");
            if (action != null)
            {
                Add(" action=\"");
                Add(action);
                Add("\"");
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
                Add("\"");
            }
            if (onsubmit != null)
            {
                Add(" onsubmit=\"");
                Add(onsubmit);
                Add("\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent _FORM()
        {
            Add("</form>");
            return this;
        }

        public HtmlContent FIELDSET_(string legend = null, byte width = 6)
        {
            Add("<fieldset class=\"uk-fieldset");
            if (width > 0)
            {
                int lo = width & 0x0f;
                int hi = width >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
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

        public HtmlContent BUTTON(string name, int subcmd, string v, bool post = true)
        {
            Add("<button class=\"uk-button uk-button-default\" formmethod=\"");
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

        public void TOOLBAR(string title = null, bool refresh = true)
        {
            var prcs = webCtx.Work.Tooled;
            TOOLBAR_();
            Add("<div class=\"uk-button-group\">");
            for (int i = 0; i < prcs?.Length; i++)
            {
                var prc = prcs[i];
                if (!prc.IsCapital)
                {
                    PutTool(prc);
                }
            }
            Add("</div>");
            _TOOLBAR(title, refresh);
        }

        public HtmlContent TOOLBAR_()
        {
            Add("<form id=\"tool-bar-form\" class=\"top-bar\">");
            return this;
        }

        public HtmlContent _TOOLBAR(string title = null, bool refresh = true)
        {
            if (title != null)
            {
                Add(title);
            }
            if (refresh)
            {
                Add("<a class=\"uk-icon-button uk-button-link\" href=\"javascript: location.reload(false);\" uk-icon=\"refresh\"></a>");
            }
            Add("</form>");
            Add("<div class=\"top-bar-placeholder\"></div>");
            return this;
        }

        public HtmlContent BOTTOMBAR_()
        {
            Add("<footer class=\"bottom-bar\">");
            return this;
        }

        public HtmlContent _BOTTOMBAR()
        {
            Add("</footer>");
            return this;
        }

        public void PAGENATION(int count, int limit = 20)
        {
            // pagination
            Procedure prc = webCtx.Procedure;
            if (prc.HasSubscript)
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
                        Add(prc.Key);
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
                    Add(prc.Key);
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

        public HtmlContent LISTVIEW<D>(D[] arr, Action<D> item, Predicate<D> filter = null)
        {
            Add("<ul class=\"uk-list uk-list-divider\">");
            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    if (filter == null || filter(obj))
                    {
                        stack[level] = obj;

                        Add("<li class=\"uk-grid\">");
                        item(obj);
                        Add("</li>");

                        stack[level] = null;
                    }
                }

                level--; // exit the level
            }

            Add("</ul>");
            return this;
        }

        public HtmlContent ACCORDION<D>(D[] arr, Action<D> title, Action<D> content)
        {
            Add("<ul uk-accordion=\"multiple: true\">");
            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;

                    Add("<li>");
                    // title
                    Add("<section class=\"uk-accordion-title\">");
                    title(obj);
                    Add("</section>");
                    // content
                    Add("<section class=\"uk-accordion-content uk-grid\">");
                    content(obj);
                    Add("</section>");

                    Add("</li>");

                    stack[level] = null;
                }

                level--; // exit the level
            }
            // pagination if any
            Add("</ul>");
            return this;
        }

        public void TABLEVIEW<D>(D[] arr, Action head, Action<D> row)
        {
            Work w = webCtx.Work;
            Work vw = w.varwork;
            Add("<div class=\"uk-card uk-card-default uk-overflow-auto\">");
            Add("<table class=\"uk-table uk-table-divider uk-table-hover\">");
            Procedure[] prcs = vw?.Tooled;
            if (head != null)
            {
                Add("<thead>");
                Add("<tr>");
                if (w.HasPick)
                {
                    Add("<th></th>"); // 
                }
                head();
                if (prcs != null)
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
                        Add("<td>");
                        Add("<input name=\"key\" type=\"checkbox\" class=\"uk-checkbox\" value=\"");
                        vw.PutVariableKey(obj, this);
                        Add("\" onchange=\"checkit(this);\">");
                        Add("</td>");
                    }
                    row(obj);
                    if (prcs != null) // triggers
                    {
                        Add("<td>");
                        Add("<form>");
                        PutTools(vw);
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

        public void GRIDVIEW<D>(D[] arr, Action<D> block)
        {
            Add("<div class=\"uk-grid uk-child-width-1-2 uk-child-width-1-3@s uk-child-width-1-4@xll\">");
            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;

                    Add("<section class=\"uk-text-center uk-padding-small\">");
                    block(obj);
                    Add("</section>");

                    stack[level] = null;
                }

                level--; // exit the level
            }
            Add("</div>");
        }

        public void CARD(Action header, Action body, Action footer = null)
        {
            Add("<article class=\"uk-card uk-card-default\">");
            // header
            if (header != null)
            {
                Add("<div class=\"uk-card-header\">");
                header();
                Add("</div>");
            }
            // body
            if (footer != null)
            {
                Add("<div class=\"uk-card-body uk-grid uk-grid-small uk-padding-small\">");
                body();
                Add("</div>");
            }
            else
            {
                body();
            }
            // footer
            if (footer != null)
            {
                Add("<div class=\"uk-card-header\">");
                footer();
                Add("</div>");
            }
            Add("</article>");
        }


        public void CARDVIEW<D>(D obj, Action<D> header, Action<D> body, Action<D> footer = null)
        {
            Add("<article class=\"uk-card uk-card-default\">");
            if (obj != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                stack[level] = obj;

                // header
                if (header != null)
                {
                    Add("<div class=\"uk-card-header\">");
                    header(obj);
                    Add("</div>");
                }
                // body
                Add("<div class=\"uk-card-body uk-grid uk-grid-small uk-padding-small\">");
                body(obj);
                Add("</div>");
                // footer
                if (footer != null)
                {
                    Add("<div class=\"uk-card-footer\">");
                    footer(obj);
                    Add("</div>");
                }

                stack[level] = null;

                level--; // exit the level
            }
            Add("</article>");
        }

        public void BOARDVIEW<D>(D[] arr, Action<D> header, Action<D> body, Action<D> footer = null)
        {
            if (arr != null)
            {
                if (stack == null) stack = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    stack[level] = obj;

                    Add("<article class=\"uk-card uk-card-default\">");
                    // header
                    if (header != null)
                    {
                        Add("<div class=\"uk-card-header\">");
                        header(obj);
                        Add("</div>");
                    }
                    // body
                    Add("<div class=\"uk-card-body uk-grid\">");
                    body(obj);
                    Add("</div>");
                    // footer
                    if (footer != null)
                    {
                        Add("<div class=\"uk-card-footer\">");
                        footer(obj);
                        Add("</div>");
                    }
                    Add("</article>");

                    stack[level] = null;
                }

                level--; // exit the level
            }
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

        public HtmlContent TOOLPAD(byte width = 0x11)
        {
            // locate the proper work
            Add("<form class=\"uk-button-group uk-flex uk-flex-center uk-margin-small-top");
            if (width > 0)
            {
                int lo = width & 0x0f;
                int hi = width >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            Add("\">");

            Work w = webCtx.Work;
            for (int i = -1; i < level; i++)
            {
                w = w.varwork;
            }
            var prcs = w.Tooled;
            for (int i = 0; i < prcs?.Length; i++)
            {
                var prc = prcs[i];
                if (!prc.IsCapital)
                {
                    PutTool(prc);
                }
            }
            Add("</form>");
            return this;
        }

        public HtmlContent TOOL(string name, int subscript = -1)
        {
            // locate the proper work
            Work w = webCtx.Work;
            for (int i = -1; i < level; i++)
            {
                w = w.varwork;
            }
            var prc = w[name];
            if (prc != null)
            {
                PutTool(prc, subscript);
            }
            return this;
        }

        public HtmlContent LINK_(string prcname, int subscript = -1)
        {
            // locate the proper work
            Work w = webCtx.Work;
            for (int i = -1; i < level; i++)
            {
                w = w.varwork;
            }
            var prc = w[prcname];
            return LINK_(prc, subscript);
        }

        public HtmlContent LINK_(Procedure prc, int subscript = -1)
        {
            var tool = prc.Tool;

            // check procedure's availability
            bool ok = !tool.Auth || prc.DoAuthorize(webCtx, out _);
            if (ok && level >= 0)
            {
                ok = prc.DoState(webCtx, stack, level);
            }

            Add("<a class=\"");
            if (tool.Style > 0)
            {
                Add("uk-button");
                Add(prc == webCtx.Procedure ? " uk-button-default" : " uk-button-link");
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
            Add(prc.RPath);
            if (subscript >= 0)
            {
                Add('-');
                Add(subscript);
            }
            Add("\"");

            if (!ok)
            {
                Add(" disabled onclick=\"return false;\"");
            }
            else if (tool.HasConfirm)
            {
                Add(" onclick=\"return ");
                if (tool.MustPick)
                {
                    Add("!serialize(this.form) ? false : ");
                }
                Add("confirm('");
                Add(prc.Tip ?? prc.Label);
                Add("');\"");
            }
            else if (tool.HasPrompt)
            {
                OnClickDialog(2, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasShow)
            {
                OnClickDialog(4, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasOpen)
            {
                OnClickDialog(8, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasScript)
            {
                Add(" onclick=\"return by"); // prefix to avoid js naming conflict
                Add(prc.Lower);
                Add("(this);\"");
            }
            else if (tool.HasCrop)
            {
                Add(" onclick=\"return crop(this,");
                Add(tool.Ordinals);
                Add(',');
                Add(tool.Size);
                Add(",'");
                Add(prc.Tip);
                Add("');\"");
            }
            Add(">");

            return this;
        }

        public HtmlContent _LINK()
        {
            Add("</a>");
            return this;
        }

        public HtmlContent LINK(Procedure prc, int subscript = -1, string caption = null)
        {
            LINK_(prc, subscript);
            Add(caption ?? prc.Label);
            _LINK();
            return this;
        }


        void PutTools(Work work)
        {
            var prcs = work.Tooled;
            if (prcs == null)
            {
                return;
            }
            for (int i = 0; i < prcs.Length; i++)
            {
                var prc = prcs[i];
                if (!prc.IsCapital)
                {
                    PutTool(prc);
                }
            }
        }

        void PutTool(Procedure prc, int subscript = -1)
        {
            var tool = prc.Tool;
            if (tool.IsAnchorTag)
            {
                LINK(prc, subscript);
            }
            else
            {
                BUTTON(prc, subscript);
            }
        }

        public HtmlContent BUTTON_(Procedure prc, int subscript = -1)
        {
            var tool = prc.Tool;

            // check procedure's availability
            bool ok = !tool.Auth || prc.DoAuthorize(webCtx, out _);
            if (ok && level >= 0)
            {
                ok = prc.DoState(webCtx, stack, level);
            }

            Add("<button  class=\"uk-button uk-border-rounded");

            var style = tool.Style;
            if (style == Style.Default) Add(" uk-button-default");
            else if (style == Style.Primary) Add(" uk-button-primary");
            else if (style == Style.Secondary) Add(" uk-button-secondary");
            else if (style == Style.Danger) Add(" uk-button-danger");
            else if (style == Style.Text) Add(" uk-button-text");
            else if (style == Style.Link) Add(" uk-button-link");

            Add("\" name=\"");
            Add(prc.Key);
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
            Add(prc.Key);
            if (subscript >= 0)
            {
                Add('-');
                Add(subscript);
            }
            Add("\" formmethod=\"post\"");


            if (!ok)
            {
                Add(" disabled onclick=\"return false;\"");
            }
            else if (tool.HasConfirm)
            {
                Add(" onclick=\"return ");
                if (tool.MustPick)
                {
                    Add("!serialize(this.form) ? false : ");
                }
                Add("confirm('");
                Add(prc.Tip ?? prc.Label);
                Add("');\"");
            }
            else if (tool.HasPrompt)
            {
                OnClickDialog(2, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasShow)
            {
                OnClickDialog(4, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasOpen)
            {
                OnClickDialog(8, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasScript)
            {
                Add(" onclick=\"return by"); // prefix to avoid js naming conflict
                Add(prc.Lower);
                Add("(this);\"");
            }
            else if (tool.HasCrop)
            {
                Add(" onclick=\"return crop(this,");
                Add(tool.Ordinals);
                Add(',');
                Add(tool.Size);
                Add(",'");
                Add(prc.Tip);
                Add("');\"");
            }
            Add(">");

            return this;
        }

        public HtmlContent _BUTTON()
        {
            Add("</button>");
            return this;
        }

        public HtmlContent BUTTON(Procedure prc, int subscript = -1, string caption = null)
        {
            BUTTON_(prc, subscript);
            Add(caption ?? prc.Label);
            _BUTTON();
            return this;
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
            if (label != null) FIELD_(label);

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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent TEL(string name, string val, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            if (label != null) FIELD_(label);

            Add("<input class=\"uk-input\" type=\"tel\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(val);
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
            _FIELD();
            return this;
        }

        public HtmlContent SEARCH(string name, string val, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool required = false, byte wid = 0)
        {
            if (label != null) FIELD_(label);

            Add("<div class=\"uk-inline");
            if (wid > 0)
            {
                int lo = wid & 0x0f;
                int hi = wid >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
            Add("\"><input type=\"search\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(val);
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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent PASSWORD(string name, string val, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            if (label != null) FIELD_(label);

            Add("<input type=\"password\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(val);
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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent DATE(string name, DateTime val, string label = null, DateTime max = default, DateTime min = default, bool @readonly = false, bool required = false, int step = 0)
        {
            if (label != null) FIELD_(label);

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

            if (label != null) _FIELD();
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
            if (label != null) FIELD_(label);

            bool grp = !step.Equals(default(V)); // input group with up and down
            if (grp)
            {
                Add("<div class=\"uk-inline uk-width-1-2\">");
                Add("<a class=\"uk-form-icon\" href=\"#\" uk-icon=\"icon: minus-circle; ratio: 1.5\" onclick=\"this.nextSibling.stepDown();this.nextSibling.form.oninput();\"></a>");
            }
            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent NUMBER(string name, decimal v, string label = null, string tip = null, decimal max = decimal.MaxValue, decimal min = decimal.MinValue, decimal step = 0.00m, bool @readonly = false, bool required = false)
        {
            if (label != null) FIELD_(label);

            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
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

            if (tip != null)
            {
                Add("<mark>");
                Add(tip);
                Add("</mark>");
            }

            if (label != null) _FIELD();
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

        public HtmlContent CHECKBOX(string name, bool val, string label = null, bool required = false, byte width = 6)
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

        public HtmlContent CHECKBOXSET(string name, string[] val, string[] opt, string legend = null, byte box = 0x0c)
        {
            FIELDSET_(legend, box);
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

        public HtmlContent RADIOSET<K, V>(string name, K val, Map<K, V> opt = null, string legend = null, bool required = false, byte width = 0x0c)
        {
            FIELDSET_(legend, width);
            if (opt != null)
            {
                lock (opt)
                {
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.At(i);
                        if (e.IsHead)
                        {
                            STATIC_(null);
                            Add(e.Value.ToString());
                            _STATIC();
                        }
                        else
                        {
                            FIELD_(null);
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
                            if (e.Key.Equals(val)) Add(" checked");
                            if (required) Add(" required");
                            Add(">");
                            Add(e.Value.ToString());
                            Add("</label>");
                            _FIELD();
                        }
                    }
                }
            }
            _FIELDSET();
            return this;
        }

        public HtmlContent RADIOSET(string name, string v, string[] opt, string legend = null, bool required = false, byte width = 0x11)
        {
            FIELDSET_(legend, width);
            for (int i = 0; i < opt.Length; i++)
            {
                var o = opt[i];
                RADIO(name, o, o, o.Equals(v));
            }
            _FIELDSET();
            return this;
        }

        public HtmlContent TEXTAREA(string name, string v, string label = null, string tip = null, short max = 0, short min = 0, bool @readonly = false, bool required = false, byte width = 0)
        {
            if (label != null) FIELD_(label);

            Add("<textarea class=\"uk-textarea");
            if (width > 0)
            {
                int lo = width & 0x0f;
                int hi = width >> 4;
                Add(" uk-width-");
                Add(hi);
                Add('-');
                Add(lo);
            }
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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent TEXTAREA(string name, string[] val, string label = null, string tip = null, short max = 0, short min = 0, bool @readonly = false, bool required = false)
        {
            if (label != null) FIELD_(label);

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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent SELECT_(string name, string label = null, bool multiple = false, bool required = false, int size = 0)
        {
            FIELD_(label);

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
            _FIELD();
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

        public HtmlContent SELECT<K, V>(string name, K v, Map<K, V> opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false)
        {
            if (label != null) FIELD_(label);

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
            if (v == null)
            {
                Add("<option disabled selected></option>");
            }
            if (opt != null)
            {
                lock (opt)
                {
                    bool grpopen = false;
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.At(i);
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
            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K[] v, Map<K, V> opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false)
        {
            if (label != null) FIELD_(label);

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
            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent SELECT(string name, string v, string[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false)
        {
            if (label != null) FIELD_(label);

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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent SELECT(string name, string[] v, string[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false)
        {
            if (label != null) FIELD_(label);

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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K val, V[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false) where V : IKeyable<K>
        {
            if (label != null) FIELD_(label);

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

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K[] v, V[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false) where V : IKeyable<K>
        {
            if (label != null) FIELD_(label);

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

            if (label != null) _FIELD();
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
            if (label != null) FIELD_(label);

            Add("<output class=\"uk-output\" name=\"");
            Add(name);
            Add("\">");
            AddPrimitive(v);
            Add("</output>");

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent OUTPUT(string name, decimal v, string label = null)
        {
            if (label != null) FIELD_(label);

            Add("<output class=\"uk-output\" name=\"");
            Add(name);
            Add("\">¥");
            Add(v);
            Add("</output>");

            if (label != null) _FIELD();
            return this;
        }

        public HtmlContent METER()
        {
            T("</tbody>");
            return this;
        }
    }
}