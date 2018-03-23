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
        object[] chain;
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


        public HtmlContent A_(string href, char style = 'l', char size = (char) 0, byte width = 0, char target = (char) 0)
        {
            Add("<a href=\"");
            Add(href);
            Add("\" class=\"uk-button");
            switch (style)
            {
                case (char) 0:
                    Add(" uk-button-default");
                    break;
                case 'p':
                    Add(" uk-button-primary");
                    break;
                case 's':
                    Add(" uk-button-secondary");
                    break;
                case 'd':
                    Add(" uk-button-danger");
                    break;
                case 't':
                    Add(" uk-button-text");
                    break;
                case 'l':
                    Add(" uk-button-link");
                    break;
            }

            if (size == 's')
            {
                Add(" uk-button-small");
            }
            else if (size == 'l')
            {
                Add(" uk-button-large");
            }

//            Width(width);

            Add("\""); // end of class

            if (target == 'p')
            {
                Add(" target=\"_parent\"");
            }
            Add(">");
            return this;
        }

        public HtmlContent BUTTON_(char style = (char) 0, char size = (char) 0, byte width = 0, bool disabled = false)
        {
            Add("<button class=\"uk-button");
            switch (style)
            {
                case (char) 0:
                    Add(" uk-button-default");
                    break;
                case 'p':
                    Add(" uk-button-primary");
                    break;
                case 's':
                    Add(" uk-button-secondary");
                    break;
                case 'd':
                    Add(" uk-button-danger");
                    break;
                case 't':
                    Add(" uk-button-text");
                    break;
                case 'l':
                    Add(" uk-button-link");
                    break;
            }

            if (size == 's')
            {
                Add(" uk-button-small");
            }
            else if (size == 'l')
            {
                Add(" uk-button-large");
            }

//            Width(width);

            Add("\""); // end of class

            if (disabled)
            {
                Add(" disabled");
            }
            Add(">");
            return this;
        }

        public HtmlContent CARD_(char style, bool hover, char pad = (char) 0, bool body = false)
        {
            Add("<div class\"uk-card");

            if (style == 'd')
            {
                Add(" uk-card-default");
            }
            else if (style == 'p')
            {
                Add(" uk-card-primary");
            }
            else if (style == 's')
            {
                Add(" uk-card-secondary");
            }

            if (hover)
            {
                Add(" uk-card-hover");
            }

            if (pad == 's')
            {
                Add(" uk-card-small");
            }
            else if (pad == 'l')
            {
                Add(" uk-card-large");
            }

            if (body)
            {
                Add(" uk-card-body");
            }

            Add(" \">");
            return this;
        }

        public HtmlContent MODAL_(string id, char size, char media, char close = 'd', bool body = false)
        {
            Add("<div class\"uk-card"); // overlay

            if (media == 'i')
            {
                Add(" uk-modal>");
            }

            Add(" \" uk-modal>");

            Add("<div class\"uk-modal-dialog"); // dialog
            if (body)
            {
                Add("  uk-modal-body");
            }

            if (close == 'd')
            {
                Add("<button class=\"uk-modal-close-default\" type=\"button\" uk-close></button>");
            }
            else if (close == 'o')
            {
                Add("<button class=\"uk-modal-close-outside\" type=\"button\" uk-close></button>");
            }


            Add(" \">");

            return this;
        }

        public HtmlContent _MODAL()
        {
            Add("</div>"); // dialog
            Add("</div>"); // overlay
            return this;
        }

        public HtmlContent MODAL_HEADER_()
        {
            Add("<div class=\"uk-modal-header\">");
            Add("</div>"); // overlay
            return this;
        }

        public HtmlContent _MODAL_HEADER()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent MODAL_HEADER(string title, short h = 2)
        {
            MODAL_HEADER_();
            Add("<h");
            Add(h);
            Add(" class=\"uk-modal-title\">");
            Add(title);
            Add("</h");
            Add(h);
            Add(">");
            _MODAL_HEADER();
            return this;
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

        public HtmlContent BR()
        {
            Add("<br>");
            return this;
        }

        public HtmlContent EM(decimal v, string fix = null)
        {
            Add("<em>");
            if (fix != null)
            {
                if (fix == "¥" || fix == "$")
                {
                    Add(fix);
                    Add(v);
                }
                else
                {
                    Add(v);
                    Add(fix);
                }
            }
            else
            {
                Add(v);
            }
            Add("</em>");
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
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent _T(int v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent _T(long v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent _T(double v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent _T(DateTime v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent _IF(DateTime v)
        {
            if (v != default)
            {
                Add("&nbsp;");
                Add(v);
            }
            return this;
        }

        public HtmlContent _IF(string v)
        {
            if (v != default)
            {
                Add("&nbsp;");
                Add(v);
            }
            return this;
        }

        public HtmlContent IF(string v)
        {
            if (v != default)
            {
                Add(v);
            }
            return this;
        }

        public HtmlContent _T(decimal v)
        {
            Add("&nbsp;");
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
            Add("&nbsp;");
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

        public HtmlContent SEP()
        {
            Add("&nbsp;/&nbsp;");
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


        public HtmlContent A(string v, string href, bool? hollow = null, string targ = null)
        {
            Add("<a class=\"uk-button uk-button-link\" href=\"");
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

        public HtmlContent A_CLOSE(string v, bool? hollow = null)
        {
            Add("<a href=\"#\" onclick=\"closeup(false); return false;\"");
            if (hollow.HasValue)
            {
                if (hollow == true)
                {
                    Add(" class=\"button primary round hollow\"");
                }
                else
                {
                    Add(" class=\"button primary round\"");
                }
            }
            Add(">");
            Add(v);
            Add("</a>");
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


        public HtmlContent FIELD_(string label = null, byte width = 0x11)
        {
            Add("<div class=\"uk-field");
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
            if (label != null)
            {
                Add("<label class=\"uk-label\">");
                Add(label);
                Add("</label>");
            }
            return this;
        }

        public HtmlContent _FIELD()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent FIELD<V>(V v, string label = null, string pre = null, string pst = null, string tag = null, byte width = 0x11)
        {
            FIELD_(label, width);
            if (tag != null)
            {
                Add('<');
                Add(tag);
                Add('>');
            }
            if (pre != null)
            {
                Add(pre);
            }
            AddPrimitive(v);
            if (pst != null)
            {
                Add(pst);
            }
            if (tag != null)
            {
                Add('<');
                Add('/');
                Add(tag);
                Add('>');
            }
            _FIELD();
            return this;
        }

        public HtmlContent BOX_(byte width = 0x11)
        {
            Add("<div class=\"uk-box");
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

        public HtmlContent _BOX()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent P_(string label = null, byte width = 0x11)
        {
            Add("<p");
            if (width > 0)
            {
                int lo = width & 0x0f;
                int hi = width >> 4;
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

        public HtmlContent P<V>(V v, string label = null, string pre = null, string pst = null, string tag = null, byte width = 0x11)
        {
            P_(label, width);
            if (tag != null)
            {
                Add('<');
                Add(tag);
                Add('>');
            }
            if (pre != null)
            {
                Add(pre);
            }
            AddPrimitive(v);
            if (pst != null)
            {
                Add(pst);
            }
            if (tag != null)
            {
                Add('<');
                Add('/');
                Add(tag);
                Add('>');
            }
            _P();
            return this;
        }

        public HtmlContent IMG(string src, string href = null, byte box = 0x0c)
        {
            FIELD_(null, box);
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

        public HtmlContent THUMBNAIL(string src, string href = null, byte box = 0x0c)
        {
            FIELD_(null, box);
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

        public HtmlContent ICON(string src, string alt = null, string href = null, byte width = 0)
        {
            FIELD_(null, width);
            if (href != null)
            {
                Add("<a href=\"");
                Add(href);
                Add("\">");
            }
            Add("<img class=\"icon uk-border-circle\" src=\"");
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
            _FIELD();
            return this;
        }

        public HtmlContent QRCODE(string val = null)
        {
            Add("<div class=\"uk-qrcode\">");
            Add("<script type=\"text/javascript\">");
            Add("var scripte = document.scripts[document.scripts.length - 1];");
            Add("new QRCode(scripte.parentNode, \"");
            Add(val);
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

        public HtmlContent FORM_(string action = null, bool post = true, bool mp = false)
        {
            Add("<form class=\"uk-grid\"");
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
            Add("<div class=\"uk-grid\">");
            return this;
        }

        public HtmlContent _FIELDSET()
        {
            Add("</div>");
            Add("</fieldset>");
            return this;
        }

        public HtmlContent BUTTON(string val, bool post = true, bool top = false)
        {
            Add("<button class=\"uk-button uk-button-default\" formmethod=\"");
            Add(post ? "post" : "get");
            if (top)
            {
                Add("\" formtarget=\"_top");
            }
            Add("\">");
            AddEsc(val);
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

        public HtmlContent _BUTTON()
        {
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
                    Tool(prc);
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

        public HtmlContent LISTVIEW<D>(D[] arr, Action<D> item)
        {
            Add("<ul class=\"uk-list uk-list-divider uk-list-striped\">");
            if (arr != null)
            {
                if (chain == null) chain = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    chain[level] = obj;

                    Add("<li>");
                    item(obj);
                    Add("</li>");

                    chain[level] = null;
                }

                level--; // exit the level
            }

            Add("</ul>");
            return this;
        }

        public HtmlContent ACCORDIONVIEW<D>(D[] arr, Action<D> title, Action<D> content)
        {
            Add("<ul uk-accordion=\"multiple: true\">");
            if (arr != null)
            {
                if (chain == null) chain = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    chain[level] = obj;

                    Add("<li>");
                    // title
                    Add("<div class=\"uk-accordion-title\">");
                    title(obj);
                    Add("</div>");
                    // content
                    Add("<form class=\"uk-accordion-content uk-grid\">");
                    content(obj);
                    Add("</form>");

                    Add("</li>");

                    chain[level] = null;
                }

                level--; // exit the level
            }
            // pagination if any
            Add("</ul>");
            return this;
        }

        public void TABLEVIEW<D>(D[] arr, Action head, Action<D> row)
        {
            Work work = webCtx.Work;
            Work varwork = work.varwork;
            Add("<table class=\"uk-table uk-table-divider uk-table-hover\">");
            Procedure[] prcs = varwork?.Tooled;
            if (head != null)
            {
                Add("<thead>");
                Add("<tr>");
                if (work.HasPick)
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
                if (chain == null) chain = new object[4]; // init contexts
                level++; // enter a new level

                Add("<tbody>");
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    chain[level] = obj;

                    Add("<tr>");
                    if (varwork != null && work.HasPick)
                    {
                        Add("<td>");
                        Add("<input name=\"key\" type=\"checkbox\" class=\"uk-checkbox\" value=\"");
                        varwork.PutVariableKey(obj, this);
                        Add("\" onchange=\"checkit(this);\">");
                        Add("</td>");
                    }
                    row(obj);
                    if (prcs != null) // triggers
                    {
                        Add("<td>");
                        Add("<form>");
                        Tools(varwork);
                        Add("</form>");
                        Add("</td>");
                    }
                    Add("</tr>");

                    chain[level] = null;
                }
                Add("</tbody>");

                level--; // exit the level
            }
            Add("</table>");
        }

        public void GRIDVIEW<D>(D[] arr, Action<D> block)
        {
            Add("<div class=\"uk-grid uk-child-width-1-2@s uk-child-width-1-3@m uk-child-width-1-4@l uk-child-width-1-4@xl\">");
            if (arr != null)
            {
                if (chain == null) chain = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    chain[level] = obj;

                    Add("<section>");
                    block(obj);
                    Add("</section>");

                    chain[level] = null;
                }

                level--; // exit the level
            }
            Add("</div>");
        }


        public void CARDVIEW<D>(D obj, Action<D> header, Action<D> body, Action<D> footer = null)
        {
            Add("<article class=\"uk-card uk-card-default\">");
            if (obj != null)
            {
                if (chain == null) chain = new object[4]; // init contexts
                level++; // enter a new level

                chain[level] = obj;

                // header
                Add("<div class=\"uk-card-header\">");
                header?.Invoke(obj);
                Add("</div>");
                // body
                Add("<div class=\"uk-card-body uk-grid uk-grid-small uk-padding-small\">");
                body(obj);
                Add("</div>");
                // footer
                Add("<div class=\"uk-card-header\">");
                footer?.Invoke(obj);
                Add("</div>");

                chain[level] = null;

                level--; // exit the level
            }
            Add("</article>");
        }

        public void BOARDVIEW<D>(D[] arr, Action<D> header, Action<D> body, Action<D> footer = null)
        {
            Add("<main class=\"board\">");
            if (arr != null)
            {
                if (chain == null) chain = new object[4]; // init contexts
                level++; // enter a new level

                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    chain[level] = obj;

                    Add("<article class=\"uk-card uk-card-default\" id=\"card-");
                    // header
                    Add("<div class=\"uk-card-header\">");
                    header(obj);
                    Add("</div>");
                    // body
                    Add("<div class=\"uk-card-body uk-grid uk-grid-small uk-padding-small\">");
                    body(obj);
                    Add("</div>");
                    // footer
                    Add("<div class=\"uk-card-header\">");
                    footer?.Invoke(obj);
                    Add("</div>");
                    Add("</article>");

                    chain[level] = null;
                }

                level--; // exit the level
            }
            Add("</main>");
        }


        void Dialog(sbyte mode, bool pick, sbyte size, string tip)
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

        public HtmlContent TOOL(string name, int subscript = -1)
        {
            Work w = webCtx.Work;
            for (int i = -1; i < level; i++)
            {
                w = w.varwork;
            }
            var prc = w[name];
            if (prc != null)
            {
                Tool(prc, subscript);
            }
            return this;
        }

        void Tools(Work work)
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
                    Tool(prc);
                }
            }
        }

        void Tool(Procedure prc, int subscript = -1)
        {
            // check procedure's availability
            bool ok = prc.DoAuthorize(webCtx, false);
            if (ok && level >= 0)
            {
                ok = prc.DoState(webCtx, chain[level]);
            }

            var tool = prc.Tool;
            if (tool.IsAnchor)
            {
                Add("<a class=\"uk-button");
                Add(prc == webCtx.Procedure ? " uk-button-default" : " uk-button-link");
                Add("\" href=\"");
                if (level >= 0)
                {
                    Work w = webCtx.Work;
                    for (int i = 0; i <= level; i++)
                    {
                        w = w.varwork;
                        w.PutVariableKey(chain[i], this);
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
            }
            else if (tool.IsButton)
            {
                Add("<button  class=\"uk-button uk-border-rounded");
                Add(prc.IsCapital ? " uk-button-primary " : " uk-button-default ");
                Add("\" name=\"");
                Add(prc.Key);
                Add("\" formaction=\"");
                if (level >= 0)
                {
                    Work w = webCtx.Work;
                    for (int i = 0; i <= level; i++)
                    {
                        w = w.varwork;
                        w.PutVariableKey(chain[i], this);
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
                    Add(" disabled");
                }
            }
            if (tool.HasConfirm)
            {
                Add(" onclick=\"return ");
                if (tool.MustPick)
                {
                    Add("!($(this.form).serialize()) ? false : ");
                }
                Add("confirm('");
                Add(prc.Tip ?? prc.Label);
                Add("');\"");
            }
            else if (tool.HasPrompt)
            {
                Dialog(2, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasShow)
            {
                Dialog(4, tool.MustPick, tool.Size, prc.Tip);
            }
            else if (tool.HasOpen)
            {
                Dialog(8, tool.MustPick, tool.Size, prc.Tip);
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
            Add(prc.Label);
            if (tool.IsAnchor)
            {
                Add("</a>");
            }
            else if (tool.IsButton)
            {
                Add("</button>");
            }
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

        public HtmlContent TEXT(string name, string val, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, byte width = 6)
        {
            FIELD_(label, width);
            Add("<input type=\"text\" class=\"uk-input\" name=\"");
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

        public HtmlContent TEL(string name, string val, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, byte box = 6)
        {
            FIELD_(label, box);
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

        public HtmlContent SEARCH(string name, string val, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool required = false, byte width = 6)
        {
            FIELD_(label, width);

            Add("<input type=\"search\" class=\"uk-input\" name=\"");
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

            Add("<button formmethod=\"get\" class=\"uk-icon-button\" uk-icon=\"search\"></button>");

            _FIELD();
            return this;
        }

        public HtmlContent PASSWORD(string name, string val, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, byte width = 6)
        {
            FIELD_(label, width);

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

            _FIELD();
            return this;
        }

        public HtmlContent DATE(string name, DateTime val, string label = null, DateTime max = default, DateTime min = default, bool @readonly = false, bool required = false, int step = 0, byte box = 0x0c)
        {
            FIELD_(label, box);

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

            _FIELD();
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

        public HtmlContent NUMBER<V>(string name, V val, string label = null, string tip = null, V max = default, V min = default, V step = default, bool @readonly = false, bool required = false, byte width = 6)
        {
            FIELD_(label, width);

            bool grp = !step.Equals(default(V)); // input group with up and down
            if (grp)
            {
                Add("<div class=\"uk-inline\">");
                Add("<a class=\"uk-form-icon\" href=\"#\" uk-icon=\"icon: minus-circle; ratio: 1.5\" onclick=\"this.nextSibling.stepDown()\"></a>");
            }
            Add("<input type=\"number\" class=\"uk-input\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(val);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (!min.Equals(default(V)))
            {
                Add(" min=\"");
                AddPrimitive(min);
                Add("\"");
            }
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
                Add("<a class=\"uk-form-icon uk-form-icon-flip\" href=\"#\" uk-icon=\"icon: plus-circle; ratio: 1.5\" onclick=\"this.previousSibling.stepUp()\"></a>");
                Add("</div>");
            }

            _FIELD();
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
            FIELD_(null, width);
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
            _FIELD();
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

        public HtmlContent RADIO<V>(string name, V v, string label = null, bool @checked = false, byte width = 0x11)
        {
            FIELD_(null, width);
            Add("<label>");
            Add("<input type=\"radio\" class=\"uk-radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddPrimitive(v);
            Add(@checked ? "\" checked>" : "\">");
            Add(label);
            Add("</label>");
            _FIELD();
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

        public HtmlContent TEXTAREA(string name, string val, string label = null, string help = null, short max = 0, short min = 0, bool @readonly = false, bool required = false, byte box = 6)
        {
            FIELD_(label, box);
            Add("<textarea class=\"uk-textarea\" name=\"");
            Add(name);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
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
            AddEsc(val);
            Add("</textarea>");
            _FIELD();
            return this;
        }

        public HtmlContent TEXTAREA(string name, string[] val, string label = null, string help = null, short max = 0, short min = 0, bool @readonly = false, bool required = false, byte width = 6)
        {
            FIELD_(label, width);
            Add("<textarea class=\"uk-textarea\" name=\"");
            Add(name);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
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
            _FIELD();
            return this;
        }

        public HtmlContent SELECT_(string name, string label = null, bool multiple = false, bool required = false, int size = 0, byte box = 0x0c)
        {
            FIELD_(label, box);
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

        public HtmlContent SELECT<K, V>(string name, K v, Map<K, V> opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte width = 6)
        {
            FIELD_(label, width);
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
                    bool grpopen = false;
                    for (int i = 0; i < opt.Count; i++)
                    {
                        var e = opt.At(i);
                        if (e.top)
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
            _FIELD();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K[] v, Map<K, V> opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c)
        {
            FIELD_(label, box);

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
            _FIELD();
            return this;
        }

        public HtmlContent SELECT(string name, string v, string[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<select name=\"");
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

            _FIELD();
            return this;
        }

        public HtmlContent SELECT(string name, string[] v, string[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c)
        {
            FIELD_(label, box);

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

            _FIELD();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K val, V[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte width = 6) where V : IKeyable<K>
        {
            FIELD_(label, width);

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

            _FIELD();
            return this;
        }

        public HtmlContent SELECT<K, V>(string name, K[] v, V[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c) where V : IKeyable<K>
        {
            FIELD_(label, box);

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

            _FIELD();
            return this;
        }

        public HtmlContent DATALIST(string id, string[] opt)
        {
            Add("<datalist");
            if (id != null)
            {
                Add(" id=\"");
                Add(id);
                Add("\"");
            }
            for (int i = 0; i < opt.Length; i++)
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

        public HtmlContent OUTPUT<V>(string name, V val)
        {
            Add("<output name=\"");
            Add(name);
            Add("\">");
            AddPrimitive(val);
            Add("</output>");
            return this;
        }

        public HtmlContent METER()
        {
            T("</tbody>");
            return this;
        }
    }
}