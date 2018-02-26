﻿using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor for a procedure method. A procedure public method should have one or two parameters, the first parameter must be of WebContext type.
    /// The second parameter, if presented, must be an int value.
    /// </summary>
    public class Procedure : Nodule
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

        // void procedure(WebContext)
        readonly Action<WebContext> @do;

        // async Task procedure(WebContext)
        readonly Func<WebContext, Task> doAsync;

        // void procedure(WebContext, int)
        readonly Action<WebContext, int> do2;

        // async Task procedure(WebContext, int)
        readonly Func<WebContext, int, Task> do2Async;

        internal Procedure(Work work, MethodInfo mi, bool async, bool subscript, int limit = 0) : base(
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

        public bool DoState(WebContext wc, object model)
        {
            return state == null || model == null || state.Check(wc, model);
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