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

        readonly bool subscpt;

        // void action(ActionContext)
        readonly Action<ActionContext> @do;

        // async Task action(ActionContext)
        readonly Func<ActionContext, Task> doasync;

        // void action(ActionContext, int)
        readonly Action<ActionContext, int> do2;

        // async Task action(ActionContext, int)
        readonly Func<ActionContext, int, Task> do2async;

        readonly StateAttribute state;

        internal ActionInfo(Folder folder, MethodInfo mi, bool async, bool subscpt) : base(mi.Name, mi)
        {
            this.folder = folder;
            this.async = async;
            this.subscpt = subscpt;

            if (async)
            {
                if (subscpt)
                {
                    do2async = (Func<ActionContext, int, Task>)mi.CreateDelegate(typeof(Func<ActionContext, int, Task>), folder);
                }
                else
                {
                    doasync = (Func<ActionContext, Task>)mi.CreateDelegate(typeof(Func<ActionContext, Task>), folder);
                }
            }
            else
            {
                if (subscpt)
                {
                    do2 = (Action<ActionContext, int>)mi.CreateDelegate(typeof(Action<ActionContext, int>), folder);
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

        public bool HasSubscpt => subscpt;

        public override Service Service => folder.Service;

        public StateAttribute State => state;

        internal void Do(ActionContext ac, int subscpt)
        {
            if (HasSubscpt)
            {
                do2(ac, subscpt);
            }
            else
            {
                @do(ac);
            }
        }

        internal async Task DoAsync(ActionContext ac, int subscpt)
        {
            // invoke the right action method
            if (HasSubscpt)
            {
                await do2async(ac, subscpt);
            }
            else
            {
                await doasync(ac);
            }
        }
    }
}