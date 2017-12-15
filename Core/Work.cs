using System;
using System.Collections.Generic;
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

        readonly ServiceContext serviceCtx;

        readonly WorkContext ctx;

        readonly Work parent;

        readonly Type type;

        readonly TypeInfo typeInfo;

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
        protected Work(WorkContext ctx) : base(ctx.Name, null)
        {
            this.serviceCtx = ctx.ServiceCtx;
            this.ctx = ctx;
            this.parent = ctx.Parent?.Work;

            // gather actions
            actions = new Map<string, ActionInfo>(32);
            this.type = GetType();
            typeInfo = type.GetTypeInfo();

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
            List<ActionInfo> lst = null;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo ai = actions[i];
                if (ai.HasTool)
                {
                    if (lst == null) lst = new List<ActionInfo>();
                    lst.Add(ai);
                }
            }
            tooled = lst?.ToArray();
        }

        public ServiceContext ServiceCtx => serviceCtx;

        public WorkContext Ctx => ctx;

        public Service Service => (Service) serviceCtx.Work;

        public Work Parent => parent;

        public Map<string, ActionInfo> Actions => actions;

        public ActionInfo[] Tooled => tooled;

        public bool HasPick => pick;

        public ActionInfo Default => @default;

        public Map<string, Work> Works => works;

        public Work VarWork => varwork;

        /// <summary>
        /// Create a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <typeparam name="W">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W Create<W>(string name) where W : Work
        {
            if (ctx.Level >= MaxNesting)
            {
                throw new ServiceException("allowed work nesting " + MaxNesting);
            }
            if (works == null)
            {
                works = new Map<string, Work>();
            }
            // create instance by reflection
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WorkContext)});
            if (ci == null)
            {
                throw new ServiceException(typ + "no valid constructor");
            }
            WorkContext wc = new WorkContext(name)
            {
                ServiceCtx = serviceCtx,
                Parent = ctx,
                Work = this,
                Level = ctx.Level + 1,
                Directory = (ctx.Parent == null) ? name : Path.Combine(ctx.Parent.Directory, name),
            };
            // init sub work
            W work = (W) ci.Invoke(new object[] {wc});
            Works.Add(work.Key, work);

            return work;
        }

        /// <summary>
        /// Create a variable-key subwork.
        /// </summary>
        /// <param name="keyer"></param>
        /// <param name="labeller">an attachment object</param>
        /// <typeparam name="W"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="ServiceException">Thrown if error</exception>
        protected W CreateVar<W, K>(Func<IData, K> keyer = null) where W : Work where K : IEquatable<K>
        {
            if (ctx.Level >= MaxNesting)
            {
                throw new ServiceException("allowed work nesting " + MaxNesting);
            }

            // create instance
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WorkContext)});
            if (ci == null)
            {
                throw new ServiceException(typ + " no valid constructor");
            }
            WorkContext wc = new WorkContext(VAR)
            {
                ServiceCtx = serviceCtx,
                Parent = ctx,
                Work = this,
                Level = ctx.Level + 1,
                Directory = (ctx.Parent == null) ? VAR : Path.Combine(ctx.Parent.Directory, VAR),
                Keyer = keyer,
                IsVar = true,
            };
            W work = (W) ci.Invoke(new object[] {wc});
            varwork = work;
            return work;
        }

        public string GetVariableKey(IData obj)
        {
            Delegate keyer = ctx.Keyer;
            if (keyer is Func<IData, string> fstr)
            {
                return fstr(obj);
            }
            if (keyer is Func<IData, long> flong)
            {
                return flong(obj).ToString();
            }
            if (keyer is Func<IData, int> fint)
            {
                return fint(obj).ToString();
            }
            if (keyer is Func<IData, short> fshort)
            {
                return fshort(obj).ToString();
            }
            return null;
        }

        public void PutVariableKey(IData obj, DynamicContent cont)
        {
            Delegate keyer = ctx.Keyer;
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

        public bool IsInstanceOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

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
                ac.Chain(key, work);
                return work.Resolve(ref relative, ac);
            }
            if (varwork != null) // if variable-key sub
            {
                IData prin = ac.Principal;
                if (key.Length == 0 && varwork.ctx.Keyer != null) // resolve shortcut
                {
                    if (prin == null) throw AuthorizeEx;
                    if ((key = varwork.GetVariableKey(prin)) == null)
                    {
                        throw AuthorizeEx;
                    }
                }
                ac.Chain(key, varwork);
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
                var svc = Service;
                if (!svc.TryGiveFromCache(ac))
                {
                    DoFile(rsc, rsc.Substring(dot), ac);
                    svc.Cache(ac); // try cache it
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
                var svc = Service;
                if (!svc.TryGiveFromCache(ac))
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
                    svc.Cache(ac); // try cache it
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

            string path = Path.Combine(ctx.Directory, filename);
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

        public void TRC(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Trace, 0, message, exception, null);
        }

        public void DBG(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Debug, 0, message, exception, null);
        }

        public void INF(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Information, 0, message, exception, null);
        }

        public void WAR(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Warning, 0, message, exception, null);
        }

        public void ERR(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Error, 0, message, exception, null);
        }
    }
}