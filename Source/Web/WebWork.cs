using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CloudUn.Db;

namespace CloudUn.Web
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
        Map<string, WebWork> works;

        // variable-key subwork, if any
        WebWork varwork;


        public string Name { get; internal set; }

        public UiAttribute Ui { get; internal set; }

        public AuthenticateAttribute Authenticate { get; internal set; }

        public AuthorizeAttribute Authorize { get; internal set; }

        // pre-action operation
        public BeforeAttribute Before { get; internal set; }

        // post-action operation
        public AfterAttribute After { get; internal set; }

        // to obtain a string key from a data object.
        protected WebWork()
        {
            type = GetType();

            Ui = (UiAttribute) type.GetCustomAttribute(typeof(UiAttribute), false);
            Authenticate = (AuthenticateAttribute) type.GetCustomAttribute(typeof(AuthenticateAttribute), false);
            Authorize = (AuthorizeAttribute) type.GetCustomAttribute(typeof(AuthorizeAttribute), false);
            Before = (BeforeAttribute) type.GetCustomAttribute(typeof(BeforeAttribute), false);
            After = (AfterAttribute) type.GetCustomAttribute(typeof(AfterAttribute), false);

            // gather actions
            foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                Type ret = mi.ReturnType;
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
                var a = actions.At(i).Value;
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

        public byte Group => Ui?.Group ?? 0;

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

        public Map<string, WebWork> Works => works;

        public WebWork VarWork => varwork;

        public bool IsOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

        public WebAction this[string method] => string.IsNullOrEmpty(method) ? @default : actions[method];

        public WebAction this[int index] => actions.At(index).Value;

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        // to resolve from the principal object.
        public Func<IData, object> Accessor { get; internal set; }

        public object GetAccessor(IData prin)
        {
            return Accessor?.Invoke(prin);
        }


        protected internal virtual void OnCreate()
        {
        }

        /// <summary>
        /// Create and add a variable-key subwork.
        /// </summary>
        /// <param name="accessor">to resolve key from the principal object</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param
        ///     name="authenticate">
        /// </param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <param
        ///     name="before">
        /// </param>
        /// <param
        ///     name="after">
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="WebException">Thrown if error</exception>
        protected T CreateVarWork<T>(Func<IData, object> accessor = null, UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null) where T : WebWork, new()
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
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param
        ///     name="authenticate">
        /// </param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <param
        ///     name="before">
        /// </param>
        /// <param
        ///     name="after">
        /// </param>
        /// <typeparam name="T">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="WebException">Thrown if error</exception>
        protected T CreateWork<T>(string name, UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null) where T : WebWork, new()
        {
            if (works == null)
            {
                works = new Map<string, WebWork>();
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
            };
            if (ui != null) wrk.Ui = ui;
            if (authenticate != null) wrk.Authenticate = authenticate;
            if (authorize != null) wrk.Authorize = authorize;
            if (before != null) wrk.Before = before;
            if (after != null) wrk.After = after;

            works.Add(wrk);

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
                        var act = actions.At(i).Value;
                        xc.Put(act.Key, "");
                    }
                },
                delegate
                {
                    if (works != null)
                    {
                        for (int i = 0; i < works.Count; i++)
                        {
                            var wrk = works.At(i).Value;
                            wrk.Describe(xc);
                        }
                    }

                    varwork?.Describe(xc);
                }
            );
        }

        protected void Describe(HtmlContent hc)
        {
            for (int i = 0; i < actions?.Count; i++)
            {
                var a = actions.At(i).Value;
                if (a.Tags != null)
                {
                    hc.T("<article style=\"border: 1px solid silver; padding: 8px;\">");
                    hc.T("<h3><code>").TT(a.Pathing).T("</code></h3>");
                    hc.HR();
                    for (int k = 0; k < a.Tags.Length; k++)
                    {
                        var c = a.Tags[k];
                        c.Print(hc);
                    }

                    hc.T("</article>");
                }
            }

            varwork?.Describe(hc);

            for (int i = 0; i < works?.Count; i++)
            {
                var w = works.At(i).Value;
                w.Describe(hc);
            }
        }

        readonly WebException AuthReq = new WebException("Authentication required") {Code = 401};

        readonly WebException AccessorReq = new WebException("Accessor required") {Code = 500};

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

        public bool DoAuthorize(WebContext wc)
        {
            if (Authorize != null)
            {
                // check if trusted peer
                if (wc.CallerSign != null && wc.CallerSign == Framework.sign)
                {
                    return true; // trusted without further check
                }

                return Authorize.Do(wc);
            }

            return true;
        }

        internal async Task HandleAsync(string rsc, WebContext wc)
        {
            wc.Work = this;

            if (!DoAuthorize(wc))
            {
                throw new WebException("Do authorize failure: " + Name) {Code = wc.Principal == null ? 401 : 403};
            }

            int slash = rsc.IndexOf('/');
            if (slash == -1) // this work is the target work
            {
                if (Before != null)
                {
                    if (Before.IsAsync && !await Before.DoAsync(wc) || !Before.IsAsync && Before.Do(wc))
                    {
                        return;
                    }
                }

                //
                // resolve the resource
                string name = rsc;
                int subscpt = 0;
                int dash = rsc.LastIndexOf('-');
                if (dash != -1)
                {
                    name = rsc.Substring(0, dash);
                    wc.Subscript = subscpt = rsc.Substring(dash + 1).ToInt();
                }

                var act = this[name];
                if (act == null)
                {
                    wc.Give(404, "action not found", true, 12);
                    return;
                }

                wc.Action = act;

                if (!act.DoAuthorize(wc))
                {
                    throw new WebException("Do authorize failure: " + act.Name) {Code = wc.Principal == null ? 401 : 403};
                }

                // try in the cache first
                if (!Service.TryGiveFromCache(wc))
                {
                    // invoke action method 
                    if (act.IsAsync) await act.DoAsync(wc, subscpt);
                    else act.Do(wc, subscpt);

                    Service.TryAddForCache(wc);
                }

                wc.Action = null;

                if (After != null)
                {
                    if (After.IsAsync && !await After.DoAsync(wc) || !After.IsAsync && After.Do(wc))
                    {
                        return;
                    }
                }
            }
            else // sub works
            {
                string key = rsc.Substring(0, slash);
                if (works != null && works.TryGetValue(key, out var wrk)) // if child
                {
                    // do necessary authentication before entering
                    if (wc.Principal == null && !await wrk.DoAuthenticate(wc)) return;

                    wc.Chain(wrk, key);
                    await wrk.HandleAsync(rsc.Substring(slash + 1), wc);
                }
                else if (varwork != null) // if variable-key subwork
                {
                    // do necessary authentication before entering
                    if (wc.Principal == null && !await varwork.DoAuthenticate(wc)) return;

                    var prin = wc.Principal;
                    object acc = null;
                    if (key.Length == 0) // resolve accessor
                    {
                        if (prin == null) throw AuthReq;
                        if ((acc = varwork.GetAccessor(prin)) == null)
                        {
                            throw AccessorReq;
                        }
                    }

                    wc.Chain(varwork, key, acc);
                    await varwork.HandleAsync(rsc.Substring(slash + 1), wc);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        //
        // logging methods
        //

        public static void TRC(string msg, Exception ex = null)
        {
            Framework.TRC(msg, ex);
        }

        public static void DBG(string msg, Exception ex = null)
        {
            Framework.DBG(msg, ex);
        }

        public static void INF(string msg, Exception ex = null)
        {
            Framework.INF(msg, ex);
        }

        public static void WAR(string msg, Exception ex = null)
        {
            Framework.WAR(msg, ex);
        }

        public static void ERR(string msg, Exception ex = null)
        {
            Framework.ERR(msg, ex);
        }

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            return Framework.NewDbContext(level);
        }

        public static T Obtain<T>(byte flag = 0) where T : class
        {
            return Framework.Obtain<T>(flag);
        }

        public static async Task<T> ObtainAsync<T>(byte flag = 0) where T : class
        {
            return await Framework.ObtainAsync<T>(flag);
        }


        //
        // object locator

        Cell[] cells;

        int size;

        public void Attach(object value, byte flag = 0)
        {
            if (cells == null)
            {
                cells = new Cell[16];
            }
            cells[size++] = new Cell(value, flag);
        }


        public T Lookup<T>(byte flag = 0) where T : class
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
        class Cell
        {
            readonly Type typ;

            readonly object value;

            readonly byte flag;

            internal Cell(object value, byte flag)
            {
                this.typ = value.GetType();
                this.value = value;
                this.flag = flag;
            }

            internal Cell(Type typ, object value, byte flag)
            {
                this.typ = typ;
                this.flag = flag;
                this.value = value;
            }

            public Type Typ => typ;

            public byte Flag => flag;

            public object Value => value;
        }
    }
}