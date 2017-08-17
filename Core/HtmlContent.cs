using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// For dynamic HTML5 content tooled with Zurb Foundation
    /// </summary>
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

            internal Work varWork;

            internal IData obj;

            // within a group boundary
            internal bool group;

            internal void OutputVarKey(HtmlContent cont)
            {
                varWork.OutputVarKey(obj, cont);
            }
        }

        // outputing context chain
        const int DEPTH = 4;

        readonly Ctx[] chain = new Ctx[DEPTH];

        int level = -1; // current level

        public HtmlContent(bool octet, int capacity = 32 * 1024) : base(octet, capacity)
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

        public HtmlContent T(char v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(short v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(int v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(long v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(DateTime v)
        {
            Add(v);
            return this;
        }

        public HtmlContent T(decimal v)
        {
            Add(v);
            return this;
        }

        public HtmlContent ROW_()
        {
            Add("<div class=\"row\">");
            return this;
        }

        public HtmlContent _ROW()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent CARD_()
        {
            Add("<div class=\"card\" style=\"margin: 0.25rem\">");
            return this;
        }

        public HtmlContent _CARD()
        {
            Add("</div>");
            return this;
        }

        public HtmlContent CARDITEM(string label, string v)
        {
            Add("<div class=\"row\" style=\"margin: 0;\">");
            Add("<div class=\"small-3 columns labeldiv\">");
            Add(label);
            Add("</div>");
            Add("<div class=\"small-9 columns\">");
            Add(v);
            Add("</div>");
            Add("</div>");
            return this;
        }

        public HtmlContent FORM_(string caption = null, string action = null, bool post = true, bool mp = false)
        {
            Add("<div class=\"row column align-center small-10 medium-8 large-6 container-padded\">");
            Add("<h2>");
            if (caption != null)
            {
                AddEsc(caption);
            }
            Add("</h2>");
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
            return this;
        }

        public HtmlContent _FORM()
        {
            Add("</form>");
            Add("</div>");
            return this;
        }

        public HtmlContent FIELDSET_(string legend = null)
        {
            Add("<fieldset class=\"fieldset\">");
            if (legend != null)
            {
                Add("<legend>");
                AddEsc(legend);
                Add("</legend>");
            }
            return this;
        }

        public HtmlContent _FIELDSET()
        {
            Add("</fieldset>");
            return this;
        }

        public HtmlContent CALLOUT(string v, bool closable = false)
        {
            Add("<div class=\"callout primary\"");
            if (closable)
            {
                Add(" data-closable");
            }
            Add("><p class=\"text-center\">");
            Add(v);
            Add("</p>");
            if (closable)
            {
                Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            }
            Add("</div>");
            return this;
        }

        public HtmlContent CALLOUT(Action<HtmlContent> m, bool closable)
        {
            Add("<div class=\"callout primary\"");
            if (closable)
            {
                Add(" data-closable");
            }
            Add("><p class=\"text-center\">");
            m?.Invoke(this);
            Add("</p>");
            if (closable)
            {
                Add("<button class=\"close-button\" type=\"button\" data-close><span>&times;</span></button>");
            }
            Add("</div>");
            return this;
        }

        public void TOOLBAR(Work work, object model)
        {
            Add("<div data-sticky-container>");
            Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
            Add("<div class=\"title-bar\">");

            Add("<div class=\"title-bar-left\">");
            if (work.UiActions != null)
            {
                TRIGGERS(work.UiActions, obj: model);
            }
            Add("</div>");

            Add("<div class=\"title-bar-title\">");
            Add("<a class=\"button primary hollow\" href=\"javascript: location.reload(false);\">刷新</a>");
            Add("</div>");

            Add("</div>");
            Add("</div>");
            Add("</div>");
        }

        const int VPAGES = 6;

        public void PAGENATE(ActionContext ac, int count)
        {
            // pagination
            ActionInfo ai = ac.Doer;
            if (ai.HasSubscript)
            {
                Add("<ul class=\"pagination text-center\" role=\"navigation\">");
                int subscpt = ac.Subscript;
                for (int i = 0; i <= subscpt; i++)
                {
                    if (subscpt == i)
                    {
                        Add("<li class=\"current\">");
                        Add(i + 1);
                        Add("</li>");
                    }
                    else
                    {
                        Add("<li><a href=\"");
                        Add(ai.Name);
                        Add('-');
                        Add(i);
                        Add(ac.QueryString);
                        Add("\">");
                        Add(i + 1);
                        Add("</a></li>");
                    }
                }
                if (count == ai.Limit)
                {
                    Add("<li class=\"pagination-next\"><a href=\"");
                    Add(ai.Name);
                    Add('-');
                    Add(subscpt + 1);
                    Add(ac.QueryString);
                    Add("\">");
                    Add(subscpt + 2);
                    Add("</a></li>");
                }
                Add("</ul>");
            }
        }

        public HtmlContent TH(params string[] labels)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                Add("<th>");
                Add(labels[i]);
                Add("</th>");
            }
            return this;
        }

        public HtmlContent TD(short v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(int v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(long v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(decimal v)
        {
            Add("<td style=\"text-align: right\">");
            Add(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD(string v)
        {
            Add("<td>");
            AddEsc(v);
            Add("</td>");
            return this;
        }

        public HtmlContent TD_()
        {
            Add("<td>");
            return this;
        }

        public HtmlContent _TD()
        {
            Add("</td>");
            return this;
        }

        public HtmlContent TABLE(string name, IDataInput inp, Action<IDataInput, HtmlContent, char> putter)
        {
            Add("<table class=\"unstriped\">");

            Add("<thead>");
            Add("<tr>");
            if (name != null)
            {
                Add("<th></th>");
            }
            putter(inp, this, 'L'); // putting value
            Add("</tr>");
            Add("</thead>");

            Add("<tbody>");
            while (inp.Next())
            {
                Add("<tr>");
                putter(inp, this, 'B'); // putting label
                Add("</tr>");
            }
            Add("</tbody>");
            Add("</table>");
            return this;
        }

        public HtmlContent TABLE<D>(ActionContext formctx, Work varWork, D[] arr, int proj = 0x00ff) where D : IData
        {
            bool checks = false; // to draw selection-checkboxes?
            if (formctx != null)
            {
                Add("<form id=\"tableform\">");
                TOOLBAR(formctx.Work, arr);
                checks = formctx.Work.Buttons > 0;
            }

            if (arr != null)
            {
                ++level;
                chain[level].varWork = varWork;

                Add("<table class=\"unstriped\">");

                ActionInfo[] ais = varWork?.UiActions;

                chain[level].node = TABLE_THEAD;

                Add("<thead>");
                Add("<tr>");

                if (checks && level == 0)
                {
                    Add("<th></th>");
                }

                arr[0].Write(this, proj);

                if (ais != null)
                {
                    Add("<th></th>"); // head for triggers
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

                    if (checks && level == 0)
                    {
                        Add("<td>");
                        Add("<input name=\"key\" type=\"checkbox\" value=\"");
                        varWork?.OutputVarKey(obj, this);
                        Add("</td>");
                    }

                    arr[i].Write(this, proj);

                    // acitons
                    if (ais != null)
                    {
                        Add("<td style=\"width: 1px; white-space: nowrap;\">");
                        TRIGGERS(ais);
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
                PAGENATE(formctx, arr == null ? 0 : arr.Length);

                Add("</form>");
            }
            return this;
        }

        public HtmlContent GRID(IDataInput input, Action<IDataInput, HtmlContent> pipe)
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
            return this;
        }

        public HtmlContent GRID<D>(ActionContext formCtx, Work varWork, D[] arr, int proj = 0x00ff) where D : IData
        {
            bool checks = false; // to render checkboxes?
            if (formCtx != null)
            {
                Add("<form id=\"gridform\">");
                TOOLBAR(formCtx.Work, arr);
                checks = formCtx.Work.Buttons > 0;
            }

            if (arr != null) // render grid component
            {
                ++level;
                chain[level].node = GRID_DIV;
                chain[level].varWork = varWork;

                Add("<div class=\"row expanded small-up-1 medium-up-2 large-up-3 xlarge-up-4\">");
                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<div class=\"column\">");
                    Add("<div class=\"card\">");
                    D obj = arr[i];
                    chain[level].obj = obj;

                    if (checks && level == 0)
                    {
                        Add("<div class=\"row\">");
                        Add("<div class=\"small-3 columns labeldiv\">");
                        Add("</div>");
                        Add("<div class=\"small-9 columns\" style=\"text-align: right\">");
                        Add("<input name=\"key\" type=\"checkbox\" style=\"margin: 0 0.5rem\" value=\"");
                        varWork?.OutputVarKey(obj, this);
                        Add("\">");
                        Add("</div>");
                        Add("</div>");
                    }

                    obj.Write(this, proj);

                    // output action triggers
                    ActionInfo[] ais = varWork?.UiActions;
                    if (ais != null)
                    {
                        Add("<div style=\"text-align: right; border-top: 1px solid silver\">");
                        TRIGGERS(ais, null, obj);
                        Add("</div>");
                    }
                    Add("</div>");
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

            if (formCtx != null)
            {
                // pagination controls if any
                PAGENATE(formCtx, arr == null ? 0 : arr.Length);
                Add("</form>");
            }
            return this;
        }

        public HtmlContent LIST<D>(ActionContext formed, Work varWork, D[] arr, int proj = 0x00ff) where D : IData
        {
            if (formed != null)
            {
                Add("<form id=\"listform\">");
                TOOLBAR(formed.Work, arr);
            }

            if (arr != null)
            {
                ++level;
                chain[level].node = LIST_UL;
                chain[level].varWork = varWork;

                Add("<ul>");
                for (int i = 0; i < arr.Length; i++)
                {
                    Add("<li>");
                    arr[i].Write(this, proj);
                    Add("</li>");
                }
                Add("</ul>");
                --level;
            }

            if (formed != null)
            {
                // pagination controls if any
                PAGENATE(formed, arr == null ? 0 : arr.Length);
                Add("</form>");
            }
            return this;
        }

        public HtmlContent TRIGGERS(ActionInfo[] ais, ActionContext ac = null, object obj = null)
        {
            if (ais == null) return this;

            for (int i = 0; i < ais.Length; i++)
            {
                ActionInfo ai = ais[i];

                // access check if neccessary
                if (ac != null && !ai.DoAuthorize(ac)) continue;

                UiAttribute ui = ai.Ui;

                // check state masking
                var statable = obj as IStatable;
                if (statable != null)
                {
                    int state = statable.GetState();
                    if (!ui.HasState(state)) continue;
                }

                if (ui.IsAnchor)
                {
                    Add("<a class=\"button success");
                    if (!ui.Bold) Add(" hollow");
                    Add("\" href=\"");
                    for (int lvl = 0; lvl <= level; lvl++)
                    {
                        chain[lvl].OutputVarKey(this);
                        Add('/');
                    }
                    Add(ai.Name);
                    Add("\"");
                    if (ai.Asker != null && ai.Asker(chain[level].obj))
                    {
                        Add(" style=\"color: red;\"");
                    }
                    if (ai.Disabler != null && ai.Disabler(chain[level].obj))
                    {
                        Add(" disabled onclick=\"return false;\"");
                    }
                    else if (ui.HasPrompt)
                    {
                        Add(" onclick=\"return dialog(this,2,1,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasShow)
                    {
                        Add(" onclick=\"return dialog(this,4,2,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasOpen)
                    {
                        Add(" onclick=\"return dialog(this,8,3,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasScript)
                    {
                        Add(" onclick=\"if(!confirm('");
                        Add(ui.Tip ?? ui.Label);
                        Add("')) return false;");
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
                        Add(",'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    Add(">");
                    Add(ai.Label);
                    Add("</a>");
                }
                else if (ui.IsButton)
                {
                    Add("<button class=\"button success");
                    if (!ui.Bold) Add(" hollow");
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
                    if (ai.Asker != null && ai.Asker(chain[level].obj))
                    {
                        Add(" style=\"color: red; \"");
                    }
                    if (ai.Disabler != null && ai.Disabler(chain[level].obj))
                    {
                        Add(" disabled");
                    }
                    else if (ui.HasConfirm)
                    {
                        Add(" onclick=\"return confirm('");
                        Add(ui.Tip ?? ui.Label);
                        Add("?');\"");
                    }
                    else if (ui.HasPrompt)
                    {
                        Add(" onclick=\"return dialog(this,2,1,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasShow)
                    {
                        Add(" onclick=\"return dialog(this,4,2,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    else if (ui.HasOpen)
                    {
                        Add(" onclick=\"return dialog(this,8,3,'");
                        Add(ui.Tip);
                        Add("');\"");
                    }
                    Add(">");
                    Add(ai.Label);
                    Add("</button>");
                }
            }
            return this;
        }

        public HtmlContent BUTTON(string value, bool post = true)
        {
            Add("<button class=\"button primary hollow\" formmethod=\"");
            Add(post ? "post" : "get");
            Add("\">");
            AddEsc(value);
            Add("</button>");
            return this;
        }

        public HtmlContent BUTTON(string name, int subcmd, string value, bool post = true)
        {
            Add("<button class=\"button primary hollow\" formmethod=\"");
            Add(post ? "post" : "get");
            Add("\" formaction=\"");
            Add(name);
            Add('-');
            Add(subcmd);
            Add("\">");
            AddEsc(value);
            Add("</button>");
            return this;
        }


        public HtmlContent HIDDEN(string name, string value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            AddEsc(value);
            Add("\">");
            return this;
        }

        public HtmlContent HIDDEN(string name, int value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
            return this;
        }

        public HtmlContent HIDDEN(string name, decimal value)
        {
            Add("<input type=\"hidden\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            Add("\">");
            return this;
        }

        public HtmlContent TEXT(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, IOptable<string> opt = null, bool @readonly = false, bool required = false)
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
            return this;
        }

        public HtmlContent SEARCH(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"search\" name=\"");
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

            Add(">");

            Add("</label>");
            return this;
        }

        public HtmlContent PASSWORD(string name, string v, string label = null, string help = null, string pattern = null, sbyte max = 0, sbyte min = 0, bool @readonly = false, bool required = false)
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
            return this;
        }

        public HtmlContent DATE(string name, DateTime v, string label = null, DateTime max = default(DateTime), DateTime min = default(DateTime), bool @readonly = false, bool required = false, int step = 0)
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
            return this;
        }

        public HtmlContent TIME()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent NUMBER(string name, short v, string label = null, string tip = null, short max = 0, short min = 0, short step = 0, bool opt = false, bool @readonly = false, bool required = false)
        {
            if (label != null)
            {
                Add("<label>");
                AddLabel(label, name);
            }

            bool group = step > 0; // input group with up up and down

            if (group)
            {
                Add("<div class=\"input-group\">");
                Add("<input type=\"button\" class=\"input-group-label\" onclick=\"this.form['");
                Add(name);
                Add("'].stepDown()\" value=\"-\">");
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

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
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
                Add("<input type=\"button\" class=\"input-group-label\" onclick=\"this.form['");
                Add(name);
                Add("'].stepUp()\" value=\"+\">");
                Add("</div>");
            }

            if (label != null)
            {
                Add("</label>");
            }
            return this;
        }

        public HtmlContent NUMBER(string name, int v, string label = null, string tip = null, int max = 0, int min = 0, int step = 0, bool opt = false, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
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
            return this;
        }

        public HtmlContent NUMBER(string name, long v, string label = null, string tip = null, long max = 0, long min = 0, long step = 0, bool opt = false, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
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
            return this;
        }

        public HtmlContent NUMBER(string name, decimal v, string label = null, string tip = null, decimal max = 0, decimal min = 0, decimal step = 0, bool @readonly = false, bool required = false)
        {
            Add("<label>");
            AddLabel(label, name);
            Add("<input type=\"number\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v);
            Add("\"");

            if (tip != null)
            {
                Add(" placeholder=\"");
                Add(tip);
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

            Add(" step=\"");
            if (step > 0) Add(step);
            else Add("any");
            Add("\"");

            if (@readonly) Add(" readonly");
            if (required) Add(" required");

            Add(">");
            Add("</label>");
            return this;
        }

        public HtmlContent RANGE()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent COLOR()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent CHECKBOXES(string name, IDataInput inp, Action<IDataInput, HtmlContent, char> putter)
        {
            while (inp.Next())
            {
                Add("<label>");
                Add("<input type=\"checkbox\" name=\"");
                Add(name);
                Add("\" value=\"");
                putter(inp, this, 'V'); // putting value
                Add("\">");
                putter(inp, this, 'L'); // putting label
                Add("</label>");
            }
            return this;
        }

        public HtmlContent CHECKBOX(string name, bool v, string label = null, bool required = false)
        {
            if (label != null)
            {
                Add("<label>");
            }
            Add("<input type=\"checkbox\" name=\"");
            Add(name);
            Add("\"");
            if (v) Add(" checked");
            if (required) Add(" required");
            Add(">");
            if (label != null)
            {
                Add(label);
                Add(" </label>");
            }
            return this;
        }

        public HtmlContent RADIO(string name, int value, bool @checked, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, long value, bool check, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (check)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, string value, bool check, string label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(value);
            if (check)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(label);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, Action<HtmlContent> value, bool @checked, Action<HtmlContent> label)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            value(this);
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            label(this);
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, string v1, string v2, string v3, bool @checked, string l1, string l2, string l3)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v1);
            if (v2 != null)
            {
                Add('~');
                Add(v2);
            }
            if (v3 != null)
            {
                Add('~');
                Add(v3);
            }
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(l1);
            if (l2 != null)
            {
                Add(' ');
                Add(l2);
            }
            if (l3 != null)
            {
                Add(' ');
                Add(l3);
            }
            Add("</label>");
            return this;
        }

        public HtmlContent RADIO(string name, long v1, string v2, bool @checked, long l1, string l2, string l3 = null)
        {
            Add("<label>");
            Add("<input type=\"radio\" name=\"");
            Add(name);
            Add("\" value=\"");
            Add(v1);
            if (v2 != null)
            {
                Add('-');
                Add(v2);
            }
            if (@checked)
            {
                Add("\" checked>");
            }
            else
            {
                Add("\">");
            }
            Add(l1);
            Add(' ');
            Add(l2);
            if (l3 != null)
            {
                Add(' ');
                Add(l3);
            }
            Add("</label>");
            return this;
        }

        public HtmlContent RADIOS(string name, short v, IOptable<short> opt = null, string label = null, bool required = false)
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            opt?.ForEach((key, item) =>
            {
                Add("<input type=\"radio\" name=\"");
                Add(name);

                Add("\" id=\"");
                Add(name);
                Add(key);
                Add("\"");

                Add("\" value=\"");
                Add(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");

                Add("<label for=\"");
                Add(name);
                Add(key);
                Add("\">");
                Add(item.ToString());
                Add("</label>");
                Add("<br>");
            });

            Add("</fieldset>");
            return this;
        }

        public HtmlContent RADIOS(string name, string v, IOptable<string> opt = null, string label = null, bool required = false)
        {
            Add("<fieldset>");

            Add("<legend>");
            AddLabel(label, name);
            Add("</legend>");

            opt?.ForEach((key, item) =>
            {
                Add("<input type=\"radio\" name=\"");
                Add(name);

                Add("\" id=\"");
                Add(name);
                Add(key);
                Add("\"");

                Add("\" value=\"");
                Add(key);
                Add("\"");

                if (key.Equals(v)) Add(" checked");
                if (required) Add(" required");
                Add(">");

                Add("<label for=\"");
                Add(name);
                Add(key);
                Add("\">");
                Add(item.ToString());
                Add("</label>");
            });

            Add("</fieldset>");
            return this;
        }

        public HtmlContent TEXTAREA(string name, string v, string label = null, string help = null, short max = 0, short min = 0, bool @readonly = false, bool required = false)
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
            return this;
        }

        public HtmlContent BUTTON(ActionInfo ai)
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

            if (ai.Disabler != null)
            {
                // Add(" data-if=\""); Add(state.If); Add("\"");
                // Add(" data-unif=\""); Add(state.Unif); Add("\"");
            }
            Add(">");
            AddLabel(ui.Label, ai.Name);

            Add("</button>");
            return this;
        }

        public HtmlContent SELECT(string name, short v, IOptable<short> opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0)
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

            opt?.ForEach((key, item) =>
            {
                Add("<option value=\"");
                Add(key);
                Add("\"");
                if (key == v) Add(" selected");
                Add(">");
                Add(item.ToString());
                Add("</option>");
            });

            Add("</select>");
            Add("</label>");
            return this;
        }

        public HtmlContent SELECT(string name, string v, IOptable<string> opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0)
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

            opt?.ForEach((key, text) =>
            {
                Add("<option value=\"");
                Add(key);
                Add("\"");
                if (key == v) Add(" selected");
                Add(">");
                Add(opt.Obtain(key));
                Add("</option>");
            });

            Add("</select>");
            Add("</label>");
            return this;
        }

        public HtmlContent SELECT(string name, string v, string[] opt, string label = null, bool multiple = false, bool required = false, sbyte size = 0, bool refresh = false)
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

            if (opt != null)
            {
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
            }
            Add("</select>");
            Add("</label>");
            return this;
        }

        public HtmlContent DATALIST()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent PROGRES()
        {
            T("</tbody>");
            return this;
        }

        public HtmlContent OUTPUT(string name)
        {
            Add("<output name=\"");
            Add(name);
            Add("\"></output>");
            return this;
        }

        public HtmlContent METER()
        {
            T("</tbody>");
            return this;
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

        public HtmlContent Put(string name, bool v, string label = null, Func<bool, string> opt = null)
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

        public HtmlContent Put(string name, short v, string label = null, IOptable<short> opt = null)
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
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        if (opt != null) Add(opt.Obtain(v));
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
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, int v, string label = null, IOptable<int> opt = null)
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
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        if (opt != null) Add(opt.Obtain(v));
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
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add(' ');
                    }
                    break;
                case LIST_UL:
                    break;
            }
            return this;
        }

        public HtmlContent Put(string name, long v, string label = null, IOptable<long> opt = null)
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
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add("</td>");
                    }
                    else
                    {
                        if (opt != null) Add(opt.Obtain(v));
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
                        if (opt != null) Add(opt.Obtain(v));
                        else Add(v);
                        Add("</div>");
                        Add("</div>");
                    }
                    else
                    {
                        if (opt != null) Add(opt.Obtain(v));
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

        public HtmlContent Put(string name, decimal v, string label = null, char format = '\0')
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
                        if (format == '¥')
                        {
                            Add("<strong class=\"money\">&yen;");
                            Add(v);
                            Add("</strong>");
                        }
                        else
                        {
                            Add(v);
                        }
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
                        if (format == '¥')
                        {
                            Add("<strong class=\"money\">&yen;");
                            Add(v);
                            Add("</strong>");
                        }
                        else
                        {
                            Add(v);
                        }
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

        public HtmlContent Put(string name, string v, string label = null, IOptable<string> opt = null)
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

        public HtmlContent Put(string name, short[] v, string label = null, IOptable<short> opt = null)
        {
            return this;
        }

        public HtmlContent Put(string name, int[] v, string label = null, IOptable<int> Opt = null)
        {
            return this;
        }

        public HtmlContent Put(string name, long[] v, string label = null, IOptable<long> Opt = null)
        {
            return this;
        }

        public HtmlContent Put(string name, string[] v, string label = null, IOptable<string> Opt = null)
        {
            return this;
        }

        public HtmlContent Put(string name, Dictionary<string, string> v, string label = null)
        {
            return this;
        }

        public HtmlContent Put(string name, IData v, int proj = 0x00ff, string label = null)
        {
            return this;
        }

        public HtmlContent Put<D>(string name, D[] v, int proj = 0x00ff, string label = null) where D : IData
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
                        LIST(null, null, v, proj);
                        Add("</td>");
                    }
                    else
                    {
                        LIST(null, null, v, proj);
                    }
                    break;
                case GRID_DIV:
                    if (v != null)
                    {
                        Add("<div class=\"row column\">");
                        TABLE(null, chain[level].varWork?.varWork, v, proj);
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