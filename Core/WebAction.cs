using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// An action method.
    /// </summary>
    /// <param name="wc"></param>
    public delegate void ActionDoer(WebContext wc);

    public class WebAction : IMember
    {
        private readonly ActionDoer _doer;

        public string Key { get; }

        public WebSub Controller { get; }

        internal WebAction(WebSub controller, MethodInfo mi)
        {
            // NOTE: strict method name as key here to avoid the default base url trap
            Key = mi.Name;
            Controller = controller;
            _doer = (ActionDoer) mi.CreateDelegate(typeof(ActionDoer), controller);
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

    public delegate void ActionDoer<in TZone>(WebContext wc, TZone zone);


    public class WebAction<TZone> : IMember where TZone : IZone
    {
        private readonly ActionDoer<TZone> _doer;

        public string Key { get; }

        public WebSub<TZone> Controller { get; }

        internal WebAction(WebSub<TZone> controller, MethodInfo mi)
        {
            // NOTE: strict method name as key here to avoid the default base url trap
            Key = mi.Name;
            Controller = controller;
            _doer = (ActionDoer<TZone>) mi.CreateDelegate(typeof(ActionDoer), controller);
        }

        internal void Do(WebContext wc, TZone zone)
        {
            _doer(wc, zone);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}