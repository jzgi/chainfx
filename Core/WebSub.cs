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
        readonly Roll<WebAction> actions;

        // the default doer
        readonly WebAction defaction;

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key { get; internal set; }

        public bool IsVar { get; internal set; }

        /// <summary>The service that this controller resides in.</summary>
        ///
        public WebService Service { get; internal set; }

        public WebSub Parent { get; internal set; }

        public string StaticPath { get; internal set; }

        ///
        /// The corresponding static folder contents, can be null
        ///
        public Roll<StaticContent> Statics { get; }

        /// <summary>The default static file in the corresponding folder, can be null</summary>
        ///
        public StaticContent DefaultStatic { get; }

        // the argument makes state-passing more convenient
        protected WebSub(ITie tie)
        {
            // adjust
            if (tie.Service == null)
            {
                WebService svc = this as WebService;
                if (svc == null) { throw new WebException("not a WebService"); }
                WebConfig cfg = tie as WebConfig;
                if (cfg == null) { throw new WebException("not a WebConfig"); }
                cfg.Service = svc;
            }

            // initialize
            Key = tie.Key;
            IsVar = tie.IsVar;
            Service = tie.Service;
            Parent = tie.Parent;

            StaticPath = tie.Parent == null ? Key : Path.Combine(Parent.StaticPath, Key);

            // load static files, if any
            if (StaticPath != null && Directory.Exists(StaticPath))
            {
                Statics = new Roll<StaticContent>(256);
                foreach (string path in Directory.GetFiles(StaticPath))
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
                        Statics.Add(sta);
                        if (sta.Key.StartsWith("default."))
                        {
                            DefaultStatic = sta;
                        }
                    }
                }
            }

            actions = new Roll<WebAction>(32);

            Type typ = GetType();

            // introspect doer methods
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                WebAction a = null;
                if (tie.IsVar)
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

        public WebAction GetAction(String method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return defaction;
            }
            return actions[method];
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
                WebAction a = string.IsNullOrEmpty(rsc) ? defaction : GetAction(rsc);
                if (a == null)
                {
                    wc.StatusCode = 404;
                }
                else
                {
                    a.Do(wc);
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
                WebAction a = string.IsNullOrEmpty(rsc) ? defaction : GetAction(rsc);
                if (a == null)
                {
                    wc.StatusCode = 404;
                }
                else
                {
                    a.Do(wc, var);
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
                wc.StatusCode = 404;
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