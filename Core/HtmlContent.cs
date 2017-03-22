using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// For dynamical HTML5 content tooled with Zurb Foundation
    ///
    public class HtmlContent : DynamicContent, IDataOutput<HtmlContent>
    {
        const int DEPTH = 4;

        internal const sbyte
            // multiple
            CTX_TABLE = 1,
            CTX_GRID = 2,
            CTX_LIST = 3,
            // single
            CTX_FILL = 4,
            CTX_CARD = 5;

        // per idata object outputing context
        struct Ctx
        {
            // component type
            internal sbyte type;

            // whether currently need to ouput label
            internal bool label;

            internal int ordinal;
        }

        // whether within a form
        internal bool formed;


        // outputing context chain
        Ctx[] chain = new Ctx[DEPTH];

        int level = -1; // current level

        ///
        public HtmlContent(bool sendable, bool pooled, int capacity = 16 * 1024) : base(sendable, pooled, capacity)
        {
        }

        ///
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
                        c = (char)(c - 32);
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

        public HtmlContent T(string str)
        {
            Add(str);
            return this;
        }

        int table_idx;


        public void ROW_()
        {
            Add("<div class=\"row\">");
        }

        public void _ROW()
        {
            Add("</div>");
        }

        public void FORM_()
        {
            Add("<form>");
        }

        public void _FORM()
        {
            Add("</form>");
        }


        public void FIELDSET_(string legend = null)
        {
            Add("<fieldset class=\"fieldset\">");
            if (legend != null)
            {
                Add("<legend>");
                AddEsc(legend);
                Add("</legend>");
            }
        }

        public void _FIELDSET()
        {
            Add("</fieldset>");
        }

        public void CALLOUT(string v, bool closable)
        {
            Add("<div class=\"alert callout\" data-closable>");
            Add("<p class=\"text-center\">");
            Add(v);
            Add("</p>");
            Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            Add("</div>");
        }


        public void FORM_TABLE<D>(List<ActionInfo> acts, List<D> lst, int proj = 0) where D : IData
        {
            Add("<form>");

            formed = true;

            // buttons
            if (acts != null)
            {
                Add("<div class=\"row\">");
                BUTTONS(acts);
                Add("</div>");
            }

            TABLE(lst, proj);

            Add("</form>");
        }

        public void TABLE<D>(List<D> lst, int proj = 0) where D : IData
        {
            chain[++level].type = CTX_TABLE;
            if (lst != null)
            {
                Add("<table class=\"hover\">");

                chain[level].label = true;
                Add("<thead>");
                Add("<tr>");
                for (int i = 0; i < lst.Count; i++)
                {
                    chain[level].ordinal = 0; // reset ordical
                    lst[i].WriteData(this, proj);
                }
                Add("</tr>");
                Add("</thead>");

                chain[level].label = false;
                Add("<tbody>");
                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<tr>");
                    chain[level].ordinal = 0; // reset ordical
                    lst[i].WriteData(this, proj);
                    Add("</tr>");
                }
                Add("</tbody>");
            }
            --level;
        }

        public void FORM_TABLE(List<ActionInfo> acts, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form>");

            formed = true;

            // buttons
            if (acts != null)
            {
                Add("<div class=\"row\">");
                BUTTONS(acts);
                Add("</div>");
            }

            TABLE(input, valve);

            Add("</form>");
        }

        public void TABLE(IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            if (input != null)
            {
                Add("<table class=\"hover\">");

                // ctx = CTX_GRIDTHEAD;
                Add("<thead>");
                Add("<tr>");
                valve(input, this);
                Add("</tr>");
                Add("</thead>");

                // ctx = CTX_GRIDTBODY;
                Add("<tbody>");
                while (input.Next())
                {
                    Add("<tr>");
                    valve(input, this);
                    Add("</tr>");
                }
                Add("</tbody>");
            }
        }

        public void FORM_GRID<D>(ActionContext ac, List<ActionInfo> actions, List<D> lst, int proj = 0) where D : IData
        {
            Add("<form>");

            formed = true;

            // buttons
            if (actions != null)
            {
                Add("<div class=\"row\">");
                BUTTONS(actions);
                Add("</div>");
            }

            // grid
            GRID(lst, proj);

            // pagination
            ActionInfo act = ac.Doer;
            if (act.HasSubscpt)
            {
                Add("<div class=\"row\">");
                Add("<ul class=\"pagination text-center\" role=\"navigation\">");
                int subscript = ac.Subscript;
                Add("<li class=\"pagination-previous disabled\">Previous</li>");
                for (int i = 0; i < subscript; i++)
                {
                    Add("<li><a href=\""); Add(act.Name); Add('-'); Add(subscript); Add(ac.QueryString); Add("\">"); Add(i); Add("</a></li>");
                }
                Add("<li class=\"pagination-next disabled\">Next</li>");
                Add("</ul>");
                Add("</div>");
            }

            Add("</form>");
        }

        public void GRID<D>(List<D> lst, int proj = 0) where D : IData
        {
            chain[++level].type = CTX_GRID;
            if (lst != null)
            {
                Add("<div class=\"expanded row\">");
                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<div class=\"small-12 medium-6 large-4 columns\">");
                    chain[level].ordinal = 0; // reset ordical
                    lst[i].WriteData(this, proj);
                    Add("</div>");
                }
                Add("</div>");
            }
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }
            --level;
        }

        public void FORM_GRID(List<ActionInfo> actions, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form>");

            formed = true;

            // buttons
            if (actions != null)
            {
                Add("<div class=\"row\">");
                BUTTONS(actions);
                Add("</div>");
            }

            GRID(input, valve);

            Add("</form>");
        }

        public void GRID(IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            chain[++level].type = CTX_GRID;
            if (input != null)
            {
                Add("<div class=\"expanded row\">");
                while (input.Next())
                {
                    Add("<div class=\"small-12 medium-6 large-4 columns\">");
                    chain[level].ordinal = 0; // reset ordical
                    valve(input, this);
                    Add("</div>");
                }
                Add("</div>");
            }
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }
            --level;
        }

        public void FORM_LIST<D>(List<ActionInfo> actions, List<D> lst, int proj = 0) where D : IData
        {
            Add("<form>");

            formed = true;

            // buttons
            if (actions != null)
            {
                Add("<div class=\"row\">");
                BUTTONS(actions);
                Add("</div>");
            }

            LIST(lst, proj);

            Add("</form>");
        }

        public void LIST<D>(List<D> lst, int proj = 0) where D : IData
        {
            chain[++level].type = CTX_LIST;
            if (lst != null)
            {
                Add("<ul>");
                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<li>");
                    chain[level].ordinal = 0; // reset ordical
                    lst[i].WriteData(this, proj);
                    Add("</li>");
                }
                Add("</ul>");
            }
            --level;
        }

        public void FORM_FILL(ActionInfo act, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form method=\"post\">");

            FILL(input, valve);

            Add("</form>");
        }

        public void FILL(IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            chain[++level].type = CTX_FILL;
            if (input != null)
            {
                while (input.Next())
                {
                    chain[level].ordinal = 0; // reset ordical
                    valve(input, this);
                }
            }
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }
            --level;
        }

        public void FORM_FILL(ActionInfo act, IData obj, int proj = 0)
        {
            chain[++level].type = CTX_FILL;
            Add("<form method=\"post");
            if (act != null)
            {
                Add("\" action=\""); Add(act.Name);
            }
            Add("\">");
            obj.WriteData(this, proj);
            Add("</form>");
            --level;
        }

        public void FILL(IData obj, int proj = 0)
        {
            chain[++level].type = CTX_FILL;
            obj.WriteData(this, proj);
            --level;
        }

        public void HIDDEN(string name, string value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\">");
        }

        public void TEXT(string name, string v, string Label = null, string Help = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Help != null)
            {
                Add(" placeholder=\"");
                Add(Help);
                Add("\"");
            }
            if (Pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(Pattern);
                Add("\"");
            }
            if (Max > 0)
            {
                Add(" maxlength=\"");
                Add(Max);
                Add("\"");
                Add(" size=\"");
                Add(Max);
                Add("\"");
            }
            if (Min > 0)
            {
                Add(" minlength=\"");
                Add(Min);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");

            Add(">");

            Add("</label>");
        }

        public void TEXTPicker(string name, string v, string Label = null, string Help = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Help != null)
            {
                Add(" Help=\"");
                Add(Help);
                Add("\"");
            }
            if (Pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(Pattern);
                Add("\"");
            }
            if (Max > 0)
            {
                Add(" maxlength=\"");
                Add(Max);
                Add("\"");
                Add(" size=\"");
                Add(Max);
                Add("\"");
            }
            if (Min > 0)
            {
                Add(" minlength=\"");
                Add(Min);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");

            Add(">");

            Add("</label>");
        }

        public void PASSWORD(string name, string v, string Label = null, string Help = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"password\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Help != null)
            {
                Add(" Help=\"");
                Add(Help);
                Add("\"");
            }
            if (Pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(Pattern);
                Add("\"");
            }
            if (Max > 0)
            {
                Add(" maxlength=\"");
                Add(Max);
                Add("\"");
                Add(" size=\"");
                Add(Max);
                Add("\"");
            }
            if (Min > 0)
            {
                Add(" minlength=\"");
                Add(Min);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");

            Add(">");

            Add("</label>");
        }

        public void DATE(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), bool ReadOnly = false, bool Required = false, int Step = 0)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"date\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (Max != default(DateTime))
            {
                Add(" max=\"");
                Add(Max);
                Add("\"");
            }
            if (Min != default(DateTime))
            {
                Add(" min=\"");
                Add(Min);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");
            if (Step != 0)
            {
                Add(" step=\"");
                Add(Step);
                Add("\"");
            }

            Add(">");
            Add("</label>");
        }

        public void TIME()
        {
            T("</tbody>");
        }

        public void NUMBER<V>(string name, V v, string Label = null, string Help = null, V Max = default(V), V Min = default(V), V Step = default(V), bool ReadOnly = false, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddConvert(v);
            Add("\"");

            if (Help != null)
            {
                Add(" Help=\"");
                Add(Help);
                Add("\"");
            }
            if (Max.Equals(default(V)))
            {
                Add(" max=\"");
                AddConvert(Max);
                Add("\"");
            }
            if (Min.Equals(default(V)))
            {
                Add(" min=\"");
                AddConvert(Min);
                Add("\"");
            }
            if (Step.Equals(default(V)))
            {
                Add(" step=\"");
                AddConvert(Step);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");

            Add(">");
            Add("</label>");
        }

        public void RANGE()
        {
            T("</tbody>");
        }

        public void COLOR()
        {
            T("</tbody>");
        }

        public void CHECKBOX(string name, bool v, string Label = null, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (v) Add(" checked");
            if (Required) Add(" required");
            Add(">");
            Add("</label>");
        }

        public void CHECKBOX<V>(V[] v, Map<V> opt, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(Label, null);
            Add("</legend>");

            foreach (var pair in opt)
            {
                V key = pair.Key;

                Add("<input type=\"checkbox\" name=\"");
                AddConvert(key);
                Add("\"");

                Add("\" id=\"");
                AddConvert(key);
                Add("\"");

                Add("\" value=\"");
                AddConvert(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (Required) Add(" required");
                Add(">");

                Add("<label for=\"");
                AddConvert(key);
                Add("\">");
                Add(pair.Value);
                Add("</label>");
            }
            Add("</fieldset>");
        }

        public void RADIO<V>(string name, V v, Map<V> opt, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(Label, name);
            Add("</legend>");

            foreach (var pair in opt)
            {
                Add("<input type=\"radio\" name=\"");
                Add(name);
                V key = pair.Key;

                Add("\" id=\""); Add(name);
                AddConvert(key);
                Add("\"");

                Add("\" value=\"");
                AddConvert(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (Required) Add(" required");
                Add(">");

                Add("<label for=\""); Add(name);
                AddConvert(key);
                Add("\">");
                Add(pair.Value);
                Add("</label>");
            }
            Add("</fieldset>");
        }

        public void FILE(string name, string Label = null, string Size = null, string Ratio = null, bool Required = false)
        {
            // <div class="slim"
            //      data-label="Drop your avatar here"
            //      data-fetcher="fetch.php"
            //      data-size="240,240"
            //      data-ratio="1:1">
            //     <input type="file" name="slim[]" required />
            // </div>
            //         
            Add("<div class=\"slim\" data-label=\"");
            AddLabel(Label, name);
            Add("\" data-fetcher=\"_"); Add(name); Add("_");

            if (Size != null)
            {
                Add("\" data-size=\""); Add(Size);
            }
            if (Ratio != null)
            {
                Add("\" data-ratio=\""); Add(Ratio);
            }
            Add("\">");

            Add("<input type=\"file\" name=\""); Add(name); Add("\"");
            if (Required) Add(" required");
            Add(">");
            Add("</div>");
        }

        public void TEXTAREA(string name, string v, string Label = null, string Help = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Help != null)
            {
                Add(" Help=\"");
                Add(Help);
                Add("\"");
            }
            if (Max > 0)
            {
                Add(" maxlength=\"");
                Add(Max);
                Add("\"");

                Add(" rows=\"");
                Add(Max < 200 ? 2 : Max < 400 ? 3 : 4);
                Add("\"");
            }
            if (Min > 0)
            {
                Add(" minlength=\"");
                Add(Min);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");

            Add(">");
            Add("</label>");
        }

        public void BUTTON(ActionInfo act)
        {
            Add("<button class=\"button primary\" style=\"margin-right: 5px; border-radius: 15%\"");
            Add(" formaction=\""); Add(act.Name); Add("\" formmethod=\"post\"");

            UiAttribute ui = act.Ui;

            int modal = ui?.Dialog ?? 0;
            if (modal > 0)
            {
                Add(" onclick=\"dialog(this,"); Add(modal); Add("); return false;\"");
            }

            StateAttribute state = act.State;
            if (state != null)
            {
                Add(" data-if=\""); Add(state.If); Add("\"");
                Add(" data-unif=\""); Add(state.Unif); Add("\"");
            }
            Add(">");
            // label and ison
            string icon = ui?.Icon;
            if (icon != null)
            {
                Add("<i class=\"");
                Add(icon);
                Add("\"></i>");
            }
            AddLabel(ui?.Label, act.Name);

            Add("</button>");
        }

        public void BUTTONS(List<ActionInfo> actns)
        {
            for (int i = 0; i < actns.Count; i++)
            {
                ActionInfo act = actns[i];
                Add("<button class=\"button primary\" style=\"margin-right: 5px;\" name=\""); Add(act.Name);
                Add("\" formaction=\""); Add(act.Name); Add("\" formmethod=\"post\"");

                UiAttribute ui = act.Ui;

                int modal = ui?.Dialog ?? 0;
                if (modal > 0)
                {
                    Add(" onclick=\"dialog(this,"); Add(modal); Add("); return false;\"");
                }

                StateAttribute state = act.State;
                if (state != null)
                {
                    Add(" data-if=\""); Add(state.If); Add("\"");
                    Add(" data-unif=\""); Add(state.Unif); Add("\"");
                }
                Add(">");
                // label and ison
                string icon = ui?.Icon;
                if (icon != null)
                {
                    Add("<i class=\"");
                    Add(icon);
                    Add("\"></i>");
                }
                AddLabel(ui?.Label, act.Name);

                Add("</button>");
            }
        }

        public void SELECT<V>(string name, V v, Map<V> opt, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (Required) Add(" required");
            Add(">");

            foreach (var pair in opt)
            {
                V key = pair.Key;
                Add("<option value=\"");
                AddConvert(key);
                Add("\"");
                if (key.Equals(v)) Add(" selected");
                Add(">");

                Add(pair.Value);
                Add("</option>");
            }
            Add("</select>");
            Add("</label>");
        }

        public void DATALIST()
        {
            T("</tbody>");
        }

        public void PROGRES()
        {
            T("</tbody>");
        }

        public void METER()
        {
            T("</tbody>");
        }


        //
        // ISINK
        //

        public HtmlContent PutNull(string name)
        {
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, JNumber v)
        {
            return this;
        }

        public HtmlContent Put(string name, IDataInput v)
        {
            return this;
        }

        public HtmlContent PutRaw(string name, string raw)
        {
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, bool v, Func<bool, string> Opt = null, string Label = null, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td>");
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns\">");
                    AddLabel(Label, name);
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">"); if (Opt != null) Add(Opt(v)); else Add(v); Add("</div>");
                    Add("</div>");
                    break;
                case CTX_LIST:
                    break;
                case CTX_FILL:
                    Add("<div class=\"row\">");
                    CHECKBOX(name, v, Label, Required);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short v, Map<short> Opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
            var ctx = chain[level];
            switch (ctx.type)
            {
                case CTX_TABLE:
                    if (ctx.label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (formed && level == 1)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        if (Opt != null)
                        {
                            Add(Opt[v]);
                        }
                        else
                        {
                            Add(v);
                        }
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns\">");
                    if (formed && level == 0 && chain[level].ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\""); Add(v); Add("\">");
                    }
                    else
                    {
                        AddLabel(Label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">"); if (Opt != null) Add(Opt[v]); else Add(v); Add("</div>");
                    Add("</div>");
                    break;
                case CTX_LIST:
                    break;
                case CTX_FILL:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, int v, Map<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (formed && level == 0 && chain[level].ordinal == 0)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns\">");
                    if (formed && level == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\""); Add(v); Add("\">");
                    }
                    else
                    {
                        AddLabel(Label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">"); Add(v); Add("</div>");
                    Add("</div>");
                    break;
                case CTX_LIST:
                    break;
                case CTX_FILL:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;

            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, long v, Map<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (formed && level == 1)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns\">");
                    if (formed && level == 0 && chain[level].ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\""); Add(v); Add("\">");
                    }
                    else
                    {
                        AddLabel(Label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">"); Add(v); Add("</div>");
                    Add("</div>");
                    break;
                case CTX_LIST:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case CTX_FILL:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, double v, string Label = null, string Help = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns\">"); AddLabel(Label, name); Add("</div>");
                    Add("<div class=\"small-9 columns\">"); Add(v); Add("</div>");
                    Add("</div>");
                    break;
                case CTX_LIST:
                    break;
                case CTX_FILL:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, decimal v, string Label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns\">"); AddLabel(Label, name); Add("</div>");
                    Add("<div class=\"small-9 columns\">"); Add(v); Add("</div>");
                    Add("</div>");
                    break;
                case CTX_LIST:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
                case CTX_FILL:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, string v, Map<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            var ctx = chain[level];
            switch (ctx.type)
            {
                case CTX_TABLE:
                    if (ctx.label)
                    {
                        Add("<th>"); AddLabel(Label, name); Add("</th>");
                    }
                    else
                    {
                        Add("<td>");
                        if (formed && level == 1)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns\">");
                    if (formed && level == 0 && chain[level].ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\""); Add(v); Add("\">");
                    }
                    else
                    {
                        AddLabel(Label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">"); Add(v); Add("</div>");
                    Add("</div>");
                    break;
                case CTX_LIST:
                    Add(v);
                    break;
                case CTX_FILL:
                    if (Label != null && Label.Length == 0)
                    {
                        HIDDEN(name, v);
                    }
                    else if (name.EndsWith("password"))
                    {
                        PASSWORD(name, v, Label, Help, Pattern, (sbyte)Max, (sbyte)Min, ReadOnly, Required);
                    }
                    else if (Max < 128)
                    {
                        TEXT(name, v, Label, Help, Pattern, (sbyte)Max, (sbyte)Min, ReadOnly, Required);
                    }
                    else
                    {
                        TEXTAREA(name, v, Label, Help, Max, Min, ReadOnly, Required);
                    }
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, ArraySegment<byte> v, string Label = null, string Size = null, string Ratio = null, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add("</td>");
                    }
                    break;
                case CTX_GRID:
                    break;
                case CTX_FILL:
                    Add("<div class=\"\">");
                    FILE(name, Label, Size, Ratio, Required);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short[] v, Map<short> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {

            return this;
        }

        public HtmlContent Put(string name, int[] v, Map<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, long[] v, Map<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, string[] v, Map<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, Map v, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, IData v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            return this;
        }

        public HtmlContent Put<D>(string name, List<D> v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            switch (chain[level].type)
            {
                case CTX_TABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        if (v != null) { LIST(v, proj); }
                        else
                        {
                            Add("<div class=\"row\"><span>没有记录</span></div>");
                        }
                    }
                    break;
                case CTX_GRID:
                    if (v != null)
                    {
                        TABLE(v, proj);
                    }
                    else
                    {
                        Add("<div class=\"row\"><span>没有记录</span></div>");
                    }
                    break;
                case CTX_LIST:
                    break;
            }
            chain[level].ordinal++;
            return this;
        }
    }
}