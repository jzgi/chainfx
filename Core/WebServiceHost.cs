using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Greatbone.Core
{
	public class WebServiceHost : WebHost
	{
		private List<WebService> _services;

		public WebServiceHost(IServiceCollection services, IServiceProvider provider, WebHostOptions options,
			IConfiguration config) : base(services, provider, options, config)
		{
		}

		///
		/// Creates and adds a microservice to this service host.
		///
		public TService AddMicroService<TService>(string key, Checker checker) where TService : WebService, new()
		{
			TService service = new TService()
			{
//				Key = key,
				Checker = checker
			};

			_services.Add(service);

			return service;
		}

		internal async Task Dispatch(HttpContext ctx)
		{
			// the host header to dispatch accordingly
			string host = ctx.Request.Headers["Host"];
			if (host == null)
			{
				await _services[0].Handle(ctx);
			}

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

					await wc.SendAsync();
				}
			}
		}
	}
}