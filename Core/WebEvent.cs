using System;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor of a message hook.
    /// <summary>
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

        internal void Do(WebEventContext mc)
        {
            doer(mc);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}