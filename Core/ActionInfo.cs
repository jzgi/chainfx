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
        readonly Work work;

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

        internal ActionInfo(Work work, MethodInfo mi, bool async, bool subscpt) : base(mi.Name, mi)
        {
            this.work = work;
            this.async = async;
            this.subscpt = subscpt;

            if (async)
            {
                if (subscpt)
                {
                    do2async = (Func<ActionContext, int, Task>)mi.CreateDelegate(typeof(Func<ActionContext, int, Task>), work);
                }
                else
                {
                    doasync = (Func<ActionContext, Task>)mi.CreateDelegate(typeof(Func<ActionContext, Task>), work);
                }
            }
            else
            {
                if (subscpt)
                {
                    do2 = (Action<ActionContext, int>)mi.CreateDelegate(typeof(Action<ActionContext, int>), work);
                }
                else
                {
                    @do = (Action<ActionContext>)mi.CreateDelegate(typeof(Action<ActionContext>), work);
                }
            }

            // state
            var states = (StateAttribute[])mi.GetCustomAttributes(typeof(StateAttribute), false);
            if (states.Length > 0)
            {
                state = states[0];
            }
        }

        public Work Work => work;

        public bool IsAsync => async;

        public bool HasSubscpt => subscpt;

        public override Service Service => work.Service;

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