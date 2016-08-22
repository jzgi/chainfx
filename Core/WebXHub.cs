using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
	public abstract class WebXHub : WebSub
	{
		// the added subs
		private Set<WebSub> subs;


		protected WebXHub(WebBuilder builder) : base(builder)
		{
		}

		public TSub AddSub<TSub>(string key, Checker checker) where TSub : WebSub
		{
			if (subs == null)
			{
				subs = new Set<WebSub>(16);
			}
			// create instance by reflection
			Type type = typeof(TSub);
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebBuilder)});
			if (ci == null)
			{
				throw new WebException(type + ": the WebCreationContext-param constructor not found");
			}
			WebBuilder wcc = new WebBuilder
			{
				Key = key,
				StaticPath = Path.Combine(StaticPath, key),
				Parent = this,
				Service = Service
			};
			TSub sub = (TSub) ci.Invoke(new object[] {wcc});
			// call the initialization and add
			subs.Add(sub);
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
				WebAction a = GetAction(relative);
				a?.Do(wc, wc.X);
			}
			else // not local then sub
			{
				string rsc = relative.Substring(0, slash);
				WebSub sub;
				if (subs.TryGet(rsc, out sub))
				{
					sub.Handle(rsc.Substring(slash), wc);
				}
			}
		}
	}
}