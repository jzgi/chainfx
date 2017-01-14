using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    /// The descriptor for an action method.
    ///
    public class WebAction : WebControl, IRollable
    {
        readonly WebFolder folder;

        readonly string name;

        readonly bool async;

        // void action(WebActionContext)
        readonly Action<WebActionContext> doer;

        // async Task action(WebActionContext)
        readonly Func<WebActionContext, Task> func;

        // void action(WebActionContext, string)
        readonly Action<WebActionContext, string> doer2;

        // async Task action(WebActionContext, string)
        readonly Func<WebActionContext, string, Task> func2;

        readonly UiAttribute ui;

        internal WebAction(WebFolder folder, MethodInfo mi, bool arg = false) : base(mi)
        {
            this.folder = folder;
            name = mi.Name;
            async = mi.ReturnType == typeof(Task);
            if (arg)
            {
                if (async)
                {
                    func2 = (Func<WebActionContext, string, Task>)mi.CreateDelegate(typeof(Func<WebActionContext, string, Task>), folder);
                }
                else
                {
                    doer2 = (Action<WebActionContext, string>)mi.CreateDelegate(typeof(Action<WebActionContext, string>), folder);
                }
            }
            else
            {
                if (async)
                {
                    func = (Func<WebActionContext, Task>)mi.CreateDelegate(typeof(Func<WebActionContext, Task>), folder);
                }
                else
                {
                    doer = (Action<WebActionContext>)mi.CreateDelegate(typeof(Action<WebActionContext>), folder);
                }
            }

            // initialize ui
            var uis = (UiAttribute[])mi.GetCustomAttributes(typeof(UiAttribute), false);
            if (uis.Length > 0) ui = uis[0];
        }

        public WebFolder Folder => folder;

        public string Name => name;

        public bool Async => async;

        public bool Arg => doer2 != null;

        public int Form => ui?.Form ?? 0;

        public string Label => ui?.Label ?? name;

        public string Icon => ui?.Icon;

        public int Dialog => ui?.Dialog ?? 0;

        public override WebService Service => folder.Service;

        public UiAttribute Ui => ui;

        internal void Do(WebActionContext ac, String arg)
        {
            ac.Action = this;
            // pre-
            DoBefore(ac);

            // invoke the right action method
            if (Arg)
            {
                doer2(ac, arg);
            }
            else
            {
                doer(ac);
            }

            // post-
            DoAfter(ac);
            ac.Action = null;
        }

        internal async Task DoAsync(WebActionContext ac, String arg)
        {
            ac.Action = this;
            // pre-
            DoBefore(ac);

            // invoke the right action method
            if (Arg)
            {
                await func2(ac, arg);
            }
            else
            {
                await func(ac);
            }

            // post-
            DoAfter(ac);
            ac.Action = null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}