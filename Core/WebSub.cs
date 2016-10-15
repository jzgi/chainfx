using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    ///
    /// Represents a (sub)controller that consists of a group of action methods, and optionally a folder of static files.
    ///
    public abstract class WebSub : IKeyed
    {
        readonly ISetting setg;

        // static folder path
        readonly string path;

        // the corresponding file folder contents, can be null
        readonly Roll<StaticContent> statics;

        // the default static file in the file folder, can be null
        readonly StaticContent defstatic;

        // doer declared by this controller
        readonly Roll<WebAction> actions;

        // the default action
        readonly WebAction defaction;

        // the argument makes state-passing more convenient
        protected WebSub(ISetting setg)
        {
            // adjust setting for a service
            if (setg.Service == null)
            {
                WebService svc = this as WebService;
                if (svc == null) { throw new WebException("not a WebService"); }
                WebConfig cfg = setg as WebConfig;
                if (cfg == null) { throw new WebException("not a WebConfig"); }
                cfg.Service = svc;
            }

            this.setg = setg;

            // static initialization
            path = setg.Parent == null ? Key : Path.Combine(Parent.path, Key);
            if (path != null && Directory.Exists(path))
            {
                statics = new Roll<StaticContent>(64);
                foreach (string path in Directory.GetFiles(path))
                {
                    string file = Path.GetFileName(path);
                    string ext = Path.GetExtension(path);
                    string ctyp;
                    if (StaticContent.TryGetType(ext, out ctyp))
                    {
                        byte[] content = File.ReadAllBytes(path);
                        DateTime modified = File.GetLastWriteTime(path);
                        StaticContent sta = new StaticContent
                        {
                            Key = file.ToLower(),
                            Type = ctyp,
                            Buffer = content,
                            LastModified = modified
                        };
                        statics.Add(sta);
                        if (sta.Key.StartsWith("default."))
                        {
                            defstatic = sta;
                        }
                    }
                }
            }

            // action initialization
            actions = new Roll<WebAction>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                WebAction a = null;
                if (setg.IsVar)
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
        public string Key => setg.Key;

        public bool Authen => setg.Authen;

        public bool IsVar => setg.IsVar;

        /// <summary>The service that this controller resides in.</summary>
        ///
        public WebService Service => setg.Service;

        public WebSub Parent => setg.Parent;

        public WebAction GetAction(System.String method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defaction;
            }
            return actions[method];
        }

        protected internal virtual void Do(string rsc, WebContext wc)
        {
            if (Authen && wc.Token == null)
            {
                wc.StatusCode = 401;
                wc.Response.Headers.Add("WWW-Authenticate", new StringValues("Bearer"));
                return;
            }

            if (rsc.IndexOf('.') != -1) // static
            {
                StaticContent sta;
                if (statics != null && statics.TryGet(rsc, out sta)) wc.Content = sta;
                else wc.StatusCode = 404;
            }
            else // dynamic
            {
                WebAction a = string.IsNullOrEmpty(rsc) ? defaction : GetAction(rsc);
                if (a == null) wc.StatusCode = 404;
                else if (!a.Do(wc)) wc.StatusCode = 403; // forbidden 
            }
        }

        protected internal virtual void Do(string rsc, WebContext wc, string var)
        {
            if (Authen && wc.Token == null)
            {
                wc.StatusCode = 401;
                wc.Response.Headers.Add("WWW-Authenticate", new StringValues("Bearer"));
                return;
            }

            if (rsc.IndexOf('.') != -1) // static
            {
                StaticContent sta;
                if (statics != null && statics.TryGet(rsc, out sta)) wc.Content = sta;
                else wc.StatusCode = 404;
            }
            else // dynamic
            {
                WebAction a = string.IsNullOrEmpty(rsc) ? defaction : GetAction(rsc);
                if (a == null) wc.StatusCode = 404;
                else if (!a.Do(wc, var)) wc.StatusCode = 403; // forbidden
            }
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

        public virtual void @default(WebContext wc)
        {
            StaticContent sta = defstatic;
            if (sta != null)
            {
                wc.Content = sta;
            }
            else
            {
                // send not implemented
                wc.StatusCode = 404;
            }
        }

        public virtual void @default(WebContext wc, string var)
        {
            StaticContent sta = defstatic;
            if (sta != null)
            {
                wc.Content = sta;
            }
            else
            {
                // send not implemented
                wc.StatusCode = 404;
            }
        }

    }
}