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
		public static readonly Checker AnyOne = (t) => true;

		public static readonly Checker<IZone> AnyOneZone = (t, z) => true;

		const int MqPort = 7777;

		private KestrelServerOptions _options;

		private readonly LoggerFactory _logger;

		// the attached sub controllers, if any
		private Set<WebSub> _subs;

		// the attached multiplexer, if any
		private IZoneHub _zonehub;


		// topic sent by this microservice
		private Set<WebEvent> _publishes;

		// topic received by this microservice
		private Set<WebEvent> _subscribes;

		// the embedded http server
		private readonly KestrelServer _server;

		// a discriminator of virtual host
		private readonly string _address;

		// a  virtual host
		private readonly int _port;

		// the async client
		private HttpClient[] client;


		protected WebService(WebServiceBuilder builder) : base(builder)
		{
			_address = builder.Debug ? "localhost" : builder.Host;
			_port = builder.Port;

			// create the server instance
			_logger = new LoggerFactory();
			_options = new KestrelServerOptions();
			_server = new KestrelServer(Options.Create(_options), Lifetime, _logger);
			_server.Features.Get<IServerAddressesFeature>().Addresses.Add("http://" + _address + ":" + _port);
			_server.Features.Get<IServerAddressesFeature>().Addresses.Add("http://" + _address + ":" + MqPort);
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
			_server.Start(this);

			var urls = _server.Features.Get<IServerAddressesFeature>().Addresses;

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


		public TSub AddSub<TSub>(string key, Checker checker) where TSub : WebSub
		{
			if (_subs == null)
			{
				_subs = new Set<WebSub>(16);
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

			_subs.Add(sub);

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
			_zonehub = hub;
			return hub;
		}


		public void AddPublish(string topic)

		{
			if (_publishes == null)
			{
			}
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
					if (_zonehub == null)
					{
						wc.Response.StatusCode = 501; // Not Implemented
					}
					else
					{
						string zoneKey = dir.Substring(1);
						IZone zone;
						if (_zonehub.ResolveZone(zoneKey, out zone))
						{
							wc.Zone = zone;
							_zonehub.Handle(relative.Substring(slash + 1), wc);
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