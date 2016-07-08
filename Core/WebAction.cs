using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// An action method.
    /// </summary>
    /// <param name="wc"></param>
    public delegate void Actor(WebContext wc);

    public delegate void Actor<Z>(Z zone, WebContext wc);

    public class WebAction : IMember
    {
        readonly WebSub sub;

        readonly string key;

        readonly Actor actor;

        internal WebAction(WebSub sub, MethodInfo mi)
        {
            this.sub = sub;
            this.actor = (Actor) mi.CreateDelegate(typeof(Actor), sub);
            // NOTE: strict method name as key here to avoid the default base url trap
            key = mi.Name;
        }

        public WebSub Sub => sub;

        public string Key => key;

        internal void Do(WebContext wc)
        {
            actor(wc);
        }

        public override string ToString()
        {
            return key;
        }
    }

    public class WebAction<Z> : IMember where Z : IZone
    {
        readonly string _key;

        readonly WebSub<Z> _sub;

        Actor<Z> _actor;

        internal WebAction(WebSub<Z> sub, MethodInfo mi)
        {
            _sub = sub;
            _actor = (Actor<Z>) mi.CreateDelegate(typeof(Actor), sub);
            // NOTE: strict method name as key here to avoid the default base url trap
            _key = mi.Name;
        }

        public WebSub<Z> Sub => _sub;

        public string Key => _key;

        internal void Do(WebContext wc, Z zone)
        {
            _actor(zone, wc);
        }

        public override string ToString()
        {
            return _key;
        }
    }
}