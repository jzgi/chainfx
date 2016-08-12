using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Greatbone.Core
{
	///
	/// A web service controller that may contain sub-controllers and/or a multiplexer.
	///
	/// cache-control -- elimicates redundant requests (max-age) or data queries (not-modified).
	/// response cache -- directly returns shared cached contents
	/// etag -- reduces network I/O with unchanged results
	///
	///
	public abstract class WebService : WebSub, IParent, ICacheRealm, IHttpApplication<HttpContext>
	{
		const int MsgPort = 7777;

		private KestrelServerOptions options;

		private readonly LoggerFactory logger;

		// the attached sub controllers, if any
		private Set<WebSub> subs;

		// the attached multiplexer, if any
		private IZoneHub zonehub;


		// topics published by this microservice
		private Set<MsgPublish> publishes;

		// topics subscribed by this microservice
		private Set<MsgSubscribe> subscribes;


		// the embedded http server
		private readonly KestrelServer server;

		// a discriminator of virtual host
		private readonly string address;

		// a  virtual host
		private readonly int port;

		// the async client
		private HttpClient[] client;


		protected WebService(WebServiceBuilder builder) : base(builder)
		{
			address = builder.Debug ? "localhost" : builder.Host;
			port = builder.Port;

			// create the server instance
			logger = new LoggerFactory();
			options = new KestrelServerOptions();
			server = new KestrelServer(Options.Create(options), Lifetime, logger);
			server.Features.Get<IServerAddressesFeature>().Addresses.Add("http://" + address + ":" + port);
			server.Features.Get<IServerAddressesFeature>().Addresses.Add("http://" + address + ":" + MsgPort);
		}


		public HttpContext CreateContext(IFeatureCollection features)
		{
			return new DefaultHttpContext(features);
		}

		///
		/// To asynchronously process the request.
		///
		public async Task ProcessRequestAsync(HttpContext context)
		{
			// dispatch the context accordingly

			ConnectionInfo ci = context.Connection;
			IPAddress ip = ci.LocalIpAddress;
			int port = ci.LocalPort;

			if (port == this.port && ip.Equals(address))
			{
				using (MsgContext wc = new MsgContext(context))
				{
				}
			}

			using (WebContext wc = new WebContext(context))
			{
				Handle(context.Request.Path.Value.Substring(1), wc);

				await wc.SendAsyncTask();
			}
		}

		public void DisposeContext(HttpContext context, Exception exception)
		{
		}


		public void Start()
		{
			// prepare event queue service
			//


			// start the server
			//
			server.Start(this);

			var urls = server.Features.Get<IServerAddressesFeature>().Addresses;

			Console.WriteLine("Listening on");

			var token = new CancellationToken();

			token.Register(
				state => { ((IApplicationLifetime) state).StopApplication(); },
				this
			);

			ApplicationStopping.WaitHandle.WaitOne();
		}

		public void StopApplication()
		{
		}

		public CancellationToken ApplicationStarted { get; set; }

		public CancellationToken ApplicationStopping { get; set; }

		public CancellationToken ApplicationStopped { get; set; }


		public void Subscribe(string topic, MsgDoer doer)
		{
			subscribes.Add(new MsgSubscribe(topic, doer));
		}


		public TSub AddSub<TSub>(string key, Checker checker) where TSub : WebSub
		{
			if (subs == null)
			{
				subs = new Set<WebSub>(16);
			}
			// create instance by reflection
			Type type = typeof(TSub);
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebServiceBuilder)});
			if (ci == null)
			{
				throw new WebException(type + ": the WebCreationContext parameterized constructor not found");
			}
			WebServiceBuilder wcc = new WebServiceBuilder
			{
				Key = key,
				StaticPath = Path.Combine(StaticPath, key),
				Parent = this,
				Service = this
			};
			TSub sub = (TSub) ci.Invoke(new object[] {wcc});
			sub.Checker = checker;

			subs.Add(sub);

			//
			// check declared event handler methods

			return sub;
		}

		public THub MountHub<THub, TZone>(Checker<TZone> checker) where THub : WebZoneHub<TZone> where TZone : IZone
		{
			// create instance
			Type type = typeof(THub);
			ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebServiceBuilder)});
			if (ci == null)
			{
				throw new WebException(type + ": the WebCreationContext parameterized constructor not found");
			}
			WebServiceBuilder wcc = new WebServiceBuilder
			{
				Key = "-",
				StaticPath = Path.Combine(StaticPath, "-"),
				Parent = this,
				Service = this
			};
			THub hub = (THub) ci.Invoke(new object[] {wcc});

			// call the initialization and set
			zonehub = hub;
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
					if (zonehub == null)
					{
						wc.Response.StatusCode = 501; // Not Implemented
					}
					else
					{
						string zoneKey = dir.Substring(1);
						IZone zone;
						if (zonehub.ResolveZone(zoneKey, out zone))
						{
							wc.Zone = zone;
							zonehub.Handle(relative.Substring(slash + 1), wc);
						}
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

		public long ModifiedOn { get; set; }


		///
		/// sends an event to a target service
		///
		public void Publish(string topic, string subarg, object msg)
		{
		}


		public SqlContext NewDbContext()
		{
			WebService svc = Service;
			NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder()
			{
				Host = "localhost",
				Database = svc.Key,
				Username = "postgres",
				Password = "Zou###1989"
			};
			return new SqlContext(builder);
		}

		///
		/// STATIC
		///
		static readonly WebLifetime Lifetime = new WebLifetime();

		public static void Run(params WebService[] services)
		{
			foreach (WebService svc in services)
			{
				svc.Start();
			}

			var token = new CancellationToken();

			token.Register(
				state => { ((IApplicationLifetime) state).StopApplication(); },
				Lifetime
			);

			Lifetime.ApplicationStopping.WaitHandle.WaitOne();
		}
	}
}