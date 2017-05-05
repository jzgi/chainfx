using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// For dynamical HTML5 content tooled with Zurb Foundation
    ///
    public class HtmlContent : DynamicContent, IDataOutput<HtmlContent>
    {
        internal const sbyte
            TABLE_THEAD = 1,
            TABLE_TBODY = 2,
            GRID_DIV = 3,
            LIST_UL = 4,
            PICKER = 5;

        ///
        /// The outputing context for per data object
        ///
        struct Ctx
        {
            // data control node
            internal sbyte node;

            internal Work varwork;

            internal IData obj;

            // within a group boundary
            internal bool group;

            internal void OutputVarKey(HtmlContent cont)
            {
                varwork.OutputVarKey(obj, cont);
            }
        }

        // outputing context chain
        const int DEPTH = 4;

        readonly Ctx[] chain = new Ctx[DEPTH];

        int level = -1; // current level

        public HtmlContent(bool octal, bool pooled, int capacity = 32 * 1024) : base(octal, pooled, capacity)
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

        public void ROW_()
        {
            Add("<div class=\"row\">");
        }

        public void _ROW()
        {
            Add("</div>");
        }

        public void FORM_(string action = null, bool post = true, bool mp = false)
        {
            Add("<form");
            if (action != null)
            {
                Add(" action=\"");
                Add(action);
                Add("\"");
            }
            if (post)
            {
                Add(" method=\"post\"");
            }
            if (mp)
            {
                Add(" enctype=\"multipart/form-data\"");
            }
            Add(">");
        }

        public void _FORM()
        {
            Add("</form>");
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

        public void CALLOUT(string v, bool closable)
        {
            Add("<div class=\"alert callout\" data-closable>");
            Add("<p class=\"text-center\">");
            Add(v);
            Add("</p>");
            Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            Add("</div>");
        }

        public void TOOLBAR(ActionInfo[] ais)
        {
            Add("<div data-sticky-container>");
            Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
            Add("<div class=\"title-bar\">");

            Add("<div class=\"title-bar-left\">");
            if (ais != null)
            {
                BUTTONS(ais);
            }
            Add("</div>");

            Add("<div class=\"title-bar-right\">");
            Add("<a class=\"float-right\" href=\"javascript: location.reload();\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-refresh fa-stack-1x fa-inverse\"></i></span></a>");
            Add("</div>");

            Add("</div>");
            Add("</div>");
            Add("</div>");
        }

        public void PAGENATE(ActionContext ac)
        {
            // pagination
            ActionInfo ai = ac.Doer;
            if (ai.HasSubscript)
            {
                Add("<div class=\"row\">");
                Add("<ul class=\"pagination text-center\" role=\"navigation\">");
                int subscript = ac.Subscript;
                Add("<li class=\"pagination-previous disabled\">Previous</li>");
                for (int i = 0; i < subscript; i++)
                {
                    Add("<li><a href=\"");
                    Add(ai.Name);
                    Add('-');
                    Add(subscript);
                    Add(ac.QueryString);
                    Add("\">");
                    Add(i);
                    Add("</a></li>");
                }
                Add("<li class=\"pagination-next disabled\">Next</li>");
                Add("</ul>");
                Add("</div>");
            }
        }

        public void TABLE(IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            if (input != null)
            {
                Add("<table class=\"unstriped\">");

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
                Add("</table>");
            }
        }

        public void TABLE<D>(ActionContext formctx, Work work, sbyte check, D[] arr, short proj = 0) where D : IData
        {
            if (formctx != null)
            {
                Add("<form id=\"gridform\">");

                if (check > 1) TOOLBAR(formctx.Work.UiActions);
            }

            if (arr != null)
            {
                ++level;
                chain[level].varwork = work?.varwork;

                Add("<table class=\"unstriped\">");

                ActionInfo[] ais = work.Varwork?.UiActions;


                chain[level].node = TABLE_THEAD;
                Add("<thead>");
                Add("<tr>");
                arr[0].WriteData(this, proj);

                if (ais != null)
                {
                    Add("<th></th>"); // head for controls
                }
                Add("</tr>");
                Add("</thead>");

                chain[level].node = TABLE_TBODY;
                Add("<tbody>");
                for (int i = 0; i < arr.Length; i++)
                {
                    D obj = arr[i];
                    chain[level].obj = obj;

                    Add("<tr>");
                    if (check == 1)
                    {
                        Add("<td>");
                        Add("<input name=\"key\" type=\"radio\" value=\"");
                        Add("</td>");
                        work.OutputVarKey(obj, this);
                    }
                    else if (check == 2)
                    {
                        Add("<td>");
                        Add("<input name=\"key\" type=\"checkbox\" value=\"");
                        work.OutputVarKey(obj, this);
                        Add("</td>");
                    }

                    arr[i].WriteData(this, proj);

                    // acitons
                    if (ais != null)
                    {
                        Add("<td>");
                        BUTTONS(ais, level > 0);
                        Add("</td>");
                    }

                    Add("</tr>");
                }
                Add("</tbody>");
                Add("</table>");
                --level;
            }

            if (formctx != null)
            {
                // pagination controls if any
                PAGENATE(formctx);

                Add("</form>");
            }
        }

        public void GRID(IDataInput input, Action<IDataInput, HtmlContent> pipe)
        {
            chain[++level].node = GRID_DIV;
            if (input != null)
            {
                Add("<div class=\"expanded row\">");
                while (input.Next())
                {
                    Add("<div class=\"row small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
                    pipe(input, this);
                    Add("</div>");
                }
                Add("</div>");
            }
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }
            --level;
        }

        public void GRID<D>(ActionContext formctx, Work work, sbyte check, D[] arr, short proj = 0) where D : IData
        {
            if (formctx != null)
            {
                Add("<form id=\"gridform\">");

                if (check > 1) TOOLBAR(formctx.Work.UiActions);
            }

            if (arr != null) // grid component
            {
                ++level;
                chain[level].node = GRID_DIV;
                chain[level].varwork = work?.varwork;

                Add("<div class=\"row expanded small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<div class=\"column card\">");
                    D obj = arr[i];
                    chain[level].obj = obj;

                    if (check == 1)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        Add("</div>");
                        Add("<div class=\"small-9 columns\" style=\"text-align: right\">");
                        Add("<input name=\"key\" type=\"radio\" style=\"margin: 0\" value=\"");
                        work.OutputVarKey(obj, this);
                        Add("\">");
                        Add("</div>");
                        Add("</div>");
                    }
                    else if (check == 2)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        Add("</div>");
                        Add("<div class=\"small-9 columns\" style=\"text-align: right\">");
                        Add("<input name=\"key\" type=\"checkbox\" style=\"margin: 0\" value=\"");
                        work.OutputVarKey(obj, this);
                        Add("\">");
                        Add("</div>");
                        Add("</div>");
                    }

                    obj.WriteData(this, proj);

                    // acitons
                    ActionInfo[] ais = work.Varwork?.UiActions;
                    if (ais != null)
                    {
                        Add("<div style=\"text-align: right\">");
                        BUTTONS(ais, level > 0);
                        Add("</div>");
                    }
                    Add("</div>");
                }
                Add("</div>");
                --level;
            }
            else // empty
            {
                Add("<div class=\"row\">");
                Add("</div>");
            }

            if (formctx != null)
            {
                // pagination controls if any
                PAGENATE(formctx);

                Add("</form>");
            }
        }

        public void LIST<D>(ActionContext formctx, Work work, sbyte check, D[] arr, short proj = 0) where D : IData
        {
            if (formctx != null)
            {
                Add("<form id=\"gridform\">");

                if (check > 1) TOOLBAR(formctx.Work.UiActions);
            }

            if (arr != null)
            {
                ++level;
                chain[level].node = LIST_UL;
                chain[level].varwork = work?.varwork;

                Add("<ul>");
                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<li>");
                    arr[i].WriteData(this, proj);
                    Add("</li>");
                }
                Add("</ul>");
                --level;
            }

            if (formctx != null)
            {
                // pagination controls if any
                PAGENATE(formctx);

                Add("</form>");
            }
        }

        public void BUTTONS(ActionInfo[] ais, bool hollow = false)
        {
            for (int i = 0; i < ais.Length; i++)
            {
                ActionInfo ai = ais[i];

                UiAttribute ui = ai.Ui;

                if (ui.IsLink)
                {
                    Add("<a class=\"button ");
                    Add(ui.Alert ? "warning" : "primary");
                    if (hollow)
                    {
                        Add(" hollow");
                    }
                    Add("\" href=\"");
                    for (int lvl = 0; lvl <= level; lvl++)
                    {
                        chain[lvl].OutputVarKey(this);
                        Add('/');
                    }
                    Add(ai.Name);
                    Add("\"");
                    if (ai.Enabler != null && !ai.Enabler(chain[level].obj))
                    {
                        Add(" disabled onclick=\"return false;\"");
                    }
                    else if (ui.HasDialog)
                    {
                        Add(" onclick=\"return dialog(this,1,2);\"");
                    }
                    Add(">");
                    Add(ai.Label);
                    Add("</a>");
                }
                else if (ui.IsAnchor)
                {
                    Add("<a class=\"button ");
                    Add(ui.Alert ? "warning" : "primary");
                    if (hollow)
                    {
                        Add(" hollow");
                    }
                    Add("\" href=\"");
                    for (int lvl = 0; lvl <= level; lvl++)
                    {
                        chain[lvl].OutputVarKey(this);
                        Add('/');
                    }
                    Add(ai.Name);
                    Add("\"");
                    if (ai.Enabler != null && !ai.Enabler(chain[level].obj))
                    {
                        Add(" disabled onclick=\"return false;\"");
                    }
                    else if (ui.HasDialog)
                    {
                        Add(" onclick=\"return dialog(this,2,3);\"");
                    }
                    else if (ui.HasScript)
                    {
                        Add(" onclick=\"");
                        Add(ai.Name);
                        Add("(this);return false;\"");
                    }
                    else if (ui.HasCrop)
                    {
                        Add(" onclick=\"return crop(this,");
                        Add(ui.Width);
                        Add(',');
                        Add(ui.Height);
                        Add(',');
                        Add(ui.Circle);
                        Add(");\"");
                    }
                    Add(">");
                    Add(ai.Label);
                    Add("</a>");
                }
                else if (ui.IsButton)
                {
                    Add("<button class=\"button ");
                    Add(ui.Alert ? "warning" : "primary");
                    if (hollow)
                    {
                        Add(" hollow");
                    }
                    Add("\" name=\"");
                    Add(ai.Name);
                    Add("\" formaction=\"");
                    for (int lvl = 0; lvl <= level; lvl++)
                    {
                        chain[lvl].OutputVarKey(this);
                        Add('/');
                    }
                    Add(ai.Name);
                    Add("\" formmethod=\"post\"");
                    if (ai.Enabler != null && !ai.Enabler(chain[level].obj))
                    {
                        Add(" disabled");
                    }
                    else if (ui.HasConfirm)
                    {
                        Add(" onclick=\"return confirm('");
                        Add(ai.Label);
                        Add("?');\"");
                    }
                    else if (ui.HasDialog)
                    {
                        Add(" onclick=\"return dialog(this,4,2);\"");
                    }
                    Add(">");
                    Add(ai.Label);
                    Add("</button>");
                }
            }
        }

        public void BUTTON(string value)
        {
            Add("<button class=\"button\">");
            AddEsc(value);
            Add("</button>");
        }


        public void HIDDEN(string name, string value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\">");
        }

        public void HIDDEN(string name, int value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
        }

        public void HIDDEN(string name, decimal value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
        }

        public void TEXT(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, Opt<string> opt = null, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"text\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
                Add(" size=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");

            Add("</label>");
        }

        public void PASSWORD(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"password\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(v);
            Add("\"");

            if (help != null)
            {
                Add(" Help=\"");
                Add(help);
                Add("\"");
            }
            if (pattern != null)
            {
                Add(" pattern=\"");
                AddEsc(pattern);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");
                Add(" size=\"");
                Add(max);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");

            Add("</label>");
        }

        public void DATE(string name, DateTime v, string label = null, DateTime max = default(DateTime), DateTime min = default(DateTime), bool @readonly = false, bool required = false, int step = 0)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"date\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (max != default(DateTime))
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != default(DateTime))
            {
                Add(" min=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");
            if (step != 0)
            {
                Add(" step=\"");
                Add(step);
                Add("\"");
            }

            Add(">");
            Add("</label>");
        }

        public void TIME()
        {
            T("</tbody>");
        }

        public void NUMBER(string name, short v, string label = null, string help = null, short max = 0, short min = 0, short step = 0, bool opt = false, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);

            bool group = step > 0; // input group with up up and down

            if (group)
            {
                Add("<div class=\"input-group\">");
                Add("<input type=\"button\" class=\"input-group-label\" onclick=\"this.form['qty'].stepDown()\" value=\"-\">");
            }

            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (group)
            {
                Add(" class=\"input-group-field\"");
            }

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
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
            if (opt)
            {
                Add("<input type=\"button\" value=\"...\" onclick=\"dialog(this, 1, 1)\"> ");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");

            if (group)
            {
                Add("<input type=\"button\" class=\"input-group-label\" onclick=\"this.form['qty'].stepUp()\" value=\"+\">");
                Add("</div>");
            }

            Add("</label>");
        }

        public void NUMBER(string name, int v, string label = null, string help = null, int max = 0, int min = 0, int step = 0, bool opt = false, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
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
            if (opt)
            {
                Add("<input type=\"button\" value=\"...\" onclick=\"dialog(this, 1, 1)\"> ");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            Add("</label>");
        }

        public void NUMBER(string name, long v, string label = null, string help = null, long max = 0, long min = 0, long step = 0, bool opt = false, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
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
            if (opt)
            {
                Add("<input type=\"button\" value=\"...\" onclick=\"dialog(this, 1, 1)\"> ");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            Add("</label>");
        }

        public void NUMBER(string name, decimal v, string label = null, string help = null, decimal max = 0, decimal min = 0, decimal step = 0, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (max != 0)
            {
                Add(" max=\"");
                Add(max);
                Add("\"");
            }
            if (min != 0)
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
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

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

        /// <summary>
        /// Outputs a list of checkbox controls from the values of a data input source.
        /// </summary>
        /// <param name="name">name in both the data input and the output radio control</param>
        /// <param name="inp">a data input source</param>
        /// <param name="putv">directly put value of any type from source</param>
        public void CHECKBOXES(string name, IDataInput inp, Action putv)
        {
            int i = 0;
            while (inp.Next())
            {
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\" id=\"");
                Add(name);
                Add(i);
                Add("\" value=\"");
                putv();
                Add("\">");
                Add("<label for=\"");
                Add(name);
                Add(i);
                Add("\">");
                putv();
                Add("</label>");
                i++;
            }
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

        public void CHECKBOX<V>(string name, V[] v, Opt<V> opt, string label = null, bool required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, null);
            Add("</legend>");

            foreach (var pair in opt)
            {
                V key = pair.Key;
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\"");
                Add("\" id=\"");
                AddVary(key);
                Add("\"");
                Add("\" value=\"");
                AddVary(key);
                Add("\"");

                bool equal = false;
                for (int i = 0; i < v.Length; i++)
                {
                    if (key.Equals(v[i]))
                    {
                        equal = true;
                        break;
                    }
                }
                if (equal) Add(" checked");

                if (required) Add(" required");
                Add(">");

                Add("<label for=\"");
                AddVary(key);
                Add("\">");
                Add(pair.Value);
                Add("</label>");
            }
            Add("</fieldset>");
        }

        /// <summary>
        /// Outputs a list of radio controls from the values of a data input source.
        /// </summary>
        /// <param name="name">name in both the data input and the output radio control</param>
        /// <param name="inp">a data input source</param>
        /// <param name="putv">directly put value of any type from source</param>
        public void RADIOS(string name, IDataInput inp, Action putv)
        {
            int i = 0;
            while (inp.Next())
            {
                Add("<input type=\"radio\" name=\"");
                Add(name);
                Add("\" id=\"");
                Add(name);
                Add(i);
                Add("\" value=\"");
                putv();
                Add("\">");
                Add("<label for=\"");
                Add(name);
                Add(i);
                Add("\">");
                putv();
                Add("</label>");
                i++;
            }
        }

        public void RADIO<V>(string name, V v, Opt<V> opt, string label = null, bool required = false) where V : IEquatable<V>, IConvertible
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            foreach (var pair in opt)
            {
                Add("<input type=\"radio\" name=\"");
                Add(name);
                V key = pair.Key;

                Add("\" id=\"");
                Add(name);
                AddVary(key);
                Add("\"");

                Add("\" value=\"");
                AddVary(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");

                Add("<label for=\"");
                Add(name);
                AddVary(key);
                Add("\">");
                Add(pair.Value);
                Add("</label>");
            }
            Add("</fieldset>");
        }

        public void FILE(string name, string label = null, string size = null, string ratio = null, bool required = false)
        {
            // <div class="slim"
            //      data-label="Drop your avatar here"
            //      data-fetcher="fetch.php"
            //      data-size="240,240"
            //      data-ratio="1:1">
            //     <input type="file" name="slim[]" required />
            // </div>
            //
            Add("<div class=\"slim\" data-upload-base64=\"false\" data-instant-edit=\"true\" data-label=\"");
            AddLabel(label, name);
            Add("\" data-fetcher=\"_");
            Add(name);
            Add("_");

            if (size != null)
            {
                Add("\" data-size=\"");
                Add(size);
                Add("\" data-force-size=\"");
                Add(size);
            }
            if (ratio != null)
            {
                Add("\" data-ratio=\"");
                Add(ratio);
            }
            Add("\">");

            Add("<input type=\"file\" name=\"");
            Add(name);
            Add("\"");
            if (required) Add(" required");
            Add(">");
            Add("</div>");
        }

        public void TEXTAREA(string name, string v, string label = null, string help = null, short max = 0, short min = 0, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<textarea name=\"");
            Add(name);
            Add("\"");

            if (help != null)
            {
                Add(" placeholder=\"");
                Add(help);
                Add("\"");
            }
            if (max > 0)
            {
                Add(" maxlength=\"");
                Add(max);
                Add("\"");

                Add(" rows=\"");
                Add(max < 200 ? 3 : max < 400 ? 4 : 5);
                Add("\"");
            }
            if (min > 0)
            {
                Add(" minlength=\"");
                Add(min);
                Add("\"");
            }
            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            AddEsc(v);
            Add("</textarea>");
            Add("</label>");
        }

        public void BUTTON(ActionInfo ai)
        {
            Add("<button class=\"button primary\" style=\"margin-right: 5px; border-radius: 15%\"");
            Add(" formaction=\"");
            Add(ai.Name);
            Add("\" formmethod=\"post\"");

            UiAttribute ui = ai.Ui;

            UiMode mode = ui.Mode;
            if (mode > 0)
            {
                Add(" onclick=\"dialog(this,");
                Add((int) mode);
                Add("); return false;\"");
            }

            if (ai.Enabler != null)
            {
                // Add(" data-if=\""); Add(state.If); Add("\"");
                // Add(" data-unif=\""); Add(state.Unif); Add("\"");
            }
            Add(">");
            AddLabel(ui.Label, ai.Name);

            Add("</button>");
        }

        public void SELECT(string name, short v, Opt<short> opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            Add(">");

            foreach (var pair in opt)
            {
                short key = pair.Key;
                Add("<option value=\"");
                Add(key);
                Add("\"");
                if (key == v) Add(" selected");
                Add(">");

                Add(pair.Value);
                Add("</option>");
            }
            Add("</select>");
            Add("</label>");
        }

        public void SELECT(string name, string v, Opt<string> opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            Add(">");

            foreach (var pair in opt)
            {
                string key = pair.Key;
                Add("<option value=\"");
                Add(key);
                Add("\"");
                if (key == v) Add(" selected");
                Add(">");

                Add(pair.Value);
                Add("</option>");
            }
            Add("</select>");
            Add("</label>");
        }

        public void SELECT(string name, string v, string[] opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0, bool refresh = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<select name=\"");
            Add(name);
            Add("\"");
            if (multiple) Add(" multiple");
            if (required) Add(" required");
            if (size > 0)
            {
                Add(" size=\"");
                Add(size);
                Add("\"");
            }
            if (refresh)
            {
                Add(" onchange=\"location = location.href.split('?')[0] + '?' + $(this.form).serialize();\"");
            }
            Add(">");

            for (int i = 0; i < opt.Length; i++)
            {
                string key = opt[i];
                Add("<option value=\"");
                Add(key);
                Add("\"");
                if (key == v) Add(" selected");
                Add(">");

                Add(key);
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
            return this;
        }

        public HtmlContent Put(string name, JNumber v)
        {
            return this;
        }

        public HtmlContent Put(string name, IDataInput v)
        {
            return this;
        }

        public HtmlContent PutRaw(string name, string raw)
        {
            return this;
        }

        public void Group(string label)
        {
            chain[level].group = true; // set group boundary

            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    Add("<th>");
                    AddLabel(label, null);
                    Add("</th>");
                    break;
                case TABLE_TBODY:
                    Add("<td>");
                    break;
                case GRID_DIV:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    AddLabel(label, null);
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">"); // opening
                    break;
            }
        }

        public void UnGroup()
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    Add("</th>"); // TD closing
                    break;
                case TABLE_TBODY:
                    Add("</td>");
                    break;
                case GRID_DIV:
                    Add("</div>"); // closing
                    Add("</div>");
                    break;
            }

            chain[level].group = false;
        }

        public HtmlContent Put(string name, bool v, Func<bool, string> opt = null, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (opt != null) Add(opt(v));
                        else Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        if (opt != null) Add(opt(v));
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                        if (opt != null) Add(opt(v));
                        else Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        if (opt != null) Add(opt(v));
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, short v, Opt<short> opt = null, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, int v, Opt<int> opt = null, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, long v, Opt<long> opt = null, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        if (opt != null) Add(opt[v]);
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // NUMBER(name, v);
                    Add("</div>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, double v, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        Add(v);
                        Add(' ');
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                        Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, decimal v, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        Add(v);
                        Add(' ');
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                        Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, DateTime v, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td>");
                        if (v != default(DateTime))
                        {
                            Add(v);
                        }
                        Add("</td>");
                    }
                    else
                    {
                        if (v != default(DateTime))
                        {
                            Add(v);
                            Add(' ');
                        }
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                    }
                    if (v != default(DateTime))
                    {
                        Add(v);
                        Add(' ');
                    }
                    if (!chain[level].group)
                    {
                        Add("</div>");
                        Add("</div>");
                    }
                    break;
                case LIST_UL:
                    Add("<li>");
                    Add(v);
                    Add("</li>");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, string v, Opt<string> opt = null, string label = null)
        {
            var ctx = chain[level];
            switch (ctx.node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td>");
                        Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        Add(v);
                        Add(' ');
                    }
                    break;
                case GRID_DIV:
                    if (!chain[level].group)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        AddLabel(label, name);
                        Add("</div>");
                        Add("<div class=\"small-9 columns\">");
                    }
                    Add(v);
                    Add(' ');
                    if (!chain[level].group)
                    {
                        Add("</div>");
                        Add("</div>");
                    }
                    break;
                case LIST_UL:
                    Add(v);
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, ArraySegment<byte> v, string label = null)
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    break;
                case TABLE_TBODY:
                    break;
                case GRID_DIV:
                    Add("<img src=\"data:");
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, short[] v, Opt<short> Opt = null, string label = null)
        {
            return this;
        }

        public HtmlContent Put(string name, int[] v, Opt<int> Opt = null, string label = null)
        {
            return this;
        }

        public HtmlContent Put(string name, long[] v, Opt<long> Opt = null, string label = null)
        {
            return this;
        }

        public HtmlContent Put(string name, string[] v, Opt<string> Opt = null, string label = null)
        {
            return this;
        }

        public HtmlContent Put(string name, Dictionary<string, string> v, string label = null)
        {
            return this;
        }

        public HtmlContent Put(string name, IData v, short proj = 0, string label = null)
        {
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, short proj = 0, string label = null) where D : IData
        {
            switch (chain[level].node)
            {
                case TABLE_THEAD:
                    if (!chain[level].group)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    break;
                case TABLE_TBODY:
                    if (!chain[level].group)
                    {
                        Add("<td>");
                        LIST(null, null, 0, v, proj);
                        Add("</td>");
                    }
                    else
                    {
                        LIST(null, null, 0, v, proj);
                    }
                    break;
                case GRID_DIV:
                    if (v != null)
                    {
                        Add("<div class=\"row\">");
                        TABLE(null, chain[level].varwork, 0, v, proj);
                        Add("</div>");
                    }
                    break;
                case LIST_UL:
                    break;
            }
            return this;
        }
    }
}