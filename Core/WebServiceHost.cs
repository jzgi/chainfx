using System;
using System.Collections.Generic;
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

namespace Greatbone.Core
{
	public class WebServiceHost : IHttpApplication<HttpContext>, IApplicationLifetime
	{
		private KestrelServerOptions options;

		private KestrelServer server;

		private List<WebService> _services;


		public HttpContext CreateContext(IFeatureCollection features)
		{
			return new DefaultHttpContext(features);
		}

		public async Task ProcessRequestAsync(HttpContext context)
		{
			// dispatch
			// the host header to dispatch accordingly
			string host = context.Request.Headers["Host"];
			WebService target = null;
			if (host == null)
			{
				target = _services[0];
			}
			else
			{
				for (int i = 0; i < _services.Count; i++)
				{
					WebService svc = _services[i];
					if (svc.IsTarget(host))
					{
						target = svc;
					}
				}
			}
			if (target != null)
			{
				await target.Handle(context);
			}
			else
			{
			}
			await context.Response.WriteAsync("<h2>Hello</h2>");
		}

		public void DisposeContext(HttpContext context, Exception exception)
		{
			throw new NotImplementedException();
		}


		public void Start()
		{
			var logger = new LoggerFactory();

			server = new KestrelServer(Options.Create(options), this, logger);

			server.Features.Get<IServerAddressesFeature>().Addresses.Add("http://localhost:8080");

			server.Start(this);

			Console.WriteLine("OK");

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
	}
}