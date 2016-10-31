using System;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A dynamical HTML5 content Tooled with Pure.CSS
    /// </summary> 
    ///
    public class HtContent : DynamicContent, ISink<HtContent>
    {

        const sbyte Caption = 1, Body = 2, FormList = 3;

        sbyte ctx;


        public HtContent(int capacity) : base(capacity)
        {
        }

        public override string Type => "text/html; charset=utf-8";


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

        public void a()
        {

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

        public void table<M>(Action<HtContent> ths, Action<HtContent> eachtr) where M : IPersist
        {
            T("<table class=\"pure-table pure-table-bordered\">");
            T("<thead>");
            T("<tr>");
            T("</tr>");
            T("</thead>");
            T("<tbody>");
            T("</tbody>");
            T("</table>");
        }

        public void table<M>(M[] arr, byte x = 0xff) where M : IPersist
        {
            T("<table class=\"pure-table pure-table-bordered\">");
            T("<thead>");

            M obj = arr[0];

            ctx = Caption;
            obj.Save(this);

            T("<th class=\"mdl-data-table__cell--non-numeric\">Material</th>");
            T("<th>Quantity</th>>");

            T("</thead>");
            T("<tbody>");

            for (int i = 0; i < arr.Length; i++)
            {
                obj = arr[i];
                T("<tr>");
                T("<td class=\"mdl-data-table__cell--non-numeric\">Acrylic (Transparent)</td>");
                T("<td>25</td>");

                obj.Save(this, x);

                T("</tr>");
            }

            T("</tbody>");

            T("</table>");
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
                ButtonAttribute btn = wa.Button;
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

            ctx = FormList;

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
            T("<label for=\"").T(name).T("\" class=\"mdl-checkbox mdl-js-checkbox mdl-js-ripple-effect\">");
            T("<input type=\"checkbox\" id=\"").T(name).T("\" class=\"mdl-checkbox__input\">");
            T("<span class=\"mdl-checkbox__label\">").T(name).T("</span>");
            T("</label>");

            return this;
        }

        public HtContent Put(string name, short v)
        {
            T("<div class=\"mdl-textfield mdl-js-textfield mdl-textfield--floating-label\">");
            T("<input class=\"mdl-textfield__input\" type=\"number\" id=\"").T(name).T("\" value=\""); Add(v); T("\">");
            T("<label class=\"mdl-textfield__label\" for=\"").T(name).T("\">").T(name).T("</label>");
            T("</div>");

            return this;
        }

        public HtContent Put(string name, int v)
        {
            T("<div class=\"mdl-textfield mdl-js-textfield mdl-textfield--floating-label\">");
            T("<input class=\"mdl-textfield__input\" type=\"number\" id=\"").T(name).T("\" value=\""); Add(v); T("\">");
            T("<label class=\"mdl-textfield__label\" for=\"").T(name).T("\">").T(name).T("</label>");
            T("</div>");

            return this;
        }

        public HtContent Put(string name, long v)
        {
            T("<div class=\"mdl-textfield mdl-js-textfield mdl-textfield--floating-label\">");
            T("<input class=\"mdl-textfield__input\" type=\"text\" id=\"").T(name).T("\">");
            T("<label class=\"mdl-textfield__label\" for=\"").T(name).T("\">").T(name).T("</label>");
            T("</div>");

            return this;
        }

        public HtContent Put(string name, decimal v)
        {
            T("<div class=\"mdl-textfield mdl-js-textfield mdl-textfield--floating-label\">");
            T("<input class=\"mdl-textfield__input\" type=\"text\" id=\"").T(name).T("\">");
            T("<label class=\"mdl-textfield__label\" for=\"").T(name).T("\">").T(name).T("</label>");
            T("</div>");

            return this;
        }

        public HtContent Put(string name, Number v)
        {
            T("<div class=\"mdl-textfield mdl-js-textfield mdl-textfield--floating-label\">");
            T("<input class=\"mdl-textfield__input\" type=\"text\" id=\"").T(name).T("\">");
            T("<label class=\"mdl-textfield__label\" for=\"").T(name).T("\">").T(name).T("</label>");
            T("</div>");

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
            T("<div class=\"mdl-textfield mdl-js-textfield mdl-textfield--floating-label\">");
            T("<input class=\"mdl-textfield__input\" type=\"text\" id=\"").T(name).T("\"");
            if (max > 0)
            {
                T(" maxlength=\""); Add(max); T("\">");
            }

            T("<label class=\"mdl-textfield__label\" for=\"").T(name).T("\">").T(name).T("</label>");
            T("</div>");

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
}