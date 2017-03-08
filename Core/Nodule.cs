using System;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain node of resources along the URi path.
    ///
    public abstract class Nodule : IRollable
    {
        protected static readonly AuthorizeException AuthorizeEx = new AuthorizeException();

        // name as appeared in the uri path
        readonly string name;

        internal UiAttribute ui;

        // access check
        internal AuthorizeAttribute authorize;

        // operation(s)
        readonly WorkAttribute work;

        // exception handling
        internal CatchAttribute @catch;

        internal Nodule(string name, ICustomAttributeProvider attrs)
        {
            this.name = name;

            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // ui
            var uis = (UiAttribute[])attrs.GetCustomAttributes(typeof(UiAttribute), false);
            if (uis.Length > 0)
            {
                ui = uis[0];
            }

            // authorize
            var auths = (AuthorizeAttribute[])attrs.GetCustomAttributes(typeof(AuthorizeAttribute), false);
            if (auths.Length > 0)
            {
                authorize = auths[0];
                authorize.Nodule = this;
            }

            // work
            var works = (WorkAttribute[])attrs.GetCustomAttributes(typeof(WorkAttribute), false);
            if (works.Length > 0)
            {
                work = works[0];
                work.Nodule = this;
            }

            // work
            var catches = (CatchAttribute[])attrs.GetCustomAttributes(typeof(CatchAttribute), false);
            if (catches.Length > 0)
            {
                @catch = catches[0];
                @catch.Nodule = this;
            }
        }

        public abstract Service Service { get; }

        public string Name => name;

        public UiAttribute Ui => ui;

        public AuthorizeAttribute Authorize => authorize;

        public WorkAttribute Filter => work;

        public CatchAttribute Catch => @catch;

        public string Label => ui?.Label ?? name;

        public bool HasUi => ui != null;

        public bool HasAuthorize => authorize != null;

        internal bool DoAuthorize(ActionContext ac)
        {
            if (authorize != null)
            {
                IData token = ac.Token;
                if (token == null)
                {
                    return false;
                }
                return authorize.Check(ac);
            }
            return true;
        }

        internal void DoBefore(ActionContext ac)
        {
            if (work != null && work.Before)
            {
                work.Work(ac);
            }
        }

        internal void DoAfter(ActionContext ac)
        {
            if (work != null && !work.Before)
            {
                work.Work(ac);
            }
        }

        internal void DoCatch(ActionContext ac, Exception ex)
        {
            if (@catch != null)
            {
                @catch.Catch(ac, ex);
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}