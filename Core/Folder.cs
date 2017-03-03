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
        internal Roll<Folder> subs;

        // the variable-key subfolder, if any
        internal Folder varsub;

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

                // to override annotated attributes
                if (fc.Authorizes != null)
                {
                    authorizes = fc.Authorizes;
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
        public F Create<F>(string name, AuthorizeAttribute[] authorizes = null, UiAttribute ui = null) where F : Folder
        {
            if (Level >= MaxNesting)
            {
                throw new ServiceException("allowed folder nesting " + MaxNesting);
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
                throw new ServiceException(typ + " missing FolderContext");
            }
            FolderContext fc = new FolderContext(name)
            {
                Authorizes = authorizes,
                Ui = ui,
                IsVar = false,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { fc });
            subs.Add(folder);

            return folder;
        }

        ///
        /// Create a variable-key subfolder.
        ///
        public F CreateVar<F>(AuthorizeAttribute[] authorizes = null) where F : Folder, IVar
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
                Authorizes = authorizes,
                IsVar = true,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? _VAR_ : Path.Combine(Parent.Directory, _VAR_),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { fc });
            varsub = folder;

            return folder;
        }

        public Roll<ActionInfo> Actions => actions;

        public Roll<Folder> Subs => subs;

        public Folder VarSub => varsub;
        

        public bool IsVar => fc.IsVar;

        public string Directory => fc.Directory;

        public Folder Parent => fc.Parent;

        public int Level => fc.Level;

        public override Service Service => fc.Service;


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
                if (a.IsModal && a.HasCheck(checktyp))
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
                if (a.IsModal && a.Authorize(ac))
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
            if (!Authorize(ac)) throw AuthorizeEx;

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
                if (!actn.Authorize(ac)) throw AuthorizeEx;

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