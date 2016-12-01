using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    ///
    /// A web directory is a server-side controller that realizes a virtual directory containing static/dynamic resources.
    ///
    public abstract class WebDirectory : IKeyed
    {
        // max nesting level
        const int Nesting = 3;

        const string VariableKey = "-var-";

        // state-passing
        internal readonly WebMakeContext makectx;

        // declared actions 
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;

        // sub directories, if any
        internal Roll<WebDirectory> children;

        // dealing with variable keys
        internal WebDirectory variable;


        protected WebDirectory(WebMakeContext makectx)
        {
            this.makectx = makectx;

            // init actions
            actions = new Roll<WebAction>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebActionContext))
                {
                    WebAction wa = new WebAction(this, mi);
                    actions.Add(wa);
                    if (wa.Key.Equals("default"))
                    {
                        defaction = wa;
                    }
                }
            }
        }

        public D Make<D>(string key, object state = null) where D : WebDirectory
        {
            if (Level >= Nesting) throw new WebException("nesting levels");

            if (children == null)
            {
                children = new Roll<WebDirectory>(16);
            }
            // create instance by reflection
            Type typ = typeof(D);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebMakeContext) });
            if (ci == null)
            {
                throw new WebException(typ + " missing WebDirContext");
            }
            WebMakeContext mc = new WebMakeContext
            {
                key = key,
                State = state,
                IsVariable = false,
                Parent = this,
                Level = Level + 1,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            D dir = (D)ci.Invoke(new object[] { mc });
            children.Add(dir);

            return dir;
        }

        public Roll<WebDirectory> Children => children;

        public WebDirectory Variable => variable;

        public D MakeVariable<D>(object state = null) where D : WebDirectory, IVariable
        {
            if (Level >= Nesting) throw new WebException("nesting levels");

            // create instance
            Type typ = typeof(D);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebMakeContext) });
            if (ci == null)
            {
                throw new WebException(typ + " missing WebDirContext");
            }
            WebMakeContext mc = new WebMakeContext
            {
                key = VariableKey,
                State = state,
                IsVariable = true,
                Parent = this,
                Level = Level + 1,
                Folder = (Parent == null) ? VariableKey : Path.Combine(Parent.Folder, VariableKey),
                Service = Service
            };
            D dir = (D)ci.Invoke(new object[] { mc });
            variable = dir;

            return dir;
        }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key => makectx.Key;

        public object State => makectx.State;

        public bool IsVariable => makectx.IsVariable;

        public string Folder => makectx.Folder;

        public WebDirectory Parent => makectx.Parent;

        public int Level => makectx.Level;

        public WebService Service => makectx.Service;


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
            WebAction[] was = new WebAction[len];
            for (int i = 0; i < methods.Length; i++)
            {
                string meth = methods[i];
                was[i] = string.IsNullOrEmpty(meth) ? defaction : actions[meth];
            }
            return was;
        }


        internal virtual void Handle(string relative, WebActionContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                DoRsc(relative, wc);
            }
            else // dispatch to child or multiplexer
            {
                string key = relative.Substring(0, slash);
                WebDirectory child;
                if (children != null && children.TryGet(key, out child)) // seek sub first
                {
                    child.Handle(relative.Substring(slash + 1), wc);
                }
                else if (variable == null)
                {
                    wc.StatusCode = 404; // not found
                }
                else
                {
                    wc.SetVar(key, variable);
                    variable.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }

        internal void DoRsc(string rsc, WebActionContext wc)
        {
            wc.Directory = this;

            int dot = rsc.LastIndexOf('.');
            if (dot != -1) // static
            {
                DoStatic(rsc, rsc.Substring(dot), wc);
            }
            else // dynamic
            {
                string key = rsc;
                string subscpt = null;
                int dash = rsc.LastIndexOf('-');
                if (dash != -1)
                {
                    key = rsc.Substring(0, dash);
                    subscpt = rsc.Substring(dash + 1);
                }
                WebAction wa = string.IsNullOrEmpty(key) ? defaction : GetAction(key);
                if (wa == null)
                {
                    wc.StatusCode = 404;
                }
                else
                {
                    wa.TryDo(wc);
                }
            }

            wc.Directory = null;
        }

        void DoStatic(string file, string ext, WebActionContext ac)
        {
            if (file.StartsWith("$")) // private resource
            {
                ac.StatusCode = 403; // forbidden
                return;
            }

            string ctyp;
            if (!StaticContent.TryGetCType(ext, out ctyp))
            {
                ac.StatusCode = 415; // unsupported media type
                return;
            }

            string path = Path.Combine(Folder, file);
            if (!File.Exists(path))
            {
                ac.StatusCode = 404; // not found
                return;
            }

            DateTime modified = File.GetLastWriteTime(path);
            DateTime? since = ac.HeaderDateTime("If-Modified-Since");
            if (since != null && modified <= since)
            {
                ac.StatusCode = 304; // not modified
                return;
            }

            // load file content
            byte[] cont = File.ReadAllBytes(path);
            StaticContent sta = new StaticContent(file.ToLower(), cont)
            {
                CType = ctyp,
                Modified = modified
            };
            ac.Send(200, sta, true, 5 * 60000);
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