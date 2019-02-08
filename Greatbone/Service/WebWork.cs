using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Service
{
    /// <summary>
    /// A work is a virtual web folder that contains a single or collection of resources along with operations on it or them.
    /// </summary>
    /// <remarks>A work can contain child/sub works.</remarks>
    public abstract class WebWork : WebTarget
    {
        // max nesting levels
        const int MaxNesting = 8;

        const string VarDir = "_var_", VarPathing = "<var>";

        protected readonly WebWorkInf info;

        readonly Type type;

        // declared action methods 
        readonly Map<string, WebAction> actions;

        // the default action method, can be null
        readonly WebAction @default;

        // the catch action method, can be null
        readonly WebAction @catch;

        // action method with the ToolAttribute
        readonly WebAction[] tooled;

        // subworks, if any
        internal Map<string, WebWork> works;

        // variable-key subwork, if any
        internal WebWork varwork;

        // if there is any procedure that must pick form value
        readonly bool pick;

        internal readonly AuthenticateAttribute authenticate;

        // pre-action operation
        internal readonly BeforeAttribute before;

        // post-action operation
        internal readonly AfterAttribute after;

        // to obtain a string key from a data object.
        protected WebWork(WebWorkInf wwi) : base(wwi.Name, null, wwi.Ui, wwi.Authorize)
        {
            this.info = wwi;

            this.type = GetType();

            this.authenticate = (AuthenticateAttribute) type.GetCustomAttribute(typeof(AuthenticateAttribute), false);
            this.before = (BeforeAttribute) type.GetCustomAttribute(typeof(BeforeAttribute), false);
            this.after = (AfterAttribute) type.GetCustomAttribute(typeof(AfterAttribute), false);

            // gather procedures
            actions = new Map<string, WebAction>(32);
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                Type ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task)) async = true;
                else if (ret == typeof(void)) async = false;
                else continue;

                ParameterInfo[] pis = mi.GetParameters();
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
                if (act.Tool?.MustPick == true) pick = true;
            }
            // gather tooled action methods
            var list = new ValueList<WebAction>(16);
            for (int i = 0; i < actions.Count; i++)
            {
                WebAction act = actions.At(i);
                if (act.HasTool)
                {
                    list.Add(act);
                }
            }
            tooled = list.ToArray();

            if (tooled != null) // sorting
            {
                Array.Sort(tooled, (a, b) => a.Sort - b.Sort);
            }
        }

        public WebWork Parent => info.Parent;

        public bool IsVar => info.IsVar;

        public int Level => info.Level;

        public string Directory => info.Directory;

        public string Pathing => info.Pathing;

        public bool HasAccessor => info.Accessor != null;

        public Map<string, WebAction> Actions => actions;

        public WebAction[] Tooled => tooled;

        public bool HasPick => pick;

        public WebAction Default => @default;

        public WebAction Catch => @catch;

        public Map<string, WebWork> Works => works;

        public WebWork VarWork => varwork;

        public bool IsOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

        public WebAction this[string method] => string.IsNullOrEmpty(method) ? @default : actions[method];

        public WebAction this[int index] => actions.At(index);

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        /// <summary>
        /// Create and add a variable-key subwork.
        /// </summary>
        /// <param name="accessor">to resolve key from the principal object</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="auth">to override class-wise Authorize attribute</param>
        /// <typeparam name="W"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="WebException">Thrown if error</exception>
        protected W MakeVarWork<W>(Func<IData, object> accessor = null, UiAttribute ui = null, AuthorizeAttribute auth = null) where W : WebWork
        {
            if (Level >= MaxNesting)
            {
                throw new WebException("allowed work nesting " + MaxNesting);
            }

            // create instance
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WebWorkInf)});
            if (ci == null)
            {
                throw new WebException(typ + " need public and WorkConfig");
            }

            WebWorkInf wwi = new WebWorkInf(VarDir)
            {
                Ui = ui,
                Authorize = auth,
                Parent = this,
                Level = Level + 1,
                IsVar = true,
                Directory = (Parent == null) ? VarDir : Path.Combine(Parent.Directory, VarDir),
                Pathing = Pathing + (accessor == null ? VarPathing : string.Empty) + "/",
                Accessor = accessor,
            };
            W w = (W) ci.Invoke(new object[] {wwi});
            varwork = w;
            return w;
        }

        /// <summary>
        /// Create and add a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <typeparam name="W">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="WebException">Thrown if error</exception>
        protected W MakeWork<W>(string name, UiAttribute ui = null, AuthorizeAttribute authorize = null) where W : WebWork
        {
            if (info.Level >= MaxNesting)
            {
                throw new WebException("allowed work nesting " + MaxNesting);
            }

            if (works == null)
            {
                works = new Map<string, WebWork>();
            }

            // create instance by reflection
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WebWorkInf)});
            if (ci == null)
            {
                throw new WebException(typ + " need public and WorkConfig");
            }

            WebWorkInf wwi = new WebWorkInf(name)
            {
                Ui = ui,
                Authorize = authorize,
                Parent = this,
                Level = Level + 1,
                IsVar = false,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
                Pathing = Pathing + name + "/",
            };
            // init sub work
            W w = (W) ci.Invoke(new object[] {wwi});
            works.Add(w.Key, w);
            return w;
        }

        public object GetAccessor(IData prin)
        {
            var keyer = info.Accessor;
            return keyer?.Invoke(prin);
        }

        public static void PutVariableKey(object obj, DynamicContent cont)
        {
            if (obj is IKeyable<string> kstr)
            {
                cont.Add(kstr.Key);
            }
            else if (obj is IKeyable<short> kshort)
            {
                cont.Add(kshort.Key);
            }
            else if (obj is IKeyable<int> kint)
            {
                cont.Add(kint.Key);
            }
            else if (obj is IKeyable<long> klong)
            {
                cont.Add(klong.Key);
            }
            else
            {
                cont.Add(obj.ToString());
            }
        }

        protected void Describe(XmlContent xc)
        {
            xc.ELEM(Key,
                delegate
                {
                    for (int i = 0; i < actions.Count; i++)
                    {
                        WebAction act = actions.At(i);
                        xc.Put(act.Key, "");
                    }
                },
                delegate
                {
                    if (works != null)
                    {
                        for (int i = 0; i < works.Count; i++)
                        {
                            WebWork wrk = works.At(i);
                            wrk.Describe(xc);
                        }
                    }
                    varwork?.Describe(xc);
                });
        }

        protected void Describe(HtmlContent hc)
        {
            for (int i = 0; i < actions?.Count; i++)
            {
                var a = actions.At(i);
                if (a.Comments != null)
                {
                    hc.T("<article style=\"border: 1px solid silver; padding: 8px;\">");
                    hc.T("<h3><code>").TT(a.Pathing).T("</code></h3>");
                    hc.HR();
                    for (int k = 0; k < a.Comments.Count; k++)
                    {
                        var c = a.Comments[k];
                        c.Print(hc);
                    }
                    hc.T("</article>");
                }
            }

            varwork?.Describe(hc);

            for (int i = 0; i < works?.Count; i++)
            {
                var w = works.At(i);
                w.Describe(hc);
            }
        }

        internal async Task HandleAsync(string rsc, WebContext wc)
        {
            wc.Work = this;
            try
            {
                if (authenticate != null)
                {
                    if (authenticate.IsAsync && !await authenticate.DoAsync(wc) || !authenticate.IsAsync && !authenticate.Do(wc))
                    {
                        return;
                    }
                }

                if (!DoAuthorize(wc)) throw exception;

                int slash = rsc.IndexOf('/');
                if (slash == -1) // this work is the target work
                {
                    if (before != null)
                    {
                        if (before.IsAsync && !await before.DoAsync(wc) || !before.IsAsync && before.Do(wc))
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
                    WebAction act = this[name];
                    if (act == null)
                    {
                        wc.Give(404, "action not found", true, 12);
                        return;
                    }

                    wc.Action = act;

                    if (!act.DoAuthorize(wc)) throw act.exception;

                    // try in the cache first
                    if (!Host.WebServer.TryGiveFromCache(wc))
                    {
                        // invoke action method 
                        if (act.IsAsync) await act.DoAsync(wc, subscpt);
                        else act.Do(wc, subscpt);

                        Host.WebServer.TryAddToCache(wc);
                    }
                    wc.Action = null;


                    if (after != null)
                    {
                        if (after.IsAsync && !await after.DoAsync(wc) || !after.IsAsync && after.Do(wc))
                        {
                            return;
                        }
                    }
                }
                else // sub works
                {
                    string key = rsc.Substring(0, slash);
                    if (works != null && works.TryGet(key, out var wrk)) // if child
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
                            if (prin == null) throw exception;
                            if ((acc = varwork.GetAccessor(prin)) == null)
                            {
                                throw exception;
                            }
                        }
                        wc.Chain(varwork, key, acc);
                        await varwork.HandleAsync(rsc.Substring(slash + 1), wc);
                    }
                }
            }
            catch (Exception e)
            {
                if (@catch != null) // an exception catch defined by this work
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

        //
        // object provider

        Hold[] holds;

        int size;

        public void Attach(object value, byte flag = 0)
        {
            if (holds == null)
            {
                holds = new Hold[16];
            }
            holds[size++] = new Hold(value, flag);
        }

        public void Attach<V>(Func<V> fetch, int maxAge = 60, byte flag = 0) where V : class
        {
            if (holds == null)
            {
                holds = new Hold[8];
            }
            holds[size++] = new Hold(typeof(V), fetch, maxAge, flag);
        }

        public void Attach<V>(Func<Task<V>> fetchAsync, int maxAge = 60, byte flag = 0) where V : class
        {
            if (holds == null)
            {
                holds = new Hold[8];
            }
            holds[size++] = new Hold(typeof(V), fetchAsync, maxAge, flag);
        }

        /// <summary>
        /// Search for typed object in this scope and the scopes of ancestors; 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>the result object or null</returns>
        public T Obtain<T>(byte flag = 0) where T : class
        {
            if (holds != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var h = holds[i];
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
            if (holds != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var cell = holds[i];
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
        class Hold
        {
            readonly Type typ;

            readonly Func<object> fetch;

            readonly Func<Task<object>> fetchAsync;

            readonly int maxage; //in seconds

            // tick count,   
            int expiry;

            object value;

            readonly byte flag;

            internal Hold(object value, byte flag)
            {
                this.typ = value.GetType();
                this.value = value;
                this.flag = flag;
            }

            internal Hold(Type typ, Func<object> fetch, int maxage, byte flag)
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