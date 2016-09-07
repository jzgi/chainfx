﻿using System;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>A multiplexing hub controller that is attached to a realm controller. </summary>
    ///
    public abstract class WebXHub : WebSub
    {
        // the added sub controllers
        private Set<WebSub> subs;

        protected WebXHub(WebServiceContext wsc) : base(wsc)
        {
        }

        public TSub AddSub<TSub>(string key, bool auth) where TSub : WebSub
        {
            if (subs == null)
            {
                subs = new Set<WebSub>(16);
            }
            // create instance by reflection
            Type type = typeof(TSub);
            ConstructorInfo ci = type.GetConstructor(new[] { typeof(WebServiceContext) });
            if (ci == null)
            {
                throw new WebException(type + ": the constructor not found (WebServiceContext)");
            }
            WebServiceContext wsc = new WebServiceContext
            {
                key = key,
                Parent = this,
                Service = Service,
                IsX = true
            };
            TSub sub = (TSub)ci.Invoke(new object[] { wsc });
            // call the initialization and add
            subs.Add(sub);

            return sub;
        }

        public override void HandleAction(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                WebAction a = GetAction(relative);
                a?.Do(wc, wc.X);
            }
            else // not local then sub
            {
                string rsc = relative.Substring(0, slash);
                WebSub sub;
                if (subs.TryGet(rsc, out sub))
                {
                    sub.HandleAction(rsc.Substring(slash), wc);
                }
            }
        }
    }
}