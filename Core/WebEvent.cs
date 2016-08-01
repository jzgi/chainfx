using System.Reflection;

namespace Greatbone.Core
{
    public delegate void EventDoer(WebContext wc);

    public class WebEvent : IMember
    {
        private readonly EventDoer _doer;

        public string Key { get; }

        public WebSub Controller { get; }

        internal WebEvent(WebSub controller, MethodInfo mi)
        {
            // NOTE: strict method nzame as key here to avoid the default base url trap
            Key = mi.Name;
            Controller = controller;
            _doer = (EventDoer) mi.CreateDelegate(typeof(ActionDoer), controller);
        }

        internal void Do(WebContext wc)
        {
            _doer(wc);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}