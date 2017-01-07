using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace Greatbone.Core
{
    ///
    /// For dynamical HTML5 content generation Tooled with Pure.CSS
    ///
    public class HtmlContent : DynamicContent, ISink<HtmlContent>, IMenu, ISelectOptions
    {
        const int InitialCapacity = 8 * 1024;

        const string SM = "sm", MD = "md", LG = "lg", XL = "xl";

        const sbyte TableThs = 1, TableTrs = 2, FormFields = 3;

        sbyte ctx;


        public HtmlContent(bool sendable, bool pooled, int capacity = InitialCapacity) : base(sendable, pooled, capacity)
        {
        }

        public override string MType => "text/html; charset=utf-8";


        public Dictionary<string, string> Map { get; set; }


        public void AddLabel(string key)
        {
            string label;
            if (Map != null && Map.TryGetValue(key, out label)) // translate
            {
                Add(label);
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


        public void Grid()
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

        public void NAV_menu(string heading, Action<IMenu> items)
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

        public void FORM(WebAction act, Action<HtmlContent> inner)
        {
            Add("<form class=\"pure-form pure-g\">");

            inner?.Invoke(this);

            Add("</form>");
        }

        public void FORM<D>(WebAction[] acts, D[] arr, byte flags = 0) where D : IData
        {
            Add("<form class=\"pure-form pure-g\">");

            // buttons
            BUTTONS(acts);

            if (arr != null)
            {
            }

            Add("</form>");
        }

        public void FORM<D>(WebAction act, D obj, byte flags = 0) where D : IData
        {
            Add("<form class=\"pure-form pure-g\">");

            ctx = FormFields;

            // function buttuns


            Add("</form>");
        }

        public void INPUT_hidden(string name, string value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\">");
        }

        public void INPUT_text(string name, string value, bool @readonly = false, bool required = false,
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

        public void INPUT_search(string name, string value, bool @readonly = false, bool required = false,
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

        public void INPUT_tel(string name, string value, bool @readonly = false, bool required = false,
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

        public void INPUT_url(string name, string value, bool @readonly = false, bool required = false,
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

        public void INPUT_email(string name, string value, bool @readonly = false, bool required = false,
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

        public void INPUT_password(string name, string value, bool @readonly = false, bool required = false,
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

        public void INPUT_date(string name, DateTime value, bool @readonly = false, bool required = false,
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

        public void INPUT_time()
        {
            T("</tbody>");
        }

        public void INPUT_number(string name, int value, bool @readonly = false, bool required = false,
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

        public void INPUT_number(string name, long value, bool @readonly = false, bool required = false,
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

        public void INPUT_number(string name, decimal value, bool @readonly = false, bool required = false,
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

        public void INPUT_range()
        {
            T("</tbody>");
        }

        public void INPUT_color()
        {
            T("</tbody>");
        }

        public void INPUT_checkbox(string name, bool value, bool required = false)
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

        public void INPUT_radio(string name, string[] values, int @checked = 0, bool required = false)
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

        public void INPUT_File()
        {
            T("</tbody>");
        }

        public void INPUT_button()
        {
            T("</tbody>");
        }

        public void TEXTAREA()
        {
            T("</tbody>");
        }

        public void BUTTON(WebAction act)
        {
            Add("<button class=\"pure-button");
            if (act.Form == 0) Add(" pure-button-primary");
            Add("\" formaction=\"");
            Add(act.Name);
            Add("\" formmethod=\"");
            Add(act.Form == 0 ? "get" : "post");
            if (act.Dialog != 0)
            {
                Add("\" onclick=\"dialog(this,");
                Add("); return false;");
            }
            Add("\">");
            string icon = act.Icon;
            if (icon != null)
            {
                Add("<i class=\"");
                Add(icon);
                Add("\"></i>");
            }
            Add(" ");
            AddLabel(act.Name);
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

        public void BUTTON_submit(string value)
        {
            Add("<button type=\"submit\">");
            AddLabel(value);
            Add("</button>");
        }

        public void BUTTON_reset(string value)
        {
            Add("<button type=\"reset\" class=\"pure-button pure-button-secondary\">");
            AddLabel(value);
            Add("</button>");
        }

        public void SELECT(string name, string[] options, int selected = -1, bool required = false)
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

        public void SELECT<V>(string name, IOption<V>[] options, bool required = false)
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

        public void SELECT(string name, Action<ISelectOptions> options, bool required = false)
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
            throw new NotImplementedException();
        }

        public HtmlContent Put(string name, bool v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    INPUT_checkbox(name, v);
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
                    INPUT_number(name, v);
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
                    INPUT_number(name, v);
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
                    INPUT_number(name, v);
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

        public HtmlContent Put(string name, double v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
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
                    INPUT_number(name, v);
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

        public HtmlContent Put(string name, JNumber v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Number(name, v);
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
                    INPUT_date(name, v);
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

        public HtmlContent Put(string name, NpgsqlPoint v)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // Date(name, v);
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

        public HtmlContent Put(string name, string v, bool? anylen = null)
        {
            switch (ctx)
            {
                case FormFields:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    INPUT_text(name, v);
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

        public HtmlContent Put<D>(string name, D v, byte flags = 0) where D : IData
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

        public HtmlContent Put<D>(string name, D[] v, byte flags = 0) where D : IData
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
        void OPTION(string label, string value, bool selected = false);
    }

    public interface IOption<V>
    {
        string Label { get; }

        V Value { get; }

        bool IsOn { get; }
    }
}