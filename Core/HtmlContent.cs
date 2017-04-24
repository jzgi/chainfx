using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    ///
    /// For dynamical HTML5 content tooled with Zurb Foundation
    ///
    public class HtmlContent : DynamicContent, IDataOutput<HtmlContent>
    {
        const int DEPTH = 4;

        internal const sbyte
            // multiple records
            HTABLE = 1,
            HGRID = 2,
            HLIST = 3,
            // single record 
            HFILL = 5;

        ///
        /// The outputing context for per data object
        ///
        struct Ctx
        {
            // key position in the output stream
            internal IData obj;

            internal Work varwork;

            // component
            internal sbyte comp;

            // whether currently ouput label
            internal bool label;

            // the ordinal of each field
            internal int ordinal;

            // beginning label of a field group
            internal string begin;

            internal void OutputVarKey(HtmlContent cont)
            {
                varwork.OutputVarKey(obj, cont);
            }
        }

        // whether within a form
        internal bool formed;

        // outputing context chain
        readonly Ctx[] chain = new Ctx[DEPTH];

        int level = -1; // current level

        ///
        public HtmlContent(bool octal, bool pooled, int capacity = 16 * 1024) : base(octal, pooled, capacity)
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


        public void FILLFORM(ActionInfo act, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form method=\"post\">");

            FILL(input, valve);

            Add("</form>");
        }

        public void GRIDFORM(List<ActionInfo> actions, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form>");

            formed = true;

            // buttons
            if (actions != null)
            {
                Add("<div class=\"row\">");
                // BUTTONS(actions);
                Add("</div>");
            }

            GRID(input, valve);

            Add("</form>");
        }

        public void GRIDFORM<D>(ActionContext ac, List<D> lst, int proj = 0) where D : IData
        {
            Work work = ac.Work;
            ActionInfo[] ais = work.UiActions;

            Add("<form>");

            formed = true;

            // buttons
            if (ais != null)
            {
                Add("<div data-sticky-container>");
                Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                Add("<div class=\"title-bar\">");

                Add("<div class=\"title-bar-left\">");
                TOOLS(ais);
                Add("</div>");

                Add("<div class=\"title-bar-right\"><i class=\"fa fa-refresh\"></i></div>");

                Add("</div>");
                Add("</div>");
                Add("</div>");
            }

            // grid
            GRID(work, lst, proj);

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

            Add("</form>");
        }

        public void TABLEFORM<D>(List<ActionInfo> acts, List<D> lst, int proj = 0) where D : IData
        {
            Add("<form>");

            formed = true;

            // buttons
            if (acts != null)
            {
                Add("<div class=\"row\">");
                // BUTTONS(acts);
                Add("</div>");
            }

            TABLE(lst, proj);

            Add("</form>");
        }

        public void TABLEFORM(List<ActionInfo> acts, IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            Add("<form>");

            formed = true;

            // buttons
            if (acts != null)
            {
                Add("<div class=\"row\">");
                // BUTTONS(acts);
                Add("</div>");
            }

            TABLE(input, valve);

            Add("</form>");
        }

        public void FILLFORM(ActionInfo act, IData obj, int proj = 0)
        {
            chain[++level].comp = HFILL;
            Add("<form method=\"post");
            if (act != null)
            {
                Add("\" action=\"");
                Add(act.Name);
            }
            Add("\">");
            Add("<div class=\"row small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
            obj.WriteData(this, proj);
            Add("</div>");
            Add("</form>");
            --level;
        }

        void PutPath()
        {
            for (int i = 0; i <= level; i++)
            {
                chain[i].OutputVarKey(this);
                Add('/');
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

        public void TABLE<D>(List<D> lst, int proj = 0) where D : IData
        {
            chain[++level].comp = HTABLE;
            if (lst != null)
            {
                Add("<table class=\"unstriped\">");

                chain[level].label = true;
                Add("<thead>");
                Add("<tr>");
                chain[level].ordinal = 0; // reset ordical
                lst[0].WriteData(this, proj);
                Add("</tr>");
                Add("</thead>");

                chain[level].label = false;
                Add("<tbody>");
                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<tr>");
                    chain[level].ordinal = 0; // reset ordical
                    lst[i].WriteData(this, proj);
                    Add("</tr>");
                }
                Add("</tbody>");
                Add("</table>");
            }
            --level;
        }

        public void GRID(IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            chain[++level].comp = HGRID;
            if (input != null)
            {
                Add("<div class=\"expanded row\">");
                while (input.Next())
                {
                    Add("<div class=\"row small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
                    chain[level].ordinal = 0; // reset ordical
                    valve(input, this);
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

        public void GRID<D>(Work work, List<D> lst, int proj = 0) where D : IData
        {
            ++level;
            chain[level].comp = HGRID;
            chain[level].varwork = work.varwork;

            if (lst != null)
            {
                Add("<div class=\"row expanded small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<div class=\"column card\">");
                    chain[level].ordinal = 0; // reset ordinal
                    D obj = lst[i];
                    chain[level].obj = obj; // reset ordinal
                    obj.WriteData(this, proj);
                    // acitons
                    ActionInfo[] varuias = work.Varwork?.UiActions;
                    if (varuias != null)
                    {
                        Add("<div class=\"row float-right\">");
                        TOOLS(varuias);
                        Add("</div>");
                    }
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

        public void LIST<D>(List<D> lst, int proj = 0) where D : IData
        {
            chain[++level].comp = HLIST;
            if (lst != null)
            {
                Add("<ul>");
                for (int i = 0; i < lst.Count; i++)
                {
                    Add("<li>");
                    chain[level].ordinal = 0; // reset ordical
                    lst[i].WriteData(this, proj);
                    Add("</li>");
                }
                Add("</ul>");
            }
            --level;
        }

        public void FILL(IDataInput input, Action<IDataInput, HtmlContent> valve)
        {
            chain[++level].comp = HFILL;
            if (input != null)
            {
                while (input.Next())
                {
                    chain[level].ordinal = 0; // reset ordical
                    valve(input, this);
                }
            }
            else
            {
                Add("<div class=\"row\">");
                Add("<span>没有记录</span>");
                Add("</div>");
            }
            --level;
        }

        public void FILL(IData obj, int proj = 0)
        {
            chain[++level].comp = HFILL;
            obj.WriteData(this, proj);
            --level;
        }

        public void TOOLS(ActionInfo[] acts)
        {
            for (int i = 0; i < acts.Length; i++)
            {
                ActionInfo act = acts[i];

                UiAttribute ui = act.Ui;

                if (ui.IsLink)
                {
                    Add("<a class=\"button hollow primary\" href=\"");
                    PutPath();
                    Add(act.Name);
                    Add("\"");
                    if (ui.HasDialog)
                    {
                        Add(" onclick=\"return dialog(this,1,2);\"");
                    }
                    Add(">");
                    Add(act.Label);
                    Add("</a>");
                }
                else if (ui.IsAnchor)
                {
                    Add("<a class=\"button hollow primary\" href=\"");
                    PutPath();
                    Add(act.Name);
                    Add("\"");
                    if (ui.HasDialog)
                    {
                        Add(" onclick=\"return dialog(this,2,3);\"");
                    }
                    else if (ui.HasScript)
                    {
                        Add(" onclick=\"");
                        Add(act.Name);
                        Add("(this);\"");
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
                    Add(act.Label);
                    Add("</a>");
                }
                else if (ui.IsButton)
                {
                    Add("<button class=\"button hollow primary\" name=\"");
                    Add(act.Name);
                    Add("\" formaction=\"");
                    PutPath();
                    Add(act.Name);
                    Add("\" formmethod=\"post\"");
                    if (ui.HasConfirm)
                    {
                        Add(" onclick=\"return confirm();\"");
                    }
                    else if (ui.HasDialog)
                    {
                        Add(" onclick=\"return dialog(this,4,2);\"");
                    }
                    string enable = ui.Enable;
                    if (enable != null)
                    {
                        // Add(" data-if=\""); Add(state.If); Add("\"");
                        // Add(" data-unif=\""); Add(state.Unif); Add("\"");
                    }
                    Add(">");
                    Add(act.Label);
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
            Add("</label>");
        }

        public void BUTTON(ActionInfo act)
        {
            Add("<button class=\"button primary\" style=\"margin-right: 5px; border-radius: 15%\"");
            Add(" formaction=\"");
            Add(act.Name);
            Add("\" formmethod=\"post\"");

            UiAttribute ui = act.Ui;

            UiMode mode = ui.Mode;
            if (mode > 0)
            {
                Add(" onclick=\"dialog(this,");
                Add((int) mode);
                Add("); return false;\"");
            }

            string state = ui.Enable;
            if (state != null)
            {
                // Add(" data-if=\""); Add(state.If); Add("\"");
                // Add(" data-unif=\""); Add(state.Unif); Add("\"");
            }
            Add(">");
            AddLabel(ui.Label, act.Name);

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
            chain[level].ordinal++;
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
            chain[level].ordinal++;
            return this;
        }

        public void Begin(string label)
        {
            chain[level].begin = label;
        }

        public void End()
        {
            chain[level].begin = null;
        }

        public HtmlContent Put(string name, bool v, Func<bool, string> opt = null, string label = null, bool required = false)
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td>");
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    AddLabel(label, name);
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    if (opt != null) Add(opt(v));
                    else Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    CHECKBOX(name, v, label, required);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short v, Opt<short> opt = null, string label = null, string help = null, short max = 0, short min = 0, short step = 0, bool @readonly = false, bool required = false)
        {
            var ctx = chain[level];
            switch (ctx.comp)
            {
                case HTABLE:
                    if (ctx.label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (formed && level == 0 && chain[level].ordinal == 0)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        if (opt != null)
                        {
                            Add(opt[v]);
                        }
                        else
                        {
                            Add(v);
                        }
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    if (formed && level == 0 && chain[level].ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\"");
                        Add(v);
                        Add("\">");
                    }
                    else
                    {
                        AddLabel(label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    if (opt != null) Add(opt[v]);
                    else Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    if (opt == null)
                    {
                        NUMBER(name, v);
                    }
                    else
                    {
                        SELECT(name, v, opt, label);
                    }
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, int v, Opt<int> opt = null, string label = null, string help = null, int max = 0, int min = 0, int step = 0, bool @readonly = false, bool required = false)
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (formed && level == 0 && chain[level].ordinal == 0)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    if (formed && level == 0 && chain[level].ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\"");
                        Add(v);
                        Add("\">");
                    }
                    else
                    {
                        AddLabel(label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    NUMBER(name, v, label);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, long v, Opt<long> opt = null, string label = null, string help = null, long max = 0, long min = 0, long step = 0, bool @readonly = false, bool required = false)
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        if (formed && level == 1)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    if (formed && level == 0 && chain[level].ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\"");
                        Add(v);
                        Add("\">");
                    }
                    else
                    {
                        AddLabel(label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    Add("<div class=\"pure-u-1 pure-u-md-1-2\">");
                    // NUMBER(name, v);
                    Add("</div>");
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    NUMBER(name, v, label);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, double v, string label = null, string help = null, double max = 0, double min = 0, double step = 0, bool @readonly = false, bool required = false)
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    AddLabel(label, name);
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, decimal v, string label = null, string help = null, decimal max = 0, decimal min = 0, decimal step = 0, bool @readonly = false, bool required = false)
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    AddLabel(label, name);
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    NUMBER(name, v, label);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, DateTime v, string label = null, DateTime max = default(DateTime), DateTime min = default(DateTime), int step = 0, bool @readonly = false, bool required = false)
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td>");
                        if (v != default(DateTime)) Add(v);
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    AddLabel(label, name);
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    if (v != default(DateTime)) Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    Add("<td style=\"text-align: right;\">");
                    Add(v);
                    Add("</td>");
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    DATE(name, v);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, string v, Opt<string> opt = null, string label = null, string help = null, string pattern = null, short max = 0, short min = 0, bool @readonly = false, bool required = false)
        {
            var ctx = chain[level];
            switch (ctx.comp)
            {
                case HTABLE:
                    if (ctx.label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td>");
                        if (formed && level == 0 && chain[level].ordinal == 0)
                        {
                            Add("<input type=\"checkbox\" name=\"pk\">");
                        }
                        Add(v);
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<div class=\"row\">");
                    Add("<div class=\"small-3 columns labeldiv\">");
                    if (formed && level == 0 && chain[level].ordinal == 0)
                    {
                        Add("<input type=\"checkbox\" name=\"pk\" value=\"");
                        Add(v);
                        Add("\">");
                    }
                    else
                    {
                        AddLabel(label, name);
                    }
                    Add("</div>");
                    Add("<div class=\"small-9 columns\">");
                    Add(v);
                    Add("</div>");
                    Add("</div>");
                    break;
                case HLIST:
                    Add(v);
                    break;
                case HFILL:
                    Add("<div class=\"column\">");
                    if (label != null && label.Length == 0)
                    {
                        HIDDEN(name, v);
                    }
                    else if (name.EndsWith("password") || name.EndsWith("pwd"))
                    {
                        PASSWORD(name, v, label, help, pattern, (sbyte) max, (sbyte) min, @readonly, required);
                    }
                    else if (max < 128)
                    {
                        TEXT(name, v, label, help, pattern, (sbyte) max, (sbyte) min, opt, @readonly, required);
                    }
                    else
                    {
                        TEXTAREA(name, v, label, help, max, min, @readonly, required);
                    }
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, ArraySegment<byte> v, string label = null, string size = null, string ratio = null, bool required = false)
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        Add("<td style=\"text-align: right;\">");
                        Add("</td>");
                    }
                    break;
                case HGRID:
                    Add("<img src=\"data:");
                    break;
                case HFILL:
                    Add("<div class=\"\">");
                    FILE(name, label, size, ratio, required);
                    Add("</div>");
                    break;
            }
            chain[level].ordinal++;
            return this;
        }

        public HtmlContent Put(string name, short[] v, Opt<short> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, int[] v, Opt<int> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, long[] v, Opt<long> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, string[] v, Opt<string> Opt = null, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, Dictionary<string, string> v, string Label = null, string Help = null, bool ReadOnly = false, bool Required = false)
        {
            return this;
        }

        public HtmlContent Put(string name, IData v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false)
        {
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, int proj = 0, string Label = null, string Help = null, bool ReadOnly = false, bool required = false) where D : IData
        {
            return this;
        }

        public HtmlContent Put<D>(string name, List<D> v, int proj = 0, string label = null, string help = null, bool @readonly = false, bool required = false) where D : IData
        {
            switch (chain[level].comp)
            {
                case HTABLE:
                    if (chain[level].label)
                    {
                        Add("<th>");
                        AddLabel(label, name);
                        Add("</th>");
                    }
                    else
                    {
                        if (v != null)
                        {
                            LIST(v, proj);
                        }
                        else
                        {
                            Add("<div class=\"row\"><span>没有记录</span></div>");
                        }
                    }
                    break;
                case HGRID:
                    if (v != null)
                    {
                        Add("<div class=\"row\">");
                        TABLE(v, proj);
                        Add("</div>");
                    }
                    else
                    {
                        Add("<div class=\"row\"><span>没有记录</span></div>");
                    }
                    break;
                case HLIST:
                    break;
            }
            chain[level].ordinal++;
            return this;
        }
    }
}