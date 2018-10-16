using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Greatbone
{
    /// <summary>
    /// A work is a virtual web folder that contains a single or collection of resources along with operations on it or them.
    /// </summary>
    /// <remarks>A work can contain child/sub works.</remarks>
    public abstract class Work : Nodule
    {
        // max nesting levels
        const int MaxNesting = 8;

        const string VAR = "var";

        protected readonly WorkConfig cfg;

        readonly Type type;

        // declared action methods 
        readonly Map<string, Actioner> actioners;

        // the default action method, can be null
        readonly Actioner @default;

        // the catch procedure, can be null
        readonly Actioner @catch;

        // action method with the ToolAttribute
        readonly Actioner[] tooled;

        // subworks, if any
        internal Map<string, Work> works;

        // variable-key subwork, if any
        internal Work varwork;

        // if there is any procedure that must pick form value
        readonly bool pick;

        internal readonly AuthenticateAttribute authenticate;

        // pre-action operation
        internal readonly BeforeAttribute before;

        // post-action operation
        internal readonly AfterAttribute after;

        // to obtain a string key from a data object.
        protected Work(WorkConfig cfg) : base(cfg.Name, null, cfg.Ui, cfg.Access)
        {
            this.cfg = cfg;

            this.type = GetType();

            this.authenticate = (AuthenticateAttribute) type.GetCustomAttribute(typeof(AuthenticateAttribute), false);
            this.before = (BeforeAttribute) type.GetCustomAttribute(typeof(BeforeAttribute), false);
            this.after = (AfterAttribute) type.GetCustomAttribute(typeof(AfterAttribute), false);

            // gather procedures
            actioners = new Map<string, Actioner>(32);
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                Type ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task)) async = true;
                else if (ret == typeof(void)) async = false;
                else continue;

                ParameterInfo[] pis = mi.GetParameters();
                Actioner actr;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                {
                    actr = new Actioner(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(int))
                {
                    actr = new Actioner(this, mi, async, true);
                }
                else continue;

                actioners.Add(actr);
                if (actr.Key == string.Empty) @default = actr;
                if (actr.Key == "catch") @catch = actr;
                if (actr.Tool?.MustPick == true) pick = true;
            }
            // gather tooled action methods
            Roll<Actioner> roll = new Roll<Actioner>(16);
            for (int i = 0; i < actioners.Count; i++)
            {
                Actioner actr = actioners[i];
                if (actr.HasTool)
                {
                    roll.Add(actr);
                }
            }
            tooled = roll.ToArray();

            if (tooled != null) // sort by group
            {
                Array.Sort(tooled, (a, b) => a.Group - b.Group);
            }
        }

        public Service Service => cfg.Service;

        public Work Parent => cfg.Parent;

        public bool IsVar => cfg.IsVar;

        public int Level => cfg.Level;

        public string Directory => cfg.Directory;

        public bool HasPrinci => cfg.Princi != null;

        public Map<string, Actioner> Actioners => actioners;

        public Actioner[] Tooled => tooled;

        public bool HasPick => pick;

        public Actioner Default => @default;

        public Actioner Catch => @catch;

        public Map<string, Work> Works => works;

        public Work VarWork => varwork;

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        /// <summary>
        /// Create and add a variable-key subwork.
        /// </summary>
        /// <param name="princi">to resolve key from the principal object</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="auth">to override class-wise Authorize attribute</param>
        /// <typeparam name="W"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W MakeVar<W>(Func<IData, object> princi = null, UiAttribute ui = null, AuthorizeAttribute auth = null) where W : Work
        {
            if (cfg.Level >= MaxNesting)
            {
                throw new ServiceException("allowed work nesting " + MaxNesting);
            }

            // create instance
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WorkConfig)});
            if (ci == null)
            {
                throw new ServiceException(typ + " need public and WorkConfig");
            }

            WorkConfig config = new WorkConfig(VAR)
            {
                Ui = ui,
                Access = auth,
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = true,
                Directory = (Parent == null) ? VAR : Path.Combine(Parent.Directory, VAR),
                Princi = princi,
            };
            W w = (W) ci.Invoke(new object[] {config});
            varwork = w;
            return w;
        }

        /// <summary>
        /// Create and add a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="access">to override class-wise Authorize attribute</param>
        /// <typeparam name="W">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W Make<W>(string name, UiAttribute ui = null, AuthorizeAttribute access = null) where W : Work
        {
            if (cfg.Level >= MaxNesting)
            {
                throw new ServiceException("allowed work nesting " + MaxNesting);
            }

            if (works == null)
            {
                works = new Map<string, Work>();
            }

            // create instance by reflection
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WorkConfig)});
            if (ci == null)
            {
                throw new ServiceException(typ + " need public and WorkConfig");
            }

            WorkConfig config = new WorkConfig(name)
            {
                Ui = ui,
                Access = access,
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = false,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
            };
            // init sub work
            W w = (W) ci.Invoke(new object[] {config});
            works.Add(w.Key, w);
            return w;
        }

        public object GetPrinci(IData prin)
        {
            var keyer = cfg.Princi;
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

        internal void Describe(XmlContent cnt)
        {
            cnt.ELEM(Key,
                delegate
                {
                    for (int i = 0; i < actioners.Count; i++)
                    {
                        Actioner actr = actioners[i];
                        cnt.Put(actr.Key, "");
                    }
                },
                delegate
                {
                    if (works != null)
                    {
                        for (int i = 0; i < works.Count; i++)
                        {
                            Work wrk = works[i];
                            wrk.Describe(cnt);
                        }
                    }
                    varwork?.Describe(cnt);
                });
        }

        public bool IsOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

        public Actioner this[string method] => string.IsNullOrEmpty(method) ? @default : actioners[method];

        internal async Task HandleAsync(string rsc, WebContext wc)
        {
            wc.Work = this;
            try
            {
                if (authenticate != null)
                {
                    if (authenticate.IsAsync && !await authenticate.DoAsync(wc) || !authenticate.IsAsync && authenticate.Do(wc))
                    {
                        wc.Give(401, "authentication failed"); // unauthenticated
                        return;
                    }
                }

                if (!DoAuthorize(wc)) throw except;

                int slash = rsc.IndexOf('/');
                if (slash == -1) // this work is the target work
                {
                    if (before != null)
                    {
                        if (before.IsAsync && !await before.DoAsync(wc) || !before.IsAsync && before.Do(wc))
                        {
                            goto WorkExit;
                        }
                    }
                    //
                    // resolve the resource

                    int dot = rsc.LastIndexOf('.');
                    if (dot != -1) // the resource is a static file
                    {
                        if (!Service.TryGiveFromCache(wc)) // try in cache
                        {
                            DoFile(rsc, rsc.Substring(dot), wc);
                            Service.TryCacheUp(wc);
                        }
                    }
                    else // the resource is an action
                    {
                        string name = rsc;
                        int subscpt = 0;
                        int dash = rsc.LastIndexOf('-');
                        if (dash != -1)
                        {
                            name = rsc.Substring(0, dash);
                            wc.Subscript = subscpt = rsc.Substring(dash + 1).ToInt();
                        }
                        Actioner act = this[name];
                        if (act == null)
                        {
                            wc.Give(404, "action not found", true, 12);
                            return;
                        }

                        wc.Actioner = act;

                        if (!act.DoAuthorize(wc)) throw act.except;

                        // try in the cache first
                        if (!Service.TryGiveFromCache(wc))
                        {
                            // invoke action method 
                            if (act.IsAsync) await act.DoAsync(wc, subscpt);
                            else act.Do(wc, subscpt);
                            ActionExit:
                            Service.TryCacheUp(wc);
                        }
                        wc.Actioner = null;
                    }

                    WorkExit:
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
                        object princi = null;
                        if (key.Length == 0) // resolve shortcut
                        {
                            if (prin == null) throw except;
                            if ((princi = varwork.GetPrinci(prin)) == null)
                            {
                                throw except;
                            }
                        }
                        wc.Chain(varwork, key, princi);
                        await varwork.HandleAsync(rsc.Substring(slash + 1), wc);
                    }
                }
            }
            catch (Exception e)
            {
                if (@catch != null) // an exception catch defined by this work
                {
                    wc.Except = e;
                    if (@catch.IsAsync) await @catch.DoAsync(wc, 0);
                    else @catch.Do(wc, 0);
                }
                else throw;
            }
            finally
            {
                wc.Work = null;
            }
        }

        public void DoFile(string filename, string ext, WebContext wc)
        {
            if (filename.StartsWith("$")) // private resource
            {
                wc.Give(403); // forbidden
                return;
            }

            if (!StaticContent.TryGetType(ext, out var ctyp))
            {
                wc.Give(415); // unsupported media type
                return;
            }

            string path = Path.Combine(cfg.Directory, filename);
            if (!File.Exists(path))
            {
                wc.Give(404); // not found
                return;
            }

            DateTime modified = File.GetLastWriteTime(path);
            // load file content
            byte[] bytes;
            bool gzip = false;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int len = (int) fs.Length;
                if (len > 2048)
                {
                    var ms = new MemoryStream(len);
                    using (GZipStream gzs = new GZipStream(ms, CompressionMode.Compress))
                    {
                        fs.CopyTo(gzs);
                    }

                    bytes = ms.ToArray();
                    gzip = true;
                }
                else
                {
                    bytes = new byte[len];
                    fs.Read(bytes, 0, len);
                }
            }

            StaticContent cnt = new StaticContent(bytes, bytes.Length)
            {
                Key = filename,
                Type = ctyp,
                Modified = modified,
                GZip = gzip
            };
            wc.Give(200, cnt, @public: true, maxage: 60 * 15);
        }

        // LOGGING

        public void TRC(string msg, Exception ex = null)
        {
            Service.Log(LogLevel.Trace, 0, msg, ex, null);
        }

        public void DBG(string msg, Exception ex = null)
        {
            Service.Log(LogLevel.Debug, 0, msg, ex, null);
        }

        public void INF(string msg, Exception ex = null)
        {
            Service.Log(LogLevel.Information, 0, msg, ex, null);
        }

        public void WAR(string msg, Exception ex = null)
        {
            Service.Log(LogLevel.Warning, 0, msg, ex, null);
        }

        public void ERR(string msg, Exception ex = null)
        {
            Service.Log(LogLevel.Error, 0, msg, ex, null);
        }

        //
        // object provider

        Hold[] holds;

        int size;

        public void Register(object value, byte flag = 0)
        {
            if (holds == null)
            {
                holds = new Hold[16];
            }
            holds[size++] = new Hold(value, flag);
        }

        public void Register<V>(Func<V> fetch, int maxage = 60, byte flag = 0) where V : class
        {
            if (holds == null)
            {
                holds = new Hold[8];
            }
            holds[size++] = new Hold(typeof(V), fetch, maxage, flag);
        }

        public void Register<V>(Func<Task<V>> fetchAsync, int maxage = 60, byte flag = 0) where V : class
        {
            if (holds == null)
            {
                holds = new Hold[8];
            }
            holds[size++] = new Hold(typeof(V), fetchAsync, maxage, flag);
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

        public void Invalidate<T>(byte flag = 0) where T : class
        {
        }

        public DbContext NewDbContext(IsolationLevel? level = null)
        {
            DbContext dc = new DbContext(Service);
            if (level != null)
            {
                dc.Begin(level.Value);
            }
            return dc;
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