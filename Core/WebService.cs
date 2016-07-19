using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    ///
    /// A web service controller that may contain sub-controllers and/or a multiplexer.
    ///
    public abstract class WebService : WebSub, ICacheControl
    {
        private readonly WebService _parent;

        // the attached sub controllers, if any
        private Set<WebSub> _subs;

        // the attached multiplexer, if any
        private IMux _mux;

        private Set<Publish> _publishes;

        private Set<Subscribe> _subscribes;



        protected WebService(WebService parent) : base(null)
        {
        }


        public override WebService Service => this;

        public TSub AddSub<TSub>(string key, Checker checker) where TSub : WebSub
        {
            if (_subs == null)
            {
                _subs = new Set<WebSub>(16);
            }

            // create instance of the subactivity by reflection
            Type type = typeof(TSub);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebService)});
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
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebService)});
            if (ci == null)
            {
                throw new WebException(type + " the special constructor not found");
            }
            TMux mux = (TMux) ci.Invoke(new object[] {this});
            mux.Checker = checker;

            _mux = mux;
            return mux;
        }


        public void AddPublish(string topic)

        {
            if (_publishes == null)
            {
                _publishes = new Set<Publish>(32);
            }

        }

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
                    string zoneKey = dir.Substring(1, dir.Length - 2);
                    IZone zone;
                    if (_mux.ResolveZone(zoneKey, out zone))
                    {
                        wc.Zone = zone;
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


        public void Publish(string topic, string arg, object msg)
        {
        }
    }
}