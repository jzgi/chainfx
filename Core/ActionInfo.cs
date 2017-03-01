using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The descriptor for an action method.
    ///
    public class ActionInfo : Nodule, IDoer
    {
        readonly Folder folder;

        readonly bool async;

        readonly bool arg;

        // void action(WebActionContext)
        readonly Action<ActionContext> @do;

        // async Task action(WebActionContext)
        readonly Func<ActionContext, Task> doasync;

        // void action(WebActionContext, string)
        readonly Action<ActionContext, string> do2;

        // async Task action(WebActionContext, string)
        readonly Func<ActionContext, string, Task> do2async;

        readonly StateAttribute state;

        internal ActionInfo(Folder folder, MethodInfo mi, bool async, bool arg) : base(mi.Name, mi)
        {
            this.folder = folder;
            this.async = async;
            this.arg = arg;

            if (async)
            {
                if (arg)
                {
                    do2async = (Func<ActionContext, string, Task>)mi.CreateDelegate(typeof(Func<ActionContext, string, Task>), folder);
                }
                else
                {
                    doasync = (Func<ActionContext, Task>)mi.CreateDelegate(typeof(Func<ActionContext, Task>), folder);
                }
            }
            else
            {
                if (arg)
                {
                    do2 = (Action<ActionContext, string>)mi.CreateDelegate(typeof(Action<ActionContext, string>), folder);
                }
                else
                {
                    @do = (Action<ActionContext>)mi.CreateDelegate(typeof(Action<ActionContext>), folder);
                }
            }

            // state
            var states = (StateAttribute[])mi.GetCustomAttributes(typeof(StateAttribute), false);
            if (states.Length > 0)
            {
                state = states[0];
            }
        }

        public Folder Folder => folder;

        public bool IsAsync => async;

        public bool HasArg => arg;

        public override Service Service => folder.Service;

        public StateAttribute State => state;

        internal void Do(ActionContext ac, String arg)
        {
            ac.Doer = this;
            // pre-
            DoBefore(ac);

            // invoke the right action method
            if (HasArg)
            {
                do2(ac, arg);
            }
            else
            {
                @do(ac);
            }

            // post-
            DoAfter(ac);
            ac.Doer = null;
        }

        internal async Task DoAsync(ActionContext ac, string arg)
        {
            ac.Doer = this;
            // pre-
            DoBefore(ac);

            // invoke the right action method
            if (HasArg)
            {
                await do2async(ac, arg);
            }
            else
            {
                await doasync(ac);
            }

            // post-
            DoAfter(ac);
            ac.Doer = null;
        }
    }
}