using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A web work is a server-side controller that realizes a virtual directory containing static/dynamic resources.
    /// </summary>
    ///
    public abstract class WebWork : IKeyed
    {
        // state-passing
        internal readonly WebWorkContext ctx;

        // declared actions 
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;


        const string VarKey = "-var-";

        // child controls, if any
        internal Roll<WebWork> children;

        // the attached multiplexer doer/controller, if any
        internal WebWork var;


        protected WebWork(WebWorkContext wwc)
        {
            this.ctx = wwc;

            // init actions
            actions = new Roll<WebAction>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(string))
                {
                    WebAction wa = new WebAction(this, mi);
                    if (wa.Key.Equals("default"))
                    {
                        defaction = wa;
                    }
                    actions.Add(wa);
                }
            }
        }

        public W AddChild<W>(string key, object state = null) where W : WebWork
        {
            if (children == null)
            {
                children = new Roll<WebWork>(16);
            }
            // create instance by reflection
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebWorkContext) });
            if (ci == null)
                throw new WebException(typ + " missing WebWorkContext");
            WebWorkContext ctx = new WebWorkContext
            {
                key = key,
                State = state,
                Parent = this,
                IsVar = false,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            W work = (W)ci.Invoke(new object[] { ctx });
            children.Add(work);

            return work;
        }

        public Roll<WebWork> Children => children;

        public WebWork Var => var;

        public W SetVar<W>(object state = null) where W : WebWork
        {
            // create instance
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebWorkContext) });
            if (ci == null)
                throw new WebException(typ + " missing WebWorkContext");
            WebWorkContext ctx = new WebWorkContext
            {
                key = VarKey,
                State = state,
                Parent = this,
                IsVar = true,
                Folder = (Parent == null) ? VarKey : Path.Combine(Parent.Folder, VarKey),
                Service = Service
            };
            W work = (W)ci.Invoke(new object[] { ctx });
            this.var = work;

            return work;
        }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key => ctx.Key;

        public object State => ctx.State;

        public bool IsVar => ctx.IsVar;

        public string Folder => ctx.Folder;

        public WebWork Parent => ctx.Parent;

        public WebServiceWork Service => ctx.Service;


        // public Roll<WebAction> Actions => actions;

        public WebAction Action(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defaction;
            }
            return actions[method];
        }

        public WebAction[] Actions(params string[] methods)
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
                string dir = relative.Substring(0, slash);
                WebWork child;
                if (children != null && children.TryGet(dir, out child)) // seek sub first
                {
                    child.Handle(relative.Substring(slash + 1), wc);
                }
                else if (var == null)
                {
                    wc.StatusCode = 404; // not found
                }
                else
                {
                    wc.ChainVar(dir);
                    var.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }

        internal void DoRsc(string rsc, WebContext wc)
        {
            wc.Work = this;

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
                WebAction wa = string.IsNullOrEmpty(key) ? defaction : Action(key);
                if (wa == null)
                {
                    wc.StatusCode = 404;
                }
                else
                {
                    wa.TryInvoke(wc, subscpt);
                }
            }

            wc.Work = null;
        }

        void DoStatic(string file, string ext, WebContext wc)
        {
            if (file.StartsWith("$")) // private resource
            {
                wc.StatusCode = 403;  // forbidden
                return;
            }

            string ctyp;
            if (!StaticContent.TryGetType(ext, out ctyp))
            {
                wc.StatusCode = 415;  // unsupported media type
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

        public virtual void @default(WebContext wc, string subscpt) { }

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