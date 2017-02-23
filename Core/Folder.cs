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
        // max nesting levels
        const int Nesting = 4;

        // underlying file directory name
        const string _VAR_ = "VAR";

        // state-passing
        internal readonly FolderContext context;

        // declared actions 
        readonly Roll<ActionInfo> actions;

        // the default action
        readonly ActionInfo defaction;

        // sub folders, if any
        internal Roll<Folder> subs;

        // the varied folder, if any
        internal Folder varsub;

        protected Folder(FolderContext fc) : base(fc.Name, null)
        {
            this.context = fc;

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
                ActionInfo atn = null;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ActionContext))
                {
                    atn = new ActionInfo(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(ActionContext) && pis[1].ParameterType == typeof(string))
                {
                    atn = new ActionInfo(this, mi, async, true);
                }
                else continue;

                actions.Add(atn);
                if (atn.Name.Equals("default"))
                {
                    defaction = atn;
                }

                // to override annotated attributes
                if (fc.Roles != null)
                {
                    roles = fc.Roles;
                }
                if (fc.Ui != null)
                {
                    ui = fc.Ui;
                }
            }
        }

        ///
        /// Create a subfolder.
        ///
        public F Create<F>(string key, AccessAttribute[] roles = null, UiAttribute ui = null) where F : Folder
        {
            if (Level >= Nesting)
            {
                throw new ServiceException("folder nesting more than " + Nesting);
            }

            if (subs == null)
            {
                subs = new Roll<Folder>(32);
            }
            // create instance by reflection
            Type typ = typeof(F);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(FolderContext) });
            if (ci == null)
            {
                throw new ServiceException(typ + " missing WebFolderContext");
            }
            FolderContext ctx = new FolderContext
            {
                name = key,
                Roles = roles,
                Ui = ui,
                IsVar = false,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? key : Path.Combine(Parent.Directory, key),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { ctx });
            subs.Add(folder);

            return folder;
        }

        ///
        /// Create a variable-key subfolder.
        ///
        public F CreateVar<F>(AccessAttribute[] roles = null) where F : Folder, IVar
        {
            if (Level >= Nesting)
            {
                throw new ServiceException("nesting levels");
            }

            // create instance
            Type typ = typeof(F);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(FolderContext) });
            if (ci == null)
            {
                throw new ServiceException(typ + " missing WebFolderContext");
            }
            FolderContext ctx = new FolderContext
            {
                name = _VAR_,
                Roles = roles,
                IsVar = true,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? _VAR_ : Path.Combine(Parent.Directory, _VAR_),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { ctx });
            varsub = folder;

            return folder;
        }

        public Roll<ActionInfo> Actions => actions;

        public Roll<Folder> Subs => subs;

        public Folder VarSub => varsub;

        public FolderContext Context => context;

        public bool IsVar => context.IsVar;

        public string Directory => context.Directory;

        public Folder Parent => context.Parent;

        public int Level => context.Level;

        public override Service Service => context.Service;


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
                if (subs != null)
                {
                    for (int i = 0; i < subs.Count; i++)
                    {
                        Folder child = subs[i];
                        child.Describe(cont);
                    }
                }
                if (varsub != null)
                {
                    varsub.Describe(cont);
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

        public List<ActionInfo> GetModalActions(Type checktyp)
        {
            List<ActionInfo> lst = null;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo a = actions[i];
                if (a.IsModal && a.HasRole(checktyp))
                {
                    if (lst == null) lst = new List<ActionInfo>();
                    lst.Add(a);
                }
            }
            return lst;
        }

        public List<ActionInfo> GetModalActions(ActionContext ac)
        {
            List<ActionInfo> lst = null;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo a = actions[i];
                if (a.IsModal && a.Check(ac))
                {
                    if (lst == null) lst = new List<ActionInfo>();
                    lst.Add(a);
                }
            }
            return lst;
        }

        internal Folder ResolveFolder(ref string relative, ActionContext ac)
        {
            // access check 
            if (!Check(ac)) throw AccessEx;

            int slash = relative.IndexOf('/');
            if (slash == -1)
            {
                return this;
            }

            // sub folder
            string key = relative.Substring(0, slash);
            relative = relative.Substring(slash + 1); // adjust relative
            Folder sub;
            if (subs != null && subs.TryGet(key, out sub)) // chiled
            {
                ac.Chain(key, sub);
                return sub.ResolveFolder(ref relative, ac);
            }
            if (varsub != null) // variable-key
            {
                ac.Chain(key, varsub);
                return varsub.ResolveFolder(ref relative, ac);
            }
            return null;
        }

        internal async Task HandleAsync(string rsc, ActionContext ac)
        {
            ac.Folder = this;

            // pre-
            DoBefore(ac);

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
                    ac.Reply(404); // not found
                    return;
                }

                // access check
                if (!actn.Check(ac)) throw AccessEx;

                // try in cache

                if (actn.IsAsync)
                {
                    await actn.DoAsync(ac, arg);
                }
                else
                {
                    actn.Do(ac, arg);
                }
            }

            // post-
            DoAfter(ac);

            ac.Folder = null;
        }


        void DoFile(string filename, string ext, ActionContext ac)
        {
            if (filename.StartsWith("$")) // private resource
            {
                ac.Reply(403); // forbidden
                return;
            }

            string ctyp;
            if (!StaticContent.TryGetType(ext, out ctyp))
            {
                ac.Reply(415); // unsupported media type
                return;
            }

            string path = Path.Combine(Directory, filename);
            if (!File.Exists(path))
            {
                ac.Reply(404); // not found
                return;
            }

            DateTime modified = File.GetLastWriteTime(path);
            DateTime? since = ac.HeaderDateTime("If-Modified-Since");
            if (since != null && modified <= since)
            {
                ac.Reply(304); // not modified
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
            ac.Reply(200, cont, true, 3600 * 12);
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