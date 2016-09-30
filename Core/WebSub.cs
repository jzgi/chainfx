using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// Represents a (sub)controller that consists of a group of action methods, and optionally a folder of static files.
    ///
    public abstract class WebSub : IKeyed
    {
        // doer declared by this controller
        readonly Roll<WebDoer> doers;

        // the default doer
        readonly WebDoer defdoer;

        public WebConfig Config { get; internal set; }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key => Config.Key;

        /// <summary>The service that this controller resides in.</summary>
        ///
        public WebService Service => Config.Service;

        public WebSub Parent => Config.Service;

        public string StaticPath { get; internal set; }

        ///
        /// The corresponding static folder contents, can be null
        ///
        public Roll<StaticContent> Statics { get; }

        /// <summary>The default static file in the corresponding folder, can be null</summary>
        ///
        public StaticContent DefaultStatic { get; }

        // the argument makes state-passing more convenient
        protected WebSub(WebConfig cfg)
        {

            Config = cfg;

            // initialize build for the first time
            if (cfg.Service == null)
            {
                WebService svc = this as WebService;
                WebServiceConfig svccfg = cfg as WebServiceConfig;
                if (svc == null || svccfg == null)
                {
                    throw new InvalidOperationException("not a service class");
                }
                svccfg.Service = svc;
            }

            StaticPath = cfg.Parent == null ? Key : Path.Combine(Parent.StaticPath, Key);

            // load static files, if any
            if (StaticPath != null && Directory.Exists(StaticPath))
            {
                Statics = new Roll<StaticContent>(256);
                foreach (string path in Directory.GetFiles(StaticPath))
                {
                    string file = Path.GetFileName(path);
                    string ext = Path.GetExtension(path);
                    string ctype;
                    if (StaticContent.TryGetType(ext, out ctype))
                    {
                        byte[] content = File.ReadAllBytes(path);
                        DateTime modified = File.GetLastWriteTime(path);
                        StaticContent sta = new StaticContent
                        {
                            Key = file.ToLower(),
                            Type = ctype,
                            Buffer = content,
                            LastModified = modified
                        };
                        Statics.Add(sta);
                        if (sta.Key.StartsWith("default."))
                        {
                            DefaultStatic = sta;
                        }
                    }
                }
            }

            doers = new Roll<WebDoer>(32);

            Type type = GetType();

            // introspect doer methods
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                WebDoer doer = null;
                if (cfg.IsVar)
                {
                    if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(string))
                    {
                        doer = new WebDoer(this, mi, true);
                    }
                }
                else
                {
                    if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                    {
                        doer = new WebDoer(this, mi, false);
                    }
                }
                if (doer != null)
                {
                    if (doer.Key.Equals("default")) { defdoer = doer; }
                    doers.Add(doer);
                }
            }
        }

        public WebDoer GetDoer(String method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defdoer;
            }
            return doers[method];
        }

        public virtual void Do(string rsc, WebContext wc)
        {
            if (rsc.IndexOf('.') != -1) // static handling
            {
                StaticContent sta;
                if (Statics != null && Statics.TryGet(rsc, out sta))
                {
                    wc.Content = sta;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
            else // dynamic handling
            {
                WebDoer doer = string.IsNullOrEmpty(rsc) ? defdoer : GetDoer(rsc);
                if (doer == null)
                {
                    wc.StatusCode = 404;
                }
                else
                {
                    doer.Do(wc);
                }
            }
        }

        public virtual void Do(string rsc, WebContext wc, string var)
        {
            if (rsc.IndexOf('.') != -1) // static handling
            {
                StaticContent sta;
                if (Statics != null && Statics.TryGet(rsc, out sta))
                {
                    wc.Content = sta;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
            else // dynamic handling
            {
                WebDoer doer = string.IsNullOrEmpty(rsc) ? defdoer : GetDoer(rsc);
                if (doer == null)
                {
                    wc.StatusCode = 404;
                }
                else
                {
                    doer.Do(wc, var);
                }
            }
        }

        public virtual void @default(WebContext wc)
        {
            StaticContent sta = DefaultStatic;
            if (sta != null)
            {
                wc.Content = sta;
            }
            else
            {
                // send not implemented
                wc.Response.StatusCode = 404;
            }
        }

        public virtual void @default(WebContext wc, string var)
        {
            StaticContent sta = DefaultStatic;
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