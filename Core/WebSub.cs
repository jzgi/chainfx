using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    ///
    /// <summary>
    /// The controlling sub-routines pertaining to a virtual web directory, that handles request for static and dynamic contents.
    /// </summary>
    ///
    public abstract class WebSub : IKeyed
    {
        internal readonly WebArg arg;

        // declared actions 
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;

        // the argument makes state-passing more convenient
        protected WebSub(WebArg arg)
        {
            this.arg = arg;

            // action initialization
            actions = new Roll<WebAction>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                WebAction a = null;
                if (arg.IsVar)
                {
                    if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(string))
                    {
                        a = new WebAction(this, mi, true);
                    }
                }
                else
                {
                    if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                    {
                        a = new WebAction(this, mi, false);
                    }
                }
                if (a != null)
                {
                    if (a.Key.Equals("default")) { defaction = a; }
                    actions.Add(a);
                }
            }
        }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key => arg.Key;

        public bool Auth => arg.Auth;

        public bool IsVar => arg.IsVar;

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

        internal bool CheckAuth(WebContext wc)
        {
            if (Auth && wc.Token == null)
            {
                wc.StatusCode = 401; // unauthorized
                wc.Response.Headers.Add("WWW-Authenticate", new StringValues("Bearer"));
                return false;
            }
            return true;
        }

        internal virtual void Handle(string rsc, WebContext wc)
        {
            if (!CheckAuth(wc)) return;

            wc.Control = this;
            Do(rsc, wc);
        }

        internal virtual void Handle(string rsc, WebContext wc, string var)
        {
            if (!CheckAuth(wc)) return;

            wc.Control = this;
            Do(rsc, wc, var);
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

        protected internal virtual void Do(string rsc, WebContext wc)
        {
            int dot = rsc.IndexOf('.');
            if (dot != -1) // static
            {
                DoStatic(rsc, rsc.Substring(dot), wc);
            }
            else // dynamic
            {
                WebAction a = string.IsNullOrEmpty(rsc) ? defaction : GetAction(rsc);
                if (a == null) { wc.StatusCode = 404; }
                else if (!a.TryDo(wc)) wc.StatusCode = 403; // forbidden 
            }
        }

        protected internal virtual void Do(string rsc, WebContext wc, string var)
        {
            int dot = rsc.IndexOf('.');
            if (dot != -1) // static
            {
                DoStatic(rsc, rsc.Substring(dot), wc);
            }
            else // dynamic
            {
                WebAction a = string.IsNullOrEmpty(rsc) ? defaction : GetAction(rsc);
                if (a == null) wc.StatusCode = 404;
                else if (!a.TryDo(wc, var)) wc.StatusCode = 403; // forbidden
            }

        }

        public virtual void @default(WebContext wc)
        {
            DoStatic("default.html", ".html", wc);
        }

        public virtual void @default(WebContext wc, string var)
        {
            DoStatic("default.html", ".html", wc);
        }

        //
        // LOGGING METHODS
        //

        public void Trace(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Trace, 0, message, exception, null);
        }

        public void Debug(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Debug, 0, message, exception, null);
        }

        public void Info(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Information, 0, message, exception, null);
        }

        public void Warning(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Warning, 0, message, exception, null);
        }

        public void Error(string message, Exception exception = null)
        {
            Service.Log(LogLevel.Error, 0, message, exception, null);
        }

    }

}