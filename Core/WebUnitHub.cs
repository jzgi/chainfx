using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
    internal interface IUnitHub
    {
        void Handle(string basis, WebContext wc);

        bool ResolveUnit(string unitKey, out IUnit unit);
    }

    public abstract class WebUnitHub<TUnit> : WebSub<TUnit>, IUnitHub, IParent where TUnit : IUnit
    {
        // the added subs
        private Set<WebSub<TUnit>> _subs;


        public WebUnitHub(WebServiceBuilder builder) : base(builder)
        {
        }

        public TSub AddSub<TSub>(string key, Checker<TUnit> checker) where TSub : WebSub<TUnit>
        {
            if (_subs == null)
            {
                _subs = new Set<WebSub<TUnit>>(16);
            }
            // create instance by reflection
            Type type = typeof(TSub);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebServiceBuilder)});
            if (ci == null)
            {
                throw new WebException(type + ": the WebCreationContext-param constructor not found");
            }
            WebServiceBuilder wcc = new WebServiceBuilder
            {
                Key = key,
                StaticPath = Path.Combine(StaticPath, key),
                Parent = this,
                Service = Service
            };
            TSub sub = (TSub) ci.Invoke(new object[] {wcc});
            // call the initialization and add
            _subs.Add(sub);
            return sub;
        }


        public bool ResolveUnit(string unitKey, out IUnit unit)
        {
            throw new NotImplementedException();
        }

        public override void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                WebAction<TUnit> a = GetAction(relative);
                a?.Do(wc, (TUnit) (wc.Unit));
            }
            else // not local then sub
            {
                string rsc = relative.Substring(0, slash);
                WebSub<TUnit> sub;
                if (_subs.TryGet(rsc, out sub))
                {
                    sub.Handle(rsc.Substring(slash), wc);
                }
            }
        }
    }
}