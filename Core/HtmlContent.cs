using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// For dynamical HTML5 content tooled with Zurb Foundation
    ///
    public class HtmlContent : DynamicContent, IDataOutput<HtmlContent>, ISelectOptions
    {
        const string Primary = "primary", Secondary = "secondary", Success = "success", Alert = "alert", Warning = "warning";

        public const sbyte
            GridTheadCtx = 1,
            GridTbodyCtx = 2, // grid
            SheetTrs = 2, // property sheet
            FormCtx = 3;

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

        public void FORM_grid<D>(List<WebAction> actions, List<D> list, ushort proj = 0) where D : IData
        {
            Add("<form>");

            // buttons
            BUTTONS(actions);

            if (list != null)
            {
                Add("<table>");

                ctx = GridTheadCtx;
                Add("<thead>");
                Add("<tr>");
                for (int i = 0; i < list.Count; i++)
                {
                    IData obj = list[i];
                    obj.WriteData(this, proj);
                }
                Add("</tr>");
                Add("</thead>");

                ctx = GridTbodyCtx;
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

        public void FORM(WebAction action, IData obj, ushort proj = 0)
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

        public void TEXT(string name, string v, string Label = null, bool Pick = false, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
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

            if (Pick)
            {
                Add("<input type=\"button\" onclick=\"\"");
            }
            Add("</label>");
        }

        public void TEL(string name, string v, string Label = null, bool Pick = false, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"tel\" name=\"");
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

            if (Pick)
            {
                Add("<input type=\"button\" onclick=\"\"");
            }
            Add("</label>");
        }

        public void URL(string name, string v, string Label = null, bool Pick = false, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"url\" name=\"");
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

            if (Pick)
            {
                Add("<input type=\"button\" onclick=\"\"");
            }
            Add("</label>");
        }

        public void EMAIL(string name, string v, string Label = null, bool Pick = false, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<input type=\"email\" name=\"");
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

            if (Pick)
            {
                Add("<input type=\"button\" onclick=\"\"");
            }
            Add("</label>");
        }

        public void PASSWORD(string name, string v, string Label = null, bool Pick = false, string Placeholder = null, string Pattern = null, sbyte Max = 0, sbyte Min = 0, bool ReadOnly = false, bool Required = false)
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

            if (Pick)
            {
                Add("<input type=\"button\" onclick=\"\"");
            }
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

        public void NUMBER(string name, int v, string Label = null, bool Pick = false, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (Placeholder != null)
            {
                Add(" placedholder=\"");
                Add(Placeholder);
                Add("\"");
            }
            if (Max != 0)
            {
                Add(" max=\"");
                Add(Max);
                Add("\"");
            }
            if (Min != 0)
            {
                Add(" min=\"");
                Add(Min);
                Add("\"");
            }
            if (Step != 0)
            {
                Add(" step=\"");
                Add(Step);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");

            Add(">");
            Add("</label>");
        }


        public void NUMBER(string name, long v, string Label = null, bool Pick = false, string Placeholder = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (Placeholder != null)
            {
                Add(" placedholder=\"");
                Add(Placeholder);
                Add("\"");
            }
            if (Max != 0)
            {
                Add(" max=\"");
                Add(Max);
                Add("\"");
            }
            if (Min != 0)
            {
                Add(" min=\"");
                Add(Min);
                Add("\"");
            }
            if (Step != 0)
            {
                Add(" step=\"");
                Add(Step);
                Add("\"");
            }
            if (ReadOnly) Add(" readonly");
            if (Required) Add(" required");

            Add(">");
            Add("</label>");
        }

        public void NUMBER(string name, decimal v, string Label = null, bool Pick = false, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
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

        public void RADIO(string name, string[] values, int Checked = 0, string Label = null, bool Required = false)
        {
            Add("<fieldset>");
            Add("<legend>");
            AddLabel(Label, name);
            Add("</legend>");
            for (int i = 0; i < values.Length; i++)
            {
                Add("<label>");
                Add("<input type=\"radio\" name=\"");
                Add(name);
                Add("\" value=\"");
                Add(i);
                Add("\"");
                if (Checked == i) Add(" checked");
                if (Required) Add(" required");
                Add("\">");
                Add(values[i]);
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

        public void SELECT(string name, string[] options, int selected = -1, string Label = null, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (Required) Add(" required");
            Add("\">");

            for (int i = 0; i < options.Length; i++)
            {
                string opt = options[i];
                Add("<option value=\"");
                Add(opt);
                Add("\"");
                if (selected == i) Add(" selected");
                Add("\">");
                Add(opt);
                Add("</option>");
            }
            Add("</label>");
        }

        public void SELECT<V>(string name, IOption<V>[] options, string Label = null, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (Required) Add(" required");
            Add("\">");

            for (int i = 0; i < options.Length; i++)
            {
                IOption<V> opt = options[i];
                Add("<option value=\"");
                Add(opt.ToString());
                Add("\"");
                if (opt.IsOn) Add(" selected");
                Add("\">");
                Add(opt.Label);
                Add("</option>");
            }
            Add("</label>");
        }

        public void SELECT(string name, Action<ISelectOptions> options, string Label = null, bool Required = false)
        {
            Add("<label>");
            AddLabel(Label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (Required) Add(" required");
            Add("\">");
            options(this);
            Add("</label>");
        }

        public void OPTION(string text, string value, bool selected = false)
        {
            Add("<option value=\"");
            Add(value);
            Add("\"");
            if (selected) Add(" selected");
            Add("\">");
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

        public HtmlContent Put(string name, bool v, string Label = null, bool Required = false)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    CHECKBOX(name, v);
                    Add("</div>");
                    break;
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
                    Add("<td>");
                    Add(v);
                    Add("</td>");
                    break;
            }
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short v, string Label = null, bool Pick = false, string Placeholder = null, short Max = 0, short Min = 0, short Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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

        public HtmlContent Put(string name, int v, string Label = null, bool Pick = false, string Placeholder = null, int Max = 0, int Min = 0, int Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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

        public HtmlContent Put(string name, long v, string Label = null, bool Pick = false, string Placeholder = null, long Max = 0, long Min = 0, long Step = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case FormCtx:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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

        public HtmlContent Put(string name, string v, string Label = null, bool Pick = false, string Placeholder = null, string Pattern = null, short Max = 0, short Min = 0, bool ReadOnly = false, bool Required = false)
        {
            switch (ctx)
            {
                case FormCtx:
                    if (name.EndsWith("password"))
                    {
                        PASSWORD(name, v, Label, Pick, Placeholder, Pattern, (sbyte)Max, (sbyte)Min, ReadOnly, Required);
                    }
                    else if (Max < 128)
                    {
                        TEXT(name, v, Label, Pick, Placeholder, Pattern, (sbyte)Max, (sbyte)Min, ReadOnly, Required);
                    }
                    else
                    {
                        TEXTAREA(name, v, Label, Placeholder, Max, Min, ReadOnly, Required);
                    }
                    break;
                case GridTheadCtx:
                    Add("<th>");
                    AddLabel(Label, name);
                    Add("</th>");
                    break;
                case GridTbodyCtx:
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

        public HtmlContent Put(string name, short[] v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, int[] v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, long[] v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, string[] v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, IData v, ushort proj = 0)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, ushort proj = 0) where D : IData
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put<D>(string name, List<D> v, ushort proj = 0) where D : IData
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, IDataInput v)
        {
            ordinal++;
            return this;
        }

        public HtmlContent Put(string name, Dictionary<string, string> v)
        {
            ordinal++;
            return this;
        }
    }


    public interface ITableThead
    {
        void thead(string text, string href = "#");
    }

    public interface ISelectOptions
    {
        void OPTION(string label, string value, bool selected = false);
    }

    public interface IOption<V>
    {
        string Label { get; }

        V Value { get; }

        bool IsOn { get; }
    }
}