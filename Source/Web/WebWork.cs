using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SkyChain.Db;
using SkyChain.Source.Web;

namespace SkyChain.Web
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


        protected internal virtual void OnMake()
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
        protected T MakeVarWork<T>(Func<IData, string, object> accessor = null, object state = null,
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

            wrk.OnMake();
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
        protected T MakeWork<T>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null) where T : WebWork, new()
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
                State = state
            };
            if (ui != null) wrk.Ui = ui;
            if (authenticate != null) wrk.Authenticate = authenticate;
            if (authorize != null) wrk.Authorize = authorize;
            if (before != null) wrk.Before = before;
            if (after != null) wrk.After = after;

            works.Add(wrk);

            wrk.OnMake();
            return wrk;
        }

        protected void MakeWork<T1, T2>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null)
            where T1 : WebWork, new() where T2 : WebWork, new()
        {
            MakeWork<T1>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T2>(name, state, ui, authenticate, authorize, before, after);
        }

        protected void MakeWork<T1, T2, T3>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null)
            where T1 : WebWork, new() where T2 : WebWork, new() where T3 : WebWork, new()
        {
            MakeWork<T1>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T2>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T3>(name, state, ui, authenticate, authorize, before, after);
        }

        protected void MakeWork<T1, T2, T3, T4>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null)
            where T1 : WebWork, new() where T2 : WebWork, new() where T3 : WebWork, new() where T4 : WebWork, new()
        {
            MakeWork<T1>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T2>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T3>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T4>(name, state, ui, authenticate, authorize, before, after);
        }

        protected void MakeWork<T1, T2, T3, T4, T5>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null)
            where T1 : WebWork, new() where T2 : WebWork, new() where T3 : WebWork, new() where T4 : WebWork, new() where T5 : WebWork, new()
        {
            MakeWork<T1>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T2>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T3>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T4>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T5>(name, state, ui, authenticate, authorize, before, after);
        }

        protected void MakeWork<T1, T2, T3, T4, T5, T6>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null)
            where T1 : WebWork, new() where T2 : WebWork, new() where T3 : WebWork, new() where T4 : WebWork, new() where T5 : WebWork, new() where T6 : WebWork, new()
        {
            MakeWork<T1>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T2>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T3>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T4>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T5>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T6>(name, state, ui, authenticate, authorize, before, after);
        }

        protected void MakeWork<T1, T2, T3, T4, T5, T6, T7>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null)
            where T1 : WebWork, new() where T2 : WebWork, new() where T3 : WebWork, new() where T4 : WebWork, new() where T5 : WebWork, new() where T6 : WebWork, new() where T7 : WebWork, new()
        {
            MakeWork<T1>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T2>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T3>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T4>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T5>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T6>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T7>(name, state, ui, authenticate, authorize, before, after);
        }

        protected void MakeWork<T1, T2, T3, T4, T5, T6, T7, T8>(string name, object state = null,
            UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, BeforeAttribute before = null, AfterAttribute after = null)
            where T1 : WebWork, new() where T2 : WebWork, new() where T3 : WebWork, new() where T4 : WebWork, new() where T5 : WebWork, new() where T6 : WebWork, new() where T7 : WebWork, new() where T8 : WebWork, new()
        {
            MakeWork<T1>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T2>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T3>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T4>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T5>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T6>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T7>(name, state, ui, authenticate, authorize, before, after);
            MakeWork<T8>(name, state, ui, authenticate, authorize, before, after);
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

            for (int i = 0; i < works?.Count; i++)
            {
                var w = works.EntryAt(i).Value;
                w.Describe(h);
            }
        }

        readonly WebException AuthReq = new WebException("Authentication required") {Code = 401};

        readonly WebException AccessorReq = new WebException("Accessor required") {Code = 403};

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
                return Authorize.Do(wc);
            }

            return true;
        }


        internal WebWork GetSubWork(WebContext wc, string key)
        {
            if (works == null)
            {
                return null;
            }
            var ety = works.EntryOf(key);
            var siz = ety.Size;
            if (siz < 1)
            {
                return null;
            }
            if (siz == 1)
            {
                return ety.Value;
            }
            if (wc[0].Accessor is IForkable forkable)
            {
                short frk = forkable.Fork;
                for (var i = 0; i < siz; i++)
                {
                    var wrk = ety[i];
                    if (frk == wrk.Ui?.Fork)
                    {
                        return wrk;
                    }
                }
            }
            return null;
        }

        internal WebWork ResolveWork(WebContext wc, int idx)
        {
            if (works == null)
            {
                return null;
            }
            var ety = works.EntryAt(idx);
            var siz = ety.Size;
            if (siz < 1)
            {
                return null;
            }
            if (siz == 1)
            {
                return ety.Value;
            }
            if (wc[0].Accessor is IForkable forkable)
            {
                short frk = forkable.Fork;
                if (frk != 0) // zero forie doesn't match anything
                {
                    for (var i = 0; i < siz; i++)
                    {
                        var wrk = ety[i];
                        if (frk == wrk.Ui?.Fork)
                        {
                            return wrk;
                        }
                    }
                }
            }
            return null;
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
                    wc.Give(404, "action not found", true, 30);
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
                var wrk = GetSubWork(wc, key);
                if (wrk != null) // if child
                {
                    // do necessary authentication before entering
                    if (wc.Principal == null && !await wrk.DoAuthenticate(wc)) return;

                    wc.AppendSeg(wrk, key);
                    await wrk.HandleAsync(rsc.Substring(slash + 1), wc);
                }
                else if (varwork != null) // if variable-key subwork
                {
                    // do necessary authentication before entering
                    if (wc.Principal == null && !await varwork.DoAuthenticate(wc)) return;

                    var prin = wc.Principal;
                    object accessor;
                    if (key.Length == 0)
                    {
                        if (prin == null) throw AuthReq;
                        accessor = varwork.GetAccessor(prin, null);
                        if (accessor == null)
                        {
                            throw AccessorReq;
                        }
                    }
                    else
                    {
                        accessor = varwork.GetAccessor(prin, key);
                    }
                    // append the segment
                    wc.AppendSeg(varwork, key, accessor);
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
            Application.TRC(msg, ex);
        }

        public static void DBG(string msg, Exception ex = null)
        {
            Application.DBG(msg, ex);
        }

        public static void INF(string msg, Exception ex = null)
        {
            Application.INF(msg, ex);
        }

        public static void WAR(string msg, Exception ex = null)
        {
            Application.WAR(msg, ex);
        }

        public static void ERR(string msg, Exception ex = null)
        {
            Application.ERR(msg, ex);
        }

        public static DbContext NewDbContext(IsolationLevel? level = null)
        {
            return Db.Chain.NewDbContext(level);
        }

        public static Map<K, V> ObtainMap<K, V>(byte flag = 0) where K : IComparable<K>
        {
            return Db.Chain.Grab<K, V>(flag);
        }

        public static async Task<Map<K, V>> ObtainMapAsync<K, V>(byte flag = 0) where K : IComparable<K>
        {
            return await Db.Chain.GrabAsync<K, V>(flag);
        }

        public static V Obtain<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            return Db.Chain.GrabObject<K, V>(key, flag);
        }

        public static async Task<V> ObtainAsync<K, V>(K key, byte flag = 0) where K : IComparable<K>
        {
            return await Db.Chain.GrabObjectAsync<K, V>(key, flag);
        }

        public static Map<K, V> ObtainSub<S, K, V>(S discr, byte flag = 0) where K : IComparable<K>
        {
            return Db.Chain.GrabMap<S, K, V>(discr, flag);
        }

        public static async Task<Map<K, V>> ObtainSubAsync<D, K, V>(D discr, byte flag = 0) where K : IComparable<K>
        {
            return await Db.Chain.GrabMapAsync<D, K, V>(discr, flag);
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

            public Type Typ => typ;

            public byte Flag => flag;

            public object Value => value;
        }
    }
}