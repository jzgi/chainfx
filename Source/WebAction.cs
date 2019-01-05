using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Greatbone;

namespace Greatbone.Service
{
    /// <summary>
    /// The descriptor for an action method. An action method should have one or two parameters, the first parameter must be of WebContext type.
    /// The second parameter, if presented, must be an int value.
    /// </summary>
    public sealed class WebAction : WebTarget
    {
        readonly WebWork work;

        // relative path
        readonly string relative;

        readonly bool async;

        // the name of subscript parameter (the second parameter), may be null
        readonly string subscript;

        // the action pathing 
        readonly string pathing;

        // tool annotation for ui
        internal readonly ToolAttribute tool;

        // state check annotation
        internal readonly StateAttribute state;

        // 4 possible forms of the action method
        readonly Action<WebContext> @do;
        readonly Func<WebContext, Task> doAsync;
        readonly Action<WebContext, int> do2;
        readonly Func<WebContext, int, Task> do2Async;

        readonly List<TagAttribute> comments;

        internal WebAction(WebWork work, MethodInfo mi, bool async, string subscript) : base(mi.Name == "default" ? string.Empty : mi.Name, mi)
        {
            this.work = work;
            this.relative = Key == string.Empty ? "./" : Key;
            this.async = async;
            this.subscript = subscript;

            this.tool = (ToolAttribute) mi.GetCustomAttribute(typeof(ToolAttribute), true);
            this.state = (StateAttribute) mi.GetCustomAttribute(typeof(StateAttribute), true);

            // create a doer delegate
            if (async)
            {
                if (HasSubscript)
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
                if (HasSubscript)
                {
                    do2 = (Action<WebContext, int>) mi.CreateDelegate(typeof(Action<WebContext, int>), work);
                }
                else
                {
                    @do = (Action<WebContext>) mi.CreateDelegate(typeof(Action<WebContext>), work);
                }
            }

            // comments
            foreach (var m in mi.GetCustomAttributes())
            {
                if (m is TagAttribute c)
                {
                    if (comments == null) comments = new List<TagAttribute>(4);
                    comments.Add(c);
                }
            }

            // resolve the action pathing
            StringBuilder sb = new StringBuilder(work.Pathing);
            sb.Append(Key);
            if (HasSubscript)
            {
                sb.Append('-').Append('<').Append(subscript).Append('>');
            }
            pathing = sb.ToString();
        }

        public WebWork Work => work;

        public string Relative => relative;

        public bool IsAsync => async;

        public bool HasSubscript => subscript != null;

        public string Pathing => pathing;

        public bool HasTool => tool != null;

        public ToolAttribute Tool => tool;

        public List<TagAttribute> Comments => comments;

        public bool CheckState(WebContext wc, object[] stack, int level)
        {
            return state == null || stack == null || state.Check(wc, stack, level);
        }

        internal void Do(WebContext wc, int subscript)
        {
            if (HasSubscript)
            {
                do2(wc, subscript);
            }
            else
            {
                @do(wc);
            }
        }

        // invoke the right method
        internal async Task DoAsync(WebContext wc, int subscript)
        {
            if (HasSubscript)
            {
                await do2Async(wc, subscript);
            }
            else
            {
                await doAsync(wc);
            }
        }
    }
}