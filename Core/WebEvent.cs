using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// 
    /// The descriptor of an event handler method.
    /// 
    public class WebEvent : IHandler
    {
        readonly WebService service;

        readonly string key;

        readonly bool async;

        readonly bool arg;

        // void event(WebEventContext)
        readonly Action<WebEventContext> @do;

        // async Task event(WebEventContext)
        readonly Func<WebEventContext, Task> doasync;

        // void action(WebActionContext, string)
        readonly Action<WebEventContext, string> do2;

        // async Task action(WebActionContext, string)
        readonly Func<WebEventContext, string, Task> do2async;

        internal WebEvent(WebService service, MethodInfo mi, bool async, bool arg)
        {
            this.key = mi.Name;
            this.async = async;
            this.arg = arg;

            if (async)
            {
                if (arg)
                {
                    do2async = (Func<WebEventContext, string, Task>)mi.CreateDelegate(typeof(Func<WebEventContext, string, Task>), service);
                }
                else
                {
                    doasync = (Func<WebEventContext, Task>)mi.CreateDelegate(typeof(Func<WebEventContext, Task>), service);
                }
            }
            else
            {
                if (arg)
                {
                    do2 = (Action<WebEventContext, string>)mi.CreateDelegate(typeof(Action<WebEventContext, string>), service);
                }
                else
                {
                    @do = (Action<WebEventContext>)mi.CreateDelegate(typeof(Action<WebEventContext>), service);
                }
            }
        }

        public WebService Service => service;

        public string Name => key;

        public bool IsAsync => async;

        public bool HasArg => arg;

        // invoke the right event method
        internal void Do(WebEventContext ec, string arg)
        {
            if (HasArg)
            {
                do2(ec, arg);
            }
            else
            {
                @do(ec);
            }
        }

        // invoke the right event method
        internal async Task DoAsync(WebEventContext ec, string arg)
        {
            if (HasArg)
            {
                await do2async(ec, arg);
            }
            else
            {
                await doasync(ec);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}