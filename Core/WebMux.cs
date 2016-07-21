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

    public abstract class WebMux<TZone> : WebSub<TZone>, IMux where TZone : IZone
    {
        // the added subs
        private Set<WebSub<TZone>> _subs;

        public new WebService Parent { get; internal set; }

        public TSub AddSub<TSub>(string key, Checker<TZone> checker) where TSub : WebSub<TZone>, new()
        {
            if (_subs == null)
            {
                _subs = new Set<WebSub<TZone>>(16);
            }
            // create instance
            TSub sub = new TSub
            {
                Parent = this,
                Service = this.Service,
                Key = key,
                Checker = checker
            };
            // call the initialization and add
            sub.Init();
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