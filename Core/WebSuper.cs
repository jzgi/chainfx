using System;
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

		protected WebSuper(WebServiceContext wsc) : base(wsc)
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
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebServiceContext)});
			if (ci == null)
			{
				throw new WebException(type + ": the constructor with WebServiceContext not defined");
			}
//			WebServiceContext wcc = new WebServiceContext
//			{
//				Key = key,
//				StaticPath = Path.Combine(StaticPath, key),
//				Parent = this,
//				Service = Service
//			};
			TSub sub = (TSub) ci.Invoke(new object[] {null});

			subs.Add(sub);

			//
			// check declared event handler methods

			return sub;
		}

		public THub MountHub<THub>(bool auth) where THub : WebXHub
		{
			// create instance
			Type type = typeof(THub);
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebServiceContext)});
			if (ci == null)
			{
				throw new WebException(type + ": the constructor with WebBuilder not defined");
			}
//			WebBuilder wb = new WebBuilder
//			{
//				Key = "-",
//				StaticPath = Path.Combine(StaticPath, "-"),
//				Parent = this,
//				Service = Service
//			};
			THub hub = (THub) ci.Invoke(new object[] {null});

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
						string x = dir.Substring(1);
						wc.X = x;
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