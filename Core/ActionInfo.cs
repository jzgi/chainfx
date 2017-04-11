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

        readonly bool subscript;

        // void action(ActionContext)
        readonly Action<ActionContext> @do;

        // async Task action(ActionContext)
        readonly Func<ActionContext, Task> doasync;

        // void action(ActionContext, int)
        readonly Action<ActionContext, int> do2;

        // async Task action(ActionContext, int)
        readonly Func<ActionContext, int, Task> do2async;

        internal ActionInfo(Work work, MethodInfo mi, bool async, bool subscript) : base(mi.Name, mi)
        {
            this.work = work;
            this.async = async;
            this.subscript = subscript;

            if (async)
            {
                if (subscript)
                {
                    do2async = (Func<ActionContext, int, Task>) mi.CreateDelegate(typeof(Func<ActionContext, int, Task>), work);
                }
                else
                {
                    doasync = (Func<ActionContext, Task>) mi.CreateDelegate(typeof(Func<ActionContext, Task>), work);
                }
            }
            else
            {
                if (subscript)
                {
                    do2 = (Action<ActionContext, int>) mi.CreateDelegate(typeof(Action<ActionContext, int>), work);
                }
                else
                {
                    @do = (Action<ActionContext>) mi.CreateDelegate(typeof(Action<ActionContext>), work);
                }
            }
        }

        public Work Work => work;

        public bool IsAsync => async;

        public bool HasSubscript => subscript;

        public override Service Service => work.Service;

        internal void Do(ActionContext ac, int subscpt)
        {
            if (HasSubscript)
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
            if (HasSubscript)
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