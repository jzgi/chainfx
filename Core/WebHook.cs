using System;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor of a message hook.
    /// <summary>
    public class WebHook : IKeyed
    {
        public WebService Service { get; }

        public string Key { get; }

        readonly Action<WebEvent> doer;

        internal WebHook(WebService service, MethodInfo mi)
        {
            Key = mi.Name;
            doer = (Action<WebEvent>)mi.CreateDelegate(typeof(Action<WebEvent>), service);
        }

        internal void Do(WebEvent mc)
        {
            doer(mc);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}