using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// A work is a virtual web folder that contains a single or collection of resources along with operations on it or them.
    /// A work can contain child/sub works.
    ///
    public abstract class Work : Nodule
    {
        internal static readonly AuthorizeException AuthorizeEx = new AuthorizeException();

        // max nesting levels
        const int MaxNesting = 6;

        // underlying file directory name
        const string _VAR_ = "VAR";

        // state-passing
        readonly WorkContext wc;

        readonly string major;

        readonly short minor;

        // declared actions 
        readonly Roll<ActionInfo> actions;

        // the default action, can be null
        readonly ActionInfo @default;

        // the underscore action, can be null. 
        readonly ActionInfo under;

        // the goto action, can be null
        readonly ActionInfo @goto;

        // actions with Ui attribute
        readonly ActionInfo[] uiactions;

        // child works, if any
        internal Roll<Work> children;

        // the variable-key subwork, if any
        internal Work varsub;

        internal Func<IData, string> varkeyer;

        protected Work(WorkContext wc) : base(wc.Name, null)
        {
            this.wc = wc;
            // separate major and minor name parts
            int dash = Name.IndexOf('-');
            if (dash != -1)
            {
                major = Name.Substring(0, dash);
                minor = Name.Substring(dash + 1).ToShort();
            }
            else
            {
                major = Name;
            }

            // gather actions
            actions = new Roll<ActionInfo>(32);
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                Type ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task)) async = true;
                else if (ret == typeof(void)) async = false;
                else continue;

                ParameterInfo[] pis = mi.GetParameters();
                ActionInfo ai = null;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(ActionContext))
                {
                    ai = new ActionInfo(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(ActionContext) && pis[1].ParameterType == typeof(int))
                {
                    ai = new ActionInfo(this, mi, async, true);
                }
                else continue;

                actions.Add(ai);
                if (ai.Name.Equals("default")) { @default = ai; }
                if (ai.Name.Equals("_")) { under = ai; }
                if (ai.Name.Equals("goto")) { @goto = ai; }
            }
            if (@default == null) @default = under;

            // to override annotated attributes
            if (wc.Ui != null)
            {
                ui = wc.Ui;
            }
            if (wc.Authorize != null)
            {
                authorize = wc.Authorize;
            }

            // preprocess start-end annotations
            AuthorizeAttribute start = null;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo ai = actions[i];
                AuthorizeAttribute auth = ai.authorize;

                if (start != null)
                {
                    if (auth == null) ai.authorize = start;
                    else auth.Or(start);
                }

                if (auth != null && auth.Start)
                {
                    auth.Start = false;
                    start = auth;
                }
                if (auth != null && auth.End)
                {
                    auth.End = false;
                    start = null;
                }
            }

            // gather ui actions
            List<ActionInfo> uias = null;
            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo a = actions[i];
                if (a.HasUi)
                {
                    if (uias == null) uias = new List<ActionInfo>();
                    uias.Add(a);
                }
            }
            uiactions = uias?.ToArray();
        }

        ///
        /// Create a child work.
        ///
        public W Create<W>(string name, UiAttribute ui = null, AuthorizeAttribute auth = null) where W : Work
        {
            if (Level >= MaxNesting)
            {
                throw new ServiceException("allowed work nesting " + MaxNesting);
            }

            if (children == null)
            {
                children = new Roll<Work>(16);
            }
            // create instance by reflection
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WorkContext) });
            if (ci == null)
            {
                throw new ServiceException(typ + " missing WorkContext");
            }
            WorkContext wc = new WorkContext(name)
            {
                Ui = ui,
                Authorize = auth,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? name : Path.Combine(Parent.Directory, name),
                Service = Service
            };
            W work = (W)ci.Invoke(new object[] { wc });
            children.Add(work);

            return work;
        }

        ///
        /// Create a variable-key subwork.
        ///
        public W CreateVar<W>(Func<IData, string> keyer = null, UiAttribute ui = null, AuthorizeAttribute auth = null) where W : Work, IVar
        {
            if (Level >= MaxNesting)
            {
                throw new ServiceException("allowed work nesting " + MaxNesting);
            }

            // create instance
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WorkContext) });
            if (ci == null)
            {
                throw new ServiceException(typ + " missing WorkContext");
            }
            WorkContext wc = new WorkContext(_VAR_)
            {
                Ui = ui,
                Authorize = auth,
                Parent = this,
                Level = Level + 1,
                Directory = (Parent == null) ? _VAR_ : Path.Combine(Parent.Directory, _VAR_),
                Service = Service
            };
            W work = (W)ci.Invoke(new object[] { wc });
            varkeyer = keyer;
            varsub = work;
            return work;
        }

        public string Major => major;

        public short Minor => minor;

        public Roll<ActionInfo> Actions => actions;

        public ActionInfo[] UiActions => uiactions;

        public bool HasDefault => @default != null;

        public bool HasUnder => under != null;

        public bool HasGoto => @goto != null;

        public Roll<Work> Children => children;

        public Work VarSub => varsub;

        public Func<IData, string> VarKeyer => varkeyer;

        public string Directory => wc.Directory;

        public Work Parent => wc.Parent;

        public int Level => wc.Level;

        public override Service Service => wc.Service;

        internal void Describe(XmlContent cont)
        {
            cont.ELEM(Name,
            delegate
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    ActionInfo act = Actions[i];
                    cont.Put(act.Name, "");
                }
            },
            delegate
            {
                if (children != null)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        Work work = children[i];
                        work.Describe(cont);
                    }
                }
                if (varsub != null)
                {
                    varsub.Describe(cont);
                }
            });
        }

        public ActionInfo GetAction(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                return @default;
            }
            return actions[method];
        }

        internal Work Resolve(ref string relative, ActionContext ac, ref bool recover)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) { return this; }

            // seek as child/sub work
            string key = relative.Substring(0, slash);
            relative = relative.Substring(slash + 1); // adjust relative
            Work work;
            if (children != null && children.TryGet(key, out work)) // if child
            {
                ac.Chain(key, work);
                return work.Resolve(ref relative, ac, ref recover);
            }
            if (varsub != null) // if variable-key sub
            {
                if (key.Length == 0 && varkeyer != null) // resolve varkey
                {
                    if (ac.Principal == null) throw AuthorizeEx;
                    if ((key = varkeyer(ac.Principal)) == null)
                    {
                        if (@goto != null) { recover = true; }
                        return null;
                    }
                }
                ac.Chain(key, varsub);
                return varsub.Resolve(ref relative, ac, ref recover);
            }
            return null;
        }

        internal async Task HandleAsync(string rsc, ActionContext ac)
        {
            ac.Work = this;

            // access check 
            if (!DoAuthorize(ac)) throw AuthorizeEx;

            // pre-
            BeforeAttribute bef = Before;
            if (bef != null) { if (bef.IsAsync) await bef.DoAsync(ac); else bef.Do(ac); }

            int dot = rsc.LastIndexOf('.');
            if (dot != -1) // file
            {
                // try in cache 

                DoFile(rsc, rsc.Substring(dot), ac);
            }
            else // action
            {
                string name = rsc;
                int subscpt = 0;
                int dash = rsc.LastIndexOf('-');
                if (dash != -1)
                {
                    name = rsc.Substring(0, dash);
                    ac.Subscript = subscpt = rsc.Substring(dash + 1).ToInt();
                }
                ActionInfo act = string.IsNullOrEmpty(name) ? @default : GetAction(name);
                if (act == null)
                {
                    ac.Give(404); // not found
                    return;
                }

                ac.Doer = act;

                // access check
                if (!act.DoAuthorize(ac)) throw AuthorizeEx;

                // try in cache

                BeforeAttribute actbef = act.Before;
                if (actbef != null) { if (actbef.IsAsync) await actbef.DoAsync(ac); else actbef.Do(ac); }

                // method invocation
                if (act.IsAsync)
                {
                    await act.DoAsync(ac, subscpt); // invoke action method
                }
                else
                {
                    act.Do(ac, subscpt);
                }

                AfterAttribute actaft = act.After;
                if (actaft != null) { if (actaft.IsAsync) await actaft.DoAsync(ac); else actaft.Do(ac); }

                ac.Doer = null;
            }

            // post-
            AfterAttribute aft = After;
            if (aft != null) { if (aft.IsAsync) await aft.DoAsync(ac); else aft.Do(ac); }

            ac.Work = null;
        }

        public void DoFile(string filename, ActionContext ac)
        {
            int dot = filename.LastIndexOf('.');
            DoFile(filename, filename.Substring(dot), ac);
        }

        void DoFile(string filename, string ext, ActionContext ac)
        {
            if (filename.StartsWith("$")) // private resource
            {
                ac.Give(403); // forbidden
                return;
            }

            string ctyp;
            if (!StaticContent.TryGetType(ext, out ctyp))
            {
                ac.Give(415); // unsupported media type
                return;
            }

            string path = Path.Combine(Directory, filename);
            if (!File.Exists(path))
            {
                ac.Give(404); // not found
                return;
            }

            DateTime modified = File.GetLastWriteTime(path);
            DateTime? since = ac.HeaderDateTime("If-Modified-Since");
            if (since != null && modified <= since)
            {
                ac.Give(304); // not modified
                return;
            }

            // load file content
            byte[] bytes = File.ReadAllBytes(path);
            StaticContent cont = new StaticContent(true, bytes, bytes.Length)
            {
                Name = filename,
                Type = ctyp,
                Modified = modified
            };
            ac.Give(200, cont, true, 3600 * 12);
        }
    }
}