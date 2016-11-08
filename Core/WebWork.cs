using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// A web work is a server-side /controller that realizes a virtual directory containing static/dynamic resources.
    /// </summary>
    ///
    public abstract class WebWork : IKeyed
    {
        // makes state-passing convenient
        internal readonly WebHierarchyContext ctx;

        // declared actions 
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;


        protected WebWork(WebHierarchyContext whc)
        {
            this.ctx = whc;

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

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key => ctx.Key;

        public object State => ctx.State;

        public bool IsMux => ctx.IsVar;

        public string Folder => ctx.Folder;

        public IParent Parent => ctx.Parent;

        public WebService Service => ctx.Service;


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
            DoRsc(relative, wc);
        }

        internal void DoRsc(string rsc, WebContext wc)
        {
            wc.Doer = this;

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

            wc.Doer = null;
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