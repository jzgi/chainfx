using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// 
    /// The descriptor of a event handler method that handles a loosely-coupled event.
    /// 
    public class WebEvent : IRollable
    {
        public WebService Service { get; }

        readonly string name;

        readonly bool async;

        // void event(WebEventContext)
        readonly Action<WebEventContext> @do;

        // async Task event(WebEventContext)
        readonly Func<WebEventContext, Task> doasync;

        internal WebEvent(WebService service, MethodInfo mi, bool async)
        {
            this.name = mi.Name;
            this.async = async;
            if (async)
            {
                doasync = (Func<WebEventContext, Task>)mi.CreateDelegate(typeof(Func<WebEventContext, Task>), service);

            }
            else
            {
                @do = (Action<WebEventContext>)mi.CreateDelegate(typeof(Action<WebEventContext>), service);
            }
        }

        public string Name => name;

        public bool Async => async;

        internal void Do(WebEventContext ec)
        {
            @do(ec);
        }

        internal async Task DoAsync(WebEventContext ec)
        {
            await doasync(ec);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}