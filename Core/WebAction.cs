using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// An action method.
    /// </summary>
    /// <param name="wc"></param>
    public delegate void Actor(WebContext wc);

    public class WebAction : IMember
    {
        private readonly WebSub _control;

        private readonly string _key;

        private readonly Actor _actor;

        internal WebAction(WebSub control, MethodInfo mi)
        {
            _control = control;
            _actor = (Actor) mi.CreateDelegate(typeof(Actor), control);
            // NOTE: strict method name as key here to avoid the default base url trap
            _key = mi.Name;
        }

        public WebSub Control => _control;

        public string Key => _key;

        internal void Do(WebContext wc)
        {
            _actor(wc);
        }

        public override string ToString()
        {
            return _key;
        }
    }

    public delegate void Actor<in TZone>(WebContext wc, TZone zone);


    public class WebAction<TZone> : IMember where TZone : IZone
    {
        // the declaring sub
        readonly WebSub<TZone> _control;

        readonly string _key;

        Actor<TZone> _actor;

        internal WebAction(WebSub<TZone> control, MethodInfo mi)
        {
            _control = control;
            _actor = (Actor<TZone>) mi.CreateDelegate(typeof(Actor), control);
            // NOTE: strict method name as key here to avoid the default base url trap
            _key = mi.Name;
        }

        public WebSub<TZone> Control => _control;

        public string Key => _key;

        internal void Do(WebContext wc, TZone zone)
        {
            _actor(wc, zone);
        }

        public override string ToString()
        {
            return _key;
        }
    }
}