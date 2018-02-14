using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor and doer for an action method.
    /// </summary>
    public class ActionDoer : Nodule, IDoer
    {
        readonly Work work;

        // relative path
        readonly string rpath;

        readonly bool async;

        readonly bool subscript;

        readonly int limit;

        // ui tool annotation
        internal readonly ToolAttribute tool;

        // state check annotation
        internal readonly StateAttribute state;

        // void action(ActionContext)
        readonly Action<WebContext> @do;

        // async Task action(ActionContext)
        readonly Func<WebContext, Task> doAsync;

        // void action(ActionContext, int)
        readonly Action<WebContext, int> do2;

        // async Task action(ActionContext, int)
        readonly Func<WebContext, int, Task> do2Async;

        internal ActionDoer(Work work, MethodInfo mi, bool async, bool subscript, int limit = 0) : base(
            mi.Name == "default" ? string.Empty : mi.Name,
            mi
        )
        {
            this.work = work;
            this.rpath = Key == string.Empty ? "./" : Key;
            this.async = async;
            this.subscript = subscript;
            this.limit = limit;

            this.tool = (ToolAttribute) mi.GetCustomAttribute(typeof(ToolAttribute), false);
            this.state = (StateAttribute) mi.GetCustomAttribute(typeof(StateAttribute), false);

            // create a doer delegate
            if (async)
            {
                if (subscript)
                {
                    do2Async = (Func<WebContext, int, Task>) mi.CreateDelegate(typeof(Func<WebContext, int, Task>), work);
                }
                else
                {
                    doAsync = (Func<WebContext, Task>) mi.CreateDelegate(typeof(Func<WebContext, Task>), work);
                }
            }
            else
            {
                if (subscript)
                {
                    do2 = (Action<WebContext, int>) mi.CreateDelegate(typeof(Action<WebContext, int>), work);
                }
                else
                {
                    @do = (Action<WebContext>) mi.CreateDelegate(typeof(Action<WebContext>), work);
                }
            }
        }

        public Work Work => work;

        public string RPath => rpath;

        public bool IsAsync => async;

        public bool HasSubscript => subscript;

        public bool HasTool => tool != null;

        public ToolAttribute Tool => tool;

        public int Limit => limit;

        public bool DoState(WebContext ac, object model)
        {
            return state == null || model == null || state.Check(ac, model);
        }

        internal void Do(WebContext ac, int subscpt)
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

        internal async Task DoAsync(WebContext ac, int subscpt)
        {
            // invoke the right action method
            if (HasSubscript)
            {
                await do2Async(ac, subscpt);
            }
            else
            {
                await doAsync(ac);
            }
        }
    }
}