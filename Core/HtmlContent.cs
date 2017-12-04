using System;

namespace Greatbone.Core
{
    /// <summary>
    /// For dynamic HTML5 content tooled with Zurb Foundation
    /// </summary>
    public class HtmlContent : DynamicContent
    {
        readonly ActionContext actionCtx;

        // used for current level content
        int ordinal;

        // used for lower lever (var) content
        IData model;

        public HtmlContent(ActionContext actionCtx, bool octet, int capacity = 32 * 1024) : base(octet, capacity)
        {
            this.actionCtx = actionCtx;
        }

        public override string Type => "text/html; charset=utf-8";

        void AddLabel(string label, string alt)
        {
            if (label != null)
            {
                Add(label);
            }
            else // alt uppercase
            {
                for (int i = 0; i < alt.Length; i++)
                {
                    char c = alt[i];
                    if (c >= 'a' && c <= 'z')
                    {
                        c = (char) (c - 32);
                    }
                    Add(c);
                }
            }
        }

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

        public HtmlContent TT(string str)
        {
            Add(str);
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

        public HtmlContent T(string[] v)
        {
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add("&nbsp;");
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

        public HtmlContent _T(decimal v)
        {
            Add("&nbsp;");
            Add(v);
            return this;
        }

        public HtmlContent _T(string str)
        {
            Add("&nbsp;");
            Add(str);
            return this;
        }

        public HtmlContent _T(string[] v)
        {
            if (v == null) return this;
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0)
                {
                    Add(" ");
                }
                Add(v[i]);
            }
            return this;
        }

        public HtmlContent SEP()
        {
            Add("&nbsp;/&nbsp;");
            return this;
        }

        public HtmlContent A(string v, string href)
        {
            Add("<a href=\"");
            Add(href);
            Add("\">");
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

        public HtmlContent TD(decimal v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
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
            Add("</span>  <span>");
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
                    if (i > 0) Add(" ");
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

        public HtmlContent FIELD(short v, string label = null, string suffix = null, byte box = 0x0c)
        {
            FIELD_(label, box);
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FIELD(int v, string label = null, string suffix = null, byte box = 0x0c)
        {
            FIELD_(label, box);
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FIELD(long v, string label = null, string suffix = null, byte box = 0x0c)
        {
            FIELD_(label, box);
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FIELD(decimal v, string label = null, string suffix = null, byte box = 0x0c)
        {
            FIELD_(label, box);
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FIELD(DateTime v, string label = null, string suffix = null, byte box = 0x0c)
        {
            FIELD_(label, box);
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FIELD(string v, string label = null, string suffix = null, byte box = 0x0c)
        {
            FIELD_(label, box);
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FIELD(string[] v, string label = null, string suffix = null, byte box = 0x0c)
        {
            FIELD_(label, box);
            if (v != null)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (i > 0) Add(" ");
                    Add(v[i]);
                }
            }
            if (suffix != null)
            {
                Add(suffix);
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FIELD_(string label = null, byte box = 0x0c)
        {
            BOX_(box, false);
            if (label != null)
            {
                Add("<span class=\"label\">");
                Add(label);
                Add("</span>");
            }
            return this;
        }

        void _FIELD(byte box)
        {
            if ((box & 0x0f) > 0)
            {
                Add("</div>");
            }
        }

        public HtmlContent _FIELD()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent GRID_(sbyte small = 0, sbyte medium = 0, sbyte large = 0, sbyte xlarge = 0)
        {
            Add("<div class=\"grid-x");
            if (small > 0)
            {
                Add(" small-up-");
                Add(small);
            }
            if (medium > 0)
            {
                Add(" medium-up-");
                Add(medium);
            }
            if (large > 0)
            {
                Add(" large-up-");
                Add(large);
            }
            if (xlarge > 0)
            {
                Add(" xlarge-up-");
                Add(xlarge);
            }
            Add("\">");
            return this;
        }

        public HtmlContent _GRID()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent CELL_(sbyte small = 0, sbyte medium = 0, sbyte large = 0, sbyte xlarge = 0)
        {
            Add("<div class=\"cell");
            if (small > 0)
            {
                Add(" small-");
                Add(small);
            }
            if (medium > 0)
            {
                Add(" medium-");
                Add(medium);
            }
            if (large > 0)
            {
                Add(" large-");
                Add(large);
            }
            if (xlarge > 0)
            {
                Add(" xlarge-");
                Add(xlarge);
            }
            Add("\">");
            return this;
        }

        public HtmlContent _CELL()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent BOX_(byte box = 0x0c, bool column = true)
        {
            int grid = box & 0x0f;
            if (grid > 0)
            {
                int layout = (box >> 4) & 0x0f;
                Add("<div class=\"cell small-");
                Add(grid);
                Add(" box box-");
                Add(layout);
                Add("\"");
                if (column)
                {
                    Add(" style=\"flex-direction: column; padding: 0;\"");
                }
                Add(">");
            }
            return this;
        }

        public HtmlContent _BOX()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent P(short v, string label = null, string suffix = null)
        {
            Add("<p>");
            if (label != null)
            {
                Add("<span class=\"label\">");
                Add(label);
                Add("</span>");
            }
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            Add("</p>");
            return this;
        }

        public HtmlContent P(int v, string label = null, string suffix = null)
        {
            Add("<p>");
            if (label != null)
            {
                Add("<span class=\"label\">");
                Add(label);
                Add("</span>");
            }
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            Add("</p>");
            return this;
        }

        public HtmlContent P(decimal v, string label = null, string suffix = null, char symbol = (char) 0)
        {
            Add("<p>");
            if (label != null)
            {
                Add("<span class=\"label\">");
                Add(label);
                Add("</span>");
            }
            if (symbol != 0)
            {
                Add("<em>");
                Add(symbol);
            }
            Add(v);
            if (symbol != 0) Add("</em>");
            if (suffix != null)
            {
                Add(suffix);
            }
            Add("</p>");
            return this;
        }

        public HtmlContent P(DateTime v, string label = null, string suffix = null)
        {
            Add("<p>");
            if (label != null)
            {
                Add("<span class=\"label\">");
                Add(label);
                Add("</span>");
            }
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            Add("</p>");
            return this;
        }

        public HtmlContent P(string v, string label = null, string suffix = null)
        {
            Add("<p>");
            if (label != null)
            {
                Add("<span class=\"label\">");
                Add(label);
                Add("</span>");
            }
            Add(v);
            if (suffix != null)
            {
                Add(suffix);
            }
            Add("</p>");
            return this;
        }

        public HtmlContent P(string[] v, string label = null)
        {
            if (v == null) return this;
            Add("<p>");
            if (label != null)
            {
                Add("<span class=\"label\">");
                Add(label);
                Add("</span>");
            }
            for (int i = 0; i < v.Length; i++)
            {
                if (i > 0)
                {
                    Add("&nbsp;");
                }
                Add(v[i]);
            }
            Add("</p>");
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
            _FIELD(box);
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
            _FIELD(box);
            return this;
        }

        public HtmlContent ICON(string src, string href = null, byte box = 0x0c)
        {
            FIELD_(null, box);
            if (href != null)
            {
                Add("<a href=\"");
                Add(href);
                Add("\">");
            }
            Add("<img class=\"icon\" src=\"");
            Add(src);
            Add("\">");
            if (href != null)
            {
                Add("</a>");
            }
            _FIELD(box);
            return this;
        }

        public HtmlContent FORM_(string action = null, bool post = true, bool mp = false)
        {
            Add("<form class=\"grid-x\"");
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

        public HtmlContent FIELDSET_(string legend = null, byte box = 0x0c)
        {
            Add("<fieldset");
            if (box > 0)
            {
                Add(" class=\"cell small-");
                Add(box);
                Add("\"");
            }
            Add(">");
            if (legend != null)
            {
                Add("<legend>");
                AddEsc(legend);
                Add("</legend>");
            }
            Add("<div class=\"grid-x\">");
            return this;
        }

        public HtmlContent _FIELDSET()
        {
            Add("</div>");
            Add("</fieldset>");
            return this;
        }

        public HtmlContent BUTTON(string value, bool post = true)
        {
            Add("<button class=\"button primary hollow\" formmethod=\"");
            Add(post ? "post" : "get");
            Add("\">");
            AddEsc(value);
            Add("</button>");
            return this;
        }

        public HtmlContent BUTTON(string name, int subcmd, string value, bool post = true)
        {
            Add("<button class=\"button primary hollow\" formmethod=\"");
            Add(post ? "post" : "get");
            Add("\" formaction=\"");
            Add(name);
            Add('-');
            Add(subcmd);
            Add("\">");
            AddEsc(value);
            Add("</button>");
            return this;
        }

        public HtmlContent _BUTTON()
        {
            Add("</button>");
            return this;
        }

        public HtmlContent CALLOUT(string v, bool closable = false)
        {
            Add("<div class=\"callout primary\"");
            if (closable)
            {
                Add(" data-closable");
            }
            Add("><p class=\"text-center\">");
            Add(v);
            Add("</p>");
            if (closable)
            {
                Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            }
            Add("</div>");
            return this;
        }

        public HtmlContent CALLOUT(Action<HtmlContent> m, bool closable)
        {
            Add("<div class=\"callout primary\"");
            if (closable)
            {
                Add(" data-closable");
            }
            Add("><p class=\"text-center\">");
            m?.Invoke(this);
            Add("</p>");
            if (closable)
            {
                Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            }
            Add("</div>");
            return this;
        }

        public void TOOLBAR(short group = 0, bool refresh = true)
        {
            var work = actionCtx.Work;
            var ais = work.Tooled;
            if (ais == null)
            {
                TOOLBAR_(work.Label);
            }
            else
            {
                TOOLBAR_();
                for (int i = 0; i < ais.Length; i++)
                {
                    var ai = ais[i];
                    if (ai.Group != group)
                    {
                        continue;
                    }
                    Tool(ais[i], null);
                }
            }
            _TOOLBAR(refresh);
        }

        public HtmlContent TOOLBAR_(string title = null)
        {
            Add("<header data-sticky-container>");
            Add("<form id=\"tool-bar-form\" class=\"sticky tool-bar\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
            if (title != null)
            {
                Add(title);
            }
            return this;
        }

        public HtmlContent _TOOLBAR(bool refresh = true)
        {
            if (refresh)
            {
                Add("<a class=\"primary\" href=\"javascript: location.reload(false);\" style=\"font-size: 1.75rem; line-height: 1; margin-left: auto\">&#9851;</a>");
            }
            Add("</form>");
            Add("</header>");
            return this;
        }

        public void PAGENATE(int count)
        {
            // pagination
            ActionInfo ai = actionCtx.Doer;
            if (ai.HasSubscript)
            {
                Add("<ul class=\"pagination\" role=\"navigation\">");
                int subscpt = actionCtx.Subscript;
                for (int i = 0; i <= subscpt; i++)
                {
                    if (subscpt == i)
                    {
                        Add("<li class=\"current\">");
                        Add(i + 1);
                        Add("</li>");
                    }
                    else
                    {
                        Add("<li><a href=\"");
                        Add(ai.Key);
                        Add('-');
                        Add(i);
                        Add(actionCtx.QueryString);
                        Add("\">");
                        Add(i + 1);
                        Add("</a></li>");
                    }
                }
                if (count == ai.Limit)
                {
                    Add("<li class=\"pagination-next\"><a href=\"");
                    Add(ai.Key);
                    Add('-');
                    Add(subscpt + 1);
                    Add(actionCtx.QueryString);
                    Add("\">");
                    Add(subscpt + 2);
                    Add("</a></li>");
                }
                Add("</ul>");
            }
        }

        public void SHEETVIEW<D>(D[] arr, Action<HtmlContent> head, Action<HtmlContent, D> row) where D : IData
        {
            Work work = actionCtx.Work;
            Work varwork = work.varwork;
            Add("<main class=\"sheet-view table-scroll);\">");
            Add("<table>");
            ActionInfo[] ais = varwork?.Tooled;

            if (head != null)
            {
                Add("<thead>");
                Add("<tr>");
                // for checkboxes
                head(this);
                if (ais != null)
                {
                    Add("<th></th>"); // for triggers
                }
                Add("</tr>");
                Add("</thead>");
            }

            if (arr != null && row != null) // tbody if having data objects
            {
                Add("<tbody>");
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    Add("<tr>");
                    row(this, obj);
                    if (ais != null) // triggers
                    {
                        Add("<td>");
                        Add("<form>");
                        Tools(varwork, 0, obj);
                        Add("</form>");
                        Add("</td>");
                    }
                    Add("</tr>");
                }
                Add("</tbody>");
            }
            Add("</table>");
            // pagination controls if any
            PAGENATE(arr?.Length ?? 0);
            Add("</main>");
        }

        public void BOARDVIEW(params Action<HtmlContent>[] cards)
        {
            BOARDVIEW_();
            for (int i = 0; i < cards.Length; i++)
            {
                CARD_();
                cards[i](this);
                _CARD();
            }
            _BOARDVIEW();
        }

        public void BOARDVIEW<D>(D[] models, Action<HtmlContent, D> card) where D : IData
        {
            BOARDVIEW_();
            if (models != null)
            {
                for (int i = 0; i < models.Length; i++)
                {
                    D obj = models[i];
                    model = obj;
                    CARD_();
                    card(this, obj);
                    _CARD();
                }
            }
            // pagination if any
            PAGENATE(models?.Length ?? 0);
            _BOARDVIEW();
        }

        public HtmlContent BOARDVIEW_()
        {
            Add("<main class=\"board-view grid-x small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
            ordinal = 0;
            model = null;
            return this;
        }

        public HtmlContent _BOARDVIEW()
        {
            Add("</main>");
            model = null;
            ordinal = 0;
            return this;
        }

        public HtmlContent CARD_()
        {
            Add("<form class=\"cell board-view-cell\" id=\"card-");
            Add("\"><article class=\"card grid-x\">");
            ordinal++;
            return this;
        }

        public HtmlContent _CARD()
        {
            Add("</article>");
            Add("</form>");
            return this;
        }

        public HtmlContent CAPTION(bool checkbox, string title, string flag = null, bool? on = null)
        {
            CAPTION_(checkbox);
            Add(title);
            _CAPTION(flag, on);
            return this;
        }

        public HtmlContent CAPTION_(bool checkbox)
        {
            Add("<div class=\"cell card-caption small-12\">");
            if (checkbox)
            {
                if (model != null)
                {
                    Work varwork = actionCtx.Work.VarWork;
                    if (varwork != null)
                    {
                        Add("<input name=\"key\" type=\"checkbox\" form=\"tool-bar-form\" value=\"");
                        varwork.PutVarKey(model, this);
                        Add("\" onchange=\"checkit(this);\">");
                    }
                }
            }
            return this;
        }

        public HtmlContent _CAPTION(string flag = null, bool? on = null)
        {
            if (flag != null)
            {
                Add("<span style=\"margin-left: auto\" class=\"card-flag-");
                if (on.HasValue)
                {
                    Add(on.Value ? "on" : "off");
                }
                Add("\">");
                Add(flag);
                Add("</span>");
            }
            Add("</div>");
            return this;
        }

        public HtmlContent TAIL(string flag = null, bool? on = null, short group = 0)
        {
            TAIL_(flag, on);
            _TAIL(group);
            return this;
        }

        public HtmlContent TAIL_(string flag = null, bool? on = null)
        {
            Add("<div class=\"cell card-tail\">");
            if (flag != null)
            {
                Add("<span class=\"float-right card-flag");
                if (on.HasValue)
                {
                    Add(on.Value ? "-on" : "-off");
                }
                Add("\">");
                Add(flag);
                Add("</span>");
            }
            return this;
        }

        public HtmlContent _TAIL(short group = 0)
        {
            if (model == null)
            {
                var work = actionCtx.Work;
                Add("<div style=\"margin-left: auto\">");
                Tools(work, (short) ordinal, null); // negative orderinal as group
                Add("</div>");
            }
            else
            {
                Work work = actionCtx.Work.VarWork;
                if (work != null)
                {
                    Add("<div style=\"margin-left: auto\">");
                    Tools(work, group, model);
                    Add("</div>");
                }
            }
            Add("</div>");
            return this;
        }

        void Dialog(sbyte style, sbyte size, string tip)
        {
            Add(" onclick=\"return dialog(this,");
            Add(style);
            Add(",");
            Add(size);
            Add(",'");
            Add(tip);
            Add("');\"");
        }

        void Tools(Work work, short group, IData obj)
        {
            var ais = work.Tooled;
            if (ais == null)
            {
                return;
            }
            for (int i = 0; i < ais.Length; i++)
            {
                var ai = ais[i];
                if (ai.Group != group)
                {
                    continue;
                }
                Tool(ais[i], obj);
            }
        }

        public HtmlContent TOOL(string name, int subscript = -1)
        {
            if (model == null)
            {
                var work = actionCtx.Work;
                var ai = work.GetAction(name);
                Tool(ai, null, subscript);
            }
            else
            {
                var work = actionCtx.Work.VarWork;
                var ai = work.GetAction(name);
                Tool(ai, model, subscript);
            }
            return this;
        }

        void Tool(ActionInfo ai, IData obj, int subscript = -1)
        {
            var tool = ai.Tool;
            bool ok = ai.CheckState(obj);
            if (tool.IsAnchor)
            {
                Add("<a class=\"button primary");
                Add(ai == actionCtx.Doer ? " hollow" : " clear");
                Add("\" href=\"");
                if (obj != null)
                {
                    ai.Work.PutVarKey(obj, this);
                    Add('/');
                }
                Add(ai.RPath);
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
                Add("<button class=\"button primary");
                if (!ai.IsCapitalized) Add(" hollow");
                Add("\" name=\"");
                Add(ai.Key);
                Add("\" formaction=\"");
                if (obj != null)
                {
                    ai.Work.PutVarKey(obj, this);
                    Add('/');
                }
                Add(ai.Key);
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
                Add(" onclick=\"return confirm('");
                Add(ai.Tip ?? ai.Label);
                Add("?');\"");
            }
            else if (tool.HasPrompt)
            {
                Dialog(2, tool.Size, ai.Tip);
            }
            else if (tool.HasShow)
            {
                Dialog(4, tool.Size, ai.Tip);
            }
            else if (tool.HasOpen)
            {
                Dialog(8, tool.Size, ai.Tip);
            }
            else if (tool.HasScript)
            {
                Add(" onclick=\"return ");
                Add(ai.Lower);
                Add("(this) || false;\"");
            }
            else if (tool.HasCrop)
            {
                Add(" onclick=\"return crop(this,");
                Add(tool.Ordinals);
                Add(',');
                Add(tool.Size);
                Add(",");
                Add(tool.Circle);
                Add(",'");
                Add(ai.Tip);
                Add("');\"");
            }
            Add(">");
            if (ai.IsEllipsized)
            {
                string filter = null;
                var f = actionCtx.Query;
                if (f.Count > 0) filter = f[0];
                if (filter != null)
                {
                    Add(filter);
                    Add("...");
                }
                else Add(ai.Label);
            }
            else
            {
                Add(ai.Label);
            }
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

        public HtmlContent HIDDEN(string name, string value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\">");
            return this;
        }

        public HtmlContent HIDDEN(string name, int value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
            return this;
        }

        public HtmlContent HIDDEN(string name, decimal value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
            return this;
        }

        public HtmlContent TEXT(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"text\" name=\"");
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

            _FIELD(box);
            return this;
        }

        public HtmlContent TEXT(string name, string[] vs, string label = null, string tip = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            if (vs != null)
            {
                for (int i = 0; i > vs.Length; i++)
                {
                    if (i > 0) Add(' ');
                    AddEsc(vs[i]);
                }
            }
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

            _FIELD(box);
            return this;
        }

        public HtmlContent TEL(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"tel\" name=\"");
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

            _FIELD(box);
            return this;
        }

        public HtmlContent SEARCH(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<div class=\"input-group\">");
            Add("<input type=\"search\" name=\"");
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

            Add("<a class=\"input-group-label\" onclick=\"$(this).closest('form').submit();\">&#128270;</a>");
            Add("</div>");

            _FIELD(box);
            return this;
        }

        public HtmlContent PASSWORD(string name, string v, string label = null, string tip = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"password\" name=\"");
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

            _FIELD(box);
            return this;
        }

        public HtmlContent DATE(string name, DateTime v, string label = null, DateTime max = default, DateTime min = default, bool @readonly = false, bool required = false, int step = 0, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"date\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
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

            _FIELD(box);
            return this;
        }

        public HtmlContent TIME()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent NUMBER(string name, short v, string label = null, string tip = null, short max = short.MaxValue, short min = short.MinValue, short step = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            bool group = step > 0; // input group with up up and down
            if (group)
            {
                Add("<div class=\"input-group\">");
                Add("<a class=\"input-group-label round\" onclick=\"$(this).next()[0].stepDown()\">-</a>");
            }

            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (group)
            {
                Add(" class=\"input-group-field\"");
            }

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
                Add("\"");
            }
            if (max != short.MaxValue)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != short.MinValue)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");

            if (group)
            {
                Add("<a class=\"input-group-label round\" onclick=\"$(this).prev()[0].stepUp()\">+</a>");
                Add("</div>");
            }

            _FIELD(box);
            return this;
        }

        public HtmlContent NUMBER(string name, int v, string label = null, string tip = null, int max = int.MaxValue, int min = int.MinValue, int step = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"number\" name=\"");
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
            if (max != int.MaxValue)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != int.MinValue)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");

            _FIELD(box);
            return this;
        }

        public HtmlContent NUMBER(string name, long v, string label = null, string tip = null, long max = 0, long min = 0, long step = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"number\" name=\"");
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
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");

            _FIELD(box);
            return this;
        }

        public HtmlContent NUMBER(string name, decimal v, string label = null, string tip = null, decimal max = 0, decimal min = 0, decimal step = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"number\" name=\"");
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
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }

            Add(" step=\"");
            if (step > 0) Add(step);
            else Add("any");
            Add("\"");
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");

            _FIELD(box);
            return this;
        }

        public HtmlContent NUMBER(string name, double v, string label = null, string tip = null, double max = 0, double min = 0, double step = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);

            Add("<input type=\"number\" name=\"");
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
            if (!max.Equals(0))
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (!min.Equals(0))
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }

            Add(" step=\"");
            if (step > 0) Add(step);
            else Add("any");
            Add("\"");
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            Add(">");

            _FIELD(box);
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

        public HtmlContent CHECKBOXES(string name, IDataInput inp, Action<IDataInput, HtmlContent, char> putter)
        {
            while (inp.Next())
            {
                Add("<label>");
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\" value=\"");
                putter(inp, this, 'V'); // putting value
                Add("\">");
                putter(inp, this, 'L'); // putting label
                Add("</label>");
            }
            return this;
        }

        public HtmlContent CHECKBOX(string name, bool v, string label = null, bool required = false, byte box = 0x0c)
        {
            FIELD_(null, box);

            if (label != null)
            {
                Add("<label>");
            }
            Add("<input type=\"checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (v) Add(" checked");
            if (required) Add(" required");
            Add(">");
            if (label != null)
            {
                Add(label);
                Add("</label>");
            }

            _FIELD(box);
            return this;
        }

        public HtmlContent CHECKBOXGROUP(string name, string[] v, string[] opts, string legend = null, byte box = 0x0c)
        {
            if (legend != null)
            {
                FIELDSET_(legend, box);
            }

            for (int i = 0; i < opts.Length; i++)
            {
                var item = opts[i];
                Add(" <label>");
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\"");
                if (v != null && v.Contains(item))
                {
                    Add(" checked");
                }
                Add(">");
                Add(item);
                Add(" </label>");
            }

            if (legend != null)
            {
                _FIELDSET();
            }
            return this;
        }

        public HtmlContent RADIO(string name, int value, bool @checked, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add(@checked ? "\" checked>" : "\">");
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, long value, bool check, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add(check ? "\" checked>" : "\">");
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, string v, string label = null, bool @checked = false)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (@checked)
            {
                Add(" checked");
            }
            Add(">");
            Add(label ?? v);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, Action<HtmlContent> value, bool @checked, Action<HtmlContent> label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            value(this);
            Add(@checked ? "\" checked>" : "\">");
            label(this);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, string v1, string v2, string v3, bool @checked, string l1, string l2, string l3)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v1);
            if (v2 != null)
            {
                Add('~');
                Add(v2);
            }
            if (v3 != null)
            {
                Add('~');
                Add(v3);
            }
            Add(@checked ? "\" checked>" : "\">");
            Add(l1);
            if (l2 != null)
            {
                Add(' ');
                Add(l2);
            }
            if (l3 != null)
            {
                Add(' ');
                Add(l3);
            }
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, long v1, string v2, bool @checked, long l1, string l2, string l3 = null)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v1);
            if (v2 != null)
            {
                Add('-');
                Add(v2);
            }
            Add(@checked ? "\" checked>" : "\">");
            Add(l1);
            Add(' ');
            Add(l2);
            if (l3 != null)
            {
                Add(' ');
                Add(l3);
            }
            Add("</label>");
            return this;
        }

        public HtmlContent RADIOS<O>(string name, short v, Map<short, O> opt = null, string label = null, bool required = false)
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            opt?.ForEach((key, item) =>
            {
                Add("<label>");
                Add("<input type=\"radio\" name=\"");
                Add(name);

                Add("\" id=\"");
                Add(name);
                Add(key);
                Add("\"");

                Add("\" value=\"");
                Add(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");
                Add("</label>");

                Add(key);
                //                Add("<label for=\"");
                //                Add(name);
                //                Add(key);
                //                Add("\">");
                //                Add(item.ToString());
                //                Add("</label>");
            });

            Add("</fieldset>");
            return this;
        }

        public HtmlContent RADIOS<O>(string name, string v, O[] opt = null, string label = null, bool required = false)
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            if (opt != null)
            {
                for (int i = 0; i < opt.Length; i++)
                {
                    var o = opt[i];
                    Add("<label>");
                    Add("<input type=\"radio\" name=\"");
                    Add(name);

                    Add("\" id=\"");
                    Add(name);
                    Add(o.ToString());
                    Add("\"");

                    Add("\" value=\"");
                    Add(o.ToString());
                    Add("\"");

                    if (o.ToString().Equals(v)) Add(" checked");
                    if (required) Add(" required");
                    Add(">");
                    Add("</label>");
                }
            }
            Add("</fieldset>");
            return this;
        }

        public HtmlContent RADIOS<O>(string name, string v, Map<string, O> opt = null, string label = null, bool required = false)
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            opt?.ForEach((key, item) =>
            {
                Add("<input type=\"radio\" name=\"");
                Add(name);

                Add("\" id=\"");
                Add(name);
                Add(key);
                Add("\"");

                Add("\" value=\"");
                Add(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");

                Add("<label for=\"");
                Add(name);
                Add(key);
                Add("\">");
                Add(item.ToString());
                Add("</label>");
            });

            Add("</fieldset>");
            return this;
        }

        public HtmlContent RADIOGROUP(string name, string v, string[] opt, string legend = null, bool required = false)
        {
            if (legend != null)
            {
                Add("<fieldset>");
                Add("<legend>");
                Add(legend);
                Add("</legend>");
            }

            for (int i = 0; i < opt.Length; i++)
            {
                var item = opt[i];
                Add("<label>");
                Add("<input type=\"radio\" name=\"");
                Add(name);
                Add("\" value=\"");
                Add(item);
                Add("\"");

                if (item.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");

                Add(item);
                Add("</label>");
            }
            if (legend != null)
            {
                Add("</fieldset>");
            }
            return this;
        }

        public HtmlContent TEXTAREA(string name, string v, string label = null, string help = null, short max = 0, short min = 0, bool @readonly = false, bool required = false, byte box = 0x0c)
        {
            FIELD_(label, box);
            Add("<textarea name=\"");
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
            AddEsc(v);
            Add("</textarea>");
            _FIELD(box);
            return this;
        }

        public HtmlContent SELECT<O>(string name, short v, Map<short, O> opt, string label = null, bool multiple = false, bool required = false, int size = 0, byte box = 0x0c)
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

            for (int i = 0; i < opt.Count; i++)
            {
                var e = opt.At(i);
                Add("<option value=\"");
                Add(e.Key);
                Add("\"");
                if (e.Key == v) Add(" selected");
                Add(">");
                Add(e.value.ToString());
                Add("</option>");
            }
            Add("</select>");

            _FIELD(box);
            return this;
        }

        public HtmlContent SELECT<O>(string name, string v, Map<string, O> opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c)
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
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + $(this.form).serialize();\"");
            }
            Add(">");

            for (int i = 0; i < opt.Count; i++)
            {
                var e = opt.At(i);
                Add("<option value=\"");
                Add(e.key);
                Add("\"");
                if (e.key == v) Add(" selected");
                Add(">");
                Add(e.value.ToString());
                Add("</option>");
            }
            Add("</select>");

            _FIELD(box);
            return this;
        }

        public HtmlContent SELECT<O>(string name, string[] v, Map<string, O> opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c)
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

            for (int i = 0; i < opt.Count; i++)
            {
                var e = opt.At(i);
                Add("<option value=\"");
                Add(e.key);
                Add("\"");
                if (v.Contains(e.key)) Add(" selected");
                Add(">");
                Add(e.value.ToString());
                Add("</option>");
            }
            Add("</select>");

            _FIELD(box);
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
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + $(this.form).serialize();\"");
            }
            Add(">");

            if (opt != null)
            {
                for (int i = 0; i < opt.Length; i++)
                {
                    string key = opt[i];
                    Add("<option value=\"");
                    Add(key);
                    Add("\"");
                    if (key == v) Add(" selected");
                    Add(">");

                    Add(key);
                    Add("</option>");
                }
            }
            Add("</select>");

            _FIELD(box);
            return this;
        }

        public HtmlContent SELECT<D>(string name, string v, D[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c) where D : IData
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
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + $(this.form).serialize();\"");
            }
            Add(">");

            if (opt != null)
            {
                for (int i = 0; i < opt.Length; i++)
                {
                    string key = opt[i].ToString();
                    Add("<option value=\"");
                    Add(key);
                    Add("\"");
                    if (key == v) Add(" selected");
                    Add(">");

                    Add(key);
                    Add("</option>");
                }
            }
            Add("</select>");

            _FIELD(box);
            return this;
        }

        public HtmlContent SELECT<D>(string name, string[] v, D[] opt, string label = null, bool required = false, sbyte size = 0, bool refresh = false, byte box = 0x0c) where D : IData
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
                    string key = opt[i].ToString();
                    Add("<option value=\"");
                    Add(key);
                    Add("\"");
                    if (v.Contains(key)) Add(" selected");
                    Add(">");

                    Add(key);
                    Add("</option>");
                }
            }
            Add("</select>");

            _FIELD(box);
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

        public HtmlContent PROGRES()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent OUTPUT(string name)
        {
            Add("<output name=\"");
            Add(name);
            Add("\"></output>");
            return this;
        }

        public HtmlContent METER()
        {
            T("</tbody>");
            return this;
        }
    }
}