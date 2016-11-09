using System;
using System.Collections.Generic;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// For dynamical HTML5 content generation Tooled with Pure.CSS
    /// </summary> 
    ///
    public class HtmlContent : DynamicContent, ISink<HtmlContent>, IMenu, ISelectOptions
    {

        const int InitialCapacity = 8 * 1024;

        const string SM = "sm", MD = "md", LG = "lg", XL = "xl";

        const sbyte TableThs = 1, TableTrs = 2, FormFields = 3;

        sbyte ctx;


        public HtmlContent(bool raw, bool pooled, int capacity = InitialCapacity) : base(raw, pooled, capacity)
        {
        }

        public override string Type => "text/html; charset=utf-8";


        public Dictionary<string, string> Map { get; set; }


        public void AddLabel(string key)
        {
            string lbl;
            if (Map != null && Map.TryGetValue(key, out lbl)) // translate
            {
                Add(lbl);
            }
            else // key to uppercase
            {
                for (int i = 0; i < key.Length; i++)
                {
                    char c = key[i];
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


        public void grid()
        {
            T("<div class=\"pure-g\">");

            T("<div class=\"pure-u-1-2 l-box\"> ... </div>");
            T("<div class=\"pure-u-1-2 l-box\"> ... </div>");

            T("</div>");
        }

        public void A_button(int size, int active_primary_diabled, WebAction wa, string subscpt = null)
        {
            T("<a class=\"pure-button\">");
        }

        public void A_link(WebAction wa, string subscpt)
        {
            T("<a class=\"mdl-dialog\">");
        }

        public void MENU(string heading, Action<IMenu> items)
        {
            T("<nav class=\"pure-menu\" style=\"display: inline-block;\">");
            if (heading != null)
            {
                T("<span class=\"pure-menu-heading\">").T(heading).T("</span>");
            }
            T("<ul class=\"pure-menu-list\">");
            items(this);
            T("</ul>");
            T("</nav>");
        }

        public void menuitem(string text, string href = "#")
        {
            T("<li class=\"pure-menu-item\"><a href=\"").T(href).T("\" class=\"pure-menu-link\">").T(text).T("</a></li>");
        }

        public void MENU_horizontal(string heading, Action<IMenu> items)
        {
            T("<nav class=\"pure-menu pure-menu-horizontal\">");
            if (heading != null)
            {
                T("<span class=\"pure-menu-heading\">").T(heading).T("</span>");
            }
            T("<ul class=\"pure-menu-list\">");
            items(this);
            T("</ul>");
            T("</nav>");
        }

        public void dialog(string h, Action content)
        {
            T("<dialog class=\"mdl-dialog\">");
            T("<h4 class=\"mdl-dialog__title\">").T(h).T("</h4>");

            T("<div class=\"mdl-dialog__content\">");

            T("<div class=\"mdl-dialog__actions\">");
            T("<button type=\"button\" class=\"mdl-button\">Agree</button>");
            T("<button type=\"button\" class=\"mdl-button close\">Disagree</button>");
            T("</div>");

            T("</div>");
            T("</dialog>");

            T("<script>");
            T("</script>");
        }


        int table_idx;

        public void table(Action<HtmlContent> ths, Action<HtmlContent> trs)
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


        public void tr(Action<int, HtmlContent> tds)
        {
            table_idx++;

            T("<tr>");

            tds(table_idx, this);

            T("</tr>");
        }

        public void td(int v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void td(string v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void td(DateTime v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void table<B>(B[] arr, byte z = 0) where B : IBean
        {
            B obj = arr[0];

            table(
                ths =>
                {
                    ctx = TableThs;
                    obj.Dump(this, z);
                },
                trs =>
                {
                    ctx = TableTrs;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i].Dump(this, z);
                    }
                }
            );
        }


        public void form(WebAction wa, Action<HtmlContent> inner)
        {
            Add("<form class=\"pure-form pure-g\">");

            inner?.Invoke(this);

            Add("</form>");
        }

        public void form<B>(WebAction[] was, B[] beans, byte z = 0) where B : IBean
        {
            Add("<form class=\"pure-form pure-g\">");

            // buttons
            buttonlst(was);

            if (beans != null)
            {
                table(beans, z);
            }

            Add("</form>");
        }

        public void form<B>(WebAction wa, B bean, byte z = 0) where B : IBean
        {
            Add("<form class=\"pure-form pure-g\">");

            ctx = FormFields;

            bean.Dump(this);

            // function buttuns


            Add("</form>");
        }

        public void input_hidden(string name, string value)
        {
            Add("<input type=\"hidden\" name=\""); Add(name); Add("\" value=\""); AddEsc(value);
            Add("\">");
        }

        public void input_text(string name, string value, bool @readonly = false, bool required = false, string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"text\" name=\""); Add(name); Add("\" value=\""); AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (maxlength > 0)
            {
                Add(" maxlength=\""); Add(maxlength); Add("\"");
                Add(" size=\""); Add(maxlength); Add("\"");
            }
            if (minlength > 0) { Add(" minlength=\""); Add(minlength); Add("\""); }
            if (pattern != null) { Add(" pattern=\""); AddEsc(pattern); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_search(string name, string value, bool @readonly = false, bool required = false, string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"search\" name=\""); Add(name); Add("\" value=\""); AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (maxlength > 0)
            {
                Add(" maxlength=\""); Add(maxlength); Add("\"");
                Add(" size=\""); Add(maxlength); Add("\"");
            }
            if (minlength > 0) { Add(" minlength=\""); Add(minlength); Add("\""); }
            if (pattern != null) { Add(" pattern=\""); AddEsc(pattern); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_tel(string name, string value, bool @readonly = false, bool required = false, string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"search\" name=\""); Add(name); Add("\" value=\""); AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (maxlength > 0)
            {
                Add(" maxlength=\""); Add(maxlength); Add("\"");
                Add(" size=\""); Add(maxlength); Add("\"");
            }
            if (minlength > 0) { Add(" minlength=\""); Add(minlength); Add("\""); }
            if (pattern != null) { Add(" pattern=\""); AddEsc(pattern); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_url(string name, string value, bool @readonly = false, bool required = false, string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"url\" name=\""); Add(name); Add("\" value=\""); AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (maxlength > 0)
            {
                Add(" maxlength=\""); Add(maxlength); Add("\"");
                Add(" size=\""); Add(maxlength); Add("\"");
            }
            if (minlength > 0) { Add(" minlength=\""); Add(minlength); Add("\""); }
            if (pattern != null) { Add(" pattern=\""); AddEsc(pattern); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_email(string name, string value, bool @readonly = false, bool required = false, string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"email\" name=\""); Add(name); Add("\" value=\""); AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (maxlength > 0)
            {
                Add(" maxlength=\""); Add(maxlength); Add("\"");
                Add(" size=\""); Add(maxlength); Add("\"");
            }
            if (minlength > 0) { Add(" minlength=\""); Add(minlength); Add("\""); }
            if (pattern != null) { Add(" pattern=\""); AddEsc(pattern); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_password(string name, string value, bool @readonly = false, bool required = false, string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"password\" name=\""); Add(name); Add("\" value=\""); AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (maxlength > 0)
            {
                Add(" maxlength=\""); Add(maxlength); Add("\"");
                Add(" size=\""); Add(maxlength); Add("\"");
            }
            if (minlength > 0) { Add(" minlength=\""); Add(minlength); Add("\""); }
            if (pattern != null) { Add(" pattern=\""); AddEsc(pattern); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_date(string name, DateTime value, bool @readonly = false, bool required = false, string placeholder = null, int max = int.MaxValue, int min = int.MinValue, int step = 0)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"date\" name=\""); Add(name); Add("\" value=\""); Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (max != int.MaxValue) { Add(" max=\""); Add(max); Add("\""); }
            if (min != int.MinValue) { Add(" min=\""); Add(min); Add("\""); }
            if (step != 0) { Add(" step=\""); Add(step); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_time()
        {
            T("</tbody>");

        }

        public void input_number(string name, int value, bool @readonly = false, bool required = false, string placeholder = null, int max = int.MaxValue, int min = int.MinValue, int step = 0)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"number\" name=\""); Add(name); Add("\" value=\""); Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (max != int.MaxValue) { Add(" max=\""); Add(max); Add("\""); }
            if (min != int.MinValue) { Add(" min=\""); Add(min); Add("\""); }
            if (step != 0) { Add(" step=\""); Add(step); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_number(string name, long value, bool @readonly = false, bool required = false, string placeholder = null, long max = long.MaxValue, long min = long.MinValue, long step = 0)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"number\" name=\""); Add(name); Add("\" value=\""); Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (max != long.MaxValue) { Add(" max=\""); Add(max); Add("\""); }
            if (min != long.MinValue) { Add(" min=\""); Add(min); Add("\""); }
            if (step != 0) { Add(" step=\""); Add(step); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_number(string name, decimal value, bool @readonly = false, bool required = false, string placeholder = null, int max = int.MaxValue, int min = int.MinValue, int step = 0)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"number\" name=\""); Add(name); Add("\" value=\""); Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null) { Add(" placedholder=\""); AddLabel(placeholder); Add("\""); }
            if (max != int.MaxValue) { Add(" max=\""); Add(max); Add("\""); }
            if (min != int.MinValue) { Add(" min=\""); Add(min); Add("\""); }
            if (step != 0) { Add(" step=\""); Add(step); Add("\""); }
            Add("\">");
            Add("</label>");
        }

        public void input_range()
        {
            T("</tbody>");

        }

        public void input_color()
        {
            T("</tbody>");

        }

        public void input_checkbox(string name, bool value, bool required = false)
        {
            Add("<label>"); AddLabel(name);
            Add("<input type=\"checkbox\" name=\""); Add(name); Add("\" value=\""); Add(value);
            if (value) Add(" checked");
            if (required) Add(" required");
            Add("\">");
            Add("</label>");
        }

        public void input_radio(string name, string[] values, int @checked = 0, bool required = false)
        {
            Add("<fieldset>");
            Add("<legend>"); AddLabel(name); Add("</legend>");
            for (int i = 0; i < values.Length; i++)
            {
                Add("<label>");
                Add("<input type=\"radio\" name=\""); Add(name); Add("\" value=\""); Add(i);
                if (@checked == i) Add(" checked");
                if (required) Add(" required");
                Add("\">"); Add(values[i]);
                Add("</label>");
            }
            Add("</fieldset>");
        }

        public void input_file()
        {
            T("</tbody>");

        }

        public void input_button()
        {
            T("</tbody>");

        }

        public void textarea()
        {
            T("</tbody>");

        }

        public void button(WebAction wa)
        {
            Add("<button class=\"pure-button");
            if (!wa.IsGet) Add(" pure-button-primary");
            Add("\" formaction=\""); Add(wa.Key);
            Add("\" formmethod=\""); Add(wa.IsGet ? "get" : "post");
            if (wa.Dialog > 0)
            {
                Add("\" onclick=\"dialog(this,"); Add(wa.Dialog); Add("); return false;");
            }
            Add("\">");
            string icon = wa.Icon;
            if (icon != null)
            {
                Add("<i class=\""); Add(icon); Add("\"></i>");
            }
            Add(" ");
            AddLabel(wa.Key);
            Add(" </button>");
        }

        public void buttonlst(WebAction[] was)
        {
            for (int i = 0; i < was.Length; i++)
            {
                WebAction wa = was[i];
                button(wa);
            }
        }

        public void button_submit(string value)
        {
            Add("<button type=\"submit\">");
            AddLabel(value);
            Add("</button>");
        }

        public void button_reset(string value)
        {
            Add("<button type=\"reset\" class=\"pure-button pure-button-secondary\">");
            AddLabel(value);
            Add("</button>");
        }

        public void select(string name, string[] options, int selected = -1, bool required = false)
        {
            Add("<label>"); AddLabel(name);
            Add("<select name=\""); Add(name); Add("\"");
            if (required) Add(" required");
            Add("\">");

            for (int i = 0; i < options.Length; i++)
            {
                string opt = options[i];
                Add("<option value=\""); Add(opt); Add("\"");
                if (selected == i) Add(" selected");
                Add("\">"); AddLabel(opt);
                Add("</option>");
            }
            Add("</label>");
        }

        public void select<V>(string name, IOption<V>[] options, bool required = false)
        {
            Add("<label>"); AddLabel(name);
            Add("<select name=\""); Add(name); Add("\"");
            if (required) Add(" required");
            Add("\">");

            for (int i = 0; i < options.Length; i++)
            {
                IOption<V> opt = options[i];
                Add("<option value=\""); Add(opt.ToString()); Add("\"");
                if (opt.IsOn) Add(" selected");
                Add("\">"); AddLabel(opt.Label);
                Add("</option>");
            }
            Add("</label>");
        }

        public void select(string name, Action<ISelectOptions> options, bool required = false)
        {
            Add("<label>"); AddLabel(name);
            Add("<select name=\""); Add(name); Add("\"");
            if (required) Add(" required");
            Add("\">");
            options(this);
            Add("</label>");
        }

        public void option(string text, string value, bool selected = false)
        {
            Add("<option value=\""); Add(value); Add("\"");
            if (selected) Add(" selected");
            Add("\">");
        }

        public void datalist()
        {
            T("</tbody>");

        }

        public void progres()
        {
            T("</tbody>");

        }

        public void meter()
        {
            T("</tbody>");

        }


        //
        // ISINK
        //

        public HtmlContent PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public HtmlContent Put(string name, bool v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    input_checkbox(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>"); AddLabel(name); Add("</th>");
                    break;
                case TableTrs:
                    Add("<td>"); Add(v); Add("</td>");
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
                    input_number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>"); AddLabel(name); Add("</th>");
                    break;
                case TableTrs:
                    Add("<td style=\"text-align: right;\">"); Add(v); Add("</td>");
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
                    input_number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>"); AddLabel(name); Add("</th>");
                    break;
                case TableTrs:
                    Add("<td style=\"text-align: right;\">"); Add(v); Add("</td>");
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
                    input_number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>"); AddLabel(name); Add("</th>");
                    break;
                case TableTrs:
                    Add("<td style=\"text-align: right;\">"); Add(v); Add("</td>");
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
                    input_number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>"); AddLabel(name); Add("</th>");
                    break;
                case TableTrs:
                    Add("<td style=\"text-align: right;\">"); Add(v); Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, Number v)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtmlContent Put(string name, DateTime v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    input_date(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>"); AddLabel(name); Add("</th>");
                    break;
                case TableTrs:
                    Add("<td style=\"text-align: right;\">"); Add(v); Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, char[] v)
        {
            return this;
        }

        public HtmlContent Put(string name, string v, int max = 0)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    input_text(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>"); AddLabel(name); Add("</th>");
                    break;
                case TableTrs:
                    Add("<td>"); Add(v); Add("</td>");
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

        public HtmlContent Put<B>(string name, B v, byte z = 0) where B : IBean
        {
            return this;
        }

        public HtmlContent Put(string name, Obj v)
        {
            return this;
        }

        public HtmlContent Put(string name, Arr v)
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

        public HtmlContent Put<B>(string name, B[] v, byte z = 0) where B : IBean
        {
            return this;
        }

    }


    public interface IMenu
    {
        void menuitem(string text, string href = "#");
    }

    public interface ITableThead
    {
        void thead(string text, string href = "#");
    }

    public interface ISelectOptions
    {
        void option(string label, string value, bool selected = false);
    }

    public interface IOption<V>
    {

        string Label { get; }

        V Value { get; }

        bool IsOn { get; }

    }

}