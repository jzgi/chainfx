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
        readonly ISetting setg;

        // file folder contents, can be null
        readonly Roll<StaticContent> statics;

        // the default static file, can be null
        readonly StaticContent defstatic;

        // declared actions 
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
                cfg.Folder = cfg.Key;
                cfg.Service = svc;
            }

            this.setg = setg;

            // static initialization
            if (Directory.Exists(Folder))
            {
                statics = new Roll<StaticContent>(64);
                foreach (string path in Directory.GetFiles(Folder))
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

        public bool AuthRequired => setg.AuthRequired;

        public bool IsVar => setg.IsVar;

        public string Folder => setg.Folder;

        public IParent Parent => setg.Parent;

        public WebService Service => setg.Service;



        public Roll<StaticContent> Statics => statics;

        public Roll<WebAction> Actions => actions;

        public WebAction GetAction(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defaction;
            }
            return actions[method];
        }

        protected internal virtual void Handle(string rsc, WebContext wc)
        {
            if (AuthRequired && wc.Token == null)
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

        protected internal virtual void Handle(string rsc, WebContext wc, string var)
        {
            if (AuthRequired && wc.Token == null)
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