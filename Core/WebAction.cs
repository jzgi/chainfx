using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The descriptor for an action method.
    ///
    public class WebAction : WebNodule, IHandle
    {
        readonly WebFolder folder;

        readonly bool async;

        readonly bool arg;

        // void action(WebActionContext)
        readonly Action<WebActionContext> @do;

        // async Task action(WebActionContext)
        readonly Func<WebActionContext, Task> doasync;

        // void action(WebActionContext, string)
        readonly Action<WebActionContext, string> do2;

        // async Task action(WebActionContext, string)
        readonly Func<WebActionContext, string, Task> do2async;

        readonly StateAttribute state;

        internal WebAction(WebFolder folder, MethodInfo mi, bool async, bool arg) : base(mi.Name, mi)
        {
            this.folder = folder;
            this.async = async;
            this.arg = arg;

            if (async)
            {
                if (arg)
                {
                    do2async = (Func<WebActionContext, string, Task>)mi.CreateDelegate(typeof(Func<WebActionContext, string, Task>), folder);
                }
                else
                {
                    doasync = (Func<WebActionContext, Task>)mi.CreateDelegate(typeof(Func<WebActionContext, Task>), folder);
                }
            }
            else
            {
                if (arg)
                {
                    do2 = (Action<WebActionContext, string>)mi.CreateDelegate(typeof(Action<WebActionContext, string>), folder);
                }
                else
                {
                    @do = (Action<WebActionContext>)mi.CreateDelegate(typeof(Action<WebActionContext>), folder);
                }
            }

            // state
            var states = (StateAttribute[])mi.GetCustomAttributes(typeof(StateAttribute), false);
            if (states.Length > 0)
            {
                state = states[0];
            }
        }

        public WebFolder Folder => folder;

        public bool Async => async;

        public bool Arg => arg;

        public int Form => ui?.Form ?? 0;

        public int Dialog => ui?.Dialog ?? 0;

        public override WebService Service => folder.Service;

        public StateAttribute State => state;

        internal void Do(WebActionContext ac, String arg)
        {
            ac.Handle = this;
            // pre-
            DoBefore(ac);

            // invoke the right action method
            if (Arg)
            {
                do2(ac, arg);
            }
            else
            {
                @do(ac);
            }

            // post-
            DoAfter(ac);
            ac.Handle = null;
        }

        internal async Task DoAsync(WebActionContext ac, string arg)
        {
            ac.Handle = this;
            // pre-
            DoBefore(ac);

            // invoke the right action method
            if (Arg)
            {
                await do2async(ac, arg);
            }
            else
            {
                await doasync(ac);
            }

            // post-
            DoAfter(ac);
            ac.Handle = null;
        }
    }
}