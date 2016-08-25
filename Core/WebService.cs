using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
	public abstract class WebService : WebSuper, ICacheRealm, IHttpApplication<HttpContext>
	{
		const int MsgPort = 7777;

		public WebServiceContext Context { get; internal set; }

		// topics published by this microservice
		readonly Set<EvtPublish> publishes;

		// topics subscribed by this microservice
		readonly Set<EvtSubscribe> subscribes;


		readonly KestrelServerOptions options;

		readonly LoggerFactory logger;

		// the embedded http server
		readonly KestrelServer server;

		// a discriminator of virtual host
		readonly string address;

		// a  virtual host
		readonly int port;

		// the async client
		private HttpClient[] client;


		protected WebService(WebServiceContext wsc) : base(wsc)
		{
			address = wsc.debug ? "localhost" : wsc.host;
//			port = builder.port;

			// create the server instance
			logger = new LoggerFactory();
			options = new KestrelServerOptions();
			server = new KestrelServer(Options.Create(options), Lifetime, logger);
			ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
			addrs.Add("http://" + address + ":" + port);
			addrs.Add("http://" + address + ":" + MsgPort);
		}


		public virtual void OnStart()
		{
		}

		public virtual void OnStop()
		{
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
				using (EvtContext wc = new EvtContext(context))
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


		public void Subscribe(string topic, EvtDoer doer)
		{
			subscribes.Add(new EvtSubscribe(topic, doer));
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