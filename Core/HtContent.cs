using System;
using System.Collections.Generic;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A dynamical HTML5 content generator Tooled with Pure.CSS
    /// </summary> 
    ///
    public class HtContent : DynamicContent, ISink<HtContent>, IMenu
    {

        const string SM = "sm", MD = "md", LG = "lg", XL = "xl";

        const sbyte TableThs = 1, TableTrs = 2, FormFields = 3;

        sbyte ctx;


        public HtContent(int capacity) : base(capacity)
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
            else // uppercase
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

        public HtContent T(string str)
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

        public void table(Action<HtContent> ths, Action<HtContent> trs)
        {
            table_idx = 0;

            T("<table class=\"pure-table pure-table-bordered\">");
            T("<thead>");
            T("<tr>");
            ths(this);
            T("</tr>");
            T("</thead>");
            T("<tbody>");

            trs(this);

            T("</tbody>");
            T("</table>");
        }


        public void tr(Action<int, HtContent> tds)
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

        public void table<M>(M[] arr, byte x = 0xff) where M : IPersist
        {
            M obj = arr[0];

            table(
                ths =>
                {
                    obj.Save(this, x);
                },
                trs =>
                {
                    ctx = TableTrs;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i].Save(this, x);
                    }
                }
            );
        }


        public void input_hidden()
        {
            T("</tbody>");

        }

        public void input_text()
        {
            T("</tbody>");

        }

        public void input_search()
        {
            T("</tbody>");

        }

        public void input_tel()
        {
            T("</tbody>");

        }

        public void input_url()
        {
            T("</tbody>");

        }

        public void input_email()
        {
            T("</tbody>");

        }



        public void input_password()
        {
            T("</tbody>");

        }

        public void input_date()
        {
            T("</tbody>");

        }

        public void input_time()
        {
            T("</tbody>");

        }

        public void input_number()
        {
            T("</tbody>");

        }

        public void input_range()
        {
            T("</tbody>");

        }

        public void input_color()
        {
            T("</tbody>");

        }

        public void input_checkbox()
        {
            T("</tbody>");

        }

        public void input_radio()
        {
            T("</tbody>");

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
            T("<button class=\"mdl-button mdl-js-button mdl-button--raised mdl-button--colored\">");
            // T(wa);
            T("</button>");
        }

        public void buttonlst(WebAction[] was)
        {
            for (int i = 0; i < was.Length; i++)
            {
                WebAction wa = was[i];
                DialogAttribute btn = wa.Button;
                T("<button class=\"mdl-button mdl-js-button mdl-button--raised mdl-button--colored\">");
                T(wa.Key);
                T("</button>");
            }
        }

        public void select()
        {
            T("</tbody>");

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


        public void form<P>(WebAction wa, Action<HtContent> inner) where P : IPersist
        {
            T("<form>");

            inner?.Invoke(this);

            T("</form>");
        }

        public void form<M>(WebAction[] was, M[] arr, byte x = 0xff) where M : IPersist
        {
            T("<form>");

            // buttons
            buttonlst(was);

            table(arr, x);

            T("</form>");
        }

        public void form<P>(WebAction wa, P obj, byte x = 0xff) where P : IPersist
        {
            T("<form>");

            ctx = FormFields;

            obj.Save(this);

            // function buttuns


            T("</form>");
        }

        //
        // ISINK
        //

        public HtContent PutNull(string name)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, bool v)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtContent Put(string name, short v)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtContent Put(string name, int v)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtContent Put(string name, long v)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtContent Put(string name, decimal v)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtContent Put(string name, Number v)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtContent Put(string name, DateTime v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, char[] v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, string v, int max = 0)
        {
            switch (ctx)
            {
                case FormFields: break;
                case TableTrs: break;
            }
            return this;
        }

        public HtContent Put(string name, byte[] v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, ArraySegment<byte> v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put<V>(string name, V v, byte x = 0xff) where V : IPersist
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, JObj v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, JArr v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, short[] v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, int[] v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, long[] v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put(string name, string[] v)
        {
            throw new NotImplementedException();
        }

        public HtContent Put<V>(string name, V[] v, byte x = 0xff) where V : IPersist
        {
            throw new NotImplementedException();
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

}