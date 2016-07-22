using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    ///
    /// A web service controller that may contain sub-controllers and/or a multiplexer.
    ///
    public abstract class WebService : WebSub, IParent, ICacheRealm
    {
        public static readonly Checker AnyOne = (t) => true;

        public static readonly Checker<IZone> AnyOneZone = (t, z) => true;

        // the attached sub controllers, if any
        private Set<WebSub> _subs;

        // the attached multiplexer, if any
        private IMux _mux;

        private Set<Publish> _publishes;

        private Set<Subscribe> _subscribes;


        protected WebService(WebCreationContext wcc) : base(wcc)
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
            sub.Checker = checker;


            _subs.Add(sub);
            return sub;
        }

        public TMux SetMux<TMux, TZone>(Checker<TZone> checker) where TMux : WebMux<TZone> where TZone : IZone
        {
            // create instance
            Type type = typeof(TMux);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebCreationContext)});
            if (ci == null)
            {
                throw new WebException(type + ": the WebCreationContext parameterized constructor not found");
            }
            WebCreationContext wcc = new WebCreationContext
            {
                StaticPath = ""
            };
            TMux mux = (TMux) ci.Invoke(new object[] {wcc});

            // call the initialization and set
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


        // NOTE: for long-pulling support, a sending acitity must be initailized based on the context
        //
        internal Task Process(HttpContext context)
        {
//            Console.WriteLine("start Processing ... ");
            string path = context.Request.Path.Value;
            if (Statics != null && path.IndexOf('.') != -1)
            {
                Static sta;
                if (Statics.TryGet(path, out sta))
                {
                    context.Response.Body.WriteAsync(sta.Content, 0, sta.Length);
                }
                return null;
            }

            context.Response.WriteAsync("OK, this is an output.", Encoding.UTF8);
            WebContext wc = new WebContext(context);
            Handle(context.Request.Path.Value.Substring(1), wc);
            return null;
        }

        public override void Handle(string relative, WebContext wc)
        {
//            Console.WriteLine("relative: " + relative);
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                WebAction a = GetAction(relative);
//                Console.WriteLine("action: " + a);
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