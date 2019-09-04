using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Web
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
        readonly WebAction @catch;

        // actions with ToolAttribute
        readonly WebAction[] tooled;

        // subworks, if any
        internal Map<string, WebWork> works;

        // variable-key subwork, if any
        internal WebWork varwork;


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
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(string))
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

            if (tooled != null) // sorting
            {
                Array.Sort(tooled, (a, b) => a.Sort - b.Sort);
            }
        }

        public virtual string Key => Name;

        public string Label => Ui?.Label;

        public string Tip => Ui?.Tip;

        public byte Sort => Ui?.Sort ?? 0;

        readonly Type type;

        public WebService Service { get; internal set; }

        public WebWork Parent { get; internal set; }

        public int Level { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string Pathing { get; internal set; }

        // to resolve from the principal object.
        public Func<IData, object> Accessor { get; internal set; }

        public Map<string, WebAction> Actions => actions;

        public WebAction[] Tooled => tooled;


        public WebAction Default => @default;

        public WebAction Catch => @catch;

        public Map<string, WebWork> Works => works;

        public WebWork VarWork => varwork;

        public bool IsOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

        public WebAction this[string method] => string.IsNullOrEmpty(method) ? @default : actions[method];

        public WebAction this[int index] => actions.EntryAt(index).Value;

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        protected internal virtual void OnInitialize()
        {
        }

        public bool DoAuthorize(WebContext wc)
        {
            if (Authorize != null)
            {
                // check if trusted peer
                if (wc.CallerSign != null && wc.CallerSign == Framework.Signature)
                {
                    return true; // trusted without further check
                }

                return Authorize.Do(wc);
            }

            return true;
        }

        /// <summary>
        /// Create and add a variable-key subwork.
        /// </summary>
        /// <param name="accessor">to resolve key from the principal object</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
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
            return wrk;
        }

        /// <summary>
        /// Create and add a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
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
            return wrk;
        }

        public object GetAccessor(IData prin)
        {
            return Accessor?.Invoke(prin);
        }

        public static void PutVariableKey(object obj, DynamicContent cnt)
        {
            if (obj is IKeyable<string> kstr)
            {
                cnt.Add(kstr.Key);
            }
            else if (obj is IKeyable<short> kshort)
            {
                cnt.Add(kshort.Key);
            }
            else if (obj is IKeyable<int> kint)
            {
                cnt.Add(kint.Key);
            }
            else if (obj is IKeyable<long> klong)
            {
                cnt.Add(klong.Key);
            }
            else if (obj is IKeyable<(int, short)> kintshort)
            {
                var (k1, k2) = kintshort.Key;
                cnt.Add(k1);
                cnt.Add('-');
                cnt.Add(k2);
            }
            else
            {
                cnt.Add(obj.ToString());
            }
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
                    if (works != null)
                    {
                        for (int i = 0; i < works.Count; i++)
                        {
                            var wrk = works.EntryAt(i).Value;
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
                var a = actions.EntryAt(i).Value;
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
                var w = works.EntryAt(i).Value;
                w.Describe(hc);
            }
        }

        internal async Task HandleAsync(string rsc, WebContext wc)
        {
            wc.Work = this;
            try
            {
                if (Authenticate != null)
                {
                    if (Authenticate.IsAsync && !await Authenticate.DoAsync(wc) || !Authenticate.IsAsync && !Authenticate.Do(wc))
                    {
                        return;
                    }
                }

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
                    string subscpt = null;
                    int dash = rsc.LastIndexOf('-');
                    if (dash != -1)
                    {
                        name = rsc.Substring(0, dash);
                        wc.Subscript = subscpt = rsc.Substring(dash + 1);
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
                        wc.Chain(wrk, key);
                        await wrk.HandleAsync(rsc.Substring(slash + 1), wc);
                    }
                    else if (varwork != null) // if variable-key sub
                    {
                        IData prin = wc.Principal;
                        object acc = null;
                        if (key.Length == 0) // resolve prinlet
                        {
//                            if (prin == null) throw exception;
                            if ((acc = varwork.GetAccessor(prin)) == null)
                            {
//                                throw exception;
                            }
                        }

                        wc.Chain(varwork, key, acc);
                        await varwork.HandleAsync(rsc.Substring(slash + 1), wc);
                    }
                }
            }
            catch (Exception e)
            {
                if (@catch != null) // If existing catch defined by this work
                {
                    wc.Exception = e;
                    if (@catch.IsAsync) await @catch.DoAsync(wc, null);
                    else @catch.Do(wc, null);
                }
                else throw;
            }
            finally
            {
                wc.Work = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }


        //
        // object provider

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

        public void Attach<V>(Func<V> fetch, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new Cell[8];
            }

            cells[size++] = new Cell(typeof(V), fetch, maxage, flag);
        }

        public void Attach<V>(Func<Task<V>> fetchAsync, int maxage = 60, byte flag = 0) where V : class
        {
            if (cells == null)
            {
                cells = new Cell[8];
            }

            cells[size++] = new Cell(typeof(V), fetchAsync, maxage, flag);
        }

        /// <summary>
        /// Search for typed object in this scope and the scopes of ancestors; 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>the result object or null</returns>
        public T Obtain<T>(byte flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var h = cells[i];
                    if (h.Flag == 0 || (h.Flag & flag) > 0)
                    {
                        if (!h.IsAsync && typeof(T).IsAssignableFrom(h.Typ))
                        {
                            return h.GetValue() as T;
                        }
                    }
                }
            }

            return Parent?.Obtain<T>(flag);
        }

        public async Task<T> ObtainAsync<T>(byte flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var cell = cells[i];
                    if (cell.Flag == 0 || (cell.Flag & flag) > 0)
                    {
                        if (cell.IsAsync && typeof(T).IsAssignableFrom(cell.Typ))
                        {
                            return await cell.GetValueAsync() as T;
                        }
                    }
                }
            }

            if (Parent == null)
            {
                return null;
            }

            return await Parent.ObtainAsync<T>(flag);
        }


        /// <summary>
        /// A object holder in registry.
        /// </summary>
        class Cell
        {
            readonly Type typ;

            readonly Func<object> fetch;

            readonly Func<Task<object>> fetchAsync;

            readonly int maxage; //in seconds

            // tick count,   
            int expiry;

            object value;

            readonly byte flag;

            internal Cell(object value, byte flag)
            {
                this.typ = value.GetType();
                this.value = value;
                this.flag = flag;
            }

            internal Cell(Type typ, Func<object> fetch, int maxage, byte flag)
            {
                this.typ = typ;
                this.flag = flag;
                if (fetch is Func<Task<object>> fetch2)
                {
                    this.fetchAsync = fetch2;
                }
                else
                {
                    this.fetch = fetch;
                }

                this.maxage = maxage;
            }

            public Type Typ => typ;

            public byte Flag => flag;

            public bool IsAsync => fetchAsync != null;

            public object GetValue()
            {
                if (fetch == null) // simple object
                {
                    return value;
                }

                lock (fetch) // cache object
                {
                    if (Environment.TickCount >= expiry)
                    {
                        value = fetch();
                        expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
                    }

                    return value;
                }
            }

            public async Task<object> GetValueAsync()
            {
                if (fetchAsync == null) // simple object
                {
                    return value;
                }

                int lexpiry = this.expiry;
                int ticks = Environment.TickCount;
                if (ticks >= lexpiry)
                {
                    value = await fetchAsync();
                    expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
                }

                return value;
            }
        }
    }
}