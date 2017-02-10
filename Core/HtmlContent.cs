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
        const string SM = "sm", MD = "md", LG = "lg", XL = "xl";

        const sbyte
            GridThs = 1,
            GridTrs = 2, // grid
            SheetTrs = 2, // property sheet
            FormFields = 3;

        sbyte ctx;


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

        public void FORM_grid<D>(WebAction[] actions, D[] arr, ushort proj = 0) where D : IData
        {
            Add("<form>");

            // buttons
            BUTTONS(actions);

            if (arr != null)
            {
                Add("<table>");

                ctx = GridThs;
                Add("<thead>");
                Add("<tr>");

                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<th>");
                    D obj = arr[i];

                    obj.WriteData(this, proj);
                    Add("</th>");
                }
                Add("</tr>");
                Add("</thead>");

                ctx = GridTrs;
                Add("<tbody>");

                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<tr>");
                    D obj = arr[i];

                    obj.WriteData(this, proj);
                    Add("</tr>");
                }
                Add("</tbody>");
            }

            Add("</form>");
        }

        public void FORM(WebAction action, IData obj, ushort proj = 0)
        {
            Add("<form class=\"pure-form pure-g\">");

            ctx = FormFields;

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

        public void TEXT(string name, string value, Ui<sbyte>? ui = null)
        {
            Textual("text", name, value, ui);
        }

        public void SEARCH(string name, string value, Ui<sbyte>? ui = null)
        {
            Textual("search", name, value, ui);
        }

        public void TEL(string name, string value, Ui<sbyte>? ui = null)
        {
            Textual("tel", name, value, ui);
        }

        public void URL(string name, string value, Ui<sbyte>? ui = null)
        {
            Textual("url", name, value, ui);
        }

        public void EMAIL(string name, string value, Ui<sbyte>? ui = null)
        {
            Textual("email", name, value, ui);
        }

        public void PASSWORD(string name, string value, Ui<sbyte>? ui = null)
        {
            Textual("password", name, value, ui);
        }

        void Textual(string type, string name, string value, Ui<sbyte>? ui = null)
        {
            Add("<label>");
            AddLabel(ui?.Label, name);
            Add("<input type=\"");
            Add(type);
            Add("\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\"");
            if (ui != null)
            {
                if (ui.Value.ReadOnly) Add(" readonly");
                if (ui.Value.Required) Add(" required");
                string ph = ui.Value.Placeholder;
                if (ph != null)
                {
                    Add(" placedholder=\"");
                    Add(ph);
                    Add("\"");
                }
                sbyte max = ui.Value.Max;
                if (max > 0)
                {
                    Add(" maxlength=\"");
                    Add(max);
                    Add("\"");
                    Add(" size=\"");
                    Add(max);
                    Add("\"");
                }
                sbyte min = ui.Value.Max;
                if (min > 0)
                {
                    Add(" minlength=\"");
                    Add(min);
                    Add("\"");
                }
                string pat = ui.Value.Pattern;
                if (pat != null)
                {
                    Add(" pattern=\"");
                    AddEsc(pat);
                    Add("\"");
                }
            }
            Add(">");
            Add("</label>");
        }

        public void DATE(string name, DateTime v, Ui<DateTime>? ui = null)
        {
            Add("<label>");
            AddLabel(ui?.Label, name);
            Add("<input type=\"date\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (ui != null)
            {
                if (ui.Value.ReadOnly) Add(" readonly");
                if (ui.Value.Required) Add(" required");
                string ph = ui.Value.Placeholder;
                if (ph != null)
                {
                    Add(" placedholder=\"");
                    Add(ph);
                    Add("\"");
                }
                DateTime max = ui.Value.Max;
                if (max != default(DateTime))
                {
                    Add(" max=\"");
                    Add(max);
                    Add("\"");
                }
                DateTime min = ui.Value.Max;
                if (min != default(DateTime))
                {
                    Add(" min=\"");
                    Add(min);
                    Add("\"");
                }
                int step = ui.Value.Step;
                if (step != 0)
                {
                    Add(" step=\"");
                    Add(step);
                    Add("\"");
                }
            }
            Add(">");
            Add("</label>");
        }

        public void TIME()
        {
            T("</tbody>");
        }

        public void NUMBER(string name, int v, Ui<int>? ui = null)
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (ui != null)
            {
                if (ui.Value.ReadOnly) Add(" readonly");
                if (ui.Value.Required) Add(" required");
                string ph = ui.Value.Placeholder;
                if (ph != null)
                {
                    Add(" placedholder=\"");
                    Add(ph);
                    Add("\"");
                }
                int max = ui.Value.Max;
                if (max != 0)
                {
                    Add(" max=\"");
                    Add(max);
                    Add("\"");
                }
                int min = ui.Value.Min;
                if (min != 0)
                {
                    Add(" min=\"");
                    Add(min);
                    Add("\"");
                }
                int step = ui.Value.Step;
                if (step != 0)
                {
                    Add(" step=\"");
                    Add(step);
                    Add("\"");
                }
            }
            Add(">");
            Add("</label>");
        }

        public void NUMBER(string name, long v, Ui<long>? ui = null)
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (ui != null)
            {
                if (ui.Value.ReadOnly) Add(" readonly");
                if (ui.Value.Required) Add(" required");
                string ph = ui.Value.Placeholder;
                if (ph != null)
                {
                    Add(" placedholder=\"");
                    Add(ph);
                    Add("\"");
                }
                long max = ui.Value.Max;
                if (max != 0)
                {
                    Add(" max=\"");
                    Add(max);
                    Add("\"");
                }
                long min = ui.Value.Min;
                if (min != 0)
                {
                    Add(" min=\"");
                    Add(min);
                    Add("\"");
                }
                int step = ui.Value.Step;
                if (step != 0)
                {
                    Add(" step=\"");
                    Add(step);
                    Add("\"");
                }
            }
            Add(">");
            Add("</label>");
        }

        public void NUMBER(string name, decimal v, Ui<decimal>? ui = null)
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (ui != null)
            {
                if (ui.Value.ReadOnly) Add(" readonly");
                if (ui.Value.Required) Add(" required");
                string ph = ui.Value.Placeholder;
                if (ph != null)
                {
                    Add(" placedholder=\"");
                    Add(ph);
                    Add("\"");
                }
                decimal max = ui.Value.Max;
                if (max != 0)
                {
                    Add(" max=\"");
                    Add(max);
                    Add("\"");
                }
                decimal min = ui.Value.Min;
                if (min != 0)
                {
                    Add(" min=\"");
                    Add(min);
                    Add("\"");
                }
                decimal step = ui.Value.Step;
                if (step != 0)
                {
                    Add(" step=\"");
                    Add(step);
                    Add("\"");
                }
            }
            Add(">");
            Add("</label>");
        }

        public void NUMBER(string name, double v, Ui<double>? ui = null)
        {
            Add("<label>");
            AddLabel(null, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");
            if (ui != null)
            {
                if (ui.Value.ReadOnly) Add(" readonly");
                if (ui.Value.Required) Add(" required");
                string ph = ui.Value.Placeholder;
                if (ph != null)
                {
                    Add(" placedholder=\"");
                    Add(ph);
                    Add("\"");
                }
                double max = ui.Value.Max;
                if (max != 0)
                {
                    Add(" max=\"");
                    Add(max);
                    Add("\"");
                }
                double min = ui.Value.Min;
                if (min != 0)
                {
                    Add(" min=\"");
                    Add(min);
                    Add("\"");
                }
                int step = ui.Value.Step;
                if (step != 0)
                {
                    Add(" step=\"");
                    Add(step);
                    Add("\"");
                }
            }
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

        public void INPUT_button()
        {
            T("</tbody>");
        }

        public void TEXTAREA()
        {
            T("</tbody>");
        }

        public void BUTTON(WebAction atn)
        {
            Add("<button class=\"pure-button");
            if (atn.Form == 0) Add(" pure-button-primary");
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

        public void BUTTONS(WebAction[] was)
        {
            for (int i = 0; i < was.Length; i++)
            {
                WebAction wa = was[i];
                BUTTON(wa);
            }
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


        public HtmlContent PutEnter(bool multi)
        {
            throw new NotImplementedException();
        }

        public HtmlContent PutExit(bool multi)
        {
            throw new NotImplementedException();
        }

        public HtmlContent PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public HtmlContent PutRaw(string name, string raw)
        {
            throw new NotImplementedException();
        }

        public HtmlContent Put(string name, bool v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    CHECKBOX(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td>");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, short v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, int v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, long v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, double v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, decimal v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    NUMBER(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, JNumber v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Number(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, DateTime v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    DATE(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, NpgsqlPoint v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Date(name, v);
                    Add("</div>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(null, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, char[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, string v, Ui<short>? ctl = null)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<label>");
                    AddLabel(ctl?.Label, name);
                    TEXT(name, v);
                    Add("</label>");
                    break;
                case GridThs:
                    Add("<th>");
                    AddLabel(ctl?.Label, name);
                    Add("</th>");
                    break;
                case GridTrs:
                    Add("<td>");
                    Add(v);
                    Add("</td>");
                    break;
            }
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

        public HtmlContent Put(string name, short[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, int[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, long[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, string[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, IData v, ushort proj = 0)
        {
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, ushort proj = 0) where D : IData
        {
            return this;
        }

        public HtmlContent Put<D>(string name, List<D> v, ushort proj = 0) where D : IData
        {
            throw new NotImplementedException();
        }

        public HtmlContent Put(string name, IDataInput v)
        {
            throw new NotImplementedException();
        }

        public HtmlContent PutEnter()
        {
            throw new NotImplementedException();
        }

        public HtmlContent PutExit()
        {
            throw new NotImplementedException();
        }

        public HtmlContent Put(string name, Dictionary<string, string> v)
        {
            throw new NotImplementedException();
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