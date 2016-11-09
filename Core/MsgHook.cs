using System;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor of a message hook.
    /// <summary>
    public class MsgHook : IKeyed
    {
        public WebServiceWork Service { get; }

        public string Key { get; }

        readonly Action<MsgContext> doer;

        internal MsgHook(WebServiceWork service, MethodInfo mi)
        {
            Key = mi.Name;
            doer = (Action<MsgContext>)mi.CreateDelegate(typeof(Action<MsgContext>), service);
        }

        internal void Do(MsgContext mc)
        {
            doer(mc);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}