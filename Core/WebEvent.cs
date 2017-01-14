using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// 
    /// The descriptor of a event handler method in a service.
    /// 
    public class WebEvent : IRollable
    {
        public WebService Service { get; }

        public string Name { get; }

        // void event(WebEventContext)
        readonly Action<WebEventContext> doer;

        // async Task event(WebEventContext)
        readonly Func<WebEventContext, Task> func;

        internal WebEvent(WebService service, MethodInfo mi)
        {
            Name = mi.Name;
            doer = (Action<WebEventContext>)mi.CreateDelegate(typeof(Action<WebEventContext>), service);
        }

        internal void Do(WebEventContext ec)
        {
            doer(ec);
        }

        internal async Task DoAsync(WebEventContext ec)
        {
            await func(ec);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}