using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone
{
    /// <summary>
    /// The descriptor for an action method. A procedure public method should have one or two parameters, the first parameter must be of WebContext type.
    /// The second parameter, if presented, must be an int value.
    /// </summary>
    public class Actioner : Nodule
    {
        readonly Work work;

        // relative path
        readonly string rpath;

        readonly bool async;

        readonly bool subscript;

        // ui tool annotation
        internal readonly ToolAttribute tool;

        // state check annotation
        internal readonly StateAttribute state;

        // 4 possible forms of the action method
        readonly Action<WebContext> @do;
        readonly Func<WebContext, Task> doAsync;
        readonly Action<WebContext, int> do2;
        readonly Func<WebContext, int, Task> do2Async;

        internal Actioner(Work work, MethodInfo mi, bool async, bool subscript) : base(mi.Name == "default" ? string.Empty : mi.Name, mi)
        {
            this.work = work;
            this.rpath = Key == string.Empty ? "./" : Key;
            this.async = async;
            this.subscript = subscript;

            this.tool = (ToolAttribute) mi.GetCustomAttribute(typeof(ToolAttribute), true);
            this.state = (StateAttribute) mi.GetCustomAttribute(typeof(StateAttribute), true);

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

        public string RealPath => rpath;

        public bool IsAsync => async;

        public bool HasSubscript => subscript;


        public bool HasTool => tool != null;

        public ToolAttribute Tool => tool;

        public bool CheckState(WebContext wc, object[] stack, int level)
        {
            return state == null || stack == null || state.Check(wc, stack, level);
        }

        internal void Do(WebContext wc, int subscpt)
        {
            if (HasSubscript)
            {
                do2(wc, subscpt);
            }
            else
            {
                @do(wc);
            }
        }

        internal async Task DoAsync(WebContext wc, int subscpt)
        {
            // invoke the right procedure method
            if (HasSubscript)
            {
                await do2Async(wc, subscpt);
            }
            else
            {
                await doAsync(wc);
            }
        }
    }
}