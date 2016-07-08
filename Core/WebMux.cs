using System;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
    internal interface IMux
    {
        void Handle(string basis, WebContext wc);
    }

    public abstract class WebMux<TZone> : WebSub<TZone>, IMux where TZone : IZone
    {
        private readonly WebHub _hub;

        private Set<WebSub<TZone>> _subs;

        protected WebMux(WebHub hub) : base(null)
        {
            _hub = hub;
        }

        public WebHub Hub => _hub;

        public T AddSub<T>(string key, Checker<TZone> checker) where T : WebSub<TZone>
        {
            if (_subs == null)
            {
                _subs = new Set<WebSub<TZone>>(16);
            }

            // create instance by reflection
            Type type = typeof(T);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebHub)});
            if (ci == null)
            {
                throw new WebException(type + ": A required constructor not found");
            }
            T sub = (T) ci.Invoke(new object[] {this});
            sub.Key = key;
            sub.Checker = checker;

            _subs.Add(sub);
            return sub;
        }

        public override void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                WebAction<TZone> a = GetAction(relative);
                a?.Do(wc, (TZone) (wc.zone));
            }
            else // not local then sub
            {
                string rsc = relative.Substring(0, slash);
                WebSub<TZone> sub;
                if (_subs.TryGet(rsc, out sub))
                {
                    sub.Handle(rsc.Substring(slash), wc);
                }
            }
        }
    }
}