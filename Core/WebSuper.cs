using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
	public abstract class WebSuper : WebSub
	{
		// the added sub controllers, if any
		private Set<WebSub> subs;

		// the attached multiplexer, if any
		private WebXHub xhub;

		protected WebSuper(WebBuilder builder) : base(builder)
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
				throw new WebException(type + ": the constructor with WebBuilder not defined");
			}
			WebBuilder wcc = new WebBuilder
			{
				Key = key,
				StaticPath = Path.Combine(StaticPath, key),
				Parent = this,
				Service = Service
			};
			TSub sub = (TSub) ci.Invoke(new object[] {wcc});
			sub.Checker = checker;

			subs.Add(sub);

			//
			// check declared event handler methods

			return sub;
		}

		public THub MountHub<THub>(Checker checker) where THub : WebXHub
		{
			// create instance
			Type type = typeof(THub);
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebBuilder)});
			if (ci == null)
			{
				throw new WebException(type + ": the constructor with WebBuilder not defined");
			}
			WebBuilder wcc = new WebBuilder
			{
				Key = "-",
				StaticPath = Path.Combine(StaticPath, "-"),
				Parent = this,
				Service = Service
			};
			THub hub = (THub) ci.Invoke(new object[] {wcc});

			// call the initialization and set
			xhub = hub;
			return hub;
		}


		public override void Handle(string relative, WebContext wc)
		{
			int slash = relative.IndexOf('/');
			if (slash == -1) // without a slash then handle it locally
			{
				base.Handle(relative, wc);
			}
			else // not local then sub & mux
			{
				string dir = relative.Substring(0, slash);
				if (dir.StartsWith("-")) // mux
				{
					if (xhub == null)
					{
						wc.Response.StatusCode = 501; // Not Implemented
					}
					else
					{
						string zoneKey = dir.Substring(1);
						wc.X = zoneKey;
						xhub.Handle(relative.Substring(slash + 1), wc);
					}
				}
				else
				{
					WebSub sub;
					if (subs.TryGet(dir, out sub))
					{
						sub.Handle(relative.Substring(slash + 1), wc);
					}
				}
			}
		}
	}
}