using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
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

		private static int DebugPort = 8080;

		// a discriminator of virtual host
		private string subdomain;

		// a discriminator of virtual host
		private string port = (DebugPort++).ToString();

		// the attached sub controllers, if any
		private Set<WebSub> _subs;

		// the attached multiplexer, if any
		private IZoneHub _hub;


		// topic sent by this microservice
		private Set<WebEvent> _publishes;

		// topic received by this microservice
		private Set<WebEvent> _subscribes;

		// the async client
		private HttpClient[] client;


		protected WebService(WebServiceContext wsc) : base(wsc)
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
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebServiceContext)});
			if (ci == null)
			{
				throw new WebException(type + ": the WebCreationContext parameterized constructor not found");
			}
			WebServiceContext wcc = new WebServiceContext
			{
				Key = key,
				StaticPath = Path.Combine(StaticPath, key),
				Parent = this,
				Service = this
			};
			TSub sub = (TSub) ci.Invoke(new object[] {wcc});
			sub.Checker = checker;

			_subs.Add(sub);

			//
			// check declared event handler methods

			return sub;
		}

		public THub MountHub<THub, TZone>(Checker<TZone> checker) where THub : WebZoneHub<TZone> where TZone : IZone
		{
			// create instance
			Type type = typeof(THub);
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebServiceContext)});
			if (ci == null)
			{
				throw new WebException(type + ": the WebCreationContext parameterized constructor not found");
			}
			WebServiceContext wcc = new WebServiceContext
			{
				Key = "-",
				StaticPath = Path.Combine(StaticPath, "-"),
				Parent = this,
				Service = this
			};
			THub hub = (THub) ci.Invoke(new object[] {wcc});

			// call the initialization and set
			_hub = hub;
			return hub;
		}

		internal bool IsTarget(string host)
		{
			if (host.StartsWith("localhost:"))
			{
				return host.EndsWith(port);
			}
			return host.StartsWith(subdomain);
		}

		public void AddPublish(string topic)

		{
			if (_publishes == null)
			{
			}
		}


		// NOTE: for long-pulling support, a sending acitity must be initailized based on the context
		//
		internal Task Handle(HttpContext context)
		{
			string host = context.Request.Headers["Host"];

			//            Console.WriteLine("Host: " + host);

			if (host.EndsWith("9090")) // request for events (topics)
			{
				// msg
			}
			else // request for action or static
			{
				using (WebContext wc = new WebContext(context))
				{
					Handle(context.Request.Path.Value.Substring(1), wc);

					return wc.SendAsync();
				}
			}
			return null;
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
					if (_hub == null)
					{
						wc.Response.StatusCode = 501; // Not Implemented
					}
					else
					{
						string zoneKey = dir.Substring(1);
						IZone zone;
						if (_hub.ResolveZone(zoneKey, out zone))
						{
							wc.Zone = zone;
							_hub.Handle(relative.Substring(slash + 1), wc);
						}
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


		///
		/// sends an event to a target service
		///
		public void Publish(string topic, string subarg, IData msg)
		{
		}
	}
}