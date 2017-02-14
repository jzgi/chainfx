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

            THeadCtx = 1,

            TBodyCtx = 2,

            GridTHeadCtx = 3,

            GridTBodyCtx = 4,

            SheetCtx = 5,

            FormCtx = 7;

        internal sbyte ctx;

        internal int ordinal;


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

        public void TABLE(Action<HtmlContent> ths, Action<HtmlContent> trs)
        {
            table_idx = 0;

            Add("<table class=\"pure-table pure-table-bordered\">");
            Add("<thead>");
            Add("<tr>");

            ths(this);

            Add("</tr>");
            Add("</thead>");
            Add("<tbody>");

            trs(this);

            Add("</tbody>");
            Add("</table>");
        }


        public void TR(Action<int, HtmlContent> tds)
        {
            table_idx++;

            T("<tr>");

            tds(table_idx, this);

            T("</tr>");
        }

        public void TD(int v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void TD(string v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void TD(DateTime v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

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

        public void FORM(WebAction action, Action<HtmlContent> inner)
        {
            Add("<form class=\"pure-form pure-g\">");

            inner?.Invoke(this);

            Add("</form>");
        }

        public void FORM_grid<D>(List<WebAction> actions, List<D> list, int proj = 0) where D : IData
        {
            Add("<form>");

            // buttons
            BUTTONS(actions);

            if (list != null)
            {
                Add("<table>");

                ctx = GridTHeadCtx;
                Add("<thead>");
                Add("<tr>");
                for (int i = 0; i < list.Count; i++)
                {
                    IData obj = list[i];
                    obj.WriteData(this, proj);
                }
                Add("</tr>");
                Add("</thead>");

                ctx = GridTBodyCtx;
                Add("<tbody>");

                for (int i = 0; i < list.Count; i++)
                {
                    Add("<tr>");
                    D obj = list[i];
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

        public void FORM(WebAction action, IData obj, int proj = 0)
        {
            Add("<form class=\"pure-form pure-g\">");

            ctx = FormCtx;

            // function buttuns


            Add("</form>");
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
                Add(" placedholder=\"");
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
                Add(" placedholder=\"");
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
                Add(" placedholder=\"");
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
                Add(" placedholder=\"");
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
                Add(" placedholder=\"");
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
            Add("<button type=\"button\"");
            if (atn.Form == 0) Add(" class=\"button secondary");
            Add("\" formaction=\"");
            Add(atn.Name);
            Add("\" formmethod=\"");
            Add(atn.Form == 0 ? "get" : "post");
            if (atn.Dialog != 0)
            {
                Add("\" onclick=\"dialog(this,");
                Add("); return false;");
            }
            Add("\">");
            string icon = atn.Icon;
            if (icon != null)
            {
                Add("<i class=\"");
                Add(icon);
                Add("\"></i>");
            }
            Add(" ");
            AddLabel(atn.Label, atn.Name);
            Add(" </button>");
        }

        public void BUTTONS(params WebAction[] atns)
        {
            for (int i = 0; i < atns.Length; i++)
            {
                WebAction atn = atns[i];
                BUTTON(atn);
            }
        }

        public void BUTTONS(List<WebAction> atns)
        {
            Add("<div class=\"row\">");
            for (int i = 0; i < atns.Count; i++)
            {
                WebAction atn = atns[i];
                BUTTON(atn);
            }
            Add("</div>");
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

        public HtmlContent Put(string name, bool v, string Label = null, bool Required = false, Func<bool, string> Options = null)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    CHECKBOX(name, v);
                    Add("</div>");
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
                    Add("<td>");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short v, string Label = null, string Placeholder = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false, IDictionary<short, string> Options = null)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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

        public HtmlContent Put(string name, int v, string Label = null, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false, IDictionary<int, string> Options = null)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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

        public HtmlContent Put(string name, long v, string Label = null, string Placeholder = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false, IDictionary<long, string> Options = null)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    Add("</div>");
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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
                case FormCtx:
                    NUMBER(name, v);
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Number(name, v);
                    Add("</div>");
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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
                case FormCtx:
                    DATE(name, v);
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Date(name, v);
                    Add("</div>");
                    break;
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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

        public HtmlContent Put(string name, string v, string Label = null, string Placeholder = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false, IDictionary<string, string> Options = null)
        {
            switch (ctx)
            {
                case FormCtx:
                    if (name.EndsWith("password"))
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
                case GridTHeadCtx:
                    Add("<th>");
                    AddLabel(Label, name);
                    Add("</th>");
                    break;
                case GridTBodyCtx:
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

        public HtmlContent Put(string name, short[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<short, string> Options = null)
        {

            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, int[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<int, string> Options = null)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, long[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<long, string> Options = null)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, string[] v, string Label = null, string Placeholder = null, bool ReadOnly = false, bool Required = false, IDictionary<string, string> Options = null)
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