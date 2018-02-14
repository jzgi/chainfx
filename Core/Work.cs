using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    /// <summary>
    /// A work is a virtual web folder that contains a single or collection of resources along with operations on it or them.
    /// </summary>
    /// <remarks>A work can contain child/sub works.</remarks>
    public abstract class Work : Nodule
    {
        internal static readonly AuthorizeException AuthorizeEx = new AuthorizeException();

        // max nesting levels
        const int MaxNesting = 8;

        const string VAR = "var";

        protected readonly WorkConfig cfg;

        readonly Type type;

        // declared actions 
        readonly Map<string, ActionDoer> actions;

        // the default action, can be null
        readonly ActionDoer @default;

        // actions with UiToolAttribute
        readonly ActionDoer[] tooled;

        // subworks, if any
        internal Map<string, Work> works;

        // variable-key subwork, if any
        internal Work varwork;

        // if there is any action that must pick form value
        readonly bool pick;

        // to obtain a string key from a data object.
        protected Work(WorkConfig cfg) : base(cfg.Name, null, cfg.Ui, cfg.Authorize)
        {
            this.cfg = cfg;

            // gather actions
            actions = new Map<string, ActionDoer>(32);
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
                ActionDoer ad;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                {
                    ad = new ActionDoer(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(int))
                {
                    LimitAttribute limit = (LimitAttribute) pis[1].GetCustomAttribute(typeof(LimitAttribute));
                    ad = new ActionDoer(this, mi, async, true, limit?.Value ?? 20);
                }
                else continue;

                actions.Add(ad);
                if (ad.Key == string.Empty) @default = ad;

                if (ad.Tool?.MustPick == true) pick = true;
            }

            // gather styled actions
            Roll<ActionDoer> roll = new Roll<ActionDoer>(16);
            for (int i = 0; i < actions.Count; i++)
            {
                ActionDoer ad = actions[i];
                if (ad.HasTool)
                {
                    roll.Add(ad);
                }
            }
            tooled = roll.ToArray();
        }

        public Service Service => cfg.Service;

        public Work Parent => cfg.Parent;

        public bool IsVar => cfg.IsVar;

        public int Level => cfg.Level;

        public string Directory => cfg.Directory;

        public bool HasKeyer => cfg.Keyer != null;

        public Map<string, ActionDoer> Actions => actions;

        public ActionDoer[] Tooled => tooled;

        public bool HasPick => pick;

        public ActionDoer Default => @default;

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
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <typeparam name="W"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W CreateVar<W, K>(Func<IData, K> keyer = null, UiAttribute ui = null, AuthorizeAttribute authorize = null) where W : Work
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
            WorkConfig wc = new WorkConfig(VAR)
            {
                Ui = ui,
                Authorize = authorize,
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = true,
                Directory = (Parent == null) ? VAR : Path.Combine(Parent.Directory, VAR),
                Keyer = keyer,
            };
            W w = (W) ci.Invoke(new object[] {wc});
            varwork = w;
            return w;
        }

        /// <summary>
        /// Create a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <typeparam name="W">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W Create<W>(string name, UiAttribute ui = null, AuthorizeAttribute authorize = null) where W : Work
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
            WorkConfig wc = new WorkConfig(name)
            {
                Ui = ui,
                Authorize = authorize,
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = false,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
            };
            // init sub work
            W w = (W) ci.Invoke(new object[] {wc});
            works.Add(w.Key, w);

            return w;
        }

        public object GetVariableKey(IData obj)
        {
            Delegate keyer = cfg.Keyer;
            if (keyer is Func<IData, string> fstr)
            {
                return fstr(obj);
            }
            if (keyer is Func<IData, int> fint)
            {
                return fint(obj);
            }
            if (keyer is Func<IData, long> flong)
            {
                return flong(obj);
            }
            if (keyer is Func<IData, short> fshort)
            {
                return fshort(obj);
            }
            if (keyer is Func<IData, string[]> fstrs)
            {
                return fstrs(obj);
            }
            if (keyer is Func<IData, int[]> fints)
            {
                return fints(obj);
            }
            if (keyer is Func<IData, long[]> flongs)
            {
                return flongs(obj);
            }
            if (keyer is Func<IData, short[]> fshorts)
            {
                return fshorts(obj);
            }
            return null;
        }

        public void PutVariableKey(IData obj, DynamicContent cont)
        {
            Delegate keyer = cfg.Keyer;
            if (keyer is Func<IData, string> fstr)
            {
                cont.Add(fstr(obj));
            }
            else if (keyer is Func<IData, long> flong)
            {
                cont.Add(flong(obj));
            }
            else if (keyer is Func<IData, int> fint)
            {
                cont.Add(fint(obj));
            }
            else if (keyer is Func<IData, short> fshort)
            {
                cont.Add(fshort(obj));
            }
        }

        internal void Describe(XmlContent cont)
        {
            cont.ELEM(Key,
                delegate
                {
                    for (int i = 0; i < actions.Count; i++)
                    {
                        ActionDoer act = actions[i];
                        cont.Put(act.Key, "");
                    }
                },
                delegate
                {
                    if (works != null)
                    {
                        for (int i = 0; i < works.Count; i++)
                        {
                            Work wrk = works[i];
                            wrk.Describe(cont);
                        }
                    }
                    varwork?.Describe(cont);
                });
        }

        public bool IsOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

        public ActionDoer GetAction(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return @default;
            }
            return actions[method];
        }

        internal Work Resolve(ref string relative, WebContext ac)
        {
            if (!DoAuthorize(ac)) throw AuthorizeEx;

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
                ac.Chain(work, key);
                return work.Resolve(ref relative, ac);
            }
            if (varwork != null) // if variable-key sub
            {
                IData prin = ac.Principal;
                object princi = null;
                if (key.Length == 0) // resolve shortcut
                {
                    if (prin == null) throw AuthorizeEx;
                    if ((princi = varwork.GetVariableKey(prin)) == null)
                    {
                        throw AuthorizeEx;
                    }
                }
                ac.Chain(varwork, key, princi);
                return varwork.Resolve(ref relative, ac);
            }
            return null;
        }

        /// <summary>
        /// To hndle a request/response context. authorize, before/after filters
        /// </summary>
        /// <param name="rsc">the resource path</param>
        /// <param name="ac">ActionContext</param>
        /// <exception cref="AuthorizeException">Thrown when authorization is required and false is returned by checking</exception>
        /// <seealso cref="AuthorizeAttribute.Check"/>
        internal async Task HandleAsync(string rsc, WebContext ac)
        {
            ac.Work = this;
            // any before filterings
            if (Before?.Do(ac) == false) goto WorkExit;
            if (BeforeAsync != null && !(await BeforeAsync.DoAsync(ac))) goto WorkExit;
            int dot = rsc.LastIndexOf('.');
            if (dot != -1) // file
            {
                if (!Service.TryGiveFromCache(ac)) // try in cache
                {
                    DoFile(rsc, rsc.Substring(dot), ac);
                    Service.TryCacheUp(ac);
                }
            }
            else // action
            {
                string name = rsc;
                int subscpt = 0;
                int dash = rsc.LastIndexOf('-');
                if (dash != -1)
                {
                    name = rsc.Substring(0, dash);
                    ac.Subscript = subscpt = rsc.Substring(dash + 1).ToInt();
                }
                ActionDoer ad = string.IsNullOrEmpty(name) ? @default : GetAction(name);
                if (ad == null)
                {
                    ac.Give(404); // not found
                    return;
                }

                if (!ad.DoAuthorize(ac)) throw AuthorizeEx;
                ac.Doer = ad;
                // any before filterings
                if (ad.Before?.Do(ac) == false) goto ActionExit;
                if (ad.BeforeAsync != null && !(await ad.BeforeAsync.DoAsync(ac))) goto ActionExit;

                // try in cache
                if (!Service.TryGiveFromCache(ac))
                {
                    // method invocation
                    if (ad.IsAsync)
                    {
                        await ad.DoAsync(ac, subscpt); // invoke action method
                    }
                    else
                    {
                        ad.Do(ac, subscpt);
                    }
                    Service.TryCacheUp(ac);
                }
                ActionExit:
                // action's after filtering
                ad.After?.Do(ac);
                if (ad.AfterAsync != null) await ad.AfterAsync.DoAsync(ac);
                ac.Doer = null;
            }
            WorkExit:
            After?.Do(ac);
            if (AfterAsync != null) await AfterAsync.DoAsync(ac);
            ac.Work = null;
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

        Cell[] registry;

        int size;

        public void Register(object value)
        {
            if (registry == null)
            {
                registry = new Cell[16];
            }
            registry[size++] = new Cell(value);
        }

        public void Register(params object[] values)
        {
            foreach (var val in values)
            {
                Register(val);
            }
        }

        public void Register<V>(Func<V> loader, int maxage = 3600) where V : class
        {
            if (registry == null)
            {
                registry = new Cell[8];
            }
            registry[size++] = new Cell(typeof(V), loader, maxage);
        }

        public void Register<V>(Func<Task<V>> loaderAsync, int maxage = 3600) where V : class
        {
            if (registry == null)
            {
                registry = new Cell[8];
            }
            registry[size++] = new Cell(typeof(V), loaderAsync, maxage);
        }

        /// <summary>
        /// Search for typed object in this scope and the scopes of ancestors; 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>the result object or null</returns>
        public T Obtain<T>() where T : class
        {
            if (registry != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var cell = registry[i];
                    if (!cell.IsAsync && typeof(T).IsAssignableFrom(cell.Typ))
                    {
                        return cell.GetValue() as T;
                    }
                }
            }
            return Parent?.Obtain<T>();
        }

        public async Task<T> ObtainAsync<T>() where T : class
        {
            if (registry != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var cell = registry[i];
                    if (cell.IsAsync && typeof(T).IsAssignableFrom(cell.Typ))
                    {
                        return await cell.GetValueAsync() as T;
                    }
                }
            }
            if (Parent == null) return null;
            return await Parent.ObtainAsync<T>();
        }

        /// <summary>
        /// A registry cell.
        /// </summary>
        class Cell
        {
            readonly Type typ;

            readonly Func<object> loader;

            readonly Func<Task<object>> loaderAsync;

            readonly int maxage; //in seconds

            // tick count,   
            int expiry;

            object value;

            Exception excep;

            internal Cell(object value)
            {
                this.typ = value.GetType();
                this.value = value;
            }

            internal Cell(Type typ, Func<object> loader, int maxage = 3600 * 12)
            {
                this.typ = typ;
                if (loader is Func<Task<object>> loader2)
                {
                    this.loaderAsync = loader2;
                }
                else
                {
                    this.loader = loader;
                }
                this.maxage = maxage;
            }

            public Type Typ => typ;

            public bool IsAsync => loaderAsync != null;

            public int MaxAge => maxage;

            public Exception Excep => excep;

            public object GetValue()
            {
                if (loader == null) // simple object
                {
                    return value;
                }
                lock (loader) // cache object
                {
                    if (Environment.TickCount >= expiry)
                    {
                        try
                        {
                            value = loader();
                        }
                        catch (Exception ex)
                        {
                            excep = ex;
                        }
                        finally
                        {
                            expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
                        }
                    }
                    return value;
                }
            }

            public async Task<object> GetValueAsync()
            {
                if (loaderAsync == null) // simple object
                {
                    return value;
                }
                int lexpiry = this.expiry;
                int ticks = Environment.TickCount;
                if (ticks >= lexpiry)
                {
                    try
                    {
                        value = await loaderAsync();
                    }
                    catch (Exception ex)
                    {
                        excep = ex;
                    }
                    finally
                    {
                        expiry = (Environment.TickCount & int.MaxValue) + maxage * 1000;
                    }
                }
                return value;
            }
        }
    }
}