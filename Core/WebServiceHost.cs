using System;
using System.Collections.Generic;
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

		///
		/// To dispatch a HTTP context to appropriate target microservice for processing.
		///
		internal async Task Dispatch(HttpContext ctx)
		{
			// the host header to dispatch accordingly
			string host = ctx.Request.Headers["Host"];
			WebService target = null;
			if (host == null)
			{
				target = _services[0];
			}
			else
			{
				foreach (WebService svc in _services)
				{
					if (svc.IsTarget(host))
					{
						target = svc;
						break;
					}
				}
			}
			if (target != null)
			{
				await target.Handle(ctx);
			}
			else
			{
			}
		}
	}
}