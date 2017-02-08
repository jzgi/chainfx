using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    ///
    /// A web folder realizes a virtual folder containing static/dynamic resources.
    ///
    public abstract class WebFolder : WebNodule, IRollable
    {
        // max nesting levels
        const int Nesting = 4;

        // underlying file directory name
        const string _VAR_ = "VAR";

        // state-passing
        internal readonly WebFolderContext context;

        // declared actions 
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;

        // sub folders, if any
        internal Roll<WebFolder> subs;

        // the varied folder, if any
        internal WebFolder varsub;

        protected WebFolder(WebFolderContext context) : base(null)
        {
            this.context = context;

            // init actions
            actions = new Roll<WebAction>(32);
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
                WebAction atn = null;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebActionContext))
                {
                    atn = new WebAction(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebActionContext) && pis[1].ParameterType == typeof(string))
                {
                    atn = new WebAction(this, mi, async, true);
                }
                else continue;

                actions.Add(atn);
                if (atn.Name.Equals("default"))
                {
                    defaction = atn;
                }

                if (context.Roles != null)
                {
                    roles = context.Roles;
                }
            }
        }

        ///
        /// Create a child folder.
        ///
        public F Create<F>(string key, RoleAttribute[] grants = null) where F : WebFolder
        {
            if (Level >= Nesting) throw new WebException("nesting levels");

            if (subs == null)
            {
                subs = new Roll<WebFolder>(16);
            }
            // create instance by reflection
            Type typ = typeof(F);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebFolderContext) });
            if (ci == null)
            {
                throw new WebException(typ + " missing WebFolderContext");
            }
            WebFolderContext ctx = new WebFolderContext
            {
                name = key,
                Roles = grants,
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

        public Roll<WebAction> Actions => actions;

        public Roll<WebFolder> Subs => subs;

        public WebFolder VarSub => varsub;

        ///
        /// Make a variable-key subdirectory.
        ///
        public F CreateVar<F>(RoleAttribute[] grants = null) where F : WebFolder, IVar
        {
            if (Level >= Nesting) throw new WebException("nesting levels");

            // create instance
            Type typ = typeof(F);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebFolderContext) });
            if (ci == null)
            {
                throw new WebException(typ + " missing WebFolderContext");
            }
            WebFolderContext ctx = new WebFolderContext
            {
                name = _VAR_,
                Roles = grants,
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

        public WebFolderContext Context => context;

        public string Name => context.Name;

        public RoleAttribute[] Grants => context.Roles;

        public bool IsVar => context.IsVar;

        public string Directory => context.Directory;

        public WebFolder Parent => context.Parent;

        public int Level => context.Level;

        public override WebService Service => context.Service;


        internal void Describe(XmlContent cont)
        {
            cont.ELEM(Name,
            delegate
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    WebAction action = Actions[i];
                    cont.Put(action.Name, "");
                }
            },
            delegate
            {
                if (subs != null)
                {
                    for (int i = 0; i < subs.Count; i++)
                    {
                        WebFolder child = subs[i];
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

        public WebAction GetAction(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defaction;
            }
            return actions[method];
        }

        public WebAction[] GetActions(params string[] methods)
        {
            int len = methods.Length;
            WebAction[] atn = new WebAction[len];
            for (int i = 0; i < methods.Length; i++)
            {
                string mthd = methods[i];
                atn[i] = string.IsNullOrEmpty(mthd) ? defaction : actions[mthd];
            }
            return atn;
        }

        public List<WebAction> GetUiActions(Type checktyp)
        {
            List<WebAction> lst = null;
            for (int i = 0; i < actions.Count; i++)
            {
                WebAction a = actions[i];
                if (a.Ui != null && a.HasRole(checktyp))
                {
                    if (lst == null) lst = new List<WebAction>();
                    lst.Add(a);
                }
            }
            return lst;
        }

        internal WebFolder Locate(ref string relative, WebActionContext ac)
        {
            if (!Check(ac))
            {
                ac.Reply(403); // forbidden
                return null;
            }

            int slash = relative.IndexOf('/');
            if (slash == -1)
            {
                return this;
            }

            // sub folder
            string key = relative.Substring(0, slash);
            relative = relative.Substring(slash + 1); // adjust relative
            WebFolder sub;
            if (subs != null && subs.TryGet(key, out sub)) // chiled
            {
                return sub.Locate(ref relative, ac);
            }
            if (varsub != null) // variable-key
            {
                ac.ChainVar(key, varsub);
                return varsub.Locate(ref relative, ac);
            }

            ac.Reply(404); // not found
            return null;
        }

        internal async Task HandleAsync(string rsc, WebActionContext ac)
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
                WebAction atn = string.IsNullOrEmpty(name) ? defaction : GetAction(name);
                if (atn == null)
                {
                    ac.Reply(404); // not found
                }
                else if (!atn.Check(ac))
                {
                    ac.Reply(403); // forbidden
                }
                else
                {
                    // try in cache

                    if (atn.Async)
                    {
                        await atn.DoAsync(ac, arg);
                    }
                    else
                    {
                        atn.Do(ac, arg);
                    }
                }
            }

            // post-
            DoAfter(ac);

            ac.Folder = null;
        }


        void DoFile(string filename, string ext, WebActionContext ac)
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