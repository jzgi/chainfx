using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    ///
    /// A web folder is a server-side controller that realizes a virtual folder containing static/dynamic resources.
    ///
    public abstract class WebFolder : WebControl, IRollable
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
        internal Roll<WebFolder> children;

        // variable-key subfolder
        internal WebFolder variable;

        protected WebFolder(WebFolderContext context) : base(null)
        {
            this.context = context;

            // init actions
            actions = new Roll<WebAction>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebActionContext))
                {
                    WebAction atn = new WebAction(this, mi);
                    actions.Add(atn);
                    if (atn.Name.Equals("default"))
                    {
                        defaction = atn;
                    }
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebActionContext) && pis[1].ParameterType == typeof(Var))
                {
                    WebAction atn = new WebAction(this, mi, true);
                    actions.Add(atn);
                    if (atn.Name.Equals("default"))
                    {
                        defaction = atn;
                    }
                }
            }
        }

        ///
        /// Create a child folder.
        ///
        public F Make<F>(string name, object state = null) where F : WebFolder
        {
            if (Level >= Nesting) throw new WebException("nesting levels");

            if (children == null)
            {
                children = new Roll<WebFolder>(16);
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
                name = name,
                State = state,
                Var = false,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { ctx });
            children.Add(folder);

            return folder;
        }

        public Roll<WebAction> Actions => actions;

        public Roll<WebFolder> Children => children;

        public WebFolder Variable => variable;

        ///
        /// Make a variable-key subdirectory.
        ///
        public F MakeVariable<F>(object state = null) where F : WebFolder, IVariable
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
                State = state,
                Var = true,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? _VAR_ : Path.Combine(Parent.Directory, _VAR_),
                Service = Service
            };
            F folder = (F)ci.Invoke(new object[] { ctx });
            variable = folder;

            return folder;
        }

        public string Name => context.Name;

        public object State => context.State;

        public bool IsVar => context.Var;

        public string Directory => context.Directory;

        public WebFolder Parent => context.Parent;

        public int Level => context.Level;

        public override WebService Service => context.Service;


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

        public List<WebAction> GetUiActions(Type role)
        {
            List<WebAction> lst = null;
            for (int i = 0; i < actions.Count; i++)
            {
                WebAction a = actions[i];
                if (a.Ui != null && a.HasRole(role))
                {
                    if (lst == null) lst = new List<WebAction>();
                    lst.Add(a);
                }
            }
            return lst;
        }

        internal virtual void Handle(string relative, WebActionContext ac)
        {
            if (!Check(ac)) return;
            // pre-
            BeforeDo(ac);

            int slash = relative.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                Do(relative, ac);
            }
            else // dispatch to child or var
            {
                string key = relative.Substring(0, slash);
                WebFolder child;
                if (children != null && children.TryGet(key, out child)) // chiled
                {
                    child.Handle(relative.Substring(slash + 1), ac);
                }
                else if (variable != null) // variable-key
                {
                    ac.ChainKey(key, variable);
                    variable.Handle(relative.Substring(slash + 1), ac);
                }
                else
                {
                    ac.Reply(404); // not found
                }
            }

            // post-
            AfterDo(ac);
        }

        internal void Do(string rsc, WebActionContext ac)
        {
            ac.Folder = this;

            int dot = rsc.LastIndexOf('.');
            if (dot != -1) // static
            {
                DoStatic(rsc, rsc.Substring(dot), ac);
            }
            else // dynamic
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
                if (atn != null)
                {
                    atn.Do(ac, new Var(arg, null));
                }
                else
                {
                    ac.Reply(404);
                }
            }

            ac.Folder = null;
        }


        void DoStatic(string filename, string ext, WebActionContext ac)
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
            DateTime? since = ac.HeaderAsDateTime("If-Modified-Since");
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
            ac.Reply(200, cont, true, 5 * 60000);
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