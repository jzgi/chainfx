using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// For dynamical HTML5 content tooled with Zurb Foundation
    ///
    public class HtmlContent : DynamicContent, IDataOutput<HtmlContent>
    {
        const int DEPTH = 8;

        internal const sbyte
            CTX_TABLE = 0x10,
            CTX_TABLE_ROW = 0x11,
            CTX_LIST = 0x20,
            CTX_LIST_ITEM = 0x21,
            CTX_GRID = 0x30,
            CTX_GRID_PANE = 0x31,
            CTX_INPUT = 0x50;

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

        int level; // current level

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

        public void GRIDPANE(IData obj, int proj = 0)
        {
        }

        public void FORM(ActionInfo actn, Action<HtmlContent> inner)
        {
            Add("<form class=\"pure-form pure-g\">");

            inner?.Invoke(this);

            Add("</form>");
        }

        public void FORM(ActionInfo actn, IData obj, int proj = 0)
        {
            Add("<form class=\"pure-form pure-g\">");

            chain[level].type = CTX_INPUT;

            // function buttuns
            Add("</form>");
        }

        public void GRID<D>(List<ActionInfo> actns, List<D> lst, int proj = 0) where D : IData
        {
            Add("<form>");

            // buttons
            if (actns != null)
            {
                BUTTONS(actns);
            }

            if (lst != null)
            {
                Put(null, lst, proj);
            }
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }

            Add("</form>");
        }

        public void GRID(List<ActionInfo> actns, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form>");

            // buttons
            BUTTONS(actns);

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
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }

            Add("</form>");
        }

        public void TABLE<D>(List<D> lst, int proj = 0) where D : IData
        {
            if (lst != null)
            {
                Put(null, lst, proj);
            }
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

        public void CHECKBOX<V>(V[] v, Set<V> opt, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
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

        public void RADIO<V>(string name, V v, Set<V> opt, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
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

        public void FILE(string name, string Label = null, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (Required) Add(" required");
            Add(">");
            Add("</label>");
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

        public void BUTTON(ActionInfo actn)
        {
            Add("<button class=\"button primary\"");
            Add(" formaction=\""); Add(actn.Name); Add("\" formmethod=\"post\"");

            UiAttribute ui = actn.Ui;

            int modal = ui?.Modal ?? 0;
            if (modal > 0)
            {
                Add(" onclick=\"dialog(this,"); Add(modal); Add("); return false;\"");
            }

            StateAttribute state = actn.State;
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
            AddLabel(ui?.Label, actn.Name);

            Add("</button>");
        }

        public void BUTTONS(List<ActionInfo> actns)
        {
            for (int i = 0; i < actns.Count; i++)
            {
                ActionInfo act = actns[i];
                BUTTON(act);
            }
        }

        public void SELECT<V>(string name, V v, Set<V> opt, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
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

        public HtmlContent PutRaw(string name, string raw)
        {
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, bool v, Func<bool, string> Opt = null, string Label = null, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE_ROW:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    Add("<td>");
                    Add(v);
                    Add("</td>");
                    break;
                case CTX_LIST_ITEM:
                    break;
                case CTX_GRID_PANE:
                    break;
                case CTX_INPUT:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    CHECKBOX(name, v);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short v, Set<short> Opt = null, string Label = null, string Help = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
            var ctx = chain[level];
            switch (ctx.type)
            {
                case CTX_TABLE_ROW:
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
                case CTX_LIST_ITEM:
                    break;
                case CTX_INPUT:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, int v, Set<int> Opt = null, string Label = null, string Help = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE_ROW:

                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;

                    Add("<td style=\"text-align: right;\">");
                    if (formed && level == 1)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\">");
                    }
                    Add(v);
                    Add("</td>");
                    break;
                case CTX_LIST_ITEM:
                    break;
                case CTX_GRID_PANE:
                    break;
                case CTX_INPUT:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;

            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, long v, Set<long> Opt = null, string Label = null, string Help = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (chain[level].type)
            {
                case CTX_TABLE_ROW:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");

                    Add("<td style=\"text-align: right;\">");
                    if (formed && level == 1)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\">");
                    }
                    Add(v);
                    Add("</td>");
                    break;
                case CTX_LIST_ITEM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case CTX_GRID_PANE:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case CTX_INPUT:
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
                case CTX_TABLE_ROW:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_LIST_ITEM:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
                case CTX_GRID_PANE:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
                case CTX_INPUT:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, decimal v, string Label = null, string Help = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, JNumber v)
        {
            return this;
        }

        public HtmlContent Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, NpgsqlPoint v)
        {
            return this;
        }

        public HtmlContent Put(string name, char[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, string v, Set<string> Opt = null, string Label = null, string Help = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            var ctx = chain[level];
            switch (ctx.type)
            {
                case CTX_TABLE_ROW:
                    if (ctx.label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
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
                case CTX_LIST_ITEM:
                    Add(v);
                    break;
                case CTX_GRID_PANE:
                    Add(v);
                    break;
                case CTX_INPUT:
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

        public HtmlContent Put(string name, byte[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, ArraySegment<byte> v)
        {
            return this;
        }

        public HtmlContent Put(string name, JObj v)
        {
            return this;
        }

        public HtmlContent Put(string name, JArr v)
        {
            return this;
        }

        public HtmlContent Put(string name, short[] v, Set<short> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {

            return this;
        }

        public HtmlContent Put(string name, int[] v, Set<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, long[] v, Set<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, string[] v, Set<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
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
            var ctx = chain[level];
            switch (ctx.type)
            {
                case CTX_TABLE:
                    if (v != null)
                    {
                        level++;
                        chain[level].type = CTX_TABLE_ROW;

                        Add("<table class=\"hover\">");
                        Add("<thead>");

                        chain[level].label = true;

                        Add("<tr>");
                        for (int i = 0; i < v.Count; i++)
                        {
                            IData obj = v[i];
                            obj.WriteData(this, proj);
                        }
                        Add("</tr>");
                        Add("</thead>");

                        chain[level].label = false;
                        Add("<tbody>");

                        for (int i = 0; i < v.Count; i++)
                        {
                            Add("<tr>");
                            D obj = v[i];

                            chain[level].ordinal = 0; // reset ordical
                            obj.WriteData(this, proj);

                            Add("</tr>");
                        }
                        Add("</tbody>");
                        level--;
                    }
                    else
                    {
                        Add("<div class=\"row\">");
                        Add("<span>没有记录</span>");
                        Add("</div>");
                    }
                    break;
                case CTX_TABLE_ROW:
                    if (ctx.label)
                    {
                        Add("<th>");
                        AddLabel(Label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td>");
                        if (formed && level == 1)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }

                        // set type to LIST 
                        chain[++level].type = CTX_LIST;
                        Put(name, v, proj, Label, Help, ReadOnly, Required);
                        level--;

                        Add("</td>");
                    }
                    break;
                case CTX_LIST:
                    if (v != null)
                    {
                        Add("<ul>");
                        level++;
                        chain[level].type = CTX_LIST_ITEM;
                        for (int i = 0; i < v.Count; i++)
                        {
                            IData obj = v[i];
                            Add("<li>");
                            obj.WriteData(this, proj);
                            Add("</li>");
                        }
                        Add("</ul>");
                        level--;
                    }
                    else
                    {
                        Add("<div class=\"row\">");
                        Add("<span>没有记录</span>");
                        Add("</div>");
                    }
                    break;
                case CTX_GRID:
                    if (v != null)
                    {
                        Add("<ul>");
                        level++;
                        chain[level].type = CTX_GRID_PANE;
                        for (int i = 0; i < v.Count; i++)
                        {
                            IData obj = v[i];
                            Add("<li>");
                            obj.WriteData(this, proj);
                            Add("</li>");
                        }
                        Add("</ul>");
                        level--;
                    }
                    else
                    {
                        Add("<div class=\"row\">");
                        Add("<span>没有记录</span>");
                        Add("</div>");
                    }
                    break;
                case CTX_GRID_PANE:

                    // set type to TABLE 
                    chain[++level].type = CTX_TABLE;
                    Put(name, v, proj, Label, Help, ReadOnly, Required);
                    level--;
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, IDataInput v)
        {
            return this;
        }

        public HtmlContent Put<D>(string name, Map<D> v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            throw new NotImplementedException();
        }
    }

}