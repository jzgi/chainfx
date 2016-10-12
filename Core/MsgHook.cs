using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A delegate of message handler methods.
    /// </summary>
    public delegate void Handler(MsgContext mc);

    public class MsgHook : IKeyed
    {
        public WebSub Controller { get; }

        public string Key { get; }

        readonly Handler handler;

        internal MsgHook(WebSub controller, MethodInfo mi)
        {
            Key = mi.Name;
            handler = (Handler)mi.CreateDelegate(typeof(Handler), controller);
        }

        internal void Handle(MsgContext mc)
        {
            handler(mc);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}