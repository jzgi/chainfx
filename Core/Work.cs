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
        readonly Map<string, ActionInfo> actions;

        // the default action, can be null
        readonly ActionInfo @default;

        // actions with UiToolAttribute
        readonly ActionInfo[] tooled;

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
            actions = new Map<string, ActionInfo>(32);
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
                ActionInfo ai;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ActionContext))
                {
                    ai = new ActionInfo(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(ActionContext) && pis[1].ParameterType == typeof(int))
                {
                    LimitAttribute limit = (LimitAttribute) pis[1].GetCustomAttribute(typeof(LimitAttribute));
                    ai = new ActionInfo(this, mi, async, true, limit?.Value ?? 20);
                }
                else continue;

                actions.Add(ai);
                if (ai.Key == string.Empty) @default = ai;

                if (ai.Tool?.MustPick == true) pick = true;
            }

            // gather styled actions
            Roll<ActionInfo> roll = new Roll<ActionInfo>(16);
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo ai = actions[i];
                if (ai.HasTool)
                {
                    roll.Add(ai);
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

        public Map<string, ActionInfo> Actions => actions;

        public ActionInfo[] Tooled => tooled;

        public bool HasPick => pick;

        public ActionInfo Default => @default;

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
            work.Attach(attachs);
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
            work.Attach(attachs);
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
                        ActionInfo act = actions[i];
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

        public ActionInfo GetAction(string method)
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
                // try in cache 
                if (!Service.TryGiveFromCache(ac))
                {
                    DoFile(rsc, rsc.Substring(dot), ac);
                    Service.Cache(ac); // try cache it
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
                ActionInfo ai = string.IsNullOrEmpty(name) ? @default : GetAction(name);
                if (ai == null)
                {
                    ac.Give(404); // not found
                    return;
                }

                if (!ai.DoAuthorize(ac)) throw AuthorizeEx;
                ac.Doer = ai;
                // any before filterings
                if (ai.Before?.Do(ac) == false) goto ActionExit;
                if (ai.BeforeAsync != null && !(await ai.BeforeAsync.DoAsync(ac))) goto ActionExit;
                // try in cache
                if (!Service.TryGiveFromCache(ac))
                {
                    // method invocation
                    if (ai.IsAsync)
                    {
                        await ai.DoAsync(ac, subscpt); // invoke action method
                    }
                    else
                    {
                        ai.Do(ac, subscpt);
                    }
                    Service.Cache(ac); // try cache it
                }
                ActionExit:
                // action's after filtering
                ai.After?.Do(ac);
                if (ai.AfterAsync != null) await ai.AfterAsync.DoAsync(ac);
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
        // OBJECT PROVIDER

        object[] attachs;

        int objc;

        public void Attach(object v)
        {
            if (attachs == null)
            {
                attachs = new object[8];
            }
            attachs[objc++] = v;
        }

        public void Attach(params object[] attachs)
        {
            if (this.attachs == null)
            {
                this.attachs = attachs;
            }
        }

        public T Obtain<T>() where T : class
        {
            if (attachs != null)
            {
                for (int i = 0; i < objc; i++)
                {
                    if (attachs[i] is T v) return v;
                }
            }
            return Parent?.Obtain<T>();
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
    }
}