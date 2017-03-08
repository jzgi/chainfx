using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    ///
    /// A virtual web folder that contains static/dynamic resources.
    ///
    public abstract class Folder : Nodule
    {
        static readonly VarKeyException VarkeyEx = new VarKeyException();

        // max nesting levels
        const int MaxNesting = 6;

        // underlying file directory name
        const string _VAR_ = "VAR";

        // state-passing
        readonly FolderContext fc;

        // declared actions 
        readonly Roll<ActionInfo> actions;

        // the default action
        readonly ActionInfo defaction;

        // subfolders, if any
        internal Roll<Folder> subfolders;

        // the variable-key subfolder, if any
        internal Folder varfolder;

        internal Func<IData, string> varkeyer;

        protected Folder(FolderContext fc) : base(fc.Name, null)
        {
            this.fc = fc;

            // init actions
            actions = new Roll<ActionInfo>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                Type ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task)) async = true;
                else if (ret == typeof(void)) async = false;
                else continue;

                ParameterInfo[] pis = mi.GetParameters();
                ActionInfo ai = null;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ActionContext))
                {
                    ai = new ActionInfo(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(ActionContext) && pis[1].ParameterType == typeof(string))
                {
                    ai = new ActionInfo(this, mi, async, true);
                }
                else continue;

                actions.Add(ai);
                if (ai.Name.Equals("default"))
                {
                    defaction = ai;
                }
            }

            // to override annotated attributes
            if (fc.Ui != null)
            {
                ui = fc.Ui;
            }
            if (fc.Authorize != null)
            {
                authorize = fc.Authorize;
            }

            // preprocess start-end annotations
            AuthorizeAttribute start = null;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo ai = actions[i];
                AuthorizeAttribute auth = ai.authorize;

                if (start != null)
                {
                    if (auth == null) ai.authorize = start;
                    else auth.Or(start);
                }

                if (auth != null && auth.Start)
                {
                    auth.Start = false;
                    start = auth;
                }
                if (auth != null && auth.End)
                {
                    auth.End = false;
                    start = null;
                }
            }
        }

        ///
        /// Create a subfolder.
        ///
        public F AddSub<F>(string name, UiAttribute ui = null, AuthorizeAttribute auth = null) where F : Folder
        {
            if (Level >= MaxNesting)
            {
                throw new ServiceException("allowed folder nesting " + MaxNesting);
            }

            if (subfolders == null)
            {
                subfolders = new Roll<Folder>(32);
            }
            // create instance by reflection
            Type typ = typeof(F);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(FolderContext) });
            if (ci == null)
            {
                throw new ServiceException(typ + " missing FolderContext");
            }
            FolderContext fc = new FolderContext(name)
            {
                Ui = ui,
                Authorize = auth,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { fc });
            subfolders.Add(folder);

            return folder;
        }

        ///
        /// Create a variable-key subfolder.
        ///
        public F CreateVar<F>(Func<IData, string> keyer = null, UiAttribute ui = null, AuthorizeAttribute auth = null) where F : Folder, IVar
        {
            if (Level >= MaxNesting)
            {
                throw new ServiceException("allowed folder nesting " + MaxNesting);
            }

            // create instance
            Type typ = typeof(F);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(FolderContext) });
            if (ci == null)
            {
                throw new ServiceException(typ + " missing FolderContext");
            }
            FolderContext fc = new FolderContext(_VAR_)
            {
                Ui = ui,
                Authorize = auth,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? _VAR_ : Path.Combine(Parent.Directory, _VAR_),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { fc });
            varkeyer = keyer;
            varfolder = folder;

            return folder;
        }

        public Roll<ActionInfo> Actions => actions;

        public Roll<Folder> SubFolders => subfolders;

        public Folder VarFolder => varfolder;

        public Func<IData, string> VarKeyer => varkeyer;

        public string Directory => fc.Directory;

        public Folder Parent => fc.Parent;

        public int Level => fc.Level;

        public override Service Service => fc.Service;

        public string GetVarKey(IData token) => varkeyer?.Invoke(token);

        internal void Describe(XmlContent cont)
        {
            cont.ELEM(Name,
            delegate
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    ActionInfo action = Actions[i];
                    cont.Put(action.Name, "");
                }
            },
            delegate
            {
                if (subfolders != null)
                {
                    for (int i = 0; i < subfolders.Count; i++)
                    {
                        Folder child = subfolders[i];
                        child.Describe(cont);
                    }
                }
                if (varfolder != null)
                {
                    varfolder.Describe(cont);
                }
            });
        }


        // public Roll<WebAction> Actions => actions;

        public ActionInfo GetAction(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defaction;
            }
            return actions[method];
        }

        public List<ActionInfo> GetUiActions(ActionContext ac)
        {
            List<ActionInfo> lst = null;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo a = actions[i];
                if (a.HasUi && a.DoAuthorize(ac))
                {
                    if (lst == null) lst = new List<ActionInfo>();
                    lst.Add(a);
                }
            }
            return lst;
        }

        internal Folder ResolveFolder(ref string relative, ActionContext ac)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1)
            {
                return this;
            }

            // sub folder
            string key = relative.Substring(0, slash);
            relative = relative.Substring(slash + 1); // adjust relative
            Folder sub;
            if (subfolders != null && subfolders.TryGet(key, out sub)) // chiled
            {
                ac.Chain(key, sub);
                return sub.ResolveFolder(ref relative, ac);
            }
            if (varfolder != null) // variable-key
            {
                if (key.Length == 0) // resolve varkey
                {
                    if (varkeyer == null) return null;
                    key = varkeyer(ac.Token);
                    if (key == null)
                    {
                        throw VarkeyEx;
                    }
                }
                ac.Chain(key, varfolder);
                return varfolder.ResolveFolder(ref relative, ac);
            }
            return null;
        }

        internal async Task HandleAsync(string rsc, ActionContext ac)
        {
            ac.Folder = this;

            // access check 
            if (!DoAuthorize(ac)) throw AuthorizeEx;

            // pre-
            WorkAttribute wrk = Work;
            if (wrk != null && wrk.Before) { if (wrk.IsAsync) await wrk.WorkAsync(ac); else wrk.Work(ac); }

            int dot = rsc.LastIndexOf('.');
            if (dot != -1) // file
            {
                // try in cache 

                DoFile(rsc, rsc.Substring(dot), ac);
            }
            else // action
            {
                string name = rsc;
                string arg = null;
                int dash = rsc.LastIndexOf('-');
                if (dash != -1)
                {
                    name = rsc.Substring(0, dash);
                    arg = rsc.Substring(dash + 1);
                }
                ActionInfo actn = string.IsNullOrEmpty(name) ? defaction : GetAction(name);
                if (actn == null)
                {
                    ac.Give(404); // not found
                    return;
                }

                try
                {
                    ac.Doer = actn;

                    // access check
                    if (!actn.DoAuthorize(ac)) throw AuthorizeEx;

                    // try in cache

                    // work before
                    WorkAttribute awrk = actn.Work;
                    if (awrk != null && awrk.Before) { if (awrk.IsAsync) await awrk.WorkAsync(ac); else awrk.Work(ac); }
                    // method invocation
                    if (actn.IsAsync)
                    {
                        await actn.DoAsync(ac, arg); // invoke action method
                    }
                    else
                    {
                        actn.Do(ac, arg);
                    }
                    // work after
                    if (awrk != null && !awrk.Before) { if (awrk.IsAsync) await awrk.WorkAsync(ac); else awrk.Work(ac); }

                    ac.Doer = null;
                }
                catch (Exception ex)
                {
                    CatchAttribute @catch = actn.Catch;
                    if (@catch != null)
                    {
                        @catch.Catch(ac, ex);
                    }
                    else { throw ex; }
                }
            }

            // post-
            if (wrk != null && !wrk.Before) { if (wrk.IsAsync) await wrk.WorkAsync(ac); else wrk.Work(ac); }

            ac.Folder = null;
        }


        void DoFile(string filename, string ext, ActionContext ac)
        {
            if (filename.StartsWith("$")) // private resource
            {
                ac.Give(403); // forbidden
                return;
            }

            string ctyp;
            if (!StaticContent.TryGetType(ext, out ctyp))
            {
                ac.Give(415); // unsupported media type
                return;
            }

            string path = Path.Combine(Directory, filename);
            if (!File.Exists(path))
            {
                ac.Give(404); // not found
                return;
            }

            DateTime modified = File.GetLastWriteTime(path);
            DateTime? since = ac.HeaderDateTime("If-Modified-Since");
            if (since != null && modified <= since)
            {
                ac.Give(304); // not modified
                return;
            }

            // load file content
            byte[] bytes = File.ReadAllBytes(path);
            StaticContent cont = new StaticContent(bytes)
            {
                Name = filename,
                Type = ctyp,
                Modified = modified
            };
            ac.Give(200, cont, true, 3600 * 12);
        }

        //
        // LOGGING METHODS
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