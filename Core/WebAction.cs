using System;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// The descriptor for an action method.
    ///
    public class WebAction : WebAccess, IRollable
    {
        readonly WebDirectory directory;

        readonly string name;

        readonly Action<WebActionContext> doer;

        readonly CheckAttribute[] checks;

        // if requires auth through header or cookie
        readonly bool header, cookie;

        readonly UiAttribute ui;

        internal WebAction(WebDirectory dir, MethodInfo mi) : base(mi)
        {
            directory = dir;
            name = mi.Name;
            doer = (Action<WebActionContext>)mi.CreateDelegate(typeof(Action<WebActionContext>), dir);
        }

        public WebDirectory Directory => directory;

        public string Name => name;

        public bool GET => ui?.GET ?? false;

        public string Label => ui?.Label ?? name;

        public string Icon => ui?.Icon;

        public int Dialog => ui?.Dialog ?? 3;

        // for generating unique digest nonce
        const string PrivateKey = "3e43a7180";

        internal void Do(WebActionContext ac)
        {
            ac.Action = this;
            if (Check(ac)) // authorize check
            {
                // pre-
                BeforeDo(ac);
                // invoke the action method
                doer(ac);
                // post-
                AfterDo(ac);
            }
            ac.Action = null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}