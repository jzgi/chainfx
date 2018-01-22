using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
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
        protected Work(WorkConfig cfg) : base(cfg.Name, null)
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
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ActionContext))
                {
                    ad = new ActionDoer(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(ActionContext) && pis[1].ParameterType == typeof(int))
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
        /// Create a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="attachs"></param>
        /// <typeparam name="W">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W Create<W>(string name, params object[] attachs) where W : Work
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
                throw new ServiceException(typ + " constructor missing WorkConfig");
            }
            WorkConfig wc = new WorkConfig(name)
            {
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = false,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
            };
            // init sub work
            W work = (W) ci.Invoke(new object[] {wc});
            work.Register(attachs);
            works.Add(work.Key, work);

            return work;
        }

        /// <summary>
        /// Create a variable-key subwork.
        /// </summary>
        /// <param name="keyer"></param>
        /// <param name="attachs"></param>
        /// <typeparam name="W"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W CreateVar<W, K>(Func<IData, K> keyer = null, params object[] attachs) where W : Work
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
                throw new ServiceException(typ + " constructor missing WorkConfig");
            }
            WorkConfig wc = new WorkConfig(VAR)
            {
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = true,
                Directory = (Parent == null) ? VAR : Path.Combine(Parent.Directory, VAR),
                Keyer = keyer,
            };
            W work = (W) ci.Invoke(new object[] {wc});
            work.Register(attachs);
            varwork = work;
            return work;
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

        internal Work Resolve(ref string relative, ActionContext ac)
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
        internal async Task HandleAsync(string rsc, ActionContext ac)
        {
            ac.Work = this;
            // any before filterings
            if (Before?.Do(ac) == false) goto WorkExit;
            if (BeforeAsync != null && !(await BeforeAsync.DoAsync(ac))) goto WorkExit;
            int dot = rsc.LastIndexOf('.');
            if (dot != -1) // file
            {
                if (!TryGiveFromCache(ac)) // try in cache
                {
                    DoFile(rsc, rsc.Substring(dot), ac);
                    CacheUp(ac); // try cache it
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
                if (!TryGiveFromCache(ac))
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
                    CacheUp(ac); // try cache it
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

        public void DoFile(string filename, string ext, ActionContext ac)
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
                if (len > 4024)
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

        //
        // RESPONSE CACHE

        // cache entries
        readonly ConcurrentDictionary<string, Entry> entries = new ConcurrentDictionary<string, Entry>(Environment.ProcessorCount, 1024);

        internal void Clean(int now)
        {
            // a single loop to clean up expired items
            using (var enm = entries.GetEnumerator())
            {
                while (enm.MoveNext())
                {
                    Entry ety = enm.Current.Value;
                    ety.TryClean(now);
                }
            }
            // recursively clean descendants
            varwork?.Clean(now);
            for (int i = 0; i < works?.Count; i++)
            {
                works[i].Clean(now);
            }
        }

        internal void CacheUp(ActionContext ac)
        {
            if (!ac.InCache && ac.Public == true && Entry.IsCacheable(ac.Status))
            {
                Entry ety = new Entry(ac.Status, ac.Content, ac.MaxAge, Environment.TickCount);
                entries.AddOrUpdate(ac.Uri, ety, (k, old) => ety.MergeWith(old));
                ac.InCache = true;
            }
        }

        internal bool TryGiveFromCache(ActionContext ac)
        {
            if (entries.TryGetValue(ac.Uri, out var ca))
            {
                return ca.TryGive(ac, Environment.TickCount);
            }
            return false;
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

        int count;

        public void Register(object val)
        {
            if (holds == null)
            {
                holds = new Hold[8];
            }
            holds[count++] = new Hold(val);
        }

        public void Register<V>(Func<V> loader, int maxage = 3600) where V : class
        {
            if (holds == null)
            {
                holds = new Hold[8];
            }
            holds[count++] = new Hold(loader, maxage);
        }

        public void Register(params object[] vals)
        {
            foreach (var val in vals)
            {
                Register(val);
            }
        }

        public T Obtain<T>() where T : class
        {
            if (holds != null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (holds[i].GetValue() is T) // test on possibly-stale value
                    {
                        return holds[i].GetValue(true) as T; // reget a fresh value
                    }
                }
            }
            return Parent?.Obtain<T>();
        }

        public struct Hold
        {
            readonly Func<object> loader;

            readonly int maxage; //in seconds

            // tick count,   
            int expiry;

            object val;

            Exception excep;

            public Hold(object v)
            {
                loader = null;
                maxage = 0;
                expiry = 0;
                this.val = v;
                excep = null;
            }

            public Hold(Func<object> loader, int maxage = 3600 * 12)
            {
                this.loader = loader;
                this.maxage = maxage;
                expiry = 0;
                this.val = null;
                excep = null;
            }

            public int MaxAge => maxage;

            public Exception Excep => excep;

            public object GetValue(bool fresh = false)
            {
                if (loader == null) // simple object
                {
                    return val;
                }
                object v = val; // atomic get
                if (!fresh && v != null) // return possibly-stale value to reduce lock contention
                {
                    return v;
                }
                lock (loader) // cache object
                {
                    if (Environment.TickCount >= expiry)
                    {
                        try
                        {
                            val = loader();
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
                    return val;
                }
            }
        }

        /// <summary>
        /// An entry in the service response cache.
        /// </summary>
        public class Entry
        {
            // response status, 0 means cleared, otherwise one of the cacheable status
            int status;

            // can be set to null
            IContent content;

            // maxage in seconds
            int maxage;

            // time ticks when entered
            int stamp;

            int hits;

            internal Entry(int status, IContent content, int maxage, int stamp)
            {
                this.status = status;
                this.content = content;
                this.maxage = maxage;
                this.stamp = stamp;
            }

            /// <summary>
            ///  RFC 7231 cacheable status codes.
            /// </summary>
            public static bool IsCacheable(int code)
            {
                return code == 200 || code == 203 || code == 204 || code == 206 || code == 300 || code == 301 || code == 404 || code == 405 || code == 410 || code == 414 || code == 501;
            }

            internal void TryClean(int ticks)
            {
                lock (this)
                {
                    if (status == 0) return;

                    if (((stamp + maxage * 1000) - ticks) / 1000 <= 0)
                    {
                        status = 0;
                        content = null; // NOTE: the buffer won't return to the pool
                        maxage = 0;
                        stamp = 0;
                    }
                }
            }

            public int Hits => hits;

            internal bool TryGive(ActionContext ac, int ticks)
            {
                lock (this)
                {
                    if (status == 0) return false;

                    int remain = ((stamp + maxage * 1000) - ticks) / 1000;
                    if (remain > 0)
                    {
                        ac.InCache = true;
                        ac.Give(status, content, true, remain);

                        Interlocked.Increment(ref hits);

                        return true;
                    }

                    return false;
                }
            }

            internal Entry MergeWith(Entry old)
            {
                Interlocked.Add(ref hits, old.Hits);
                return this;
            }
        }
    }
}