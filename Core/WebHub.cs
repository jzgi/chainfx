using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    public abstract class WebHub : WebSub, IStateSet
    {
        private Set<WebSub> _subs;

        private IMux _mux;

        protected WebHub(WebHub parent) : base(parent)
        {
        }

        public TSub AddSub<TSub>(string key, Checker checker) where TSub : WebSub
        {
            if (_subs == null)
            {
                _subs = new Set<WebSub>(16);
            }

            // create instance by reflection
            Type type = typeof(TSub);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebHub)});
            if (ci == null)
            {
                throw new WebException(type + " the special constructor not found");
            }
            TSub sub = (TSub) ci.Invoke(new object[] {this});
            sub.Key = key;
            sub.Checker = checker;

            _subs.Add(sub);
            return sub;
        }

        public TMux SetMux<TMux, TZone>(Checker<TZone> checker) where TMux : WebMux<TZone> where TZone : IZone
        {
            // create instance by reflection
            Type type = typeof(TMux);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebHub)});
            if (ci == null)
            {
                throw new WebException(type + " the special constructor not found");
            }
            TMux mux = (TMux) ci.Invoke(new object[] {this});
            mux.Checker = checker;

            _mux = mux;
            return mux;
        }

        protected abstract bool ResolveZone(string zoneId, WebContext wc);


        internal async Task Process(HttpContext context)
        {
            Console.WriteLine("start Processing ... ");
            WebContext wc = new WebContext(context);
            Handle(context.Request.Path.Value.Substring(1), wc);
        }

        public override void Handle(string relative, WebContext wc)
        {
            Console.WriteLine("relative: " + relative);
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                WebAction a = GetAction(relative);
                Console.WriteLine("action: " + a);
                a?.Do(wc);
            }
            else // not local then sub & mux
            {
                string dir = relative.Substring(0, slash);
                if (dir.StartsWith("-") && dir.EndsWith("-")) // mux
                {
                    if (_mux == null)
                    {
                        // send not implemented
                    }
                    string zoneId = dir.Substring(1, dir.Length - 2);
                    if (ResolveZone(zoneId, wc))
                    {
                        _mux.Handle(relative.Substring(slash + 1), wc);
                    }
                }
                else
                {
                    WebSub sub;
                    if (_subs.TryGet(dir, out sub))
                    {
                        sub.Handle(relative.Substring(slash + 1), wc);
                    }
                }
            }
        }

        public long ModifiedOn { get; set; }
    }
}