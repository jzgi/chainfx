using System;
using System.Reflection;

namespace Greatbone.Core
{
    /// 
    /// The descriptor of a event handler method in a service.
    /// 
    public class WebEvent : IRollable
    {
        public WebService Service { get; }

        public string Name { get; }

        readonly Action<WebEventContext> doer;

        internal WebEvent(WebService service, MethodInfo mi)
        {
            Name = mi.Name;
            doer = (Action<WebEventContext>)mi.CreateDelegate(typeof(Action<WebEventContext>), service);
        }

        internal void Do(WebEventContext ec)
        {
            doer(ec);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}