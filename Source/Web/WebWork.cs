using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ChainFx.Web
{
    /// <summary>
    /// A work is a virtual web folder that contains a single or collection of resources along with operations on it or them.
    /// </summary>
    public abstract class WebWork : IKeyable<string>
    {
        const string VARDIR = "_var_", VARPATHING = "<var>";

        // declared actions 
        readonly Map<string, WebAction> actions = new Map<string, WebAction>(32);

        // the default action, can be null
        readonly WebAction @default;

        // the catch action, can be null
        internal readonly WebAction @catch;

        // actions with ToolAttribute
        readonly WebAction[] tooled;

        // subworks, if any
        Map<string, WebWork> subworks;

        // variable-key subwork, if any
        WebWork varwork;


        public string Name { get; internal set; }

        public string Folder { get; internal set; }

        public UiAttribute Ui { get; internal set; }

        public AuthenticateAttribute Authenticate { get; internal set; }

        public AuthorizeAttribute Authorize { get; internal set; }

        // pre-action operation
        public BeforeAttribute Before { get; internal set; }

        // post-action operation
        public AfterAttribute After { get; internal set; }

        public object State { get; set; }


        // to obtain a string key from a data object.
        protected WebWork()
        {
            type = GetType();

            Ui = (UiAttribute) type.GetCustomAttribute(typeof(UiAttribute), true);
            Authenticate = (AuthenticateAttribute) type.GetCustomAttribute(typeof(AuthenticateAttribute), false);
            Authorize = (AuthorizeAttribute) type.GetCustomAttribute(typeof(AuthorizeAttribute), false);
            Before = (BeforeAttribute) type.GetCustomAttribute(typeof(BeforeAttribute), false);
            After = (AfterAttribute) type.GetCustomAttribute(typeof(AfterAttribute), false);

            // gather actions
            foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                var ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task)) async = true;
                else if (ret == typeof(void)) async = false;
                else continue;

                var pis = mi.GetParameters();
                WebAction act;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                {
                    act = new WebAction(this, mi, async, null);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(int))
                {
                    act = new WebAction(this, mi, async, pis[1].Name);
                }
                else continue;

                actions.Add(act);
                if (act.Key == string.Empty) @default = act;
                if (act.Key == "catch") @catch = act;
            }

            // gather tooled action methods
            var list = new ValueList<WebAction>(16);
            for (int i = 0; i < actions.Count; i++)
            {
                var a = actions.EntryAt(i).Value;
                if (a.Tool != null)
                {
                    list.Add(a);
                }
            }

            tooled = list.ToArray();
        }

        public virtual string Key => Name;

        public string Label => Ui?.Label;

        public string Tip => Ui?.Tip;

        public string Icon => Ui?.Icon;

        public short Group => Ui?.Group ?? 0;

        readonly Type type;

        public WebService Service { get; internal set; }

        public WebWork Parent { get; internal set; }

        public int Level { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string Pathing { get; internal set; }

        public Map<string, WebAction> Actions => actions;

        public WebAction[] Tooled => tooled;

        public WebAction Default => @default;

        public WebAction Catch => @catch;

        public Map<string, WebWork> SubWorks => subworks;

        public WebWork VarWork => varwork;

        public bool HasVarWork => varwork != null;

        public bool IsOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

        public WebAction this[string method] => string.IsNullOrEmpty(method) ? @default : actions[method];

        public WebAction this[int index] => actions.EntryAt(index).Value;

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        // to resolve from the principal object.
        public Func<IData, string, object> Accessor { get; internal set; }

        public object GetAccessor(IData prin, string key)
        {
            return Accessor?.Invoke(prin, key);
        }


        protected internal virtual void OnCreate()
        {
        }

        /// <summary>
        /// Create and add a variable-key subwork.
        /// </summary>
        /// <param name="accessor">to resolve key from the principal object</param>
        /// <param name="state"> </param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authenticate"></param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="WebException">Thrown if error</exception>
        protected T CreateVarWork<T>(Func<IData, string, object> accessor = null, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null) where T : WebWork, new()
        {
            var wrk = new T
            {
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = true,
                Directory = (Parent == null) ? VARDIR : Path.Combine(Parent.Directory, VARDIR),
                Pathing = Pathing + (accessor == null ? VARPATHING : string.Empty) + "/",
                Accessor = accessor,
                State = state
            };
            if (ui != null) wrk.Ui = ui;
            if (authenticate != null) wrk.Authenticate = authenticate;
            if (authorize != null) wrk.Authorize = authorize;
            if (before != null) wrk.Before = before;
            if (after != null) wrk.After = after;

            varwork = wrk;

            wrk.OnCreate();
            return wrk;
        }

        /// <summary>
        /// Create and add a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="state"></param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authenticate"></param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        /// <typeparam name="T">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="WebException">Thrown if error</exception>
        protected T CreateWork<T>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null) where T : WebWork, new()
        {
            if (subworks == null)
            {
                subworks = new Map<string, WebWork>();
            }

            var wrk = new T
            {
                Name = name,
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = false,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
                Pathing = Pathing + name + "/",
                State = state
            };
            if (ui != null) wrk.Ui = ui;
            if (authenticate != null) wrk.Authenticate = authenticate;
            if (authorize != null) wrk.Authorize = authorize;
            if (before != null) wrk.Before = before;
            if (after != null) wrk.After = after;

            subworks.Add(wrk);

            wrk.OnCreate();
            return wrk;
        }

        protected void Describe(XmlContent xc)
        {
            xc.ELEM(Key,
                delegate
                {
                    for (int i = 0; i < actions.Count; i++)
                    {
                        var act = actions.EntryAt(i).Value;
                        xc.Put(act.Key, "");
                    }
                },
                delegate
                {
                    if (subworks != null)
                    {
                        for (int i = 0; i < subworks.Count; i++)
                        {
                            var wrk = subworks.EntryAt(i).Value;
                            wrk.Describe(xc);
                        }
                    }

                    varwork?.Describe(xc);
                }
            );
        }

        protected void Describe(HtmlContent h)
        {
            for (int i = 0; i < actions?.Count; i++)
            {
                var a = actions.ValueAt(i);
                if (a.Tags != null)
                {
                    h.ARTICLE_("uk-card uk-card-primary");
                    h.HEADER_("uk-card-header").TT(a.Pathing)._HEADER();
                    h.MAIN_("uk-card-body");
                    for (int k = 0; k < a.Tags.Length; k++)
                    {
                        var c = a.Tags[k];
                        c.Describe(h);
                    }

                    h._MAIN();
                    h._ARTICLE();
                }
            }

            varwork?.Describe(h);

            for (int i = 0; i < subworks?.Count; i++)
            {
                var w = subworks.EntryAt(i).Value;
                w.Describe(h);
            }
        }

        internal async Task<bool> DoAuthenticate(WebContext wc)
        {
            if (Authenticate != null)
            {
                if (Authenticate.IsAsync && !await Authenticate.DoAsync(wc) || !Authenticate.IsAsync && !Authenticate.Do(wc))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DoAuthorize(WebContext wc, bool mock)
        {
            if (Authorize != null)
            {
                return Authorize.Do(wc, mock);
            }

            return true;
        }


        public override string ToString()
        {
            return Name;
        }

        //
        // object locator

        Hold[] cells;

        int size;

        public void Attach(object value, short flag = 0)
        {
            if (cells == null)
            {
                cells = new Hold[16];
            }

            cells[size++] = new Hold(value, flag);
        }


        public T Lookup<T>(short flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var c = cells[i];
                    if (c.Flag == 0 || (c.Flag & flag) > 0)
                    {
                        if (typeof(T).IsAssignableFrom(c.Typ))
                        {
                            return c.Value as T;
                        }
                    }
                }
            }
            return Parent?.Lookup<T>(flag);
        }


        /// <summary>
        /// A object holder in registry.
        /// </summary>
        class Hold
        {
            readonly Type typ;

            readonly object value;

            readonly short flag;

            internal Hold(object value, short flag)
            {
                this.typ = value.GetType();
                this.value = value;
                this.flag = flag;
            }

            public Type Typ => typ;

            public short Flag => flag;

            public object Value => value;
        }
    }
}