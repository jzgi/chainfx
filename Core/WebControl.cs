using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    ///
    /// <summary>
    /// The web controller pertaining to a virtual directory, that handles request for static and dynamic contents.
    /// </summary>
    ///
    public abstract class WebControl : IKeyed
    {
        // makes state-passing convenient
        internal readonly WebArg arg;

        // declared actions 
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;


        List<WebInterface> interfaces;

        protected WebControl(WebArg arg)
        {
            this.arg = arg;

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

            // init interfaces
            Type[] ityps = typ.GetInterfaces();
            for (int i = 0; i < ityps.Length; i++)
            {
                Type ityp = ityps[i];
                string ns = ityp.Namespace;
                if (ns.Equals("Greatbone.Core") || ns.StartsWith("System") || ns.StartsWith("Microsoft")) continue; // a system interface

                if (interfaces == null) interfaces = new List<WebInterface>(4);
                WebInterface wi = new WebInterface(ityp);
                foreach (MethodInfo mi in ityp.GetMethods())
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(string))
                    {
                        WebAction wa;
                        if (actions.TryGet(mi.Name, out wa))
                        {
                            wi.Add(wa);
                        }
                    }
                }
                interfaces.Add(wi);
            }
        }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key => arg.Key;

        public object State => arg.State;

        public bool IsMultiplex => arg.IsMultiplex;

        public string Folder => arg.Folder;

        public IParent Parent => arg.Parent;

        public WebService Service => arg.Service;


        public Roll<WebAction> Actions => actions;

        public WebAction GetAction(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defaction;
            }
            return actions[method];
        }

        public WebInterface GetInterface(Type type)
        {
            if (interfaces != null)
            {
                for (int i = 0; i < interfaces.Count; i++)
                {
                    WebInterface wi = interfaces[i];
                    if (wi.Type == type)
                    {
                        return wi;
                    }
                }
            }
            return null;
        }

        internal virtual void Handle(string relative, WebContext wc)
        {
            wc.Control = this;
            Do(relative, wc);
            wc.Control = null;
        }

        protected internal virtual void Do(string rsc, WebContext wc)
        {
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
                WebAction a = string.IsNullOrEmpty(key) ? defaction : GetAction(key);
                if (a == null)
                {
                    wc.StatusCode = 404;
                }
                else
                {
                    a.TryDo(wc, subscpt);
                }
            }
        }

        void DoStatic(string file, string ext, WebContext wc)
        {
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
            if (since != null && modified <= since) // not modified
            {
                wc.StatusCode = 304;
                return;
            }

            // load file content
            byte[] content = File.ReadAllBytes(path);
            StaticContent sta = new StaticContent
            {
                Key = file.ToLower(),
                Type = ctyp,
                Buffer = content,
                LastModified = modified
            };
            wc.Out(200, sta, true, 5 * 60000);
        }

        public virtual void @default(WebContext wc, string subscpt)
        {
            DoStatic("default.html", ".html", wc);
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