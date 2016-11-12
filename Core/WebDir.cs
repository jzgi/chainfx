using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    ///
    /// A web directory is a server-side controller that realizes a virtual directory containing static/dynamic resources.
    ///
    public abstract class WebDir : IKeyed
    {
        // max nesting level
        const int NestingLevel = 3;

        const string VariableKey = "_var_";


        // state-passing
        internal readonly WebDirContext ctx;

        // declared actions 
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;

        // child directories, if any
        internal Roll<WebDir> children;

        // multiplexer dealing with variable keys
        internal WebDir mux;


        protected WebDir(WebDirContext ctx)
        {
            this.ctx = ctx;

            // init actions
            actions = new Roll<WebAction>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                WebAction wa = null;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                {
                    wa = new WebAction(this, mi, null);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext))
                {
                    Type pt = pis[1].ParameterType; // second parameter's type
                    if (WebAction.IsSubscriptType(pt))
                    {
                        wa = new WebAction(this, mi, pt);
                    }
                }

                if (wa == null) continue;

                actions.Add(wa);
                if (wa.Key.Equals("default"))
                {
                    defaction = wa;
                }
            }
        }

        public D Add<D>(string key, object state = null) where D : WebDir
        {
            if (Level == NestingLevel)
                throw new WebException("nesting levels");

            if (children == null)
            {
                children = new Roll<WebDir>(16);
            }
            // create instance by reflection
            Type typ = typeof(D);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebDirContext) });
            if (ci == null)
                throw new WebException(typ + " missing WebDirContext");
            WebDirContext wdc = new WebDirContext
            {
                key = key,
                State = state,
                IsVariable = false,
                Parent = this,
                Level = Level + 1,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            D dir = (D)ci.Invoke(new object[] { wdc });
            children.Add(dir);

            return dir;
        }

        public Roll<WebDir> Children => children;

        public WebDir Variable => mux;

        public D SetMux<D>(object state = null) where D : WebDir, IMux
        {
            if (Level == NestingLevel)
                throw new WebException("nesting levels");

            // create instance
            Type typ = typeof(D);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebDirContext) });
            if (ci == null)
                throw new WebException(typ + " missing WebDirContext");
            WebDirContext wdc = new WebDirContext
            {
                key = VariableKey,
                State = state,
                IsVariable = true,
                Parent = this,
                Level = Level + 1,
                Folder = (Parent == null) ? VariableKey : Path.Combine(Parent.Folder, VariableKey),
                Service = Service
            };
            D dir = (D)ci.Invoke(new object[] { wdc });
            mux = dir;

            return dir;
        }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key => ctx.Key;

        public object State => ctx.State;

        public bool IsVariable => ctx.IsVariable;

        public string Folder => ctx.Folder;

        public WebDir Parent => ctx.Parent;

        public int Level => ctx.Level;

        public WebService Service => ctx.Service;


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


        internal virtual void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                DoRsc(relative, wc);
            }
            else // dispatch to child or multiplexer
            {
                string key = relative.Substring(0, slash);
                WebDir child;
                if (children != null && children.TryGet(key, out child)) // seek sub first
                {
                    child.Handle(relative.Substring(slash + 1), wc);
                }
                else if (mux == null)
                {
                    wc.StatusCode = 404; // not found
                }
                else
                {
                    wc.Chain(mux, key);
                    mux.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }

        internal void DoRsc(string rsc, WebContext wc)
        {
            wc.Dir = this;

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
                    wa.TryInvoke(wc, subscpt);
                }
            }

            wc.Dir = null;
        }

        void DoStatic(string file, string ext, WebContext wc)
        {
            if (file.StartsWith("$")) // private resource
            {
                wc.StatusCode = 403; // forbidden
                return;
            }

            string ctyp;
            if (!StaticContent.TryGetType(ext, out ctyp))
            {
                wc.StatusCode = 415; // unsupported media type
                return;
            }

            string path = Path.Combine(Folder, file);
            if (!File.Exists(path))
            {
                wc.StatusCode = 404; // not found
            }

            DateTime modified = File.GetLastWriteTime(path);
            DateTime? since = wc.HeaderDateTime("If-Modified-Since");
            if (since != null && modified <= since)
            {
                wc.StatusCode = 304; // not modified
                return;
            }

            // load file content
            byte[] content = File.ReadAllBytes(path);
            StaticContent sta = new StaticContent
            {
                Key = file.ToLower(),
                Type = ctyp,
                ByteBuffer = content,
                Modified = modified
            };
            wc.Send(200, sta, true, 5 * 60000);
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