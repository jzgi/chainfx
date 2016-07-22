using System;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
    internal interface IMux
    {
        void Handle(string basis, WebContext wc);

        bool ResolveZone(string zoneKey, out IZone zone);
    }

    public abstract class WebMux<TZone> : WebSub<TZone>, IMux, IParent where TZone : IZone
    {
        // the added subs
        private Set<WebSub<TZone>> _subs;


        public WebMux(WebCreationContext wcc) : base(wcc)
        {
        }

        public TSub AddSub<TSub>(string key, Checker<TZone> checker) where TSub : WebSub<TZone>
        {
            if (_subs == null)
            {
                _subs = new Set<WebSub<TZone>>(16);
            }
            // create instance by reflection
            Type type = typeof(TSub);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebCreationContext)});
            if (ci == null)
            {
                throw new WebException(type + ": the WebCreationContext parameterized constructor not found");
            }
            WebCreationContext wcc = new WebCreationContext
            {
                Key = key,
                StaticPath = ""
            };
            TSub sub = (TSub) ci.Invoke(new object[] {wcc});
            // call the initialization and add
            _subs.Add(sub);
            return sub;
        }


        public bool ResolveZone(string zoneKey, out IZone zone)
        {
            throw new NotImplementedException();
        }

        public override void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                WebAction<TZone> a = GetAction(relative);
                a?.Do(wc, (TZone) (wc.Zone));
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