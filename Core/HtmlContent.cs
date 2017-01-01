using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// For dynamical HTML5 content generation Tooled with Pure.CSS
    ///
    public class HtmlContent : DynamicContent, IMenu, ISelectOptions
    {
        const int InitialCapacity = 8 * 1024;

        const string SM = "sm", MD = "md", LG = "lg", XL = "xl";

        const sbyte TableThs = 1, TableTrs = 2, FormFields = 3;

        sbyte ctx;


        public HtmlContent(bool sendable, bool pooled, int capacity = InitialCapacity) : base(sendable, pooled, capacity)
        {
        }

        public override string MimeType => "text/html; charset=utf-8";


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
            T("<li class=\"pure-menu-item\"><a href=\"")
                .T(href)
                .T("\" class=\"pure-menu-link\">")
                .T(text)
                .T("</a></li>");
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

        public void Dialog(string h, Action content)
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

        public void Table(Action<HtmlContent> ths, Action<HtmlContent> trs)
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


        public void Tr(Action<int, HtmlContent> tds)
        {
            table_idx++;

            T("<tr>");

            tds(table_idx, this);

            T("</tr>");
        }

        public void Td(int v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void Td(string v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void Td(DateTime v)
        {
            T("<td>");
            Put(null, v);
            T("</td>");
        }

        public void Form(WebAction wa, Action<HtmlContent> inner)
        {
            Add("<form class=\"pure-form pure-g\">");

            inner?.Invoke(this);

            Add("</form>");
        }

        public void Form<D>(WebAction[] acts, D[] datas, byte z = 0) where D : IData
        {
            Add("<form class=\"pure-form pure-g\">");

            // buttons
            buttonlst(acts);

            if (datas != null)
            {
            }

            Add("</form>");
        }

        public void Form<D>(WebAction wa, D dat, byte z = 0) where D : IData
        {
            Add("<form class=\"pure-form pure-g\">");

            ctx = FormFields;

            // function buttuns


            Add("</form>");
        }

        public void Hidden(string name, string value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\">");
        }

        public void Text(string name, string value, bool @readonly = false, bool required = false,
            string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
                Add("\"");
            }
            if (maxlength > 0)
            {
                Add(" maxlength=\"");
                Add(maxlength);
                Add("\"");
                Add(" size=\"");
                Add(maxlength);
                Add("\"");
            }
            if (minlength > 0)
            {
                Add(" minlength=\"");
                Add(minlength);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            Add("\">");
            Add("</label>");
        }

        public void Search(string name, string value, bool @readonly = false, bool required = false,
            string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"search\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
                Add("\"");
            }
            if (maxlength > 0)
            {
                Add(" maxlength=\"");
                Add(maxlength);
                Add("\"");
                Add(" size=\"");
                Add(maxlength);
                Add("\"");
            }
            if (minlength > 0)
            {
                Add(" minlength=\"");
                Add(minlength);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            Add("\">");
            Add("</label>");
        }

        public void Tel(string name, string value, bool @readonly = false, bool required = false,
            string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"search\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
                Add("\"");
            }
            if (maxlength > 0)
            {
                Add(" maxlength=\"");
                Add(maxlength);
                Add("\"");
                Add(" size=\"");
                Add(maxlength);
                Add("\"");
            }
            if (minlength > 0)
            {
                Add(" minlength=\"");
                Add(minlength);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            Add("\">");
            Add("</label>");
        }

        public void Url(string name, string value, bool @readonly = false, bool required = false,
            string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"url\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
                Add("\"");
            }
            if (maxlength > 0)
            {
                Add(" maxlength=\"");
                Add(maxlength);
                Add("\"");
                Add(" size=\"");
                Add(maxlength);
                Add("\"");
            }
            if (minlength > 0)
            {
                Add(" minlength=\"");
                Add(minlength);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            Add("\">");
            Add("</label>");
        }

        public void Email(string name, string value, bool @readonly = false, bool required = false,
            string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"email\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
                Add("\"");
            }
            if (maxlength > 0)
            {
                Add(" maxlength=\"");
                Add(maxlength);
                Add("\"");
                Add(" size=\"");
                Add(maxlength);
                Add("\"");
            }
            if (minlength > 0)
            {
                Add(" minlength=\"");
                Add(minlength);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            Add("\">");
            Add("</label>");
        }

        public void Password(string name, string value, bool @readonly = false, bool required = false,
            string placeholder = null, int maxlength = 0, int minlength = 0, string pattern = null)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"password\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
                Add("\"");
            }
            if (maxlength > 0)
            {
                Add(" maxlength=\"");
                Add(maxlength);
                Add("\"");
                Add(" size=\"");
                Add(maxlength);
                Add("\"");
            }
            if (minlength > 0)
            {
                Add(" minlength=\"");
                Add(minlength);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            Add("\">");
            Add("</label>");
        }

        public void Date(string name, DateTime value, bool @readonly = false, bool required = false,
            string placeholder = null, int max = int.MaxValue, int min = int.MinValue, int step = 0)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"date\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
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
            Add("\">");
            Add("</label>");
        }

        public void input_time()
        {
            T("</tbody>");
        }

        public void Number(string name, int value, bool @readonly = false, bool required = false,
            string placeholder = null, int max = int.MaxValue, int min = int.MinValue, int step = 0)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
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
            Add("\">");
            Add("</label>");
        }

        public void Number(string name, long value, bool @readonly = false, bool required = false,
            string placeholder = null, long max = long.MaxValue, long min = long.MinValue, long step = 0)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
                Add("\"");
            }
            if (max != long.MaxValue)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != long.MinValue)
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
            Add("\">");
            Add("</label>");
        }

        public void Number(string name, decimal value, bool @readonly = false, bool required = false,
            string placeholder = null, int max = int.MaxValue, int min = int.MinValue, int step = 0)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (placeholder != null)
            {
                Add(" placedholder=\"");
                AddLabel(placeholder);
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
            Add("\">");
            Add("</label>");
        }

        public void Range()
        {
            T("</tbody>");
        }

        public void Color()
        {
            T("</tbody>");
        }

        public void Checkbox(string name, bool value, bool required = false)
        {
            Add("<label>");
            AddLabel(name);
            Add("<input type=\"checkbox\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (value) Add(" checked");
            if (required) Add(" required");
            Add("\">");
            Add("</label>");
        }

        public void Radio(string name, string[] values, int @checked = 0, bool required = false)
        {
            Add("<fieldset>");
            Add("<legend>");
            AddLabel(name);
            Add("</legend>");
            for (int i = 0; i < values.Length; i++)
            {
                Add("<label>");
                Add("<input type=\"radio\" name=\"");
                Add(name);
                Add("\" value=\"");
                Add(i);
                if (@checked == i) Add(" checked");
                if (required) Add(" required");
                Add("\">");
                Add(values[i]);
                Add("</label>");
            }
            Add("</fieldset>");
        }

        public void File()
        {
            T("</tbody>");
        }

        public void input_button()
        {
            T("</tbody>");
        }

        public void TextArea()
        {
            T("</tbody>");
        }

        public void Button(WebAction wa)
        {
            Add("<button class=\"pure-button");
            if (wa.FormPost == true) Add(" pure-button-primary");
            Add("\" formaction=\"");
            Add(wa.Name);
            Add("\" formmethod=\"");
            Add(wa.FormPost == true ? "get" : "post");
            if (wa.DialogPost != null)
            {
                Add("\" onclick=\"dialog(this,");
                Add("); return false;");
            }
            Add("\">");
            string icon = wa.Icon;
            if (icon != null)
            {
                Add("<i class=\"");
                Add(icon);
                Add("\"></i>");
            }
            Add(" ");
            AddLabel(wa.Name);
            Add(" </button>");
        }

        public void buttonlst(WebAction[] was)
        {
            for (int i = 0; i < was.Length; i++)
            {
                WebAction wa = was[i];
                Button(wa);
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

        public void Select(string name, string[] options, int selected = -1, bool required = false)
        {
            Add("<label>");
            AddLabel(name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (required) Add(" required");
            Add("\">");

            for (int i = 0; i < options.Length; i++)
            {
                string opt = options[i];
                Add("<option value=\"");
                Add(opt);
                Add("\"");
                if (selected == i) Add(" selected");
                Add("\">");
                AddLabel(opt);
                Add("</option>");
            }
            Add("</label>");
        }

        public void Select<V>(string name, IOption<V>[] options, bool required = false)
        {
            Add("<label>");
            AddLabel(name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (required) Add(" required");
            Add("\">");

            for (int i = 0; i < options.Length; i++)
            {
                IOption<V> opt = options[i];
                Add("<option value=\"");
                Add(opt.ToString());
                Add("\"");
                if (opt.IsOn) Add(" selected");
                Add("\">");
                AddLabel(opt.Label);
                Add("</option>");
            }
            Add("</label>");
        }

        public void Select(string name, Action<ISelectOptions> options, bool required = false)
        {
            Add("<label>");
            AddLabel(name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (required) Add(" required");
            Add("\">");
            options(this);
            Add("</label>");
        }

        public void Option(string text, string value, bool selected = false)
        {
            Add("<option value=\"");
            Add(value);
            Add("\"");
            if (selected) Add(" selected");
            Add("\">");
        }

        public void DataList()
        {
            T("</tbody>");
        }

        public void Progres()
        {
            T("</tbody>");
        }

        public void Meter()
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
                    Checkbox(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>");
                    AddLabel(name);
                    Add("</th>");
                    break;
                case TableTrs:
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
                    Number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>");
                    AddLabel(name);
                    Add("</th>");
                    break;
                case TableTrs:
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
                    Number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>");
                    AddLabel(name);
                    Add("</th>");
                    break;
                case TableTrs:
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
                    Number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>");
                    AddLabel(name);
                    Add("</th>");
                    break;
                case TableTrs:
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
                    Number(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>");
                    AddLabel(name);
                    Add("</th>");
                    break;
                case TableTrs:
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
                    Date(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>");
                    AddLabel(name);
                    Add("</th>");
                    break;
                case TableTrs:
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

        public HtmlContent Put(string name, string v, int max = 0)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    Text(name, v);
                    Add("</div>");
                    break;
                case TableThs:
                    Add("<th>");
                    AddLabel(name);
                    Add("</th>");
                    break;
                case TableTrs:
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

        public HtmlContent Put<B>(string name, B v, byte z = 0) where B : IData
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

        public HtmlContent Put<B>(string name, B[] v, byte z = 0) where B : IData
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
        void Option(string label, string value, bool selected = false);
    }

    public interface IOption<V>
    {
        string Label { get; }

        V Value { get; }

        bool IsOn { get; }
    }
}