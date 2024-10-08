﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ChainFX.Web
{
    /// <summary>
    /// A work is a virtual web folder that contains a single or collection of resources along with operations on it or them.
    /// </summary>
    public abstract class WebWork : IKeyable<string>
    {
        const string VARDIR = "_var_", VARPATHING = "<var>";

        // declared actions 
        readonly Map<string, WebAction> actions = new Map<string, WebAction>(32);

        // the catch action, can be null
        internal readonly WebAction @catch;

        // the default action, can be null
        readonly WebAction @default;

        // actions with ToolAttribute
        readonly WebAction[] tooled;

        // actions with TwinSpyAttribute
        readonly WebAction[] twinSpied;


        readonly Type type;

        //
        // object locator

        Hold[] cells;

        int size;

        // subworks, if any
        Map<string, WebWork> subWorks;

        // variable-key subwork, if any
        WebWork varWork;

        // to obtain a string key from a data object.
        protected WebWork()
        {
            type = GetType();

            Ui = (UiAttribute)type.GetCustomAttribute(typeof(UiAttribute), true);
            Authenticate = (AuthenticateAttribute)type.GetCustomAttribute(typeof(AuthenticateAttribute), false);
            Authorize = (AuthorizeAttribute)type.GetCustomAttribute(typeof(AuthorizeAttribute), false);

            // gather actions
            foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                var ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task)) async = true;
                else if (ret == typeof(void)) async = false;
                else continue;

                var pis = mi.GetParameters();
                WebAction act;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                {
                    act = new WebAction(this, mi, async, null);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) && pis[1].ParameterType == typeof(int))
                {
                    act = new WebAction(this, mi, async, pis[1].Name);
                }
                else
                {
                    continue;
                }

                actions.Add(act);
                if (act.Key == string.Empty)
                {
                    act.IsDefault = true;
                    @default = act;
                }
                if (act.Key == "catch")
                {
                    @catch = act;
                }
            }

            // gather tooled action methods
            var toollst = new ValueList<WebAction>(16);
            var spylst = new ValueList<WebAction>(8);
            for (int i = 0; i < actions.Count; i++)
            {
                var act = actions.ValueAt(i);
                if (act.Tool != null)
                {
                    toollst.Add(act);
                }
                if (act.Watch != null)
                {
                    spylst.Add(act);
                }
            }

            tooled = toollst.ToArray();
            twinSpied = spylst.ToArray();
        }


        public string Name { get; internal set; }

        public string Folder { get; internal set; }

        public UiAttribute Ui { get; internal set; }

        public AuthenticateAttribute Authenticate { get; internal set; }

        public AuthorizeAttribute Authorize { get; internal set; }

        public object State { get; set; }

        public string Header { get; set; }

        public string Label => Ui?.Label;

        public string Tip => Ui?.Tip;

        public string Icon => Ui?.Icon;

        public short Status => Ui?.Status ?? 0;

        public WebService Service { get; internal set; }

        public WebWork Parent { get; internal set; }

        public int Level { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string Pathing { get; internal set; }

        public Map<string, WebAction> Actions => actions;

        public WebAction[] Tooled => tooled;

        public WebAction[] TwinSpied => twinSpied;

        public WebAction Default => @default;

        public WebAction Catch => @catch;

        public Map<string, WebWork> SubWorks => subWorks;

        public WebWork VarWork => varWork;

        public bool HasVarWork => varWork != null;

        public WebAction this[string method] => string.IsNullOrEmpty(method) ? @default : actions[method];

        public WebAction this[int index] => actions.EntryAt(index).Value;

        // to resolve from the principal object.
        public Func<IData, string, object> Accessor { get; internal set; }

        public virtual string Key => Name;

        public bool IsOf(Type typ) => this.type == typ || typ.IsAssignableFrom(this.type);

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        public object GetAccessor(IData prin, string key)
        {
            return Accessor?.Invoke(prin, key);
        }


        protected internal virtual void OnCreate()
        {
        }

        /// <summary>
        /// Create and add a variable-key subwork.
        /// </summary>
        /// <param name="accessor">to resolve key from the principal object</param>
        /// <param name="state"> </param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authenticate"></param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The newly created subwork instance.</returns>
        /// <exception cref="ForbiddenException">Thrown if error</exception>
        protected T CreateVarWork<T>(Func<IData, string, object> accessor = null, object state = null, UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null) where T : WebWork, new()
        {
            var wrk = new T
            {
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = true,
                Directory = (Parent == null) ? VARDIR : Path.Combine(Parent.Directory, VARDIR),
                Pathing = Pathing + (accessor == null ? VARPATHING : string.Empty) + "/",
                Accessor = accessor,
                State = state
            };
            if (ui != null) wrk.Ui = ui;
            if (authenticate != null) wrk.Authenticate = authenticate;
            if (authorize != null) wrk.Authorize = authorize;

            varWork = wrk;

            wrk.OnCreate();
            return wrk;
        }

        /// <summary>
        /// Create and add a fixed-key subwork.
        /// </summary>
        /// <param name="name">the identifying name for the work</param>
        /// <param name="state"></param>
        /// <param name="ui">to override class-wise UI attribute</param>
        /// <param name="authenticate"></param>
        /// <param name="authorize">to override class-wise Authorize attribute</param>
        /// <param name="header"></param>
        /// <typeparam name="T">the type of work to create</typeparam>
        /// <returns>The newly created and subwork instance.</returns>
        /// <exception cref="ForbiddenException">Thrown if error</exception>
        protected T CreateWork<T>(string name, object state = null, UiAttribute ui = null, AuthenticateAttribute authenticate = null, AuthorizeAttribute authorize = null, string header = null) where T : WebWork, new()
        {
            if (subWorks == null)
            {
                subWorks = new Map<string, WebWork>();
            }

            var wrk = new T
            {
                Name = name,
                Service = Service,
                Parent = this,
                Level = Level + 1,
                IsVar = false,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
                Pathing = Pathing + name + "/",
                State = state,
                Header = header
            };
            if (ui != null) wrk.Ui = ui;
            if (authenticate != null) wrk.Authenticate = authenticate;
            if (authorize != null) wrk.Authorize = authorize;

            subWorks.Add(wrk);

            wrk.OnCreate();

            return wrk;
        }

        protected void GenerateRestDoc(HtmlBuilder h)
        {
            for (int i = 0; i < actions?.Count; i++)
            {
                var a = actions.ValueAt(i);
                if (a.Rests != null)
                {
                    h.ARTICLE_("uk-card uk-card-primary");
                    h.HEADER_("uk-card-header").TT(a.Pathing)._HEADER();
                    h.MAIN_("uk-card-body");

                    foreach (var c in a.Rests)
                    {
                        c.Render(h);
                    }

                    h._MAIN();
                    h._ARTICLE();
                }
            }

            varWork?.GenerateRestDoc(h);

            for (int i = 0; i < subWorks?.Count; i++)
            {
                var w = subWorks.EntryAt(i).Value;
                w.GenerateRestDoc(h);
            }
        }

        internal async Task<bool> DoAuthenticateAsync(WebContext wc, bool @default)
        {
            if (Authenticate != null)
            {
                if (Authenticate.OmitDefault && @default)
                {
                    return true;
                }
                if (Authenticate.IsAsync && !await Authenticate.DoAsync(wc) || !Authenticate.IsAsync && !Authenticate.Do(wc))
                {
                    return false;
                }
            }

            return true;
        }

        public bool DoAuthorize(WebContext wc, bool mock)
        {
            if (Authorize != null && wc.Principal != null)
            {
                var yes = Authorize.DoCheck(wc, out var super);
                if (yes && !mock)
                {
                    wc.Role = Authorize.Role;
                    wc.Super = super;
                }
                return yes;
            }

            return true;
        }


        internal bool HasNewNotice(int noticeId)
        {
            if (twinSpied != null)
            {
                foreach (var ntc in twinSpied)
                {
                    var n = ntc.Watch.Peek(noticeId);
                    if (n > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public override string ToString()
        {
            return Name;
        }

        public void Attach(object value, short flag = 0)
        {
            if (cells == null)
            {
                cells = new Hold[16];
            }

            cells[size++] = new Hold(value, flag);
        }


        public T Lookup<T>(short flag = 0) where T : class
        {
            if (cells != null)
            {
                for (int i = 0; i < size; i++)
                {
                    var c = cells[i];
                    if (c.Flag == 0 || (c.Flag & flag) > 0)
                    {
                        if (typeof(T).IsAssignableFrom(c.Typ))
                        {
                            return c.Value as T;
                        }
                    }
                }
            }
            return Parent?.Lookup<T>(flag);
        }


        /// <summary>
        /// A object holder in registry.
        /// </summary>
        class Hold
        {
            readonly short flag;
            readonly Type typ;

            readonly object value;

            internal Hold(object value, short flag)
            {
                this.typ = value.GetType();
                this.value = value;
                this.flag = flag;
            }

            public Type Typ => typ;

            public short Flag => flag;

            public object Value => value;
        }
    }
}