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
        public const sbyte

            CTX_THREAD = 1,

            CTX_TBODY = 2,

            CTX_GRIDTHEAD = 3,

            CTX_GRIDTBODY = 4,

            CTX_SHEET = 5,

            CTX_FORM = 7;

        internal sbyte ctx;

        internal int ordinal;

        // during data output
        int[] counts;
        int level; // current level

        public HtmlContent(bool sendable, bool pooled, int capacity = 16 * 1024) : base(sendable, pooled, capacity)
        {
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

        public void SHEET(IData obj, int proj = 0)
        {
        }

        public void SHEET(Action<HtmlContent> inner)
        {
        }

        public void FORM(WebAction action, Action<HtmlContent> inner)
        {
            Add("<form class=\"pure-form pure-g\">");

            inner?.Invoke(this);

            Add("</form>");
        }

        public void FORM(WebAction action, IData obj, int proj = 0)
        {
            Add("<form class=\"pure-form pure-g\">");

            ctx = CTX_FORM;

            // function buttuns
            Add("</form>");
        }

        public void GRID<D>(List<WebAction> actions, List<D> lst, int proj = 0) where D : IData
        {
            Add("<form>");

            // buttons
            if (actions != null)
            {
                BUTTONS(actions);
            }

            if (lst != null)
            {
                Add("<table class=\"hover\">");

                ctx = CTX_GRIDTHEAD;
                Add("<thead>");
                Add("<tr>");
                for (int i = 0; i < lst.Count; i++)
                {
                    IData obj = lst[i];
                    obj.WriteData(this, proj);
                }
                Add("</tr>");
                Add("</thead>");

                ctx = CTX_GRIDTBODY;
                Add("<tbody>");

                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<tr>");
                    D obj = lst[i];
                    ordinal = 0; // reset ordical
                    obj.WriteData(this, proj);
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

        public void GRID(List<WebAction> actions, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form>");

            // buttons
            BUTTONS(actions);

            if (input != null)
            {
                Add("<table class=\"hover\">");

                ctx = CTX_GRIDTHEAD;
                Add("<thead>");
                Add("<tr>");
                valve(input, this);
                Add("</tr>");
                Add("</thead>");

                ctx = CTX_GRIDTBODY;
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
                Add("<table class=\"hover\">");

                ctx = CTX_GRIDTHEAD;
                Add("<thead>");
                Add("<tr>");
                for (int i = 0; i < lst.Count; i++)
                {
                    IData obj = lst[i];
                    obj.WriteData(this, proj);
                }
                Add("</tr>");
                Add("</thead>");

                ctx = CTX_GRIDTBODY;
                Add("<tbody>");

                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<tr>");
                    D obj = lst[i];
                    ordinal = 0; // reset ordical
                    obj.WriteData(this, proj);
                    Add("</tr>");
                }
                Add("</tbody>");
            }
        }

        public void TABLE(IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            if (input != null)
            {
                Add("<table class=\"hover\">");

                ctx = CTX_GRIDTHEAD;
                Add("<thead>");
                Add("<tr>");
                valve(input, this);
                Add("</tr>");
                Add("</thead>");

                ctx = CTX_GRIDTBODY;
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

        public void TEXT(string name, string v, string Label = null, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Placeholder != null)
            {
                Add(" placeholder=\"");
                Add(Placeholder);
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

        public void TEXTPicker(string name, string v, string Label = null, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Placeholder != null)
            {
                Add(" placeholder=\"");
                Add(Placeholder);
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

        public void PASSWORD(string name, string v, string Label = null, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"password\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Placeholder != null)
            {
                Add(" placeholder=\"");
                Add(Placeholder);
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

        public void NUMBER<V>(string name, V v, string Label = null, string Placeholder = null, V Max = default(V), V Min = default(V), V Step = default(V), bool ReadOnly = false, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddConvert(v);
            Add("\"");

            if (Placeholder != null)
            {
                Add(" placeholder=\"");
                Add(Placeholder);
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

        public void CHECKBOX<V>(V[] v, IDictionary<V, string> options, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(Label, null);
            Add("</legend>");

            foreach (var pair in options)
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

        public void RADIO<V>(string name, V v, IDictionary<V, string> options, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(Label, name);
            Add("</legend>");

            foreach (var pair in options)
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

        public void TEXTAREA(string name, string v, string Label = null, string Placeholder = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (Placeholder != null)
            {
                Add(" placeholder=\"");
                Add(Placeholder);
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

        public void BUTTON(WebAction atn)
        {
            Add("<button");
            if (atn.Form == 0) Add(" class=\"button primary");
            Add("\" formaction=\"");
            Add(atn.Name);
            Add("\" formmethod=\"post\"");
            Add(">");
            string icon = atn.Icon;
            if (icon != null)
            {
                Add("<i class=\"");
                Add(icon);
                Add("\"></i>");
            }
            AddLabel(atn.Label, atn.Name);
            Add("</button>");
        }

        public void BUTTONS(List<WebAction> actions)
        {
            Add("<ul class=\"menu\">");
            for (int i = 0; i < actions.Count; i++)
            {
                WebAction atn = actions[i];
                Add("<li>");
                Add("<button");
                if (atn.Form == 0) Add(" class=\"button primary");
                Add("\" formaction=\""); Add(atn.Name);
                Add("\" formmethod=\"post\" onclick=\"dialog(); return false;\"");

                StateAttribute sta = atn.State;
                if (sta != null)
                {
                    Add(" data-if=\""); Add(sta.If); Add("\"");
                    Add(" data-unif=\""); Add(sta.Unif); Add("\"");
                }
                Add(">");
                string icon = atn.Icon;
                if (icon != null)
                {
                    Add("<i class=\"");
                    Add(icon);
                    Add("\"></i>");
                }
                AddLabel(atn.Label, atn.Name);
                Add("</button>");
                Add("</li>");
            }
            Add("</ul>");
        }

        public void SELECT<V>(string name, V v, IDictionary<V, string> options, string Label = null, bool Required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (Required) Add(" required");
            Add(">");

            foreach (var pair in options)
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
            ordinal++;
            return this;
        }

        public HtmlContent PutRaw(string name, string raw)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, bool v, Func<bool, string> Options = null, string Label = null, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    CHECKBOX(name, v);
                    Add("</div>");
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td>");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short v, IDictionary<short, string> Options = null, string Label = null, string Placeholder = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    if (ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\">");
                    }
                    if (Options != null)
                    {
                        Add(Options[v]);
                    }
                    else
                    {
                        Add(v);
                    }
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, int v, IDictionary<int, string> Options = null, string Label = null, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    if (ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\">");
                    }
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, long v, IDictionary<long, string> Options = null, string Label = null, string Placeholder = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    if (ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\">");
                    }
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, double v, string Label = null, string Placeholder = null, double Max = 0, double Min = 0, double Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    Add("</div>");
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, decimal v, string Label = null, string Placeholder = null, decimal Max = 0, decimal Min = 0, decimal Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    NUMBER(name, v);
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, JNumber v)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Number(name, v);
                    Add("</div>");
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, DateTime v, string Label = null, DateTime Max = default(DateTime), DateTime Min = default(DateTime), int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    DATE(name, v);
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, NpgsqlPoint v)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Date(name, v);
                    Add("</div>");
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, char[] v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, string v, IDictionary<string, string> Options = null, string Label = null, string Placeholder = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case CTX_FORM:
                    if (Label != null && Label.Length == 0)
                    {
                        HIDDEN(name, v);
                    }
                    else if (name.EndsWith("password"))
                    {
                        PASSWORD(name, v, Label, Placeholder, Pattern, (sbyte)Max, (sbyte)Min, ReadOnly, Required);
                    }
                    else if (Max < 128)
                    {
                        TEXT(name, v, Label, Placeholder, Pattern, (sbyte)Max, (sbyte)Min, ReadOnly, Required);
                    }
                    else
                    {
                        TEXTAREA(name, v, Label, Placeholder, Max, Min, ReadOnly, Required);
                    }
                    break;
                case CTX_GRIDTHEAD:
                    Add("<th>");
                    AddLabel(Label, name);
                    Add("</th>");
                    break;
                case CTX_GRIDTBODY:
                    Add("<td>");
                    if (ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\">");
                    }
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, byte[] v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, ArraySegment<byte> v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, JObj v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, JArr v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short[] v, IDictionary<short, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {

            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, int[] v, IDictionary<int, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, long[] v, IDictionary<long, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, string[] v, IDictionary<string, string> Options = null, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, Dictionary<string, string> v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, IData v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put<D>(string name, List<D> v, int proj = 0, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false) where D : IData
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, IDataInput v)
        {
            ordinal++;
            return this;
        }
    }

}