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

        // to obtain a string key from a data object.
        protected Work(WorkConfig cfg) : base(cfg.Name, null, cfg.Ui, cfg.Access)
        {
            this.cfg = cfg;

            // gather procedures
            actioners = new Map<string, Actioner>(32);
            this.type = GetType();
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

        public bool HasKeyer => cfg.Keyer != null;

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
        /// Create a variable-key subwork.
        /// </summary>
        /// <param name="keyer"></param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="access">to override class-wise Authorize attribute</param>
        /// <typeparam name="W"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W CreateVar<W, K>(Func<object, K> keyer = null, UiAttribute ui = null, AccessAttribute access = null) where W : Work
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
                Access = access,
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = true,
                Directory = (Parent == null) ? VAR : Path.Combine(Parent.Directory, VAR),
                Keyer = keyer,
            };
            W w = (W) ci.Invoke(new object[] {config});
            varwork = w;
            return w;
        }

        /// <summary>
        /// Create a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="access">to override class-wise Authorize attribute</param>
        /// <typeparam name="W">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W Create<W>(string name, UiAttribute ui = null, AccessAttribute access = null) where W : Work
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

        public object GetVariableKey(object obj)
        {
            Delegate keyer = cfg.Keyer;
            if (keyer is Func<object, string> fstr)
            {
                return fstr(obj);
            }
            if (keyer is Func<object, int> fint)
            {
                return fint(obj);
            }
            if (keyer is Func<object, long> flong)
            {
                return flong(obj);
            }
            if (keyer is Func<object, short> fshort)
            {
                return fshort(obj);
            }
            if (keyer is Func<object, string[]> fstrs)
            {
                return fstrs(obj);
            }
            if (keyer is Func<object, int[]> fints)
            {
                return fints(obj);
            }
            if (keyer is Func<object, long[]> flongs)
            {
                return flongs(obj);
            }
            if (keyer is Func<object, short[]> fshorts)
            {
                return fshorts(obj);
            }
            return null;
        }

        public void PutVariableKey(object obj, DynamicContent cont)
        {
            Delegate keyer = cfg.Keyer;
            if (keyer is Func<object, string> fstr)
            {
                cont.Add(fstr(obj));
            }
            else if (keyer is Func<object, long> flong)
            {
                cont.Add(flong(obj));
            }
            else if (keyer is Func<object, int> fint)
            {
                cont.Add(fint(obj));
            }
            else if (keyer is Func<object, short> fshort)
            {
                cont.Add(fshort(obj));
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

        internal Work Resolve(ref string relative, WebContext wc)
        {
            if (!CheckAccess(wc, out AccessException except)) throw except;

            int slash = relative.IndexOf('/');
            if (slash == -1)
            {
                return this;
            }

            // seek subworks/varwork
            string key = relative.Substring(0, slash);
            relative = relative.Substring(slash + 1); // adjust relative
            if (works != null && works.TryGet(key, out var work)) // if child
            {
                wc.Chain(work, key);
                return work.Resolve(ref relative, wc);
            }

            if (varwork != null) // if variable-key sub
            {
                IData prin = wc.Principal;
                object prinkey = null;
                if (key.Length == 0) // resolve shortcut
                {
                    if (prin == null) throw AccessException.NoPrincipalEx;
                    if ((prinkey = varwork.GetVariableKey(prin)) == null)
                    {
                        throw AccessException.FalseResultEx;
                    }
                }

                wc.Chain(varwork, key, prinkey);
                return varwork.Resolve(ref relative, wc);
            }

            return null;
        }

        /// <summary>
        /// To hndle a request/response context. authorize, before/after filters
        /// </summary>
        /// <param name="rsc">the resource path</param>
        /// <param name="wc">WebContext</param>
        /// <exception cref="AccessException">Thrown when authorization is required and false is returned by checking</exception>
        /// <seealso cref="AccessAttribute.Check"/>
        internal async Task HandleAsync(string rsc, WebContext wc)
        {
            wc.Work = this;
            // any before filterings
            if (Before?.Do(wc) == false) goto WorkExit;
            if (BeforeAsync != null && !(await BeforeAsync.DoAsync(wc))) goto WorkExit;
            int dot = rsc.LastIndexOf('.');
            if (dot != -1) // file
            {
                if (!Service.TryGiveFromCache(wc)) // try in cache
                {
                    DoFile(rsc, rsc.Substring(dot), wc);
                    Service.TryCacheUp(wc);
                }
            }
            else // procedure
            {
                string name = rsc;
                int subscpt = 0;
                int dash = rsc.LastIndexOf('-');
                if (dash != -1)
                {
                    name = rsc.Substring(0, dash);
                    wc.Subscript = subscpt = rsc.Substring(dash + 1).ToInt();
                }

                Actioner actr = this[name];
                if (actr == null)
                {
                    wc.Give(404); // not found
                    return;
                }

                if (!actr.CheckAccess(wc, out AccessException except)) throw except;
                wc.Actioner = actr;
                // any before filterings
                if (actr.Before?.Do(wc) == false) goto ProcedureExit;
                if (actr.BeforeAsync != null && !(await actr.BeforeAsync.DoAsync(wc))) goto ProcedureExit;

                // try in cache
                if (!Service.TryGiveFromCache(wc))
                {
                    // method invocation
                    if (actr.IsAsync) await actr.DoAsync(wc, subscpt); // invoke procedure method
                    else actr.Do(wc, subscpt);

                    Service.TryCacheUp(wc);
                }

                ProcedureExit:
                // procedure's after filtering
                actr.After?.Do(wc);
                if (actr.AfterAsync != null) await actr.AfterAsync.DoAsync(wc);
                wc.Actioner = null;
            }

            WorkExit:
            After?.Do(wc);
            if (AfterAsync != null) await AfterAsync.DoAsync(wc);
            wc.Work = null;
        }

        public void DoFile(string filename, string ext, WebContext ac)
        {
            if (filename.StartsWith("$")) // private resource
            {
                ac.Give(403); // forbidden
                return;
            }

            if (!StaticContent.TryGetType(ext, out var ctyp))
            {
                ac.Give(415); // unsupported media type
                return;
            }

            string path = Path.Combine(cfg.Directory, filename);
            if (!File.Exists(path))
            {
                ac.Give(404); // not found
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

            StaticContent cont = new StaticContent(bytes, bytes.Length)
            {
                Key = filename,
                Type = ctyp,
                Modified = modified,
                GZip = gzip
            };
            ac.Give(200, cont, @public: true, maxage: 60 * 15);
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
        // OBJECT PROVIDER

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
                    var cell = holds[i];
                    if (cell.Flag == 0 || (cell.Flag & flag) > 0)
                    {
                        if (!cell.IsAsync && typeof(T).IsAssignableFrom(cell.Typ))
                        {
                            return cell.GetValue() as T;
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

        public void Flush<T>(byte flag = 0) where T : class
        {
        }

        public async Task FlushAsync<T>(byte flag = 0) where T : class
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